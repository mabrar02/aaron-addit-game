using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Cinemachine;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.U2D;

public class SetupHandler : NetworkBehaviour
{
    public CinemachineVirtualCamera cam; 
    public NetworkObject _player;
    private PlayableDirector _playableDirector;
    private IReadOnlyList<ulong> clientIds;

    // Start is called before the first frame update
    void Start()
    {
        _player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();

        if(GameState.playCutscenes == true)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Cutscene1", LoadSceneMode.Additive);

            GameState.sceneStartCalled += sceneStart;
        } else
        {
            enablePlayerServerRpc(NetworkManager.Singleton.LocalClient.ClientId);

            cam.Follow = _player.transform.GetChild(0).transform;

            GameState.controlEnabled = true;
        }

    }

    private void sceneStart(object a ,EventArgs e)
    {
        _playableDirector = GameState.cutsceneInstance;

        _playableDirector.stopped += _playableDirector_stopped;

    }

    private void _playableDirector_stopped(PlayableDirector obj)
    {
        NetworkManager.Singleton.SceneManager.UnloadScene(obj.gameObject.scene);

        _player.transform.GetChild(0).transform.position = new Vector3(4, -12.08f, 0);
        _player.transform.GetChild(0).transform.rotation = new Quaternion(0, 0, 0, 0);

        enablePlayerServerRpc(NetworkManager.Singleton.LocalClient.ClientId);
        cam.Follow = _player.transform.GetChild(0).transform;

        GameState.controlEnabled = true;
    }


    [ServerRpc(RequireOwnership = false)]
    void enablePlayerServerRpc(ulong uid) 
    {
        clientIds = NetworkManager.ConnectedClientsIds;
        NetworkObject _player = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(uid);
        _player.transform.GetChild(0).gameObject.GetComponent<Rigidbody2D>().simulated = true;

        foreach (ulong x in clientIds)
        {
            NetworkObjectReference xi = NetworkManager.Singleton.SpawnManager.GetPlayerNetworkObject(x);
            enablePlayerClientRpc(xi);
        }
    }

    [ClientRpc]
    void enablePlayerClientRpc(NetworkObjectReference xi)
    {
        xi.TryGet(out _player); 
        _player.transform.GetChild(0).gameObject.GetComponent<Rigidbody2D>().simulated = true;
        _player.transform.GetChild(0).transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = true;
    }

}
