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
    [SerializeField] private RectTransform doorOpenIcon;
    [SerializeField] private RectTransform doorClosedIcon;
    [SerializeField] private RectTransform homeModeIcon;
    [SerializeField] private RectTransform awayModeIcon;
    [SerializeField] private RectTransform dayTimeIcon;
    [SerializeField] private RectTransform sleepTimeIcon;
    
    #endregion
    
    public void MovetoFocus(RectTransform page_to_move) //thisis the version set from the editor
    {
        page_to_move.anchoredPosition = Vector2.zero;
        page_to_move.SetAsLastSibling();
    }

    internal void MoveToFocus(string page_to_move)
    {
        switch (page_to_move)
        {
            case Messsages.PageActiveDevices:
                ActiveDevices.anchoredPosition = Vector2.zero;
                ActiveDevices.SetAsLastSibling();
                break;
            case Messsages.PageLivingRoomAC:
                LivingRoomAC.anchoredPosition = Vector2.zero;
                LivingRoomAC.SetAsLastSibling();
                break;
            case Messsages.PageBedRoomLights:
                BedRoomLights.anchoredPosition = Vector2.zero;
                BedRoomLights.SetAsLastSibling();
                break;
            case Messsages.PageConnectWithFacebook:
                ConnectWithFacebook.anchoredPosition = Vector2.zero;
                ConnectWithFacebook.SetAsLastSibling();
                break;
            case Messsages.PageDeviceAddedSuccessfully:
                DeviceAddedSuccessfully.anchoredPosition = Vector2.zero;
                DeviceAddedSuccessfully.SetAsLastSibling();
                break;
            case Messsages.PageGetStartedPage:
                GetStartedPage.anchoredPosition = Vector2.zero;
                GetStartedPage.SetAsLastSibling();
                break;
            case Messsages.PageHomePage:
                HomePage.anchoredPosition = Vector2.zero;
                HomePage.SetAsLastSibling();
                break;
            case Messsages.PageOutdoorSecurity:
                OutdoorSecurity.anchoredPosition = Vector2.zero;
                OutdoorSecurity.SetAsLastSibling();
                break;
            case Messsages.PageMyProfile:
                MyProfile.anchoredPosition = Vector2.zero;
                MyProfile.SetAsLastSibling();
                break;
            default:
                break;
        }
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
    void Update()
    {
        
    }
}
