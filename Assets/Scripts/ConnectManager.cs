using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ConnectManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField usernameInput;
    [SerializeField] TMP_Text feedbackText;
    
    private void Start(){
        usernameInput.text = PlayerPrefs.GetString(PropertyNames.Player.NickName, "");
    }

    public void ClickConnect(){
        feedbackText.text = "";
        if(usernameInput.text.Length < 3){
            feedbackText.text = "Username min 3 characters";
            return;
        }

        //saving username
        PlayerPrefs.SetString(PropertyNames.Player.NickName, usernameInput.text);
        PhotonNetwork.NickName = usernameInput.text;
        PhotonNetwork.AutomaticallySyncScene = true;

        //connecting to the server
        PhotonNetwork.ConnectUsingSettings();
        feedbackText.text = "Connecting...";
    }

    //runs when connected
    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master");
        feedbackText.text = "Connected to Master";
        StartCoroutine(LoadLevelAfterConnectAndReady());
    }

    IEnumerator LoadLevelAfterConnectAndReady(){
        while(!PhotonNetwork.IsConnectedAndReady) yield return null;
        SceneManager.LoadScene("Lobby");
    }
}
