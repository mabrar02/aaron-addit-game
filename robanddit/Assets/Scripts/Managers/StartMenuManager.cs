using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;
using Cinemachine; 

//----------------------------------------------------------
// Class: UIManager 
// Intended to handle start menu UI appearances, input, and text display
//----------------------------------------------------------
public class StartMenuManager : MonoBehaviour 
{
    public static StartMenuManager Instance { get; private set;}

    IDictionary<string, GameObject> UICanvasChildren = new Dictionary<string, GameObject>();

    // Singleton pattern
    void Awake() {
        if(Instance != null && Instance != this) { Destroy(this);}
        else { Instance = this; }     
    }

    // Start is called before the first frame update
    private void Start()
    {
        // Too lazy to do a find objectName each time, just throw it all in a dict
        for(int i = 0; i < gameObject.transform.childCount; ++i) {
            UICanvasChildren.Add(gameObject.transform.GetChild(i).gameObject.name, gameObject.transform.GetChild(i).gameObject);
        }

        setScreen("StartMenu");
    }

    // Update is called once per frame
    private void Update() {

    }
    

    //----------------------------------------------------------
    // 
    //----------------------------------------------------------

    private void setScreen(string key = "") {

        foreach(var x in UICanvasChildren) {
            x.Value.SetActive(false);
        }

        if(key != "") UICanvasChildren[key].SetActive(true);
    }


    public void setHost() {
        GameState.singleplayer = false;
        GameState.host         = true;
        SceneManager.LoadScene("startScene");
    }


    public void joinGameScreen() {
        setScreen("JoinMenu");
    }

    public void joinGame() {
        GameState.joinCode = UICanvasChildren["JoinMenu"].transform.Find("code").GetComponent<TMP_InputField>().text;
        GameState.singleplayer = false;
        GameState.host         = false;
        SceneManager.LoadScene("startScene");
    }

    public void setSinglePlayer() {
        GameState.singleplayer = true;
        GameState.host         = false;
        SceneManager.LoadScene("startScene");
    }


//        setScreen();
//
//        NetworkManager.Singleton.gameObject.SetActive(false);
//        MultiplayerManager.Instance.gameObject.SetActive(false);
//        Instance.TheHero = Instantiate(Instance.TheHero, new Vector3(0,0,0), Quaternion.identity);
//        Instance.CMCam.GetComponent<CinemachineVirtualCamera>().Follow = Instance.TheHero.transform; 
//        Destroy(Instance.TheHero.GetComponent<NetworkObject>());



}
