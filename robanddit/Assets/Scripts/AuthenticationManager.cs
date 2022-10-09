using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;

public class AuthenticationManager : MonoBehaviour
{

    public string profile;
    public GameObject WorldManager;

    async void Start()
	{
        var unityAutheticationInitOptions = new InitializationOptions();

        unityAutheticationInitOptions.SetProfile("prof1");

        await UnityServices.InitializeAsync(unityAutheticationInitOptions);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        
        profile = AuthenticationService.Instance.Profile;

        SetProfile(profile);
	}
    
     async public void SwitchProfiles(string profile) {
        AuthenticationService.Instance.SignOut();

        AuthenticationService.Instance.SwitchProfile(profile);

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        profile = AuthenticationService.Instance.Profile;
    
        SetProfile(profile);
    }

    public void SetProfile(string profile) {
        WorldManager.SendMessage("SetProfile", profile);  
    }


}
