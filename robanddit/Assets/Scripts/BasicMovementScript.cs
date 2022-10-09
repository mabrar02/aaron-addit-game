using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class BasicMovementScript : MonoBehaviour //NetworkBehaviour 
{
    #region VARIABLES
    [Header("Run Variables")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float accelAmount;
    [SerializeField] private float deccelAmount;
    private float horizontalDir;

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce;
    private float verticalDir;
    private float gravityScale;
    [SerializeField] private float jumpCutGravityMult;
    [SerializeField] private float fallGravityMult;
    [SerializeField] public float coyoteTime;
    [SerializeField] private float maxFallSpeed;
    [SerializeField] private float jumpBuffer;


    [Header("Components")]
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Rigidbody2D rb;


    //States
    private bool isFacingRight;
    private bool isJumping;
    private bool isJumpCut;
    private bool isJumpFalling;

    //Timers
    private float lastOnGroundTime;
    private float lastJumpTime;
    #endregion



    // Start is called before the first frame update
    private void Start()
    {

        rb = GetComponent<Rigidbody2D>();
        isFacingRight = true;
        gravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    private void Update() {

        horizontalDir = getInput().x;
        verticalDir = getInput().y;
        if (horizontalDir != 0) {
            orientCharacter(horizontalDir > 0);
        }

        lastJumpTime -= Time.deltaTime;
        lastOnGroundTime -= Time.deltaTime;


        if (Input.GetKeyDown(KeyCode.Space)) {
            jumpDownInput();
        }
        if (Input.GetKeyUp(KeyCode.Space)) {
            jumpUpInput();
        }


        if (!isJumping) {
            if (Physics2D.OverlapBox(groundCheck.position, groundCheckSize, 0, groundLayer) && !isJumping) {
                lastOnGroundTime = coyoteTime;
            }
        }


        if(isJumping && rb.velocity.y < 0) {
            isJumping = false;
            isJumpFalling = true;
        }
        if(lastOnGroundTime > 0 && !isJumping) {
            isJumpCut = false;
            isJumpFalling = false;
        }
        if (isJumping && rb.velocity.y < 0) {
            isJumping = false;
            isJumpFalling = true;
        }
        if (lastOnGroundTime > 0 && !isJumping) {
            isJumpCut = false;
            if (!isJumping) {
                isJumpFalling = false;
            }
        }
        if (canJump() && lastJumpTime > 0) {
            isJumping = true;
            isJumpCut = false;
            isJumpFalling = false;
            Jump();

        }


        if (isJumpCut) {
            setGravityScale(gravityScale * jumpCutGravityMult);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));

        }
        else if (isJumping) {
            setGravityScale(gravityScale);
        }
        else if(rb.velocity.y < 0) {
            setGravityScale(gravityScale * fallGravityMult);
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFallSpeed));
        }
        else {
            setGravityScale(gravityScale);
        }
    }
        private Vector2 getInput() {

        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }



    private void FixedUpdate()
    {
        Run();

    }

    private void setGravityScale(float scale) {
        rb.gravityScale = scale;
    }
    private void Run() {
        float targetSpeed = horizontalDir * moveSpeed;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelAmount : deccelAmount;

        float speedDif = targetSpeed - rb.velocity.x;
        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }

    private void orientCharacter(bool isMovingRight) {
        if(isMovingRight != isFacingRight) {
            Vector3 scale = transform.localScale;
            scale.x *= -1;
            transform.localScale = scale;
            isFacingRight = !isFacingRight;
        }
    }

    public void jumpDownInput() {
        lastJumpTime = jumpBuffer;
    }
    public void jumpUpInput() {
        if (canJumpCut()) {
            isJumpCut = true;
        }
    }

    private void Jump() {
        lastJumpTime = 0;
        lastOnGroundTime = 0;

        float force = jumpForce;
        if(rb.velocity.y < 0) {
            force -= rb.velocity.y;
        }
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private bool canJump() {
        return lastOnGroundTime > 0 && !isJumping;
    }
    private bool canJumpCut() {
        return isJumping && rb.velocity.y > 0;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }

}
