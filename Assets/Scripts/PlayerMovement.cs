using UnityEngine;

using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private Animator anim;

    private float x, y;
    private bool isWalking;

    public float moveSpeed;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Keyboard.current != null)
        {
            x = 0f;
            y = 0f;

            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x = 1f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x = -1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y = 1f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y = -1f;
        }

        if(x != 0 || y != 0)
        {
            if(!isWalking)
            {
                isWalking = true;
                anim.SetBool("isWalking", isWalking);
            }

            Move();
        }
        else if(isWalking)
        {
            isWalking = false;
            anim.SetBool("isWalking", isWalking);
        }
    }

    private void Move()
    {
        anim.SetFloat("X", x);
        anim.SetFloat("Y", y);

        transform.Translate(new Vector3(x, y, 0) * Time.deltaTime * moveSpeed);
    }
}
