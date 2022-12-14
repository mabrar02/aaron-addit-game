using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class hauntHighlight : MonoBehaviour
{
    private GameObject player;
    private AbilityScript abilityScript;
    private Color originalColour;
    private SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(getPlayer());
        originalColour = gameObject.GetComponent<SpriteRenderer>().color;
        sr = gameObject.GetComponent<SpriteRenderer>();    
    }

    private void OnMouseEnter() {
        if (player && abilityScript.inRange) {
            sr.color = abilityScript.HauntedColour;
        }
    }

    private void OnMouseExit() {
        if (player && (abilityScript.HauntedObject != gameObject || !abilityScript.inHauntObj)) {
            sr.color = originalColour;
        }     
    }

    private IEnumerator getPlayer() {      
        while(!player) {
            try {
                player = GameObject.FindGameObjectWithTag("Player");
                abilityScript = player.GetComponent<AbilityScript>();
            } catch {}      
            yield return null;
        }
    }
}
