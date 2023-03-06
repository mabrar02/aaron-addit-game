using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class GameState : MonoBehaviour {

    public static GameState Instance { get; private set; }

    // Multiplayer related
    public static string joinCode       { get; set;}
    public static string mode           { get; set;}
    public static string transport      { get; set;}
    public static string profile        { get; set;}



    // Cutscenes related
    public static PlayableDirector cutsceneInstance { get; set; }
    public static event EventHandler sceneStartCalled;
    public static Vector3 robbyPosition { get; set; }
    public static Vector3 clairePosition { get; set; }
    public static Quaternion robbyRotation { get; set; }
    public static Quaternion claireRotation { get; set; }
    public static bool playCutscenes { get; set; } 
    public static bool controlEnabled { get; set; }   


    public static void pingEvent()
    {
        sceneStartCalled?.Invoke(Instance, EventArgs.Empty);
    }




}
