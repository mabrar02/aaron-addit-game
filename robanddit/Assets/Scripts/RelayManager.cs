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

public class RelayManager : NetworkBehaviour
{
    Allocation allocation;
    JoinAllocation JoinAllocation;
    string GameJoinCode;   
    public GameObject WorldManager;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public async void SetupRelay() {
       try {
           WorldManager.SendMessage("AttemptingToConnect", 1);      

           allocation = await RelayService.Instance.CreateAllocationAsync(2); 

           GameJoinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId); 

           var dtlsEndpoint = allocation.ServerEndpoints.First();

           var (ipv4address, port, allocationIdBytes, connectionData, key) = (dtlsEndpoint.Host, (ushort)dtlsEndpoint.Port, allocation.AllocationIdBytes, allocation.ConnectionData, allocation.Key);

           NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(ipv4address, port, allocationIdBytes, key, connectionData, true);

           NetworkManager.Singleton.StartHost();
        
           WorldManager.SendMessage("SetCode", GameJoinCode);
        } catch {
            Debug.Log("Error in setting up relay");
        }
    }

    public string GetCode() {
           return GameJoinCode;
    }

    public async void JoinGame(string Code) {
       try {
        JoinAllocation = await RelayService.Instance.JoinAllocationAsync(Code); 

        var dtlsEndpoint = JoinAllocation.ServerEndpoints.First();

        var (ipv4address, port, allocationIdBytes, connectionData, hostConnectionData, key) = (dtlsEndpoint.Host, (ushort)dtlsEndpoint.Port, JoinAllocation.AllocationIdBytes, JoinAllocation.ConnectionData, JoinAllocation.HostConnectionData, JoinAllocation.Key);

        NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(ipv4address, port, allocationIdBytes, key, connectionData, hostConnectionData, true);

        NetworkManager.Singleton.StartClient();
        } catch {
            Debug.Log("Error in connecting to host");
        }

    }


}
