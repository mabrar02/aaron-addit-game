using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Http;
using Unity.Services.Relay.Models;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport;
using Unity.Networking.Transport.Relay;

//----------------------------------------------------------
// Class: RelayManager 
// Sets up the Unity Relay server stuff depending on if user is Host or Client 
//----------------------------------------------------------
public class RelayManager : MonoBehaviour 
{

    public string JoinCode;

    private UIManager UMan;
    
    // Start is called before the first frame update
    void Start()
    {
        UMan = GameObject.Find("UI Canvas").GetComponent<UIManager>();
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }

    //----------------------------------------------------------
    // Entry coroutines for host/client setup 
    // Requires AuthenticationManager Start() to run first
    // Execution order: UI Button -> StartHost/ClientSetup -> SetupHost/Client -> Setup/JoinRelay 
    // Script order   : UIManager -> WorldManager          -> RelayManager     -> RelayManager
    //----------------------------------------------------------
    public IEnumerator SetupHost() {
        var ServerRelay = SetupRelay();

        while (!ServerRelay.IsCompleted) {
             yield return null;
        }

        if (ServerRelay.IsFaulted) {
             Debug.Log("Couldn't start Relay Server");
             yield break;
        }
        var (ipv4address, port, allocationIdBytes, connectionData, key, joinCode) = ServerRelay.Result;

        SetJoinCode(joinCode);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(ipv4address, port, allocationIdBytes, key, connectionData, true);

        NetworkManager.Singleton.StartHost();    

        yield return null;
    }

   public IEnumerator SetupClient(string code) {
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
    private static async Task<(string ipv4address, ushort port, byte[] allocationIdBytes, byte[] connectionData, byte[] key, string joinCode)> SetupRelay() {
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

    private static async Task<(string ipv4address, ushort port, byte[] allocationIdBytes, byte[] connectionData, byte[] hostConnectionData, byte[] key)> JoinRelay(string Code) {
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
    private void SetJoinCode(string code) {
             JoinCode = code;
    }

   
}
