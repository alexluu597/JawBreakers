using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField]
    GameObject head, body;

    [SerializeField]
    int doubleJump, maxDoubleJump;

    [SerializeField]
    float runSpeed, walkSpeed, airSpeed, fullGroundJumpForce, shortGroundJumpForce, airJumpForce;

    [SerializeField]
    float movementSpeed;

    Vector3 direction;

    Rigidbody2D rbody;

    [SerializeField]
    Vector2 moveValue;

    private void Start()
    {
        rbody = GetComponent<Rigidbody2D>();
        if(transform.localEulerAngles.y == 0)
        {
            direction.x = 1;
        }
        else if(transform.localEulerAngles.y == 180)
        {
            direction.x = -1;
        }
    }

    private void FixedUpdate()
    {
        transform.Translate(Vector3.right * Time.fixedDeltaTime * movementSpeed * direction.x * moveValue);
    }

    public void Move(InputAction.CallbackContext context)
    {
        moveValue = context.ReadValue<Vector2>();
    }

    public void Move()
    {
        transform.Translate(Vector3.right * Time.deltaTime * movementSpeed * direction.x);
    }

    public void Run()
    {
        movementSpeed = runSpeed;
    }

    public void Walk()
    {
        movementSpeed = walkSpeed;
    }

    public void Air()
    {
        movementSpeed = airSpeed;
    }

    public void Stop()
    {
        movementSpeed = 0;
    }

    public void RefreshDoubleJump()
    {
        doubleJump = 0;
    }

    public void Turn()
    {
        if(moveValue.x < 0)
        {
            transform.localEulerAngles = new Vector3(0, 180, 0);
            direction.x = -1;
        }
        else if(moveValue.x > 0)
        {
            transform.localEulerAngles = new Vector3(0, 0, 0);
            direction.x = 1;
        }
    }

    public void TurnLeft()
    {
        transform.localEulerAngles = new Vector3(0, 180, 0);
        direction.x = -1;
    }

    public void TurnRight()
    {
        transform.localEulerAngles = new Vector3(0, 0, 0);
        direction.x = 1;
    }

    public void GroundJump()
    {
        rbody.AddForce(Vector3.up * fullGroundJumpForce, ForceMode2D.Impulse);
    }

    public void DoubleJump()
    {
        if (doubleJump < maxDoubleJump)
        {
            rbody.AddForce(Vector3.up * airJumpForce, ForceMode2D.Impulse);
            doubleJump++;
        }
    }

    public void Crouch()
    {
        body.transform.localScale = new Vector3(1, 0.5f, 1);
    }

    public void Stand()
    {
        body.transform.localScale = new Vector3(1, 1, 1);
    }

    public void LookUp()
    {
        head.transform.rotation = Quaternion.Euler(0, 0, 90);
    }

    public void LookForward()
    {
        head.transform.rotation = transform.rotation;
    }
}