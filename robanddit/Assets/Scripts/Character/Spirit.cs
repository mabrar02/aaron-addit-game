using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text.RegularExpressions;
using Doublsb.Dialog;

public class Spirit : MonoBehaviour
{
    public DialogManager dialogManager;
    public bool          done;
    // Start is called before the first frame update
    void Start()
    {
        done = false;
    }

    // Update is called once per frame
    void Update()
    {        

    } 

    void FixedUpdate() {

        transform.Rotate(0f, 0f, 5f); // Woo
    }

    private void OnTriggerEnter2D(Collider2D collision) {
        if(collision.gameObject.CompareTag("Player") && !done) {
            startDialogue();    
        } 
    }

    private void startDialogue() {
        string [] lines = System.IO.File.ReadAllLines(@"Assets/Dialogue/dialogue.txt");

        List<DialogData> dialogTexts = new List<DialogData>();

        Regex characterRegex = new Regex(@"(?<=\[).*(?=\])");
        Regex dialogueRegex  = new Regex(@"(?<="").*(?="")");

        foreach(string x in lines) {
            Match character = characterRegex.Match(x);
            Match dialogue  = dialogueRegex.Match(x);

            dialogTexts.Add(new DialogData(dialogue.Value, character.Value));

        }
        done = true;     
        dialogManager.Show(dialogTexts);
    }
    
}
