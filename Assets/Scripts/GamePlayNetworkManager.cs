using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GamePlayNetworkManager : MonoBehaviourPunCallbacks
{
    public void BackToMenu(){
        StartCoroutine(BackToLobbyCR());
    }

    IEnumerator BackToMenuCR(){
        PhotonNetwork.Disconnect();
        while(PhotonNetwork.IsConnected) yield return null;
        SceneManager.LoadScene("Menu Scene");
    }

    public void BackToLobby(){
        StartCoroutine(BackToLobbyCR());
    }

    IEnumerator BackToLobbyCR(){
        PhotonNetwork.LeaveRoom();
        while(PhotonNetwork.InRoom || !PhotonNetwork.IsConnectedAndReady) yield return null;
        SceneManager.LoadScene("Lobby");
    }

    public void Replay(){
        if(PhotonNetwork.IsMasterClient){
            var scene = SceneManager.GetActiveScene();
            PhotonNetwork.LoadLevel(scene.name);
        }
    }

    public void Quit(){
        StartCoroutine(QuitCR());
    }

    IEnumerator QuitCR(){
        PhotonNetwork.Disconnect();
        while(PhotonNetwork.IsConnected) yield return null;
        Application.Quit();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer){
        if(PhotonNetwork.CurrentRoom.PlayerCount == 1){
            PhotonNetwork.CurrentRoom.IsVisible = false;
            PhotonNetwork.CurrentRoom.IsOpen = false;
            BackToLobby();
            }
    }
}
