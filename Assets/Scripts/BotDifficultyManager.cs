using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.RemoteConfig;
using System;

public class BotDifficultyManager : MonoBehaviour
{
    [SerializeField] Bot bot;
    [SerializeField] int selectedDifficulty;
    [SerializeField] BotStats[] botDifficulties;
    [Header("Remote Config Parameters:")]
    [SerializeField] bool enableRemoteConfig = false;
    [SerializeField] string difficultyKey = "Difficulty";
    struct userAttributes{};
    struct appAttributes{};

    IEnumerator Start(){
        yield return new WaitUntil(() => bot.IsReady);

        var newStats = botDifficulties[selectedDifficulty];
        bot.SetStats(newStats, true);

        if(!enableRemoteConfig) yield break;
        yield return new WaitUntil(() =>
            UnityServices.State == ServicesInitializationState.Initialized
            && AuthenticationService.Instance.IsSignedIn
        );

        RemoteConfigService.Instance.FetchCompleted += OnRemoteConfigFetched;

        RemoteConfigService.Instance.FetchConfigs(new userAttributes(), new appAttributes());
    }

    private void OnDestroy(){
        RemoteConfigService.Instance.FetchCompleted -= OnRemoteConfigFetched;
    }

    private void OnRemoteConfigFetched(ConfigResponse response)
    {
        if(!RemoteConfigService.Instance.appConfig.HasKey(difficultyKey)){
            Debug.LogWarning($"Difficulty Key: {difficultyKey} not found on remote config server");
            return;
        }
        switch (response.requestOrigin){
            case ConfigOrigin.Default:
            case ConfigOrigin.Cached:
                break;
            case ConfigOrigin.Remote:
                selectedDifficulty = RemoteConfigService.Instance.appConfig.GetInt(difficultyKey);
                selectedDifficulty = Mathf.Clamp(selectedDifficulty, 0, botDifficulties.Length - 1);
                var newStats = botDifficulties[selectedDifficulty];
                bot.SetStats(newStats, true);
                break;
        }
    }
}
