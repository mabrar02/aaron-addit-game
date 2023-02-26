using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UIElements;

public class GameState : MonoBehaviour {

    public static GameState Instance { get; private set; }

    public static PlayableDirector cutsceneInstance { get; set; }

    public static event EventHandler sceneStartCalled;
    public static GameObject host       { get; set;}
    public static GameObject client     { get; set;}
    public static ulong      clientId   { get; set;} 
    public static ulong      hostId     { get; set;} 
    public static bool              playCutscenes { get; set; } 
    public static bool       controlEnabled { get; set; }   

    public static string joinCode       { get; set;}
    public static string mode           { get; set;}
    public static string transport      { get; set;}
    public static string profile        { get; set;}

    public static void pingEvent()
    {
        sceneStartCalled?.Invoke(Instance, EventArgs.Empty);
    }




}
