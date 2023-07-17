using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class btnFX : MonoBehaviour
{
    public AudioSource source;
    public AudioClip hoverSFX;
    public AudioClip clickSFX;
    public void HoverSound() {
        source.PlayOneShot(hoverSFX);
    }

    public void ClickSound() {
        source.PlayOneShot(clickSFX);
    }
}
