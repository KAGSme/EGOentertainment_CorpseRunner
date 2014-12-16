using UnityEngine;
using System.Collections;

public class CharacterMotor : MonoBehaviour {

    public string player;
    public float maxSpeedX = 10f;
    public float airSpeedX = 30f;
	public float wallSlideSpeedForce = 3f;
    public float jumpForce = 20f;
    public float wallJumpForce = 20f;
	public float inverseGravity = 5;
	float currentControlX;
    bool _grounded;
    bool _walledR;
    bool _walledL;
    public bool canControlX = true;
	float inputX;
    public Transform groundCheck;
    public Transform wallCheckR;
    public Transform wallCheckL;
    public float groundRadius = 0.2f;
    public float wallRadius = 0.2f;
    public LayerMask whatIsGround;
    public LayerMask whatIsClimbableWall;

	// Use this for initialization
	void Start () 
    {
        if (player == "2")
        {
            rigidbody.useGravity = false;
            wallSlideSpeedForce = -wallSlideSpeedForce;
            jumpForce = -jumpForce;
            wallJumpForce = -wallJumpForce;
        }
        _grounded = false;
        _walledR = false;
        _walledL = false;
        canControlX = true;
	}

    void Update()
    {
        rigidbody.velocity = new Vector3(Mathf.Clamp(rigidbody.velocity.x, -maxSpeedX, maxSpeedX), rigidbody.velocity.y, 0);

        Jumping();
    }
	
	void FixedUpdate () 
    {
        BasicMotorMovement();
        WallSlide();
	}

    /// <summary>
    /// This controls the the basic horizontal movement of the character, 
    /// it uses the booleans _grounded to determine how movement should be dealt with
    /// </summary>
    void BasicMotorMovement()
    {
        if (player == "2")
        {
            rigidbody.AddForce(new Vector3(0, inverseGravity, 0));
        }

        // these booleans determine what the character is in contact with, it uses raycasts and layermasks to do this
        _grounded = Physics.CheckSphere(groundCheck.position, groundRadius, whatIsGround);
        _walledR = Physics.CheckSphere(wallCheckR.position, wallRadius, whatIsClimbableWall);
        _walledL = Physics.CheckSphere(wallCheckL.position, wallRadius, whatIsClimbableWall);

        inputX = Input.GetAxis("Horizontal"+player);
        currentControlX = inputX * maxSpeedX;

        if (canControlX && _grounded)
        {
            rigidbody.velocity = new Vector3(currentControlX, rigidbody.velocity.y, 0);
        }

        if (canControlX && !_grounded)
        {
            if (inputX < 0) rigidbody.AddForce(new Vector3(-10, 0, 0));
            if (inputX > 0) rigidbody.AddForce(new Vector3(10, 0, 0));
        }
    }

    /// <summary>
    /// this causes the player character to slide down when in contact with the wall
    /// </summary>
    void WallSlide()
    {
        if (_walledR || _walledL)
        {
            rigidbody.AddForce(new Vector3(0, -wallSlideSpeedForce, 0));
        }
    }

    /// <summary>
    /// This deals with the player character jumping as well as the input for it
    /// </summary>
    void Jumping()
    {
        if (Input.GetButtonDown("Jump" + player))
        {
            if (_grounded)
            {
                rigidbody.AddForce(new Vector3(0, jumpForce, 0));
            }

            if (_walledL)
            {
                rigidbody.AddForce(new Vector3(300, wallJumpForce, 0));
            }

            if (_walledR)
            {
                rigidbody.AddForce(new Vector3(-300, wallJumpForce, 0));
            }
        }
    }
}
