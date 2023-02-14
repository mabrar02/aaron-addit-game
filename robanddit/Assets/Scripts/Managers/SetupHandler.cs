using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;

public class SetupHandler : NetworkBehaviour
{

    public NetworkObject _player;
    // Start is called before the first frame update
    void Start()
    {
        enablePlayerServerRpc(NetworkManager.Singleton.LocalClient.ClientId);
        if (IsClient)
        {
            NetworkObject _player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
            if(_player.transform.GetChild(0).gameObject.GetComponent<Rigidbody2D>().simulated == false) {
                _player.transform.GetChild(0).gameObject.GetComponent<Rigidbody2D>().simulated = true;
            }

        }

    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
    }



    [ServerRpc(RequireOwnership = false)]
    void enablePlayerServerRpc(ulong uid) 
    {
        NetworkObject _player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid);
        if(_player.transform.GetChild(0).gameObject.GetComponent<Rigidbody2D>().simulated == false) {
            _player.transform.GetChild(0).gameObject.GetComponent<Rigidbody2D>().simulated = true;
        }
       
    }
}
