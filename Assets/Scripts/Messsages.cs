using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Messsages
{
    //static class used to pass strings around
    public const string MainSceneName = "MainScene";

    #region Ui Pages Interfaces
    public const string PageConnectWithFacebook = "PageConnectWithFacebook";
    public const string PageBedRoomLights = "PageBedRoomLights";
    public const string PageLivingRoomAC = "PageLivingRoomAC";
    public const string PageOutdoorSecurity = "PageOutdoorSecurity";
    public const string PageDeviceAddedSuccessfully = "PageDeviceAddedSuccessfully";
    public const string PageGetStartedPage = "PageGetStartedPage";
    public const string PageActiveDevices = "PageActiveDevices";
    public const string PageHomePage = "PageHomePage";
    public const string PageMyProfile = "MyProfile";
    #endregion
    public const string NewUser = "NewUser";
    public const string RpiDeviceFound = "RpiDeviceFound";
    public const string CameraDeviceFound = "CameraDeviceFound";
    public const string devicesInfoString = "devicesInfoString";  //key in playerprefs for storage of device info

    #region 
    /*this are the sensing data keys used to call the device methods within the apps, 
    they are actually gotten from the device ID field in the telemetryDatapoint class, 
    this code also needs improvement in associating deviceID with the buttons in the UI*/
    public const string temperatureTelemetry = "tempsens123";
    public const string humidityTelemetry = "humidsens123";
    public const string doorStateTelemetry = "mydoorsensor";
    public const string motionTelemetry = "mymotionsensor";
    public const string bedRoomLightsTelemetry = "mylightsensor1";
    public const string outdoorLightTelemetry = "mylightsensor2";


    #endregion
    
}
