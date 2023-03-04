using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TutorialTrigger : MonoBehaviour
{

    public UnityEngine.UI.Image  border;
    public TextMeshProUGUI       text;
    public float frameDuration;

    private float alpha;
    public float alphaStep;

    // Start is called before the first frame update
    void Start()
    {
        border.color = new Color(border.color.r, border.color.g, border.color.b, 0f);
        text.color = new Color(text.color.r, text.color.g, text.color.b, 0f);
        Trigger.triggerEnter += playUI;            
    }


    public void playUI(object a, UIEventArgs args)
    {
        StartCoroutine(popUpSequence(args.msg));
    }


    private IEnumerator popUpSequence(string msg)
    {
        yield return StartCoroutine(popUp(msg));
        yield return new WaitForSeconds(frameDuration);
        yield return StartCoroutine(popDown());
    }


    private IEnumerator popUp(string x)
    {
        text.text = x;

        while (alpha < 1)
        {
            alpha += alphaStep;

            border.color = new Color(border.color.r, border.color.g, border.color.b, alpha);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

            yield return null;
        }

        yield return null;
    }

    private IEnumerator popDown()
    {
        while (alpha > 0)
        {
            alpha -= alphaStep;

            border.color = new Color(border.color.r, border.color.g, border.color.b, alpha);
            text.color = new Color(text.color.r, text.color.g, text.color.b, alpha);

            yield return null;
        }

        yield return null;
    }

}
