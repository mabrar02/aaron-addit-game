using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class HauntLamp : MonoBehaviour {
    private Light2D lampLight;
    [SerializeField] [Range(1f, 10f)] private float lightBoost;

    private float targetInnerRadius;
    private float targetOuterRadius;

    private Rigidbody2D rb;

    void Awake() {
        lampLight = GetComponent<Light2D>();
        targetInnerRadius = lampLight.pointLightInnerRadius * lightBoost;
        targetOuterRadius = lampLight.pointLightOuterRadius * lightBoost;
    }

    void OnEnable() {
        StartCoroutine(GrowLight());
    }

    public void knockBackForce(Vector2 playerVector, bool hauntIn) {
        if (rb) {
            switch (hauntIn) {
                case true:
                    rb.AddForce(new Vector2(playerVector.x * 100, 100), ForceMode2D.Impulse);
                    break;
                case false:
                    rb.AddForce(new Vector2(-playerVector.x * 100, 100), ForceMode2D.Impulse);
                    break;
            }
        }
        else {
            rb = GetComponent<Rigidbody2D>();
            switch (hauntIn) {
                case true:
                    rb.AddForce(new Vector2(playerVector.x * 100, 100), ForceMode2D.Impulse);
                    break;
                case false:
                    rb.AddForce(new Vector2(-playerVector.x * 100, 100), ForceMode2D.Impulse);
                    break;
            }
        }
    }

    public IEnumerator GrowLight() {
        while (lampLight.pointLightInnerRadius <= targetInnerRadius || lampLight.pointLightOuterRadius <= targetOuterRadius) {
            Debug.Log("inner: " + lampLight.pointLightInnerRadius + "\n outer: " + lampLight.pointLightOuterRadius + "\n targets: " +
                targetInnerRadius + " " + targetOuterRadius);
            if (lampLight.pointLightInnerRadius <= targetInnerRadius) {
                lampLight.pointLightInnerRadius += 0.25f;
            }
            if (lampLight.pointLightOuterRadius <= targetOuterRadius) {
                lampLight.pointLightOuterRadius += 0.25f;
            }
            yield return null;
        }
        yield return null;
    }
}
