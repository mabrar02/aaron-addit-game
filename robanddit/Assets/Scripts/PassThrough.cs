using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassThrough : MonoBehaviour
{

    private Collider2D col;
    private bool playerOnPlatform;
    private BasicMovementScript player;
    // Start is called before the first frame update
    void Start()
    {
        col = GetComponent<Collider2D>();
        
    }
    private void Update() {
        if(playerOnPlatform && Input.GetAxisRaw("Vertical") < 0) {
            col.enabled = false;
            StartCoroutine(EnableCollider());
        }
    }
    private IEnumerator EnableCollider() {
        
        yield return new WaitForSeconds(0.25f);
        col.enabled = true;
    }

    private void SetPlayerOnPlatform(Collision2D other, bool val) {
        player = other.gameObject.GetComponent<BasicMovementScript>();
        if(player != null) {
            playerOnPlatform = val;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        SetPlayerOnPlatform(collision, true);
    }

    private void OnCollisionExit2D(Collision2D collision) {
        SetPlayerOnPlatform(collision, false);
    }
}
