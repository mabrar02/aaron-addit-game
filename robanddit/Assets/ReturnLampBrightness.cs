using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ReturnLampBrightness : MonoBehaviour {
    private float initialInnerRadius;
    private float initialOuterRadius;
    private bool lightReset = false;
    private Light2D lampLight;
    private HauntLamp lightScript;
    private float targetInnerRadius;
    private float targetOuterRadius;

    void Awake() {
        lampLight = GetComponent<Light2D>();
        lightScript = GetComponent<HauntLamp>();
        lightReset = false;
        targetInnerRadius = lampLight.pointLightInnerRadius;
        targetOuterRadius = lampLight.pointLightOuterRadius;
    }
    void OnEnable() {
        StartCoroutine(ShrinkLight());
    }

    void Start() {

    }

    void Update() {
        if (lightReset) {
            lightReset = false;
            this.enabled = false;
        }
    }

    private IEnumerator ShrinkLight() {


        while (lampLight.pointLightInnerRadius >= targetInnerRadius || lampLight.pointLightOuterRadius >= targetOuterRadius) {
            if (lampLight.pointLightInnerRadius >= targetInnerRadius) {
                lampLight.pointLightInnerRadius -= 0.25f;
            }
            if (lampLight.pointLightOuterRadius >= targetOuterRadius) {
                lampLight.pointLightOuterRadius -= 0.25f;
            }
            yield return null;
        }

        lightReset = true;
        yield return null;
    }
}
