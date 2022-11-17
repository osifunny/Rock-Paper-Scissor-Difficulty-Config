using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using UnityEngine;
using Unity.Services.Core.Environments;
using Unity.Services.Authentication;

public class GameServicesInitialization : MonoBehaviour
{
    [SerializeField] string environmentName;
    async void Start(){
        if(UnityServices.State == ServicesInitializationState.Uninitialized){
            var options = new InitializationOptions();
            options.SetEnvironmentName(environmentName);
            await UnityServices.InitializeAsync(options);

            if(!AuthenticationService.Instance.IsSignedIn){
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }
        }
    }
}
