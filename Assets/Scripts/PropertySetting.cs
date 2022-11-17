using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PropertySetting : MonoBehaviourPunCallbacks
{
    [SerializeField] Slider slider;
    [SerializeField] TMP_InputField inputField;
    [SerializeField] string propertyKey;
    [SerializeField] float initialValue = 50;
    [SerializeField] float minValue = 0;
    [SerializeField] float maxValue = 100;
    [SerializeField] bool wholeNumbers = true;

    private void Start(){
        //setup semua UI
        slider.minValue = minValue;
        slider.maxValue = maxValue;
        slider.wholeNumbers = wholeNumbers;
        inputField.contentType = wholeNumbers? TMP_InputField.ContentType.IntegerNumber: TMP_InputField.ContentType.DecimalNumber;

        //ambil initial value dari server kalau ada
        if(PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(propertyKey, out var value))
            UpdateSliderInputField((float) value);
        else UpdateSliderInputField(initialValue);

        //UI hanya interactable untuk master
        if(!PhotonNetwork.IsMasterClient){
            slider.interactable = false;
            inputField.interactable = false;
        }
    }

    public void InputFromSlider(float value){
        if(PhotonNetwork.IsMasterClient){
            UpdateSliderInputField(value);
            SetCustomProperty(value);
        }
    }

    public void InputFromField(string stringValue){
        if(PhotonNetwork.IsMasterClient && float.TryParse(stringValue, out var floatValue)){
            Mathf.Clamp(floatValue, slider.minValue, slider.maxValue);
            UpdateSliderInputField(floatValue);
            SetCustomProperty(floatValue);
        }
    }

    private void SetCustomProperty(float value){
        if(PhotonNetwork.IsMasterClient){
            var property = new Hashtable();
            property.Add(propertyKey, value);
            PhotonNetwork.CurrentRoom.SetCustomProperties(property);
        }
    }

    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if(propertiesThatChanged.TryGetValue(propertyKey, out var value) && !PhotonNetwork.IsMasterClient)
            UpdateSliderInputField((float) value);
    }

    private void UpdateSliderInputField(float value){
        var floatValue = (float) value;
        slider.value = floatValue;
        if(wholeNumbers)
            inputField.text = (Mathf.RoundToInt(floatValue)).ToString("D");
        else
            inputField.text = floatValue.ToString("F2");
    }
}
