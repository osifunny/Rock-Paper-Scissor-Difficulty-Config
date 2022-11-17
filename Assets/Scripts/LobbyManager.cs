using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [SerializeField] TMP_InputField newRoomInputField;
    [SerializeField] TMP_Text feedBackText;
    [SerializeField] Button startGameButton;
    [SerializeField] GameObject roomPanel;
    [SerializeField] TMP_Text roomNameText;
    [SerializeField] GameObject roomListObject;
    [SerializeField] GameObject playerListObject;
    [SerializeField] RoomItem roomItemPrefab;
    [SerializeField] PlayerItem playerItemPrefab;
    List<RoomItem> roomItemList = new List<RoomItem>();
    List<PlayerItem> playerItemList = new List<PlayerItem>();
    Dictionary<string,RoomInfo> roomInfoCache = new Dictionary<string, RoomInfo>();

    private void Start(){
        PhotonNetwork.JoinLobby();
        roomPanel.SetActive(false);
    }

    public void ClickCreateRoom(){
        if(newRoomInputField.text.Length < 3){
            feedBackText.text = "Room name min 3 characters";
            return;
        }
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(newRoomInputField.text, roomOptions);
    }

    public void ClickStartGame(string levelName){
        if(PhotonNetwork.IsMasterClient){
            PhotonNetwork.CurrentRoom.IsOpen = false;
            PhotonNetwork.LoadLevel(levelName);
        }
    }

    public void JoinRoom(string roomName){
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnCreatedRoom()
    {
        Debug.Log("Room Created : " + PhotonNetwork.CurrentRoom.Name);
        feedBackText.text = "Room Created : " + PhotonNetwork.CurrentRoom.Name;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Room Created : " + PhotonNetwork.CurrentRoom.Name);
        feedBackText.text = "Room Created : " + PhotonNetwork.CurrentRoom.Name;
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        roomPanel.SetActive(true);

        //update player list
        UpdatePlayerList();

        //set start game button
        SetStartGameButton();
    }

    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        //update player list
        UpdatePlayerList();
    }

    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        //update player list
        UpdatePlayerList();
    }

    public override void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient)
    {
        //set start game button
        SetStartGameButton();
    }

    private void SetStartGameButton(){
        //view only in master client
        startGameButton.gameObject.SetActive(PhotonNetwork.IsMasterClient);

        //interactable button if player > 1
        startGameButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount > 1;
    }

    private void UpdatePlayerList(){
        //destroy semua player item yang ada terlebih dahulu
        foreach (var item in playerItemList) Destroy(item.gameObject);
        playerItemList.Clear();

        //recreate player list
        foreach (var (id, player) in PhotonNetwork.CurrentRoom.Players){
            PlayerItem newPlayerItem = Instantiate(playerItemPrefab, playerListObject.transform);
            newPlayerItem.Set(player);
            playerItemList.Add(newPlayerItem);

            if(player == PhotonNetwork.LocalPlayer)
                newPlayerItem.transform.SetAsFirstSibling();
        }

        //start game dapat diklik ketika mencapai jumlah pemain
        SetStartGameButton();
    }

    public override void OnCreateRoomFailed(short returnCode, string message){
        Debug.Log(returnCode + "," + message);
        feedBackText.text = returnCode.ToString() + ": " + message;
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList){
        foreach (var roomInfo in roomList){
            roomInfoCache[roomInfo.Name] = roomInfo;
        }

        foreach (var item in this.roomItemList){
            Destroy(item.gameObject);
        }

        this.roomItemList.Clear();

        var roomInfoList = new List<RoomInfo>(roomInfoCache.Count);

        //sort yang open lalu close
        foreach(var roomInfo in roomInfoCache.Values){
            if(roomInfo.IsOpen) roomInfoList.Add(roomInfo);
        }

        foreach(var roomInfo in roomInfoCache.Values){
            if(!roomInfo.IsOpen) roomInfoList.Add(roomInfo);
        }

        foreach (var roomInfo in roomInfoList){
            if(roomInfo.MaxPlayers == 0 || !roomInfo.IsVisible) continue;
            var newRoomItem = Instantiate(roomItemPrefab, roomListObject.transform);
            newRoomItem.Set(this, roomInfo);
            roomItemList.Add(newRoomItem);
        }
    }
}