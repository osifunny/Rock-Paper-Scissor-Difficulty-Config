using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class AvatarSelector : MonoBehaviour
{
    [SerializeField] Image avatarImage;
    [SerializeField] Sprite[] avatarSprites;
    private int SelectedIndex;

    private void Start(){
        SelectedIndex = PlayerPrefs.GetInt(PropertyNames.Player.AvatarIndex, 0);
        avatarImage.sprite = avatarSprites[SelectedIndex];
        SaveSelectedIndex();
    }

    public void ShiftSelectedIndex(int shift){
        SelectedIndex += shift;
        while(SelectedIndex >= avatarSprites.Length) SelectedIndex -= avatarSprites.Length;
        while(SelectedIndex < 0) SelectedIndex += avatarSprites.Length;
        avatarImage.sprite = avatarSprites[SelectedIndex];
        SaveSelectedIndex();
    }

    private void SaveSelectedIndex(){
        PlayerPrefs.SetInt(PropertyNames.Player.AvatarIndex, SelectedIndex);
        var property = new Hashtable();
        property.Add(PropertyNames.Player.AvatarIndex, SelectedIndex);
        PhotonNetwork.LocalPlayer.SetCustomProperties(property);
    }
}
