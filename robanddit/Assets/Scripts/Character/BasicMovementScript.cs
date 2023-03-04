using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class BasicMovementScript : NetworkBehaviour 
{
    #region VARIABLES
    [Header("Run Variables")]
    [SerializeField] private float targetMoveSpeed    = 17;
    [SerializeField] private float accelAmount        = 13;
    [SerializeField] public float deccelAmount        = 16;
    public float horizontalDir;

    [Header("Jump Variables")]
    [SerializeField] private float jumpForce          = 25;
    [SerializeField] private float jumpCutGravityMult = 5;
    [SerializeField] private float fallGravityMult    = 4;
    [SerializeField] public  float coyoteTime         = 0.1f;
    [SerializeField] private float maxFallSpeed       = 30;
    [SerializeField] private float maxFloatSpeed      = 7;
    [SerializeField] private float jumpBuffer         = 0.15f;
    [SerializeField] private float floatGravityMult   = 0.5f;
    public int  jumpDir;
    private float verticalDir;
    private float gravityScale;

    [Header("Components")]
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.49f, 0.03f);
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundLayer;
    private Rigidbody2D rb;
    public  Animator anim;
    private Collider2D col;
    private AbilityScript hauntScript;

    //States
    public bool isFacingRight {get; set;}
    public bool isJumping     {get; set;}
    public bool isJumpCut     {get; set;}
    private bool playerOnPlatform;

    //Timers
    public float lastOnGroundTime;
    public float lastJumpTime;

    #endregion

    // Start is called before the first frame update
    public void Start()
    {
        hauntScript = GetComponent<AbilityScript>();
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();

        isFacingRight = true;
        gravityScale = rb.gravityScale;
    }

    // Update is called once per frame
    public void Update() {
        if (!IsOwner || !GameState.controlEnabled) return;

        if (!hauntScript.currentlyHaunting) {
            BasicMovement();
        }
    }
    
    public void FixedUpdate() {
        if (!IsOwner || !GameState.controlEnabled) return;

        if (!hauntScript.currentlyHaunting) {
            ApplyForce();
            PollJumpMechanic();
        }
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        setAnimControllerServerRpc(OwnerClientId, IsHost && IsOwner);

    }

    [ServerRpc]
    public void setAnimControllerServerRpc(ulong uid, bool Host)
    {
        setAnimControllerClientRpc(uid, Host);
    }

    [ClientRpc]
    public void setAnimControllerClientRpc(ulong uid, bool Host)
    {
        NetworkObject _player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid);
        if(Host)
        {
            _player.transform.GetChild(0).GetComponent<BasicMovementScript>().anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/TheHero", typeof(RuntimeAnimatorController ));
            _player.transform.GetChild(0).transform.Find("Sprite").GetComponent<SpriteRenderer>().material = (Material)Resources.Load("Materials/RobbyGlow", typeof(Material));
        } else {
            _player.transform.GetChild(0).GetComponent<BasicMovementScript>().anim.runtimeAnimatorController = (RuntimeAnimatorController)Resources.Load("Animations/TheHeroine", typeof(RuntimeAnimatorController ));
            _player.transform.GetChild(0).transform.Find("Sprite").GetComponent<SpriteRenderer>().material = (Material)Resources.Load("Materials/ClaireGlow", typeof(Material));
        }
    }



    //----------------------------------------------------------
    // Movement type methods
    //----------------------------------------------------------
    private void BasicMovement() {
        #region ANIMATION
        anim.SetBool("isGrounded", lastOnGroundTime > 0);
        anim.SetFloat("horizontalDirection", Mathf.Abs(horizontalDir));
        anim.SetBool("isJumping", jumpDir == 1);
        anim.SetBool("isFalling", jumpDir == -1);
        #endregion

        horizontalDir = Input.GetAxisRaw("Horizontal");
        verticalDir   = Input.GetAxisRaw("Vertical");
        jumpDir       = (Mathf.RoundToInt(rb.velocity.y) > 1) ? 1 : (Mathf.RoundToInt(rb.velocity.y) < -1) ? -1 : 0;

        // Orient sprite direction
        if (horizontalDir != 0) {
            orientCharacter(horizontalDir > 0);
        }

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
        if(playerOnPlatform && verticalDir < 0) {
            StartCoroutine(EnableCollider());
        }    

    }

    private void PollJumpMechanic() {
          // Base jump
        if (lastOnGroundTime > 0 && lastJumpTime > 0) {
            isJumpCut = false;
            Jump();
        } // Falling + W key pressed
        else if (jumpDir == -1 && verticalDir > 0) {
            rb.gravityScale = gravityScale * floatGravityMult;
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -maxFloatSpeed));
        } // JumpCut
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

    private void OnCollisionEnter2D(Collision2D collision) {
        if(collision.gameObject.CompareTag("Platform")) {
            playerOnPlatform = true;
        } else {
            playerOnPlatform = false;
        }
    }

    private IEnumerator EnableCollider() {      
        col.enabled = false;
        yield return new WaitForSeconds(0.25f);
        col.enabled = true;
    }


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
        rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);
    }

    private void ApplyForce() {
        float targetSpeed = horizontalDir * targetMoveSpeed;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelAmount : deccelAmount;

        float speedDif = targetSpeed - rb.velocity.x;
        float movement = speedDif * accelRate;

        rb.AddForce(movement * Vector2.right, ForceMode2D.Force);
    }



    //----------------------------------------------------------
    // Get and Set type methods
    //----------------------------------------------------------

    //----------------------------------------------------------
    // Miscellaneous methods
    //----------------------------------------------------------
    public void orientCharacter(bool isMovingRight) {
        if(isMovingRight != isFacingRight) {
            transform.GetChild(2).transform.Rotate(0f, 180f, 0f);
            isFacingRight = !isFacingRight;
        }
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(groundCheck.position, groundCheckSize);
    }

}
