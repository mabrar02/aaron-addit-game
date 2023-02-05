using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntMovementScript : MonoBehaviour 
{
    #region VARIABLES
    [Header("Run Variables")]
    [SerializeField] private float targetMoveSpeed    = 17; // Limit on horizontal velocity
    [SerializeField] private float accelAmount        = 13; // How fast velocity changes to targetMoveSpeed
    [SerializeField] public  float deccelAmount       = 16; // How fast velocity slows down at no input 
    private float horizontalDir;

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce          = 125;
    [SerializeField] private float jumpCutGravityMult = 5;
    [SerializeField] private float fallGravityMult    = 4;
    [SerializeField] public  float coyoteTime         = 0.1f;
    [SerializeField] private float maxFallSpeed       = 30;
//    [SerializeField] private float maxFloatSpeed      = 7;
    [SerializeField] private float jumpBuffer         = 0.15f;
//    [SerializeField] private float floatGravityMult   = 0.5f;
    [HideInInspector] public int  jumpDir;
    private float verticalDir;
    private float gravityScale = 1;

    [Header("Components")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D col;
    

    //States
    public bool isFacingRight {get; set;}
    public bool isJumping     {get; set;}
    public bool isJumpCut     {get; set;}
    private bool doneKnockback = false; 
    private bool playerOnPlatform;

    //Timers
    public float lastOnGroundTime;
    public float lastJumpTime;

    #endregion

    // Start is called before the first frame update
    public void Start()
    {
        col = GetComponent<Collider2D>();
        gravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    public void Update() {
        BasicMovement();
    }
    
    public void FixedUpdate() {
        ApplyForce();
        PollJumpMechanic();
    }

   
    //----------------------------------------------------------
    // Movement type methods
    //----------------------------------------------------------
    private void BasicMovement() {

        horizontalDir = Input.GetAxisRaw("Horizontal");
        verticalDir   = Input.GetAxisRaw("Vertical");
        jumpDir       = (Mathf.RoundToInt(rb.velocity.y) > 0) ? 1 : (Mathf.RoundToInt(rb.velocity.y) < 0) ? -1 : 0;

        // Orient sprite direction
//        if (horizontalDir != 0) {
//            orientCharacter(horizontalDir > 0);
//        }

        lastJumpTime -= Time.deltaTime;
        lastOnGroundTime -= Time.deltaTime;
      
        if (Input.GetKeyDown(KeyCode.Space)) {
            isJumpCut = false;
            lastJumpTime = jumpBuffer;
        }

        if (Input.GetKeyUp(KeyCode.Space)) {
            if(jumpDir == 1) isJumpCut = true;
        }

        // Reset lastOnGroundTime when touching ground
        if (jumpDir == 0) {
            if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer)) {
                lastOnGroundTime = coyoteTime;
            }
        }
        // Pass through "Platform" tags
//        if(playerOnPlatform && verticalDir < 0) {
//            StartCoroutine(EnableCollider());
//        }    

    }

    private void PollJumpMechanic() {
          // Base jump
        if (lastOnGroundTime > 0 && lastJumpTime > 0) {
            isJumpCut = false;
            Jump();
        } // Falling + W key pressed
//        else if (jumpDir == -1 && verticalDir > 0) {
//            rb.gravityScale = gravityScale * floatGravityMult;
//            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFloatSpeed));
//        } // JumpCut
        else if (isJumpCut && jumpDir != 0) {
            rb.gravityScale = gravityScale * jumpCutGravityMult;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        } // Jumping  
        else if (jumpDir == 1) {
            rb.gravityScale = gravityScale;
        } // Falling 
        else if (rb.velocity.y < 0) {
            rb.gravityScale = gravityScale * fallGravityMult;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        } // Default conditions
        else {
            rb.gravityScale = gravityScale;
        }

    }

//    private void OnCollisionEnter2D(Collision2D collision) {
//        if(collision.gameObject.CompareTag("Platform")) {
//            playerOnPlatform = true;
//        } else {
//            playerOnPlatform = false;
//        }
//    }

//    private IEnumerator EnableCollider() {      
//        col.enabled = false;
//        yield return new WaitForSeconds(0.25f);
//        col.enabled = true;
//    }


    //----------------------------------------------------------
    // Physics related 
    //----------------------------------------------------------
    private void Jump() {
        lastJumpTime = 0;
        lastOnGroundTime = 0;

        float force = jumpForce;
        if(rb.velocity.y < 0) {
            force -= rb.velocity.y;
        }
        rb.AddForce((Vector2.up * force)*rb.mass, ForceMode2D.Impulse);
    }

    private void ApplyForce() {
        float targetSpeed = horizontalDir * targetMoveSpeed;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelAmount : deccelAmount;

        float speedDif = targetSpeed - rb.velocity.x;
        float movement = speedDif * accelRate;

        rb.AddForce((movement * Vector2.right)*rb.mass, ForceMode2D.Force);
    }



    //----------------------------------------------------------
    // Get and Set type methods
    //----------------------------------------------------------

    //----------------------------------------------------------
    // Miscellaneous methods
    //----------------------------------------------------------
//    public void orientCharacter(bool isMovingRight) {
//        if(isMovingRight != isFacingRight) {
//            transform.Rotate(0f, 180f, 0f);
//            isFacingRight = !isFacingRight;
//        }
//    }

//    private void OnMouseEnter() {
//        if (abilityScript.inRange) {
//            sr.color = abilityScript.HauntedColour;
//        }
//    }
//
//    private void OnMouseExit() {
//        if (abilityScript.HauntedObject != gameObject || !abilityScript.inHauntObj)) {
//            sr.color = originalColour;
//        }     
//    }

    public void knockBackForce(Vector2 playerVector, bool hauntIn) {    
        if(rb) {
            switch(hauntIn) {
                case true  : rb.AddForce( new Vector2(playerVector.x*500, 500), ForceMode2D.Impulse);
                        break;
                case false : rb.AddForce( new Vector2(-playerVector.x*500, 500), ForceMode2D.Impulse);
                        break;
            }      
        } else {
            rb = GetComponent<Rigidbody2D>();
            switch(hauntIn) {
                case true  : rb.AddForce( new Vector2(playerVector.x*500, 500), ForceMode2D.Impulse);
                        break;
                case false : rb.AddForce( new Vector2(-playerVector.x*500, 500), ForceMode2D.Impulse);
                        break;
            }
        }
    }


//    private void OnDrawGizmosSelected() {
//        Gizmos.color = Color.green;
//        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
//    }

}
