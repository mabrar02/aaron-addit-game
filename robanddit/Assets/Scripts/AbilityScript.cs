using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityScript : MonoBehaviour
{

    Vector2 mousePos = new Vector2();
    Vector2 relativeMousePos = new Vector2();
    RaycastHit2D hitData;
    bool currentlyHaunting;


   
    // Start is called before the first frame update
    void Start()
    {
        currentlyHaunting = false;
        
    }

    void Update() {
        if (Input.GetButtonDown("Fire1") && !currentlyHaunting) {
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
        if (hitData && hitData.collider.CompareTag("Hauntable")) {
            Debug.Log(hitData.collider.gameObject.name);
            var haunt = performHaunt(hitData.collider.gameObject);
            StartCoroutine(performHaunt(hitData.collider.gameObject));
        }
    }

    private IEnumerator performHaunt (GameObject hauntObject) {
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
