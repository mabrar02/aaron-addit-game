using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnLogSize : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 initialScale;

    private Vector3 scaleChange, positionChange;

    public bool sizeReset = false;

    void Awake() {
        initialPosition = transform.localPosition;
        initialScale = transform.localScale;
    }
    void Start()
    {
        scaleChange = GetComponent<HauntLog>().scaleChange;
        positionChange = GetComponent<HauntLog>().positionChange;
        sizeReset = false;
    }
    void Update()
    {
        if (sizeReset) {
            sizeReset = false;
            this.enabled = false;
        }
        else if(transform.localScale.y > initialScale.y) {
            transform.localScale -= scaleChange;
            transform.position -= positionChange;

            if(transform.localScale.y <= initialScale.y) {
                transform.localScale = initialScale;
                transform.position = initialPosition;
                sizeReset = true;
            }
        }
        else {
            transform.localScale += scaleChange;
            transform.position += positionChange;
            if(transform.localScale.y >= initialScale.y) {
                transform.localScale = initialScale;
                transform.position = initialPosition;
                sizeReset = true;
            }
        }
    }
}
