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
    [SerializeField] private Button ResumeButton;
    [SerializeField] private Button SaveButton;
    [SerializeField] private Button SettingsButton;
    [SerializeField] private Button MainMenuButton;


    public bool menuOn;

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
        menuOn = false;

        UICanvasChildren["OptionsMenu"].transform.Find("TransportInfo").GetComponent<TextMeshProUGUI>().text = "Transport : " + GameState.transport;
        UICanvasChildren["OptionsMenu"].transform.Find("ProfileInfo").GetComponent<TextMeshProUGUI>().text   = "Profile : " + GameState.profile;
        UICanvasChildren["OptionsMenu"].transform.Find("ModeInfo").GetComponent<TextMeshProUGUI>().text      = "Mode : " + GameState.mode;
        UICanvasChildren["OptionsMenu"].transform.Find("JoinInfo").GetComponent<TextMeshProUGUI>().text      = "Join Code : " + GameState.joinCode;

        ResumeButton.onClick.AddListener(()     => toggleMenu());
//        SaveButton.onClick.AddListener(()       => );
//        SettingsButton.onClick.AddListener(()   => StartCoroutine(loadScene("Scene3")));
//        MainMenuButton.onClick.AddListener(()   => StartCoroutine(loadScene("Scene4")));




        setScreen("");
        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            toggleMenu();
        }
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

    public void toggleMenu() {
        if(!menuOn) { 
            setScreen("OptionsMenu");
            menuOn = true;
        } else
        {
            setScreen("");
            menuOn = false;

        }
            
                
    }


//    private void setSinglePlayer() {
//       NetworkManager.Singleton.gameObject.SetActive(false);
//       MultiplayerManager.Instance.gameObject.SetActive(false);
//       Instance.TheHero = Instantiate(Instance.TheHero, new Vector3(0,0,0), Quaternion.identity);
//       Instance.CMCam.GetComponent<CinemachineVirtualCamera>().Follow = Instance.TheHero.transform; 
//       Destroy(Instance.TheHero.GetComponent<NetworkObject>());
//    }
}
