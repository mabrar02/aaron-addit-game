using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using TMPro;
using Cinemachine; 

//----------------------------------------------------------
// Class: UIManager 
// Intended to handle start menu UI appearances, input, and text display
//----------------------------------------------------------
public class UIManager : MonoBehaviour 
{
    public static UIManager Instance { get; private set;}

    public GameObject TheHero;
    
    [SerializeField] private GameObject CMCam;

    IDictionary<string, GameObject> UICanvasChildren = new Dictionary<string, GameObject>();

    // Singleton pattern
    void Awake() {
        if(Instance != null && Instance != this) { Destroy(this);}
        else { Instance = this; }     
    }

    // Start is called before the first frame update
    private void Start()
    {
        for(int i = 0; i < gameObject.transform.childCount; ++i) {
            UICanvasChildren.Add(gameObject.transform.GetChild(i).gameObject.name, gameObject.transform.GetChild(i).gameObject);
        }

        UICanvasChildren["ConnectionInfo"].SetActive(false);
        UICanvasChildren["JoinMenu"].SetActive(false);
    }

    // Update is called once per frame
    private void Update() {

    }
    

    //----------------------------------------------------------
    // 
    //----------------------------------------------------------
    public void SetConnectionInfo() {
        UICanvasChildren["StartMenu"].SetActive(false);
        UICanvasChildren["JoinMenu"].SetActive(false);
        UICanvasChildren["ConnectionInfo"].SetActive(true);
        
        UICanvasChildren["ConnectionInfo"].transform.Find("code").GetComponent<TextMeshProUGUI>().text      = MultiplayerManager.JoinCode;
        UICanvasChildren["ConnectionInfo"].transform.Find("mode").GetComponent<TextMeshProUGUI>().text      = MultiplayerManager.Mode;
        UICanvasChildren["ConnectionInfo"].transform.Find("transport").GetComponent<TextMeshProUGUI>().text = MultiplayerManager.Transport;
        UICanvasChildren["ConnectionInfo"].transform.Find("profile").GetComponent<TextMeshProUGUI>().text   = MultiplayerManager.Profile;
                
    }

    public void JoinGameScreen() {
        UICanvasChildren["StartMenu"].SetActive(false);
        UICanvasChildren["JoinMenu"].SetActive(true);
    }

    public void SetJoinCode() {
       MultiplayerManager.JoinCode = UICanvasChildren["JoinMenu"].transform.Find("code").GetComponent<TMP_InputField>().text;
    }

    public void SetSinglePlayer() {
        UICanvasChildren["StartMenu"].SetActive(false);
        UICanvasChildren["JoinMenu"].SetActive(false);
        UICanvasChildren["ConnectionInfo"].SetActive(false);

        NetworkManager.Singleton.gameObject.SetActive(false);

        MultiplayerManager.Instance.gameObject.SetActive(false);
        Instance.TheHero = Instantiate(Instance.TheHero, new Vector3(0,0,0), Quaternion.identity);
        Instance.CMCam.GetComponent<CinemachineVirtualCamera>().Follow = Instance.TheHero.transform; 
        Destroy(Instance.TheHero.GetComponent<NetworkObject>());
//        Instance.TheHero.GetComponent<ClientNetworkTransform>().enabled = false;

    }



}
