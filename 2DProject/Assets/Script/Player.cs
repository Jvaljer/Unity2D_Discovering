using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    private float move;
    private float speed = 6f;
    private float jumpPower = 12f;
    private bool facingRight = true;

    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;

    //initializing stuff
    void Start(){
        //nothing needed here yer
    }

    //used to get all inputs & overall acting (once per frame)
    void Update(){
        move = Input.GetAxisRaw("Horizontal"); //return either -1 0 or 1 depending of how we are moving

        if( Input.GetButtonDown("Jump") && Grounded() )
            body.velocity = new Vector2(body.velocity.x, jumpPower);

        if( Input.GetButtonDown("Jump") && (body.velocity.y > 0f) )
            body.velocity = new Vector2(body.velocity.x, body.velocity.y * 0.5f);

        TurnAround();
    }

    //used to handle all kind of physics
    void FixedUpdate(){
        body.velocity = new Vector2(move * speed, body.velocity.y);
    }

    //Other Methods 
    private void TurnAround(){
        //Turns the 'looking' side of the player if moving left or right
        if( (facingRight && (move < 0f)) || (!facingRight && (move > 0f)) ){
            facingRight = !facingRight;
            Vector3 localScale = transform.localScale;
            localScale.x *= -1f;
            transform.localScale = localScale;
        }
    }

    private bool Grounded(){
        //creates a circle above our player's feet and allows it to jump if colliding groundLayer
        return Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
    }
}
