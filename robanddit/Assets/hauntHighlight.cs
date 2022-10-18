using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hauntHighlight : MonoBehaviour { 

    private Vector2 mousePos = new Vector2();
    private Collider2D HauntCollider;
    private AbilityScript hauntScript;
    private GameObject hauntedObject;

    private void Start() {
        hauntScript = GetComponent<AbilityScript>();
    }
    void FixedUpdate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        HauntCollider = Physics2D.OverlapBox(mousePos, hauntScript.hauntSize, 0);

        if (HauntCollider && HauntCollider.CompareTag("Hauntable") && hauntScript.inRange && !hauntScript.currentlyHaunting) {
            hauntedObject = HauntCollider.gameObject;
            hauntedObject.GetComponent<SpriteRenderer>().color = Color.red;
        }
        else if(hauntedObject) {
            hauntedObject.GetComponent<SpriteRenderer>().color = Color.white;
        }

    }
}
