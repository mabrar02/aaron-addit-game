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
    public bool inHauntObj;
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
        inHauntObj = false;
    }

    void Update() {

        anim.SetBool("currentlyHaunting", currentlyHaunting);

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        if (!currentlyHaunting && !inHauntObj && Input.GetButtonDown("Fire1") && Vector2.Distance(transform.position,mousePos) <= hauntDistance) {
             checkHauntIn();
        }
        else if(!currentlyHaunting && inHauntObj && ((Input.GetButtonDown("Fire1")) || (Input.GetKeyDown(KeyCode.Space)))) {
            checkHauntOut();
        }


    }

    public void checkHauntIn() {

        HauntCollider = Physics2D.OverlapBox(mousePos, hauntSize, 0);

        if (HauntCollider && HauntCollider.CompareTag("Hauntable")) {
            HauntedObject = HauntCollider.gameObject;

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

        Vector2 hauntVel = new Vector2(DirToHaunt.x * HauntInSpeed, DirToHaunt.y * HauntInSpeed);

        while (DistToHaunt >= 2) {
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

        yield return null;

    }

    public void checkHauntOut() {
        rb.bodyType = RigidbodyType2D.Dynamic;
        col.enabled = true;

        HauntCollider = Physics2D.OverlapBox(mousePos, hauntSize, 0);
        HauntedObject.tag = "Hauntable";

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
        MoveScript.enabled = true;
        CalcDirToMouse();

        MoveScript.setGravityScale(MoveScript.gravityScale);
        Vector2 hauntPower = DirToMouse;
        Debug.Log(hauntPower);
        rb.AddForce(hauntPower * HauntOutSpeed , ForceMode2D.Impulse);


        yield return new WaitForSeconds(1f);
        currentlyHaunting = false;
        
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


    //----------------------------------------------------------
    // Miscellaneous junk 
    //----------------------------------------------------------
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(mousePos, hauntSize);
    }
}
