using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{

    #region VARIABLES
    private Vector2 mousePos = new Vector2();         //Absolute mouse position
    private Vector2 relativeMousePos = new Vector2(); //Mouse position relative to player's transform
    //Player related variables
    private Rigidbody2D rb;
    private Collider2D col;
   
    // Haunt related variables
    [SerializeField] private Vector2 hauntSize;
    [SerializeField] private float hauntDistance;
    private bool currentlyHaunting;
    private Collider2D HauntCollider;
    private GameObject HauntedObject;
    private bool DoneHaunt;
    private Vector2 DirToHaunt;
    [SerializeField] private float HauntWooshSpeed = 5f;
    #endregion
   
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        currentlyHaunting = false;
        DoneHaunt = false;    
    }

    void Update() {
        /*
         * currentlyHaunting used to go from one haunt to another provided that distance is valid
         * if not, you launch yourself via click or space
         * 
         */
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        relativeMousePos = mousePos - (Vector2)transform.position;

        if (Input.GetButtonDown("Fire1")  /*&& Vector2.Distance(transform.position,mousePos) <= hauntDistance*/) {
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
            Debug.Log(HauntCollider.gameObject.name);
            StartCoroutine(PerformHaunt(HauntedObject));
        }

    }
    private void CalcDirToHaunt() {
        DirToHaunt = HauntedObject.transform.position - transform.position;
    }

    private IEnumerator PerformHaunt (GameObject HauntObject) {
        /*
         * figure out dash/velocity shit
         * 
         */
        col.enabled = false;
        while(!DoneHaunt) {
            CalcDirToHaunt();
            Debug.DrawRay(transform.position, DirToHaunt);

            rb.velocity = new Vector2(DirToHaunt.x*HauntWooshSpeed, DirToHaunt.y*HauntWooshSpeed);

            if(Input.GetKeyDown(KeyCode.Space)) {
            DoneHaunt = true;
            Color originalColour = HauntObject.GetComponent<SpriteRenderer>().color;
            HauntObject.GetComponent<SpriteRenderer>().color = Color.blue;
            HauntObject.tag = "Hauntable";
            currentlyHaunting = false;
            yield return null;

            } else { 
            Color originalColour = HauntObject.GetComponent<SpriteRenderer>().color;
            HauntObject.GetComponent<SpriteRenderer>().color = Color.red;
            HauntObject.tag = "Haunted";
            currentlyHaunting = true;
            yield return null;

            }
        
        }

        DoneHaunt = false;
        Debug.Log(DirToHaunt);
        col.enabled = true;
    }

//    private void 


    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(mousePos, hauntSize);
    }
}
