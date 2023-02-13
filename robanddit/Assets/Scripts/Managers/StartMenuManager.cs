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

    [SerializeField] private Button hostButton;
    [SerializeField] private Button joinGameButton;
    [SerializeField] private Button singleplayerButton;
    [SerializeField] private Button QuitButton;

    [SerializeField] private Button scene1Button;
    [SerializeField] private Button scene2Button;
    [SerializeField] private Button scene3Button;
    [SerializeField] private Button scene4Button;
    [SerializeField] private Button scene5Button;
    [SerializeField] private Button scene6Button;
    [SerializeField] private Button scene7Button;
    [SerializeField] private Button scene8Button;

    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_Text loadingText;

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

        hostButton.onClick.AddListener(() => StartCoroutine(hostGame()));
        joinGameButton.onClick.AddListener(() => setScreen("JoinMenu"));
        singleplayerButton.onClick.AddListener(() => setScreen("SceneMenu"));
        QuitButton.onClick.AddListener(() => Application.Quit());

        joinButton.onClick.AddListener(() => StartCoroutine(joinGame()));

        scene1Button.onClick.AddListener(() => loadScene("startScene"));
        scene2Button.onClick.AddListener(() => loadScene("Scene2"));
        scene3Button.onClick.AddListener(() => loadScene("Scene3"));
        scene4Button.onClick.AddListener(() => loadScene("Scene4"));
        scene5Button.onClick.AddListener(() => loadScene("Scene5"));
        scene6Button.onClick.AddListener(() => loadScene("Scene6"));
        scene7Button.onClick.AddListener(() => loadScene("Scene7"));
        scene8Button.onClick.AddListener(() => loadScene("Scene8"));

        setScreen("StartMenu");
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

    private void loadScene(string sceneName) {
        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }


     private IEnumerator hostGame() {
        yield return StartCoroutine(MultiplayerManager.Instance.startHostTop());
        setScreen("SceneMenu");
    }

    private IEnumerator joinGame() {

        GameState.joinCode = UICanvasChildren["JoinMenu"].transform.Find("code").GetComponent<TMP_InputField>().text;
        joinButton.gameObject.SetActive(false);
        loadingText.text = "Loading...";
        yield return StartCoroutine(MultiplayerManager.Instance.startClientTop());   

    }

//    public void setSinglePlayer() {
//        GameState.singleplayer = true;
//        GameState.host         = false;
//        SceneManager.LoadScene("startScene");
//    }



}
