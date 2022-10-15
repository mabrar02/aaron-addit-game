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
   
    // Haunt related variables
    [SerializeField] private Vector2 hauntSize;
    [SerializeField] private float hauntDistance;
    public bool currentlyHaunting;
    private Collider2D HauntCollider;
    private GameObject HauntedObject;
    private Vector2 DirToHaunt;
    private float DistToHaunt;
    [SerializeField] private float HauntInSpeed = 10f;
    [SerializeField] private float HauntOutSpeed = 5f;

    private BasicMovementScript MoveScript;
    #endregion
   
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponent<Animator>();
        MoveScript = GetComponent<BasicMovementScript>();
        currentlyHaunting = false;
    }

    void Update() {
        /*
         * currentlyHaunting used to go from one haunt to another provided that distance is valid
         * if not, you launch yourself via click or space
         * 
         */
        anim.SetBool("currentlyHaunting", currentlyHaunting);

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (Input.GetButtonDown("Fire1") || Input.GetKeyDown(KeyCode.Space)  /*&& Vector2.Distance(transform.position,mousePos) <= hauntDistance*/) {
                checkHaunt();
        }


    }
    // Update is called once per frame

    void FixedUpdate()
    {

    }

    public void checkHaunt() {

        HauntCollider = Physics2D.OverlapBox(mousePos, hauntSize, 0);

        if (HauntCollider && HauntCollider.CompareTag("Hauntable")) {
            HauntedObject = HauntCollider.gameObject;
            currentlyHaunting = true;
            MoveScript.MovementEnabled = false;
            StartCoroutine(PerformHaunt(HauntedObject));
        } else {
            currentlyHaunting = false;
        }

    }

    private IEnumerator PerformHaunt (GameObject HauntObject) {
        /*
         * figure out dash/velocity shit
         * 
         */
        col.enabled = false;
        while(currentlyHaunting) {
            CalcDistToHaunt();
            CalcDirToHaunt();
            CalcDirToMouse();

            Debug.DrawRay(transform.position, DirToHaunt);
            Debug.DrawRay(transform.position, DirToMouse);
            Debug.Log(HauntCollider.gameObject.name);

            rb.velocity = new Vector2(DirToHaunt.x*HauntInSpeed, DirToHaunt.y*HauntInSpeed);
          
            yield return null;
        
        }
        rb.gravityScale = 0.5f;
        rb.AddForce(new Vector2(DirToMouse.x*HauntOutSpeed, DirToMouse.y*HauntOutSpeed), ForceMode2D.Impulse);

        currentlyHaunting = false;
        col.enabled = true;
        
        //Movement script messes with the transition out of haunt state
        yield return new WaitForSeconds(0.5f);
        MoveScript.MovementEnabled = true;
        yield return null;
    }

    //----------------------------------------------------------
    // Calculation methods 
    //----------------------------------------------------------
    private void CalcDirToHaunt() {
        DirToHaunt = (Vector2)HauntedObject.transform.position - (Vector2)transform.position;
    }

    private void CalcDirToMouse() {
        DirToMouse = mousePos - (Vector2)transform.position;
        DirToMouse = DirToMouse.normalized;
    }

    private void CalcDistToHaunt() {
        DistToHaunt = ((Vector2)HauntedObject.transform.position - (Vector2)transform.position).magnitude;
    }


    private void EnableMovement() {

    }
    //----------------------------------------------------------
    // Miscellaneous junk 
    //----------------------------------------------------------
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(mousePos, hauntSize);
    }
}
