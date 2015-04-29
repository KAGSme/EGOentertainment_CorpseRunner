using UnityEngine;
using System.Collections;

public class CharacterMotor : CommonBehaviour {

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

   


    //!cache things is your friend
    Rigidbody2D Body;  //todo - change addForces to just vector2's
    PointListSource Mid;

    void Awake() {
        base.Awake();
        Body = rigidbody2D;

        var go =  GameObject.Find("Mid");
        if(go) {
            Mid = go.GetComponent<Middle>().Src;
            Mid.Trnsfrm = go.transform;
        }
    }

    void Start() {
        if(player == "2") {
            //Body.useGravity = false;
       //     wallSlideSpeedForce = -wallSlideSpeedForce;
        //    jumpForce = -jumpForce;
     //       wallJumpForce = -wallJumpForce;
        }
        _grounded = false;
        _walledR = false;
        _walledL = false;
        canControlX = true;

        Rot = Body.rotation;
    }

    void Update() {   //?  why this not in fixed update?
        Body.velocity = new Vector2(Mathf.Clamp(Body.velocity.x, -maxSpeedX, maxSpeedX), Body.velocity.y);

        Jumping();
    }

    void FixedUpdate() {
        BasicMotorMovement();
        WallSlide();
    }


     [HideInInspector]
    //these are updated in BasicMotorMovement
    public  Vector2 xAx = Vector2.right, yAx = Vector2.up;  //todo for player2 intialise these correct
    float Rot;

    /// <summary>
    /// This controls the the basic horizontal movement of the character, 
    /// it uses the booleans _grounded to determine how movement should be dealt with
    /// </summary>
    /// 
    void BasicMotorMovement() {

        var gravD = new Vector2(0, -1.0f);
        if(player == "2") {  //! string checks are bad
            gravD *= -1;
        }
        if(Mid != null)
            Mid.gravMod(ref gravD, Body.position );
        Body.AddForce(gravD*9.8f);

        Body.MoveRotation(Rot = Mathf.LerpAngle(Rot, Mathf.Atan2(gravD.x, -gravD.y) * Mathf.Rad2Deg, 4.0f * Time.deltaTime));

        if(player == "2") {  //! string checks are bad
       //     xAx = -xAx;
        //    yAx = -yAx;
        }
        xAx = new Vector2(-gravD.y, gravD.x);
        yAx = -gravD;


        var ret = new Collider2D[1]; 
        // these booleans determine what the character is in contact with, it uses raycasts and layermasks to do this
        _grounded = Physics2D.OverlapCircleNonAlloc(groundCheck.position, groundRadius, ret, whatIsGround) != 0;
        _walledR = Physics2D.OverlapCircleNonAlloc(wallCheckR.position, wallRadius, ret, whatIsClimbableWall) != 0;
        _walledL = Physics2D.OverlapCircleNonAlloc(wallCheckL.position, wallRadius, ret, whatIsClimbableWall) != 0;

        inputX = Input.GetAxis("Horizontal" + player);
        currentControlX = inputX * maxSpeedX;

        if(canControlX && _grounded) {


            Body.velocity = currentControlX * xAx + Vector2.Dot(Body.velocity, yAx)*yAx;
        }

        if(canControlX && !_grounded) {
            float flyForce = 10, airSpeed = maxSpeedX;

            float spdMd = Vector2.Dot(Body.velocity, xAx * Mathf.Sign( inputX ) ) / airSpeed;
            spdMd *= Mathf.Abs( spdMd );
            flyForce *= Mathf.Clamp01(1.0f - spdMd );

            if(inputX < 0) Body.AddForce(xAx * -flyForce);
            if(inputX > 0) Body.AddForce(xAx * flyForce);
           
        }
    }

    /// <summary>
    /// this causes the player character to slide down when in contact with the wall
    ///! can modify gravity instead 
    /// </summary>
    void WallSlide() {
        if(_walledR || _walledL) {
            Body.AddForce( yAx * -wallSlideSpeedForce );
        }
    }

    /// <summary>
    /// This deals with the player character jumping as well as the input for it
    /// </summary>
    void Jumping() {
        if(Input.GetButtonDown("Jump" + player)) {
            if(_grounded) {
                Body.AddForce( yAx * jumpForce );
            }

            if(_walledL) {
                Body.AddForce(xAx * 300 + yAx * wallJumpForce);
            }

            if(_walledR) {
                Body.AddForce(xAx*-300 + yAx * wallJumpForce);
            }
        }
    }
}
