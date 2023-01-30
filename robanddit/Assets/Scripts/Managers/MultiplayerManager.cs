// General
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;

//Authentication
using Unity.Services.Authentication;
using System.Threading.Tasks;

//Relay 
using System;
using System.Linq;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;


using Cinemachine; 
//----------------------------------------------------------
// Class: MultiplayerManager 
//
// Authentication:
// Anonymously authenticates the player, need to set distinct profiles for
// multiplayer testing using Parallelsync
//
// Relay:
// Sets up the Unity Relay server stuff depending on if user is Host or Client 
//----------------------------------------------------------
public class MultiplayerManager : MonoBehaviour
{

    public static MultiplayerManager Instance { get; private set;}

    public string JoinCode;
    public string Profile;
    public string Transport;
    public string Mode;

    public GameObject TheHero;

    [SerializeField] private GameObject CMCam;

    // Singleton pattern
    void Awake () {
        if(Instance != null && Instance != this) { Destroy(this);}
        else { Instance = this; }     
    }

    //----------------------------------------------------------
    // Authentication process 
    //----------------------------------------------------------
    async void Start()	{
        var unityAutheticationInitOptions = new InitializationOptions();

        unityAutheticationInitOptions.SetProfile("prof1");

        await UnityServices.InitializeAsync(unityAutheticationInitOptions);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();  
	}
    
     async public void SwitchProfiles(string profile) {
        AuthenticationService.Instance.SignOut();

        AuthenticationService.Instance.SwitchProfile(profile);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
  
    }

    //----------------------------------------------------------
    // Entry points for UI buttons. Everything below these two should be private 
    //----------------------------------------------------------
    public void StartHost() {
        StartCoroutine(StartHostSetup());

    }

    public void StartClient() {
        UIManager.Instance.SetJoinCode();
        StartCoroutine(StartClientSetup());

    }

    //----------------------------------------------------------
    // Connection setup coroutines 
    // Requires Authentication to have run first
    // Calls the main SetupHost/Client + assigns variables for UI display
    // Execution order: UI Button -> StartHost/ClientSetup -> SetupHost/Client -> Setup/JoinRelay 
    //----------------------------------------------------------
    private IEnumerator StartHostSetup() {
        yield return SetupHost();

        Mode = NetworkManager.Singleton.IsHost ? "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";
        Transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
        Profile = AuthenticationService.Instance.Profile;
        UIManager.Instance.SetConnectionInfo();

        // TODO: Sets the camera to only follow host for now but this is wrong
        TheHero = GameObject.FindGameObjectWithTag("Player"); 
        CMCam.GetComponent<CinemachineVirtualCamera>().Follow = TheHero.transform; 
    }

    private IEnumerator StartClientSetup() {
        yield return SetupClient(JoinCode);

        Mode = "Client";
        Transport = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name;
        Profile = AuthenticationService.Instance.Profile;
        UIManager.Instance.SetConnectionInfo();

        //Camera controls for multiplayer
//        TheHero = GameObject.FindGameObjectWithTag("Player"); 
//        CMCam.GetComponent<CinemachineVirtualCamera>().Follow = TheHero.transform; 
    }


    //----------------------------------------------------------
    // Entry coroutines for host/client setup 
    // Requires AuthenticationManager Start() to run first
    // Execution order: UI Button -> StartHost/ClientSetup -> SetupHost/Client -> Setup/JoinRelay 
    //----------------------------------------------------------
    private IEnumerator SetupHost() {
        var ServerRelay = SetupRelay();

        while (!ServerRelay.IsCompleted) {
             yield return null;
        }

        if (ServerRelay.IsFaulted) {
             Debug.Log("Couldn't start Relay Server");
             yield break;
        }
        var (ipv4address, port, allocationIdBytes, connectionData, key, joinCode) = ServerRelay.Result;

        JoinCode = joinCode; 

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(ipv4address, port, allocationIdBytes, key, connectionData, true);

        NetworkManager.Singleton.StartHost();    

        yield return null;
    }

   private IEnumerator SetupClient(string code) {
        var ServerRelay = JoinRelay(code);

        while (!ServerRelay.IsCompleted) {

            yield return null;
        }

        if (ServerRelay.IsFaulted) {
            Debug.Log("Couldn't join Relay Server");
            yield break;
        }

        var (ipv4address, port, allocationIdBytes, connectionData, hostConnectionData, key) = ServerRelay.Result;

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(ipv4address, port, allocationIdBytes, key, connectionData, hostConnectionData, true);

        NetworkManager.Singleton.StartClient();

        yield return null;
   }
   
   
    //----------------------------------------------------------
    // Relay server setup tasks 
    //----------------------------------------------------------
    private async Task<(string ipv4address, ushort port, byte[] allocationIdBytes, byte[] connectionData, byte[] key, string joinCode)> SetupRelay() {
        Allocation allocation;
        string GameJoinCode;

        try { 
           allocation = await RelayService.Instance.CreateAllocationAsync(4); 
        } catch {
            Debug.Log("Failed creating allocation");
            throw;
        }

        try {
           GameJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId); 
        } catch {
            Debug.Log("Failed getting join code");
            throw;
        }

           var dtlsEndpoint = allocation.ServerEndpoints.First(e => e.ConnectionType == "dtls");
            
           return (dtlsEndpoint.Host, (ushort)dtlsEndpoint.Port, allocation.AllocationIdBytes, allocation.ConnectionData, allocation.Key, GameJoinCode);
    }

    private async Task<(string ipv4address, ushort port, byte[] allocationIdBytes, byte[] connectionData, byte[] hostConnectionData, byte[] key)> JoinRelay(string Code) {
        JoinAllocation allocation;

       try {
            allocation = await RelayService.Instance.JoinAllocationAsync(Code); 
        } catch {
            Debug.Log("Failed joining allocation");
            throw;
        }
        var dtlsEndpoint = allocation.ServerEndpoints.First(e => e.ConnectionType == "dtls");

        return (dtlsEndpoint.Host, (ushort)dtlsEndpoint.Port, allocation.AllocationIdBytes, allocation.ConnectionData, allocation.HostConnectionData, allocation.Key);
    }
    


    //----------------------------------------------------------
    // Miscellaneous 
    //----------------------------------------------------------


}