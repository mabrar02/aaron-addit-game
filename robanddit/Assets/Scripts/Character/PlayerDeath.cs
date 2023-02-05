using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{

    private Rigidbody2D rb;
    private SpriteRenderer sr;
    public Transform respawnPoint;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Hazard")) {
            rb.simulated = false;
            sr.enabled = false;
            gameObject.GetComponent<AbilityScript>().enabled = false;
            gameObject.GetComponent<BasicMovementScript>().enabled = false;
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn() {
        yield return new WaitForSeconds(2);
        transform.position = respawnPoint.position;

        rb.simulated = true;
        sr.enabled = true;

        gameObject.GetComponent<BasicMovementScript>().enabled = true;
        gameObject.GetComponent<AbilityScript>().enabled = true;
    }
}
