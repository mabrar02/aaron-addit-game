using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System;
using Cinemachine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Playables;
using UnityEngine.U2D;
using UnityEngine.Rendering.Universal;

public class SetupHandler : NetworkBehaviour
{
    public CinemachineVirtualCamera cam; 
    public NetworkObject _player;
    public GameObject blackScreen;
    private PlayableDirector _playableDirector;
    private IReadOnlyList<ulong> clientIds;


    // Start is called before the first frame update
    IEnumerator Start()
    {
        _player = NetworkManager.Singleton.SpawnManager.GetLocalPlayerObject();
        _player.transform.GetChild(0).transform.Find("Light").gameObject.SetActive(true);

        if(GameState.playCutscenes == true) {
            GameState.sceneStartCalled += sceneStart;
            yield return StartCoroutine(transition());

        } else {

            blackScreen.SetActive(false);
            enablePlayerServerRpc(NetworkManager.Singleton.LocalClient.ClientId);
            cam.Follow = _player.transform.GetChild(0).transform.Find("Cam").transform;
            GameState.controlEnabled = true;
        }

    }

    private IEnumerator transition() {
        NetworkManager.Singleton.SceneManager.LoadScene("Cutscene1", LoadSceneMode.Additive);
        var a = blackScreen.GetComponent<Image>().color;
        while(a.a > 0) {
            a.a -= 0.0005f;
            blackScreen.GetComponent<Image>().color = a;
            yield return null;
        }
        blackScreen.SetActive(false);
        yield return null;
    }


    private void sceneStart(object a ,EventArgs e)
    {
        _playableDirector = GameState.cutsceneInstance;

        _playableDirector.stopped += _playableDirector_stopped;

    }

    private void _playableDirector_stopped(PlayableDirector obj)
    {
        NetworkManager.Singleton.SceneManager.UnloadScene(obj.gameObject.scene);

        _player.transform.GetChild(0).transform.position = GameState.robbyPosition;
        _player.transform.GetChild(0).transform.rotation = GameState.robbyRotation;

        enablePlayerServerRpc(NetworkManager.Singleton.LocalClient.ClientId);
        cam.Follow = _player.transform.GetChild(0).transform.Find("Cam").transform;

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
        _player.transform.GetChild(0).transform.Find("Sprite").GetComponent<SpriteRenderer>().enabled = true;
        _player.transform.GetChild(0).transform.Find("Light").gameObject.SetActive(true);
    }

}
