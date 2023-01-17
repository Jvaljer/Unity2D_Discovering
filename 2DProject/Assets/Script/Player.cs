using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private float move;
    private float speed = 6f;
    private float jumpPower = 15f;
    private bool doubleJump;
    private float doubleJumpPower = 15f;
    private bool facingRight = true;

    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    private bool wallSliding;
    private float wallSlideSpeed = 2f;
    private bool wallJumping;
    private float wallJumpDirection;
    private float wallJumpTime = 0.2f;
    private float wallJumpCount;
    private float wallJumpCooldown = 0.4f;
    private Vector2 wallJumpPower = new Vector2(6f,15f);

    [SerializeField] private Transform wallCheck;
    [SerializeField] private LayerMask wallLayer;


    [SerializeField] private Transform hitBox;
    [SerializeField] private LayerMask hitLayer;

    //initializing stuff
    void Start(){
        //nothing needed here yer
    }

    //used to get all inputs & overall acting (once per frame)
    void Update(){
        move = Input.GetAxisRaw("Horizontal"); //return either -1 0 or 1 depending of how we are moving

        if( Input.GetButtonDown("Jump") ){
            if(Grounded()){
                body.velocity = new Vector2(body.velocity.x, jumpPower);
                doubleJump = true;
            }

            if(!Grounded() && doubleJump){
                body.velocity = new Vector2(body.velocity.x, doubleJumpPower);
                doubleJump = false;
            }
        }

        if( Input.GetButtonDown("Jump") && (body.velocity.y > 0f) ){
            body.velocity = new Vector2(body.velocity.x, body.velocity.y * 0.5f);
        }
        
        WallSlide();
        WallJump();
        if(!wallJumping)
            TurnAround();

        if(Hitting())
            Debug.Log("Hitting on something");
    }

    //used to handle all kind of physics
    void FixedUpdate(){
        if(!wallJumping)
            body.velocity = new Vector2(move * speed, body.velocity.y);
    }

    //Other Methods 
    private bool Grounded(){
        //creates a circle above our player's feet and allows it to jump if colliding groundLayer
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }

    private bool Walled(){
        //similar to Grounded() (same principe) but with walls (layered correctly)
        return Physics2D.OverlapCircle(wallCheck.position, 0.2f, wallLayer);
    }

    private void TurnAround(){
        //Turns the 'looking' side of the player if moving left or right
        if( (facingRight && (move < 0f)) || (!facingRight && (move > 0f)) ){
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private void WallSlide(){
        if( Walled() && !Grounded() && (move != 0f) ){
            wallSliding = true;
            body.velocity = new Vector2(body.velocity.x, Mathf.Clamp(body.velocity.y, -wallSlideSpeed, float.MaxValue));
        } else {
            wallSliding = false;
        }
    }

    private void WallJump(){
        if(wallSliding){
            wallJumping = false;
            wallJumpDirection = -transform.localScale.x;
            wallJumpCount = wallJumpTime;
            CancelInvoke(nameof(StopWallJump));
        } else {
            wallJumpCount -= Time.deltaTime;
        }

        if(Input.GetButtonDown("Jump") && (wallJumpCount > 0f) ){
            wallJumping = true;
            body.velocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpCount = 0f;
            if(transform.localScale.x != wallJumpDirection){
                facingRight = !facingRight;
                Vector3 localScale = transform.localScale;
                localScale.x *= -1f;
                transform.localScale = localScale;
            }
        }

        Invoke(nameof(StopWallJump),wallJumpCooldown);
    }

    private void StopWallJump(){
        //here we cannot simply call this line in the wanted program as a command because we are gonna use 'invoke' which takes a METHOD as argument...
        wallJumping = false;
    }

    private bool Hitting(){
        return Physics2D.OverlapCircle(hitBox.position, 0.2f,hitLayer);
    }
}
