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

    void Awake() {
        lampLight = GetComponent<Light2D>();
        lightScript = GetComponent<HauntLamp>();
        initialInnerRadius = lampLight.pointLightInnerRadius;
        initialOuterRadius = lampLight.pointLightOuterRadius;
    }

    void Start() {
        lightReset = false;
       /* StartCoroutine(ShrinkLight());*/
        Debug.Log(initialInnerRadius + " " + initialOuterRadius);
        Debug.Log(lampLight.pointLightInnerRadius + " " + lampLight.pointLightOuterRadius);

    }

    void Update() {
        /*        if (lightReset) {
                    lightReset = false;
                    this.enabled = false;
                }*/
        lampLight.pointLightInnerRadius = 1;
        lampLight.pointLightOuterRadius = 5;
        this.enabled = false;
    }

/*    private IEnumerator ShrinkLight() {
        float targetInnerRadius = initialInnerRadius;
        float targetOuterRadius = initialOuterRadius;

        while (lampLight.pointLightInnerRadius >= targetInnerRadius || lampLight.pointLightOuterRadius >= targetOuterRadius) {
            if (lampLight.pointLightInnerRadius > targetInnerRadius) {
                lampLight.pointLightInnerRadius -= 0.25f;
            }
            if (lampLight.pointLightOuterRadius > targetOuterRadius) {
                lampLight.pointLightOuterRadius -= 0.25f;
            }
            yield return null;
        }

        lightReset = true;
    }*/
}
