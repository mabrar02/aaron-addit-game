using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{

    [SerializeField] private Vector2 hauntSize;
    [SerializeField] private float hauntDistance;
    private Vector2 mousePos = new Vector2();
    private Vector2 relativeMousePos = new Vector2();
    private RaycastHit2D hitData;
    private bool currentlyHaunting;


   
    // Start is called before the first frame update
    void Start()
    {
        currentlyHaunting = false;
        
    }

    void Update() {
        /*
         * currentlyHaunting used to go from one haunt to another provided that distance is valid
         * if not, you launch yourself via click or space
         * 
         */
        if (Input.GetButtonDown("Fire1") && !currentlyHaunting && Vector2.Distance(transform.position,mousePos) <= hauntDistance) {
            checkHaunt();
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        relativeMousePos = mousePos - (Vector2)transform.position;
        hitData = Physics2D.Raycast(transform.position, relativeMousePos, 5);

    }

    public void checkHaunt() {

        Collider2D collision = Physics2D.OverlapBox(mousePos, hauntSize, 0);
        if (collision && collision.CompareTag("Hauntable")) {
            Debug.Log(collision.gameObject.name);
            StartCoroutine(performHaunt(collision.gameObject));
        }

        /*if (hitData && hitData.collider.CompareTag("Hauntable")) {
            Debug.Log(hitData.collider.gameObject.name);
            StartCoroutine(performHaunt(hitData.collider.gameObject));
        }*/
    }
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(mousePos, hauntSize);
    }

    private IEnumerator performHaunt (GameObject hauntObject) {
        /*
         * figure out dash/velocity shit
         * 
         */
        Color originalColour = hauntObject.GetComponent<SpriteRenderer>().color;
        hauntObject.GetComponent<SpriteRenderer>().color = Color.red;
        hauntObject.tag = "Haunted";
        currentlyHaunting = true;
        yield return new WaitForSeconds(2f);
        hauntObject.GetComponent<SpriteRenderer>().color = originalColour;
        hauntObject.tag = "Hauntable";
        currentlyHaunting = false;
    }
}
