using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

[RequireComponent(typeof(Authoo))]
[RequireComponent(typeof(App2Device))]
[RequireComponent(typeof(UiManager))]
[RequireComponent(typeof(ProfileDetails))]
public class MainManager : MonoBehaviour
{
    

    // Awake function from Unity's MonoBehavior
    string FBCallback = "https://telemetryfunctions.azurewebsites.net/.auth/login/facebook/callback";
    string TestFBToken = "GGQVlZASXk5T052a3gxYzZAfTWpmTm82XzdHVi1LZAmJkTDZAxZAXFXWHk1OWdyX1ZAWd2lfbG1uQUhDTk1xdEExZAUVrUF9vU0JyeVc3dVc3dDcyZAVpYUnhSX0xHck9rUEFCMnJ4clJBcEZAxaktxdUtJZA3pXSEQ0MXlxLXhfQk9hcG5jR0dDUQZDZD";

    [SerializeField] private Authoo functionAuthorizer;
    [SerializeField] private App2Device app2Device;
    [SerializeField] private UiManager uiManager;
    [SerializeField] internal ProfileDetails profileDetailsManager;
    internal AccessToken aToken = null; //to cache the access tokens
    internal Dictionary<string, TelemetryDataPoint<dynamic>> telemetryDevicesDict = new Dictionary<string, TelemetryDataPoint<dynamic>>();
    public float telemetryRequestInterval = 4f;

    void Awake ()
    {
        DontDestroyOnLoad(this.gameObject); //peserve acrossscenes
        if (functionAuthorizer == null){
            functionAuthorizer = GameObject.FindObjectOfType<Authoo>();
        }
        if (app2Device == null){
            app2Device = GameObject.FindObjectOfType<App2Device>();
        }
        if (uiManager == null){
            uiManager = GameObject.FindObjectOfType<UiManager>();
        }
        if (profileDetailsManager == null){
            profileDetailsManager = GameObject.FindObjectOfType<ProfileDetails>();
        }

        
        // if (!FB.IsInitialized) {
        //     // Initialize the Facebook SDK
        //     FB.Init(InitCallback, OnHideUnity);
        // } else {
        //     // Already initialized, signal an app activation App Event
        //     FB.ActivateApp();
        // }
    }
    IEnumerator Start() {   //main update logic of the app used in start to allow non blocking waits
    //this method will be called along side other updates so do not fear
        while (true){

            if (functionAuthorizer.currentAuthUser != null && telemetryDevicesDict.Count != 0){
                //firstly request for device telemetry
                Debug.Log("Fetching Telemetry");
                foreach (var _telemetryData in telemetryDevicesDict)
                {
                    //request for device data, the dictionary is also updated within functionAuthorizer and not here
                    functionAuthorizer.GetSetDeviceData(_telemetryData.Value.deviceId);
                }
                //then alter on screen telemetry here
                uiManager.UpdateTemperatureUI(telemetryDevicesDict[Messsages.mytemperaturesensor].property2);
                uiManager.UpdateHumidityUI(telemetryDevicesDict[Messsages.myhumiditysensor].property2);
                uiManager.UpdateMotionStateUI(telemetryDevicesDict[Messsages.mymotionsensor].property2);
                // bool presence = telemetryDevicesDict[Messsages.mymotionsensor].Misc == Messsages.homeMode;
                uiManager.UpdatePresenceUI(telemetryDevicesDict[Messsages.mymotionsensor].Misc == Messsages.homeMode);
                uiManager.UpdateSleepStateSensorUI(telemetryDevicesDict[Messsages.myrpi].Misc == Messsages.awakeMode);
                uiManager.UpdateDoorStateSensorUI(telemetryDevicesDict[Messsages.mydoorcontroller].property2 > 0);
                uiManager.UpdateDoorStateSensorUI2(telemetryDevicesDict[Messsages.mydoorcontroller].property2 > 0);
                uiManager.UpdateRoomLightStateUI(telemetryDevicesDict[Messsages.mylightsensor1].property2);
                uiManager.UpdateOutsideLightStateUI(telemetryDevicesDict[Messsages.mylightsensor2].property2);
            }
            yield return new WaitForSeconds(telemetryRequestInterval);
        }
    }

    // private void Update() { //main update logic of the app
    //     if (functionAuthorizer.currentAuthUser != null && telemetryDevicesDict.Count != 0){
    //         //firstly request for device telemetry
    //         foreach (var _telemetryData in telemetryDevicesDict)
    //         {
    //             //request for device data, the dictionary is also updated within functionAuthorizer and not here
    //             functionAuthorizer.GetSetDeviceData(_telemetryData.Value.deviceId);
    //         }
    //         //then alter on screen telemetry here
    //     }
    // }

    //this is where are concern ourselves with when user reaches the login page
    public void FBlogin()   //called from facebook login scene button 
    {
        //the aim of this login us to move to login into azure function
        FB.Android.RetrieveLoginStatus(LoginStatusCallback);    //check id user has loggen in before
        
    }
    public void FreshLogin()    //called for first time fb login or reAuth
    {
        var perms = new List<string>(){"public_profile", "email"};  //recieve user profileprofile and email
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    private void AuthCallback (ILoginResult result) {
        if (FB.IsLoggedIn) {
            // AccessToken class will have session details
            aToken = Facebook.Unity.AccessToken.CurrentAccessToken;
            //azure function side login
            functionAuthorizer.Initialize(aToken.TokenString);  
            //both existing and fresh login will converger at functionAuthorizer.initialize
            //so we move over to Authoo class


            // Print current access token's User ID
            Debug.Log("FB id is " + aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions) {
                Debug.Log("permissions granted by FB is: " + perm);
            }
            string logMessage = string.Format(
                "OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}' with AccessToken '{2}'",
                FB.IsLoggedIn,
                FB.IsInitialized, aToken.TokenString);
                Debug.Log(logMessage);
        } else {
            //bring up login failed window
            Debug.LogError("User cancelled login or error has occured");
        }
    }
    

    private void LoginStatusCallback(ILoginStatusResult result) {
        //perform fresh login if user hasnt logged in before
        //the aim of this login us to move to login into azure function
        if (!string.IsNullOrEmpty(result.Error)) {
            Debug.Log("Error: " + result.Error);
            FreshLogin();
        } else if (result.Failed) {
            Debug.Log("Failure: Access Token could not be retrieved");
            FreshLogin();
        } else {
            // Successfully logged user in
            // A popup notification will appear that says "Logged in as <User Name>"
            aToken = result.AccessToken;
            functionAuthorizer.Initialize(aToken.TokenString);
            //so we move over to Authoo class

            Debug.Log(string.Format("check if same: from FB:{0} and /n result:{1} ", Facebook.Unity.AccessToken.CurrentAccessToken.TokenString, result.AccessToken.TokenString));

            // move azure function side login
            Debug.Log("Existing login Success: " + result.AccessToken.UserId);
        }
    }


}
