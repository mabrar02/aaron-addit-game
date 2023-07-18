using UnityEngine;
using UnityEngine.Rendering.Universal;
using System.Collections;

public class HauntLamp : MonoBehaviour {
    private Light2D lampLight;
    [SerializeField] [Range(1f, 10f)] private float lightBoost;

    void Awake() {
        lampLight = GetComponent<Light2D>();
    }

    void Start() {
        StartCoroutine(GrowLight());
    }

    void Update() {
    }

    private IEnumerator GrowLight() {
        float targetInnerRadius = lampLight.pointLightInnerRadius * lightBoost;
        float targetOuterRadius = lampLight.pointLightOuterRadius * lightBoost;

        while (lampLight.pointLightInnerRadius <= targetInnerRadius || lampLight.pointLightOuterRadius <= targetOuterRadius) {
            if (lampLight.pointLightInnerRadius < targetInnerRadius) {
                lampLight.pointLightInnerRadius += 0.25f;
            }
            if (lampLight.pointLightOuterRadius < targetOuterRadius) {
                lampLight.pointLightOuterRadius += 0.25f;
            }
            yield return null;
        }
    }
}
