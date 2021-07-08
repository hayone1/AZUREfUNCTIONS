using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.EnhancedTouch;
public class UiManager: MonoBehaviour
{
    public float generalUiDelay = 2f;
    //the parents of the pages
    [SerializeField] internal RectTransform ConnectWithFacebook;
    [SerializeField] internal RectTransform BedRoomLights;
    [SerializeField] internal RectTransform OutdoorLights;
    [SerializeField] internal RectTransform LivingRoomAC;
    [SerializeField] internal RectTransform OutdoorSecurity;
    [SerializeField] internal RectTransform DeviceAddedSuccessfully;
    [SerializeField] internal RectTransform GetStartedPage;
    [SerializeField] internal RectTransform ActiveDevices;
    [SerializeField] internal RectTransform HomePage;
    [SerializeField] internal RectTransform MyProfile;
    #region add device buttons
    [SerializeField] private Button AddRpiButton;
    [SerializeField] private Button AddCameraButton;
    #endregion
    #region Outdoor control
    // [SerializeField] internal GameObject doorOpenIcon;
    // [SerializeField] internal Vector3 doorOpenIconPos{get;private set;}
    // [SerializeField] internal GameObject doorClosedIcon;
    // [SerializeField] internal Vector3 doorClosedIconPos{get;private set;}
    // [SerializeField] internal GameObject homeModeIcon;
    // [SerializeField] internal Vector3 homeModeIconPos{get;private set;}
    // [SerializeField] internal GameObject awayModeIcon;
    // [SerializeField] internal Vector3 awayModeIconPos{get;private set;}
    // [SerializeField] internal GameObject dayTimeIcon;
    // [SerializeField] internal Vector3 dayTimeIconPos{get;private set;}
    // [SerializeField] internal GameObject sleepTimeIcon;
    // [SerializeField] internal Vector3 sleepTimeIconPos{get;private set;}
    #endregion

    #region telemetry display ui
    [SerializeField] internal Text temperatureDisplay;
    [SerializeField] internal Text humidityDisplay;
    [SerializeField] internal UiToggleControl motionSensorButton;
    //for theslide guinternal ys, they automatically refernce their twin
    [SerializeField] internal UiSlideControl atHomeSlideButton;
    [SerializeField] internal UiSlideControl dayTimeSlideButton;
    [SerializeField] internal UiSlideControl lockDoorSlideButton;
    // [SerializeField] internal UiButtonControl doorUnlockedButton;
    [SerializeField] internal UiButtonControl doorUnlockedButton;
    [SerializeField] internal UiToggleControl toggleRoomLightButton;
    [SerializeField] internal UiToggleControl toggleOutSideLightButton;
    #endregion

    #region Ui input fields
    public InputAction ActuateAction;
    public event EventHandler<Vector2> OnMove;  //this event can easily be passed around
    //the vector2 would be the current position of the pointer
    #endregion
    
    private void Start() {  //this is the place to start reading control from
        //get the starting positins of the icons
        // doorOpenIconPos = doorOpenIcon.transform.position;
        // doorClosedIconPos = doorClosedIcon.transform.position;
        // homeModeIconPos = homeModeIcon.transform.position;
        // awayModeIconPos = awayModeIcon.transform.position;
        // dayTimeIconPos = dayTimeIcon.transform.position;
        // sleepTimeIconPos = sleepTimeIcon.transform.position;

        //generally this should also be disabled from the editor
        AddRpiButton.interactable = false;
        AddCameraButton.interactable = false;
        AddRpiButton.transform.localScale = Vector3.zero;
        AddCameraButton.transform.localScale = Vector3.zero;

        if (PlayerPrefs.GetInt(Messsages.NewUser, 1) == 1){ //check if its new user
            MovetoFocus(GetStartedPage);    //then take the person to get started page
            //this is where the user journey begins and the next control falls the the "getstarted" button
            //which then takes user to userProfile page
            //user profile submission takes user to login page
            //by default the user is taken to login page if its not a new user
            //when user clicks login, we are then taken into MainManager FBLogin method
        }
        else {  //also check custom Mqtt start() as that's where the telemetry devices dict is extracted from if existing user
            MovetoFocus(ConnectWithFacebook);  //take user to loginPage
        }

        ActuateAction.performed += (context) => {
            if (Touchscreen.current.primaryTouch.isInProgress){
                OnContactDelta(Touchscreen.current.primaryTouch.position.ReadValue());
                }
        };
        ActuateAction.Enable();
    }
    public void MovetoFocus(RectTransform page_to_move) //thisis the version set from the editor
    {
        page_to_move.anchoredPosition = Vector2.zero;
        page_to_move.SetAsLastSibling();
    }


    #region the codes in this region can be better really
    internal void AddDeviceUI(string deviceType)
    {
        switch (deviceType)
        {
            case Messsages.RpiDeviceFound:
                // AddRpiButton.transform.localscaScale = Vector3.one;  //scale the device for user to see
                AddRpiButton.image.rectTransform.DOScale(Vector3.one, generalUiDelay/4).OnComplete(() => AddRpiButton.interactable = true);
                break;
            case Messsages.CameraDeviceFound:
                // AddCameraButton.transform.localScale = Vector3.one;  //scale the device for user to see
                AddCameraButton.image.rectTransform.DOScale(Vector3.one, generalUiDelay/4).OnComplete(() => AddCameraButton.interactable = true);
                break;
            default:
            break;
        }
    }
    internal void RemoveDeviceUI()
    {
        //scale device discovery ui out of sight
        AddRpiButton.transform.localScale = Vector3.zero;  
        AddCameraButton.transform.localScale = Vector3.zero; 

    }
    #endregion

    internal void UpdateTemperatureUI(double _newTemperature) => temperatureDisplay.text = _newTemperature.ToString();
    internal void UpdateHumidityUI(double _newHumidity) => humidityDisplay.text = _newHumidity.ToString();
    internal void UpdateMotionStateUI(bool _activeState) => motionSensorButton.SetUI(_activeState);
    internal void UpdatePresenceUI(bool _activeState) => atHomeSlideButton.SetUI(_activeState);
    internal void UpdateSleepStateSensorUI(bool _activeState) => dayTimeSlideButton.SetUI(_activeState);
    internal void UpdateDoorStateSensorUI(bool _activeState) => doorUnlockedButton.SetUI(_activeState);
    internal void UpdateDoorStateSensorUI2(bool _activeState) => lockDoorSlideButton.SetUI(_activeState);
    internal void UpdateRoomLightStateUI(bool _activeState) => toggleRoomLightButton.SetUI(_activeState);
    internal void UpdateOutsideLightStateUI(bool _activeState) => toggleOutSideLightButton.SetUI(_activeState);
        

    void OnContactDelta(Vector2 _contactPosition)
    {
        OnMove?.Invoke(this, _contactPosition);
    }

    internal void SwapIconPositions(IUIControl _controlObject)  //of selectables
    {
        // if (_controlObject is UiSlideControl _slideControl){
        //     _slideControl.ToggleUI();
        // }
        // else if (_controlObject is UiToggleControl _toggleControl){
        //     _toggleControl.ToggleUI();
        // }
        _controlObject.ToggleUI();  //toggle back the UI
    }

    
}
