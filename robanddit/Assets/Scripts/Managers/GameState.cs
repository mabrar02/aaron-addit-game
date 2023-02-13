using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;

public class GameState : MonoBehaviour {

    public static GameState Instance { get; private set; }

    public static bool singleplayer { get; set;}
    public static bool host         { get; set;}
    public static string joinCode   { get; set;}
    public static string mode       { get; set;}
    public static string transport  { get; set;}
    public static string profile    { get; set;}





}
