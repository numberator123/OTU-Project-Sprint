using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Animator anim;
    private SpriteRenderer spriteRenderer;

    // Collision settings
    [SerializeField] private LayerMask obstacleLayer;
    [SerializeField] private float checkRadius = 0.2f;

    // Movement vectors
    private float x, y;

    // Movement speed, set this in the player in the Unity inspector
    public float moveSpeed;
    
    // The size of one grid tile
    [SerializeField] private float gridSize = 1f;

    // Time in seconds to accept inputs before moving
    [SerializeField] private float inputBufferTime = 0.05f;
    
    // Movement flags
    private bool isWalking = false;
    private bool isMoving = false;
    private Vector3 originalPos;
    private Vector3 targetPos;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();

        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(isMoving)
        {
            return;
        }

        // Movement vectors
        x = 0f;
        y = 0f;

        // Check keyboard inputs
        x = CheckHorizontalInput();
        y = CheckVerticalInput();

        // If an input is pressed
        if(x != 0 || y != 0)
        {
            // Update visual direction immediately
            UpdateAnimationDirection(x,y);

            // get the target position for the animation
            Vector3 targetPosition = transform.position + new Vector3(x, y, 0) * gridSize;
            if(canWalk(targetPosition))
            {
                // Start walking if an input is pressed and
                if (!isWalking)
                {
                    isWalking = true;
                    anim.SetBool("isWalking", isWalking);
                }

                // Start buffer window
                StartCoroutine(BufferInputWindow(x, y));
            }
        }
        // If no inputs are pressed
        else if(isWalking)
        {
            isWalking = false;
            anim.SetBool("isWalking", isWalking);
        }
    }

    private float CheckHorizontalInput()
    {
        // a/d, left/right
        if(Keyboard.current == null) return 0f;
        if(Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) return 1f;
        if(Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) return -1f;
        return 0f;
    }
    private float CheckVerticalInput()
    {
        // w/s, up/down
        if(Keyboard.current == null) return 0f;
        if(Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) return 1f;
        if(Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) return -1f;
        return 0f;
    }

    private bool canWalk(Vector3 targetPosition)
    {
        // Return whether the tile can be walked on
        return Physics2D.OverlapCircle(targetPosition, checkRadius, obstacleLayer) == null;
    }

    // Update the direction of the player visually
    private void UpdateAnimationDirection(float x, float y)
    {
        // Update direction values for animation to know what to play
        anim.SetFloat("X", x);
        anim.SetFloat("Y", y);

        // Assuming sprites face right
        if(x < 0)
        {
            // Flip for facing left
            spriteRenderer.flipX = true;
        }
        else if(x > 0)
        {
            // No flip for facing right
            spriteRenderer.flipX = false;
        }
    }

    // Free movement (deprecated)
    private void Move()
    {
        // Update direction visually
        UpdateAnimationDirection(x,y);

        transform.Translate(new Vector3(x, y, 0) * Time.deltaTime * moveSpeed);
    }

    // Input buffer for handling diagonals
    private IEnumerator BufferInputWindow(float curX, float curY)
    {
        isMoving = true;

        float elapsedTime = 0f;

        // Check input during the buffer window
        while(elapsedTime < inputBufferTime)
        {
            // Check horizontal input if not currently moving horizontally
            if(curX == 0f)
            {
                curX = CheckHorizontalInput();
            }
            // Check vertical input if not currently moving vertically
            if(curY == 0f)
            {
                curY = CheckVerticalInput();
            }

            // Update elapsed time and run in the background
            elapsedTime += Time.unscaledDeltaTime;
            yield return null;
        }

        // Update globals
        x = curX;
        y = curY;

        // Confirm buttons still being pressed
        if(x != 0 || y != 0)
        {
            yield return StartCoroutine(GridMove(new Vector3(x,y,0) * gridSize));
        }
        else
        {
            isMoving = false;
            isWalking = false;
            anim.SetBool("isWalking", isWalking);
        }
    }

    // Movement locked to a grid
    private IEnumerator GridMove(Vector3 direction)
    {
        while(direction != Vector3.zero)
        {
            // Update direction visually
            UpdateAnimationDirection(x,y);

            // Get target position
            originalPos = transform.position;
            targetPos = originalPos + direction;

            // Check collision
            if (!canWalk(targetPos))
            {
                // Stop walking animation
                isWalking = false;
                anim.SetBool("isWalking", isWalking);
                
                break;
            }

            // Use elapsedTime to update position
            float elapsedTime = 0f;
            float finalSpeed = moveSpeed;

            // Slow down move speed for diagonal movement
            if(direction.x != 0f && direction.y != 0f)
            {
                finalSpeed /= (float)Math.Sqrt(2f);
            }

            // Interpolate position between the 2 points
            while(elapsedTime < 1f)
            {
                transform.position = Vector3.Lerp(originalPos, targetPos, elapsedTime);

                // Update elapsed time and run in the background
                elapsedTime += Time.deltaTime * finalSpeed;
                yield return null;
            }

            // Snap into position at the end of the movement animatio
            transform.position = targetPos;

            // Update inputs at the end of every move
            x = CheckHorizontalInput();
            y = CheckVerticalInput();

            // Check if inputs are still being pressed
            if(x != 0 || y != 0)
            {
                // Update direction
                direction = new Vector3(x,y,0) * gridSize;
            }
            else
            {
                // Reset direction
                direction = Vector3.zero;
            }
        }

        // Finished walking
        isMoving = false;
        isWalking = false;
        anim.SetBool("isWalking", isWalking);
    }
    
    // For debugging player hitbox, comment out
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Vector3 checkPos = Application.isPlaying && targetPos != Vector3.zero ? targetPos : transform.position;
        Gizmos.DrawWireSphere(checkPos, checkRadius);
    }
}
