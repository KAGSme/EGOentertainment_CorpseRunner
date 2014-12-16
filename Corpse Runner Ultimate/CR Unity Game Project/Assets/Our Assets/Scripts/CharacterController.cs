using UnityEngine;
using System.Collections;

public class CharacterController : MonoBehaviour {
	
    public float maxSpeedX = 10f;
    public float airSpeedX = 30f;
	public float wallSlideSpeed = 3f;
    public float jumpForce = 20f;
    public float wallJumpForce = 20f;
    public float wallJumpTime = 1f;
	public float defaultGravity = 5;
	float currentControlX;
    public bool _grounded;
    public bool _walledR;
    public bool _walledL;
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
	}

    /// <summary>
    /// This controls the the basic horizontal movement of the character, 
    /// it uses the booleans _grounded to determine how movement should be dealt with
    /// </summary>
    void BasicMotorMovement()
    {
        // these booleans determine what the character is in contact with, it uses raycasts and layermasks to do this
        _grounded = Physics.CheckSphere(groundCheck.position, groundRadius, whatIsGround);
        _walledR = Physics.CheckSphere(wallCheckR.position, wallRadius, whatIsClimbableWall);
        _walledL = Physics.CheckSphere(wallCheckL.position, wallRadius, whatIsClimbableWall);

        inputX = Input.GetAxis("Horizontal");
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

    void Jumping()
    {
        if (_grounded && Input.GetButton("Jump"))
        {
            rigidbody.AddForce(new Vector3(0, jumpForce, 0));
        }
    }
}
