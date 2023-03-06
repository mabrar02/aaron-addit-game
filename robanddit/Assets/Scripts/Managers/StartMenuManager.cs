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
    [SerializeField] private Button optionsButton;
    [SerializeField] private Button QuitButton;

    [SerializeField] private Button scene1Button;
    [SerializeField] private Button scene2Button;
    [SerializeField] private Button scene3Button;
    [SerializeField] private Button scene4Button;
    [SerializeField] private Button scene5Button;
    [SerializeField] private Button scene6Button;
    [SerializeField] private Button scene7Button;
    [SerializeField] private Button scene8Button;

    [SerializeField] private Button backButton;
    [SerializeField] private Button joinButton;
    [SerializeField] private TMP_Text loadingText;

    [SerializeField] private Toggle playCutscene;

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

        hostButton.onClick.AddListener(()     => setScreen("SceneMenu"));
        joinGameButton.onClick.AddListener(() => setScreen("JoinMenu"));
        optionsButton.onClick.AddListener(()  => setScreen("OptionsMenu"));
        QuitButton.onClick.AddListener(()     => Application.Quit());

        backButton.onClick.AddListener(()     => setScreen("StartMenu"));

        joinButton.onClick.AddListener(()     => StartCoroutine(joinGame()));


        scene1Button.onClick.AddListener(()   => StartCoroutine(loadScene("startScene")));
        scene2Button.onClick.AddListener(()   => StartCoroutine(loadScene("Scene2")));
        scene3Button.onClick.AddListener(()   => StartCoroutine(loadScene("Scene3")));
        scene4Button.onClick.AddListener(()   => StartCoroutine(loadScene("Scene4")));
        scene5Button.onClick.AddListener(()   => StartCoroutine(loadScene("Scene5")));
        scene6Button.onClick.AddListener(()   => StartCoroutine(loadScene("Scene6")));
        scene7Button.onClick.AddListener(()   => StartCoroutine(loadScene("Scene7")));
        scene8Button.onClick.AddListener(()   => StartCoroutine(loadScene("Scene8")));
        
        setScreen("StartMenu");
    }


    //----------------------------------------------------------
    // 
    //----------------------------------------------------------
    private void setScreen(string key = "") {

        foreach(var x in UICanvasChildren) {
            x.Value.SetActive(false);
        }

        if (key != "")
        {
        UICanvasChildren[key].SetActive(true);
        if(key != "StartMenu")
            {
                UICanvasChildren["CommonButtons"].SetActive(true);
            }
        }

    }

    private IEnumerator loadScene(string sceneName) {
        yield return StartCoroutine(MultiplayerManager.Instance.startHostTop());

        if(playCutscene.isOn)
        {
            GameState.playCutscenes = true;
            yield return StartCoroutine(transition());
        }

        NetworkManager.Singleton.SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    private IEnumerator transition() {
        UICanvasChildren["BlackScreen"].SetActive(true);
        var a = UICanvasChildren["BlackScreen"].GetComponent<Image>().color;
        while(a.a < 1) {
            a.a += 0.001f;
            UICanvasChildren["BlackScreen"].GetComponent<Image>().color = a;
            yield return null;
        }
        yield return null;
    }


    private IEnumerator joinGame() {

        GameState.joinCode = UICanvasChildren["JoinMenu"].transform.Find("code").GetComponent<TMP_InputField>().text;
        joinButton.gameObject.SetActive(false);
        loadingText.text = "Loading...";
        yield return StartCoroutine(MultiplayerManager.Instance.startClientTop());   

    }


}
