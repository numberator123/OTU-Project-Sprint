using UnityEngine;

public class FatDude
{
    public float Speed { get; }
    public float TargetY { get; }
    public float TargetX { get; }

    public FatDude(float speed, float targetX, float targetY)
    {
        Speed = speed;
        TargetX = targetX;
        TargetY = targetY;
    }

    public void MoveUp(Rigidbody2D body)
    {
        if (body.position.y < TargetY)
        {
            body.linearVelocity = Vector2.up * Speed;
        }
        else
        {
            Stop(body);
        }
    }

    public void MoveDown(Rigidbody2D body)
    {
        if (body.position.y > -TargetY)
        {
            body.linearVelocity = Vector2.down * Speed;
        }
        else
        {
            Stop(body);
        }
    }

    public void MoveLeft(Rigidbody2D body)
    {
        if (body.position.x > -TargetX)
        {
            body.linearVelocity = Vector2.left * Speed;
        }
        else
        {
            Stop(body);
        }
    }

    public void MoveRight(Rigidbody2D body)
    {
        if (body.position.x < TargetX)
        {
            body.linearVelocity = Vector2.right * Speed;
        }
        else
        {
            Stop(body);
        }
    }

    public void Stop(Rigidbody2D body)
    {
        body.linearVelocity = Vector2.zero;
    }
}