using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using "Assets/Dialogue/DialogueFunctions.cs";

public class DialogueScript : MonoBehaviour
{
    string text = File.ReadAllText("Assets/Dialogue/NPC.txt");

    void Start() {

    }

    void Update() {
        Debug.Log(text);
    }

}

