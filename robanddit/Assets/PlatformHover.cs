using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHover : MonoBehaviour {
    [SerializeField] private float hoverAmplitude = 0.5f;
    [SerializeField] private float hoverFrequency = 1f;

    private float hoverTimer = 0f;
    private Vector3 initialPosition;

    void Awake() {
        initialPosition = transform.position;
    }

    void Update() {
        HoverPlatform();
    }

    private void OnEnable() {
        initialPosition = new Vector3(transform.position.x, initialPosition.y, transform.position.z);
    }

    private void HoverPlatform() {

        hoverTimer += Time.deltaTime;


        float hoverDisplacement = Mathf.Sin(hoverTimer * hoverFrequency) * hoverAmplitude;

        Vector3 newPosition = initialPosition + new Vector3(0f, hoverDisplacement, 0f);
        transform.position = newPosition;
    }
}
