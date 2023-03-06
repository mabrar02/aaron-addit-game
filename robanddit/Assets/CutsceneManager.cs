using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using System;

public class CutsceneManager : MonoBehaviour
{
    public GameObject colorSlider;
    public GameObject player;
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

        GameState.robbyPosition = player.transform.position;
        GameState.robbyRotation = player.transform.rotation;

        var newColor = player.transform.GetChild(0).GetComponent<SpriteRenderer>().color;

        newColor.a = colorSlider.transform.position.x;

        player.transform.GetChild(0).GetComponent<SpriteRenderer>().color = newColor;

    }

}
