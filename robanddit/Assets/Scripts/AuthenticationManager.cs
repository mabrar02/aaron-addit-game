using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Core;
using Unity.Services.Authentication;
using UnityEngine;

//----------------------------------------------------------
// Class: AuthenticationManager 
// Anonymously authenticates the player, need to set distinct profiles for
// multiplayer testing using Parallelsync
//----------------------------------------------------------
public class AuthenticationManager : MonoBehaviour
{

    async void Start()
	{
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

}
