using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System;

public class Fade : MonoBehaviour
{
    public GameObject colorSlider;
    public PlayableDirector cutscene;


    private void Awake()
    {
        GameState.controlEnabled = false; 
        GameState.cutsceneInstance = cutscene; 
        GameState.pingEvent();

    }


    // Update is called once per frame
    void Update()
    {

        var newColor = gameObject.GetComponent<SpriteRenderer>().color;

        newColor.a = colorSlider.transform.position.x;

        gameObject.GetComponent<SpriteRenderer>().color = newColor;

    }
}
