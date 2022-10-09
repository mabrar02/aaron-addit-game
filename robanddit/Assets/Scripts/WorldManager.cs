using Unity.Netcode;
using UnityEngine;

public class WorldManager : MonoBehaviour
{
    private int inGame = 0;
    private int isHost = 0;
    private int isClient = 0;
    private int connecting = 0;
    private int proset = 0;
    public string profile = "";
    public string JoinCode = "";
    public GameObject RelayManager;
    public GameObject AuthenticationManager;

    void OnGUI()
    {
        GUILayout.BeginArea(new Rect(10, 10, 300, 300));
        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer && isHost == 0 && isClient == 0)
        {
            StartButtons();
        }
        else if (inGame == 0) {
            StatusLabels();
            SetupAllocation();
        } 
        else {     

            StatusLabels();

        }
        GUILayout.EndArea();
    }

    void StartButtons()
    {
        if (GUILayout.Button("Host")) isHost = 1;
        if (GUILayout.Button("Client")) isClient = 1;
    }

    void StatusLabels()
    {
        var mode = NetworkManager.Singleton.IsHost ?
            "Host" : NetworkManager.Singleton.IsServer ? "Server" : "Client";

        GUILayout.Label("Transport: " +
            NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetType().Name);
        GUILayout.Label("Profile: " + profile);

        GUILayout.Label("Mode: " + mode);
        GUILayout.Label("Join code: " + JoinCode);
    }

    public void SetupAllocation() {
        if (isHost == 1 && connecting == 0) {
            RelayManager.SendMessage("SetupRelay");
            inGame = 1;
        } else if (isClient == 1) {
            GUILayout.Label("Enter join code: ");
            JoinCode = GUILayout.TextField(JoinCode);
            if(proset == 0) {
                AuthenticationManager.SendMessage("SwitchProfiles", "prof2");
                proset = 1;
            }
            if (GUILayout.Button("Join")) {
                JoinGame(JoinCode);
                inGame = 1;
            }
        }
    }

    public void SetProfile(string x) {
        profile = x;
    }

    public void AttemptingToConnect(int x) {
        connecting = x;
    }

    public void SetCode(string Code) {
        JoinCode = Code;
    }


    public void JoinGame(string Code) {
        RelayManager.SendMessage("JoinGame", Code);
    }
}
