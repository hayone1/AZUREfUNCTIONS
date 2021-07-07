using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
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
    [SerializeField] internal GameObject doorOpenIcon;
    [SerializeField] internal Vector3 doorOpenIconPos{get;private set;}
    [SerializeField] internal GameObject doorClosedIcon;
    [SerializeField] internal Vector3 doorClosedIconPos{get;private set;}
    [SerializeField] internal GameObject homeModeIcon;
    [SerializeField] internal Vector3 homeModeIconPos{get;private set;}
    [SerializeField] internal GameObject awayModeIcon;
    [SerializeField] internal Vector3 awayModeIconPos{get;private set;}
    [SerializeField] internal GameObject dayTimeIcon;
    [SerializeField] internal Vector3 dayTimeIconPos{get;private set;}
    [SerializeField] internal GameObject sleepTimeIcon;
    [SerializeField] internal Vector3 sleepTimeIconPos{get;private set;}
    
    #endregion
    
    private void Start() {
        //get the starting positins of the icons
        doorOpenIconPos = doorOpenIcon.transform.position;
        doorClosedIconPos = doorClosedIcon.transform.position;
        homeModeIconPos = homeModeIcon.transform.position;
        awayModeIconPos = awayModeIcon.transform.position;
        dayTimeIconPos = dayTimeIcon.transform.position;
        sleepTimeIconPos = sleepTimeIcon.transform.position;
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
                AddRpiButton.transform.localScale = Vector3.one;  //scale the device for user to see
                break;
            case Messsages.CameraDeviceFound:
                AddCameraButton.transform.localScale = Vector3.one;  //scale the device for user to see
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

    // Update is called once per frame
    internal void SwapIconPositions(GameObject _iconObject)
    {
        if (_iconObject == doorClosedIcon || _iconObject == doorOpenIcon){
            doorClosedIcon.transform.position = doorOpenIconPos;
            doorOpenIcon.transform.position = doorClosedIconPos;
        }
        else if (_iconObject == homeModeIcon || _iconObject == awayModeIcon){
            homeModeIcon.transform.position = awayModeIconPos;
            awayModeIcon.transform.position = homeModeIconPos;
        }
        else if (_iconObject == dayTimeIcon || _iconObject == sleepTimeIcon){
            dayTimeIcon.transform.position = sleepTimeIconPos;
            sleepTimeIcon.transform.position = dayTimeIconPos;
        }
    }
}
