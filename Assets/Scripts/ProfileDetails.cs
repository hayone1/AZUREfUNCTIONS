using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class ProfileDetails : MonoBehaviour
{
    //user profile submission takes user to login page
    [SerializeField] internal InputField UserName;
    [SerializeField] internal InputField PhoneNumber;
    [SerializeField] internal InputField Email;
    [SerializeField] private Button saveProfileButton;
    [SerializeField] private Text NameDisplay;

    (string userName, string phoneNumber, string email) UserDetails = ("test", "test", "test");
    // private Dictionary<string, string>

    private void Start() {
        //deactivate the save profile button
        saveProfileButton.interactable = false;
        saveProfileButton.image.color = Color.black;

        PhoneNumber.characterValidation = InputField.CharacterValidation.Decimal;
        Email.characterValidation = InputField.CharacterValidation.EmailAddress;
        UserName.characterValidation = InputField.CharacterValidation.Alphanumeric;

        UserName.onValueChanged.AddListener(AllowEditSubmit);
        Email.onValueChanged.AddListener(AllowEditSubmit);
        PhoneNumber.onValueChanged.AddListener(AllowEditSubmit);
        UserName.onValueChanged.AddListener(ChangeDisplayedName);
    }

    private void AllowEditSubmit(string changes)
    {
        if (!saveProfileButton.IsInteractable()){
        saveProfileButton.image.color = Color.white;
        saveProfileButton.interactable = true;

        }
    }
    private void DisallowEditSubmit()    //called after save profile is clicked
    {
        if (saveProfileButton.IsInteractable()){
        saveProfileButton.interactable = false;
        saveProfileButton.image.color = Color.black;

        }
    }
    private void ChangeDisplayedName(string currentName)
    {
        NameDisplay.text = currentName;
    }

    

    public void StoreUserDetails()
    {
        UserDetails.userName = UserName.text;
        UserDetails.phoneNumber = PhoneNumber.text;
        UserDetails.email = Email.text;
        var _serializedDetails = JsonConvert.SerializeObject(UserDetails);
        PlayerPrefs.SetString(Messsages.UserDetails, _serializedDetails);
    }

    (string userName, string phoneNumber, string email) RetrieveUserDetails()
    {
        var _serializedDetails = PlayerPrefs.GetString(Messsages.UserDetails, String.Empty);
        UserDetails = JsonConvert.DeserializeObject<(string userName, string phoneNumber, string email)>(_serializedDetails);
        return UserDetails;
    }


}
