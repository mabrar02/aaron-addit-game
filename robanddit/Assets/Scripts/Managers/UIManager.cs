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

    private GameObject StartMenu;
    private GameObject ConnectionInfo;
    private GameObject JoinMenu;


    // Singleton pattern
    void Awake() {
        if(Instance != null && Instance != this) { Destroy(this);}
        else { Instance = this; }     
    }

    // Start is called before the first frame update
    private void Start()
    {
        StartMenu = GameObject.Find("StartMenu");
        ConnectionInfo = GameObject.Find("ConnectionInfo");
        JoinMenu = GameObject.Find("JoinMenu");

        ConnectionInfo.SetActive(false);
        JoinMenu.SetActive(false);
    }

    // Update is called once per frame
    private void Update() {

    }
    

    public void SetConnectionInfo() {
        StartMenu.SetActive(false); 
        JoinMenu.SetActive(false);
        ConnectionInfo.SetActive(true);
        
        ConnectionInfo.transform.Find("code").GetComponent<TextMeshProUGUI>().text = MultiplayerManager.Instance.JoinCode;
        ConnectionInfo.transform.Find("mode").GetComponent<TextMeshProUGUI>().text = MultiplayerManager.Instance.Mode;
        ConnectionInfo.transform.Find("transport").GetComponent<TextMeshProUGUI>().text = MultiplayerManager.Instance.Transport;
        ConnectionInfo.transform.Find("profile").GetComponent<TextMeshProUGUI>().text = MultiplayerManager.Instance.Profile;
                
    }

    public void JoinGameScreen() {
        Instance.StartMenu.SetActive(false); 
        Instance.JoinMenu.SetActive(true);
    }

    public void SetJoinCode() {
       MultiplayerManager.Instance.JoinCode = Instance.JoinMenu.transform.Find("code").GetComponent<TMP_InputField>().text;
    }

    public void SetSingePlayer() {
        Instance.StartMenu.SetActive(false); 
        Instance.JoinMenu.SetActive(false);
        Instance.ConnectionInfo.SetActive(false);

        NetworkManager.Singleton.gameObject.SetActive(false);

        MultiplayerManager.Instance.gameObject.SetActive(false);
        MultiplayerManager.Instance.gameObject.SetActive(false);
        MultiplayerManager.Instance.gameObject.SetActive(false);
        Instance.TheHero = Instantiate(Instance.TheHero, new Vector3(0,0,0), Quaternion.identity);
        Instance.CMCam.GetComponent<CinemachineVirtualCamera>().Follow = Instance.TheHero.transform; 
    }



}
