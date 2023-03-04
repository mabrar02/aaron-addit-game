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

    public float menuSpeed = 0.01f;
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

        ResumeButton.onClick.AddListener(()     => StartCoroutine(toggleMenu()));
//        SaveButton.onClick.AddListener(()       => );
//        SettingsButton.onClick.AddListener(()   => StartCoroutine(loadScene("Scene3")));
        MainMenuButton.onClick.AddListener(()   => quit());

        
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(toggleMenu());
        }
    }


    //----------------------------------------------------------
    // 
    //----------------------------------------------------------


    public IEnumerator toggleMenu() {
        float scaleVar;

        if(!menuOn) { 
            UICanvasChildren["OptionsMenu"].SetActive(true);
            scaleVar = 0f;
            UICanvasChildren["OptionsMenu"].GetComponent<RectTransform>().localScale = new Vector3(0, 0, 1f);

            while (UICanvasChildren["OptionsMenu"].GetComponent<RectTransform>().localScale.x <= 1f && 
                   UICanvasChildren["OptionsMenu"].GetComponent<RectTransform>().localScale.y <= 1f )
            {
                UICanvasChildren["OptionsMenu"].GetComponent<RectTransform>().localScale = new Vector3(scaleVar, scaleVar, 1f);
                scaleVar += menuSpeed;
                yield return null;
            }

            menuOn = true;
        } else {
            scaleVar = 1f;
            UICanvasChildren["OptionsMenu"].GetComponent<RectTransform>().localScale = new Vector3(1f, 1f, 1f);

            while (UICanvasChildren["OptionsMenu"].GetComponent<RectTransform>().localScale.x >= 0f && 
                   UICanvasChildren["OptionsMenu"].GetComponent<RectTransform>().localScale.y >= 0f )
            {
                UICanvasChildren["OptionsMenu"].GetComponent<RectTransform>().localScale = new Vector3(scaleVar, scaleVar, 1f);
                scaleVar -= menuSpeed;
                yield return null;
            }

            UICanvasChildren["OptionsMenu"].SetActive(false);

            menuOn = false;
        }

        yield return null;

    }

    public void quit()
    {
        NetworkManager.Singleton.Shutdown();
        SceneManager.LoadScene("mainMenu");
    }


}
