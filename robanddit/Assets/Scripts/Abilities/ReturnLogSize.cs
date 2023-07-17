using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReturnLogSize : MonoBehaviour
{
    private Vector3 initialPosition;
    private Vector3 initialScale;

    private Vector3 scaleChange, positionChange;

    public bool sizeReset = false;
    private bool soundPlaying = false;
    [SerializeField] private AudioSource returnSound;

    void Awake() {
        initialPosition = transform.localPosition;
        initialScale = transform.localScale;
    }
    void Start()
    {
        scaleChange = GetComponent<HauntLog>().scaleChange/4;
        positionChange = GetComponent<HauntLog>().positionChange/4;
        sizeReset = false;
        returnSound.Play();
        returnSound.loop = true;
        soundPlaying = true;
    }
    void Update()
    {
        if (!soundPlaying) {
            returnSound.Play();
            returnSound.loop = true;
            soundPlaying = true;
        }
        if (sizeReset) {
            sizeReset = false;
            returnSound.Stop();
            soundPlaying = false;
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
