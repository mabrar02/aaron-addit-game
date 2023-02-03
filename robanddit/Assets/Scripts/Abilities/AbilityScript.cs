using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{

    #region VARIABLES
    private Vector2 mousePos = new Vector2();         //Absolute mouse position
    private Vector2 DirToMouse = new Vector2(); //Mouse position relative to player's transform
    //Player related variables
    private Rigidbody2D rb;
    private Collider2D col;
    private Animator anim;
    private GameObject arrow;
    public Color HauntedColour;
    private Color originalColour;
   
    // Haunt related variables
    [SerializeField] public Vector2 hauntSize;
    [SerializeField] private float hauntDistance;
    public bool currentlyHaunting;
    public bool inHauntObj;
    private Collider2D HauntCollider;
    [HideInInspector] public GameObject HauntedObject;
    private Vector2 DirToHaunt;
    private float DistToHaunt;
    [SerializeField] private float HauntInSpeed = 10f;
    [SerializeField] private float HauntOutSpeed = 5f;
    [SerializeField] private float hauntDuration = 1.5f;
    private float hauntReset;
    [HideInInspector] public bool inRange;

    private BasicMovementScript MoveScript;
    #endregion
   
    // Start is called before the first frame update
    void Start()
    {
        hauntReset = hauntDuration;
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        MoveScript = GetComponent<BasicMovementScript>();
        currentlyHaunting = false;
        inHauntObj = false;
        arrow = transform.Find("arrow").gameObject;
    }

    void Update() {

        anim.SetBool("currentlyHaunting", currentlyHaunting);
        anim.SetBool("inHauntObj", inHauntObj);

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        inRange = Vector2.Distance(transform.position, mousePos) <= hauntDistance;

        

        if (!currentlyHaunting && !inHauntObj && Input.GetButtonDown("Fire1") && inRange) {
             checkHauntIn();
        }
        else if(!currentlyHaunting && inHauntObj && ((Input.GetButtonDown("Fire1")) || (Input.GetKeyDown(KeyCode.Space)))) {
            checkHauntOut();
        }
        if (inHauntObj) {
            arrow.SetActive(true);
            arrow.transform.position = HauntedObject.transform.position + new Vector3(3,3,0);
        }
        else {
            arrow.SetActive(false);
        }

    }

    public void checkHauntIn() {

        HauntCollider = Physics2D.OverlapBox(mousePos, hauntSize, 0);
        if (HauntCollider && HauntCollider.CompareTag("Hauntable")) {
            HauntedObject = HauntCollider.gameObject;
            originalColour = HauntedObject.GetComponent<SpriteRenderer>().color;

            StartCoroutine(PerformHauntIn(HauntedObject));
        }
    }

    private IEnumerator PerformHauntIn(GameObject HauntObject) {
        currentlyHaunting = true;
        MoveScript.enabled = false;
        col.enabled = false;

        CalcDistToHaunt();
        CalcDirToHaunt();
        CalcDirToMouse();

        Debug.DrawRay(transform.position, DirToHaunt);
        Debug.DrawRay(transform.position, DirToMouse);
        Debug.Log(HauntCollider.gameObject.name);

        Vector3 direction = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        Vector2 hauntVel = new Vector2(DirToHaunt.x, DirToHaunt.y) * HauntInSpeed;

        while (DistToHaunt >= 3) {
            rb.velocity = hauntVel;
            CalcDistToHaunt();
            yield return null;
        }

        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        transform.position = HauntObject.transform.position;
        inHauntObj = true;
        currentlyHaunting = false;
        HauntedObject.tag = "Haunted";
        HauntedObject.GetComponent<SpriteRenderer>().color = HauntedColour;

        yield return null;

    }

    public void checkHauntOut() {
        rb.bodyType = RigidbodyType2D.Dynamic;
        col.enabled = true;

        HauntCollider = Physics2D.OverlapBox(mousePos, hauntSize, 0);
        HauntedObject.tag = "Hauntable";
        HauntedObject.GetComponent<SpriteRenderer>().color = Color.white;

        if (HauntCollider && HauntCollider.CompareTag("Hauntable") && Vector2.Distance(transform.position, mousePos) <= hauntDistance) {
            HauntedObject = HauntCollider.gameObject;
            StartCoroutine(PerformHauntIn(HauntedObject));
        }
        else {
            StartCoroutine(PerformHauntOut());
        }
        
    }



    private IEnumerator PerformHauntOut() {
        currentlyHaunting = true;
        inHauntObj = false;
        
       
        CalcDirToMouse();

        Vector3 direction = Input.mousePosition - Camera.main.WorldToScreenPoint(transform.position);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
        MoveScript.enabled = true;

        while (currentlyHaunting) {
            if(hauntDuration > 0) {
                hauntDuration -= Time.deltaTime;
                rb.velocity = DirToMouse * HauntOutSpeed;
            }
            else {
                currentlyHaunting = false;
                hauntDuration = hauntReset;
                rb.velocity = new Vector2(rb.velocity.x, DirToMouse.y);
                transform.rotation = Quaternion.identity;
                MoveScript.isFacingRight = true;
                if (DirToMouse.x >= 0) {
                    MoveScript.orientCharacter(true);
                }
                else {
                    MoveScript.orientCharacter(false);
                }
            }
            
            yield return null;
        }

        
        

        MoveScript.isJumpCut = false;
        MoveScript.isJumping = false;
        MoveScript.isJumpFalling = true;
        Debug.Log(MoveScript.isFacingRight);

        yield return null;
    }




    //----------------------------------------------------------
    // Calculation methods 
    //----------------------------------------------------------
    private void CalcDirToHaunt() {
        DirToHaunt = (Vector2)HauntedObject.transform.position - (Vector2)transform.position;
        DirToHaunt = DirToHaunt.normalized;
    }

    private void CalcDirToMouse() {
        DirToMouse = mousePos - (Vector2)transform.position;
        DirToMouse = DirToMouse.normalized;
    }

    private void CalcDistToHaunt() {
        DistToHaunt = ((Vector2)HauntedObject.transform.position - (Vector2)transform.position).magnitude;
    }


    //----------------------------------------------------------
    // Miscellaneous junk 
    //----------------------------------------------------------
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(mousePos, hauntSize);
    }
}
