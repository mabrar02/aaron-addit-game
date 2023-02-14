using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.Netcode;
public class Attempt : NetworkBehaviour 
{
    public NetworkObject _player;

    // Start is called before the first frame update
    void Start()
    {
        _player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
         
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();


        Debug.Log("spawn");
    }

}
