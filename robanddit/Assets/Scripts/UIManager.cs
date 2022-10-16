using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using TMPro;
using Cinemachine; 

//----------------------------------------------------------
// Class: UIManager 
// Intended to handle start menu UI appearances, input, and text display
//----------------------------------------------------------
public class UIManager : MonoBehaviour 
{
    public GameObject TheHero;
    
    private WorldManager WMan;
    private GameObject NMan; //NetworkManager
    private GameObject RMan; //RelayManager
    private GameObject AMan; //AuthenticationManager  
    [SerializeField] private GameObject CMCam;

    private GameObject StartMenu;
    private GameObject ConnectionInfo;
    private GameObject JoinMenu;


    // Start is called before the first frame update
    private void Start()
    {
        StartMenu = GameObject.Find("StartMenu");
        ConnectionInfo = GameObject.Find("ConnectionInfo");
        JoinMenu = GameObject.Find("JoinMenu");

        WMan = GameObject.Find("WorldManager").GetComponent<WorldManager>();
        NMan = GameObject.Find("NetworkManager");
        RMan = GameObject.Find("RelayManager");
        AMan = GameObject.Find("AuthenticationManager");

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
        
        ConnectionInfo.transform.Find("code").GetComponent<TextMeshProUGUI>().text = WMan.JoinCode;
        ConnectionInfo.transform.Find("mode").GetComponent<TextMeshProUGUI>().text = WMan.mode;
        ConnectionInfo.transform.Find("transport").GetComponent<TextMeshProUGUI>().text = WMan.transport;
        ConnectionInfo.transform.Find("profile").GetComponent<TextMeshProUGUI>().text = WMan.profile;
                
    }

    public void JoinGameScreen() {
        StartMenu.SetActive(false); 
        JoinMenu.SetActive(true);
    }

    public void SetJoinCode() {
       WMan.JoinCode = JoinMenu.transform.Find("code").GetComponent<TMP_InputField>().text;
    }

    public void SetSingePlayer() {
        StartMenu.SetActive(false); 
        JoinMenu.SetActive(false);
        ConnectionInfo.SetActive(false);
        NMan.SetActive(false);
        AMan.SetActive(false);
        RMan.SetActive(false);
        WMan.gameObject.SetActive(false);
        TheHero = Instantiate(TheHero, new Vector3(0,0,0), Quaternion.identity);
        CMCam.GetComponent<CinemachineVirtualCamera>().Follow = TheHero.transform; 
    }



}
