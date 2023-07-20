using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HauntPlatform : MonoBehaviour {
    private Rigidbody2D rb;
    private float horizontalDir;
    [SerializeField] private float targetMoveSpeed = 17;
    [SerializeField] private float accelAmount = 13;
    [SerializeField] public float deccelAmount = 16;

    void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update() {
        MovePlatform();
    }

    private void OnEnable() {
        GetComponent<PlatformHover>().enabled = false;
    }

    private void OnDisable() {
        GetComponent<PlatformHover>().enabled = true;
    }

    private void MovePlatform() {
        horizontalDir = Input.GetAxisRaw("Horizontal");
    }

    void FixedUpdate() {
        float targetSpeed = horizontalDir * targetMoveSpeed;

        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? accelAmount : deccelAmount;

        float speedDif = targetSpeed - rb.velocity.x;
        float movement = speedDif * accelRate;

        rb.AddForce((movement * Vector2.right) * rb.mass, ForceMode2D.Force);
    }
}
