using UnityEngine;
using UnityEngine.InputSystem;
//Import fat dude


public class FatScript : MonoBehaviour
{
    [SerializeField] private Rigidbody2D fatRigidBody;

    [SerializeField] private float speed = 10f;

    // Movement boundaries
    [SerializeField] private float targetX = 5f;
    [SerializeField] private float targetY = 5f;

    private FatDude fatDude;

    private void Start()
    {
        fatDude = new FatDude(speed, targetX, targetY);
    }

    private void FixedUpdate()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.wKey.isPressed)
        {
            fatDude.MoveUp(fatRigidBody);
        }
        else if (Keyboard.current.sKey.isPressed)
        {
            fatDude.MoveDown(fatRigidBody);
        }
        else if (Keyboard.current.aKey.isPressed)
        {
            fatDude.MoveLeft(fatRigidBody);
        }
        else if (Keyboard.current.dKey.isPressed)
        {
            fatDude.MoveRight(fatRigidBody);
        }
        else
        {
            fatDude.Stop(fatRigidBody);
        }
    }
}