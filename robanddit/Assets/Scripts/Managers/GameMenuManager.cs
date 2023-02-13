using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Unity.Netcode;
using TMPro;
using Cinemachine; 

//----------------------------------------------------------
// Class: GameMenuManager 
// Intended to handle start menu UI appearances, input, and text display
//----------------------------------------------------------
public class GameMenuManager : MonoBehaviour 
{
    public static GameMenuManager Instance { get; private set;}

    IDictionary<string, GameObject> UICanvasChildren = new Dictionary<string, GameObject>();

    // Singleton pattern
    void Awake() {
        if(Instance != null && Instance != this) { Destroy(this);}
        else { Instance = this; }     
    }

    // Start is called before the first frame update
    void Start()
    {
        // Too lazy to do a find objectName each time, just throw it all in a dict
        for(int i = 0; i < gameObject.transform.childCount; ++i) {
            UICanvasChildren.Add(gameObject.transform.GetChild(i).gameObject.name, gameObject.transform.GetChild(i).gameObject);
        }

        setConnectionInfo();


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

    public void setConnectionInfo() {

        setScreen("ConnectionInfo");
       
        UICanvasChildren["ConnectionInfo"].transform.Find("code").GetComponent<TextMeshProUGUI>().text      = GameState.joinCode;
        UICanvasChildren["ConnectionInfo"].transform.Find("mode").GetComponent<TextMeshProUGUI>().text      = GameState.mode;
        UICanvasChildren["ConnectionInfo"].transform.Find("transport").GetComponent<TextMeshProUGUI>().text = GameState.transport;
        UICanvasChildren["ConnectionInfo"].transform.Find("profile").GetComponent<TextMeshProUGUI>().text   = GameState.profile;
                
    }


//    private void setSinglePlayer() {
//       NetworkManager.Singleton.gameObject.SetActive(false);
//       MultiplayerManager.Instance.gameObject.SetActive(false);
//       Instance.TheHero = Instantiate(Instance.TheHero, new Vector3(0,0,0), Quaternion.identity);
//       Instance.CMCam.GetComponent<CinemachineVirtualCamera>().Follow = Instance.TheHero.transform; 
//       Destroy(Instance.TheHero.GetComponent<NetworkObject>());
//    }
}
