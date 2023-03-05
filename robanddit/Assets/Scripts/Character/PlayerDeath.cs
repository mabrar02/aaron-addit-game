using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{

    private Rigidbody2D rb;
    public SpriteRenderer sr;
    public Transform respawnPoint;

    private void Start() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        if (collision.gameObject.CompareTag("Hazard")) {
            rb.simulated = false;
            sr.enabled = false;
            GameState.controlEnabled = false;
            StartCoroutine(Respawn());
        }
    }

    private IEnumerator Respawn() {
        yield return new WaitForSeconds(2);
        transform.position = respawnPoint.position;

        rb.simulated = true;
        sr.enabled = true;
        GameState.controlEnabled = true;

    }
}
