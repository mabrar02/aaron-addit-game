using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Services.Authentication;
using Cinemachine; 

//----------------------------------------------------------
// Class: WorldManager 
// Common point between Relay, Authentication, and UI manager meant to
// consolidate data for the three, though can probably be combined with one to simplify things 
//----------------------------------------------------------
public class WorldManager : MonoBehaviour
{

    public string profile;
    public string JoinCode;
    public string transport;
    public string mode;
    public GameObject TheHero;

    private RelayManager RMan;
    private AuthenticationManager AMan;
    private UIManager UMan;
    [SerializeField] private GameObject CMCam;

    void Start() {
        RMan = GameObject.Find("RelayManager").GetComponent<RelayManager>();
        AMan = GameObject.Find("AuthenticationManager").GetComponent<AuthenticationManager>();
        UMan = GameObject.Find("UI Canvas").GetComponent<UIManager>();
    }


    //----------------------------------------------------------
    // Entry points for UI buttons 
    //----------------------------------------------------------
    public void StartHost() {
        StartCoroutine(StartHostSetup());
    }

    public void StartClient() {
        StartCoroutine(StartClientSetup());

    }

    //----------------------------------------------------------
    // Connection setup coroutines 
    // Requires AuthenticationManager Start() to run first
    // Execution order: UI Button -> StartHost/ClientSetup -> SetupHost/Client -> Setup/JoinRelay 
    // Script order   : UIManager -> WorldManager          -> RelayManager     -> RelayManager
    //----------------------------------------------------------
    private IEnumerator StartHostSetup() {
        yield return RMan.SetupHost();

        mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
        transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
        profile = AuthenticationService.Instance.Profile;
        JoinCode = RMan.JoinCode;  
        UMan.SetConnectionInfo();
        TheHero = GameObject.FindGameObjectWithTag("Player"); 
        CMCam.GetComponent<CinemachineVirtualCamera>().Follow = TheHero.transform; 
    }

    private IEnumerator StartClientSetup() {
        yield return RMan.SetupClient(JoinCode);

        mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
        transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
        profile = AuthenticationService.Instance.Profile;

        UMan.SetConnectionInfo();
        //Camera controls for multiplayer
//        TheHero = GameObject.FindGameObjectWithTag("Player"); 
//        CMCam.GetComponent<CinemachineVirtualCamera>().Follow = TheHero.transform; 
    }

    //----------------------------------------------------------
    // Miscellaneous 
    //----------------------------------------------------------
    public void SetupProfile() {
        AMan.SwitchProfiles("prof2");
    }


}

