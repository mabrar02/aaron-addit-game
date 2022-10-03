using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class CameraScript : NetworkBehaviour
{
   private List<NetworkObject> thePlayers = new List<NetworkObject>();
   private NetworkVariable<Vector3> CurrentCameraPosition = new NetworkVariable<Vector3>();
   private NetworkVariable<int> PlayerId = new NetworkVariable<int>(0);

   private Camera CameraMain;
   private NetworkManager MainNetworkManager;

    // Gets called when the NetworkObject gets spawned, message handlers are ready to be registered and the network is setup. 
    public override void OnNetworkSpawn() {           
         Debug.Log(OwnerClientId);
         MainNetworkManager = NetworkManager.Singleton;
         UpdateCameraServerRpc(PlayerId.Value);
         transform.position = CurrentCameraPosition.Value;
    }

    // Start is called before the first frame update
    void Start()
    {  
      if(IsHost) {
         CameraMain = GetComponent(typeof(Camera)) as Camera;
      }

    }

    // Update is called once per frame

    void Update()
    {    
       if(IsHost) {     
         UpdateCamera();
       }
    }

    void UpdateCamera() {
         CurrentCameraPosition.Value = Vector3.zero;
         foreach(NetworkObject player in thePlayers) {
            CurrentCameraPosition.Value += player.transform.position;
         }
         CurrentCameraPosition.Value = PlayerId.Value == 1 ? CurrentCameraPosition.Value : CurrentCameraPosition.Value/2;
         CurrentCameraPosition.Value += new Vector3(0,0,-10);
         transform.position = CurrentCameraPosition.Value; 
    }


    // Camera's NetworkObject OwnerClientId is Host's regardless of which client is being run 
   [ServerRpc(RequireOwnership=false)]
   void UpdateCameraServerRpc(int NetId) {       
         // TODO: Deal with player disconnecting
         thePlayers.Add(MainNetworkManager.ConnectedClientsList[PlayerId.Value].PlayerObject); 
         PlayerId.Value = PlayerId.Value + 1;
   }


}
