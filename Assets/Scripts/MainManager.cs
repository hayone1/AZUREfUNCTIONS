using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Facebook.Unity;

public class MainManager : MonoBehaviour
{
    

    // Awake function from Unity's MonoBehavior
    string FBCallback = "https://telemetryfunctions.azurewebsites.net/.auth/login/facebook/callback";
    string TestFBToken = "GGQVlZASXk5T052a3gxYzZAfTWpmTm82XzdHVi1LZAmJkTDZAxZAXFXWHk1OWdyX1ZAWd2lfbG1uQUhDTk1xdEExZAUVrUF9vU0JyeVc3dVc3dDcyZAVpYUnhSX0xHck9rUEFCMnJ4clJBcEZAxaktxdUtJZA3pXSEQ0MXlxLXhfQk9hcG5jR0dDUQZDZD";

    Authoo FunctionAuthorizer;
    void Awake ()
    {
        DontDestroyOnLoad(this.gameObject); //peserve acrossscenes
        // if (!FB.IsInitialized) {
        //     // Initialize the Facebook SDK
        //     FB.Init(InitCallback, OnHideUnity);
        // } else {
        //     // Already initialized, signal an app activation App Event
        //     FB.ActivateApp();
        // }
    }

    //called from inspector button
    public void FBlogin()   //called from inspector button
    {
        FB.Android.RetrieveLoginStatus(LoginStatusCallback);
        
    }
    public void FreshLogin()    //called for first time fb login or reAuth
    {
        var perms = new List<string>(){"public_profile", "email"};
        FB.LogInWithReadPermissions(perms, AuthCallback);
    }

    private void AuthCallback (ILoginResult result) {
        if (FB.IsLoggedIn) {
            // AccessToken class will have session details
            var aToken = Facebook.Unity.AccessToken.CurrentAccessToken;

            //azure function side login
            FunctionAuthorizer.Initialize(aToken.TokenString);

            // Print current access token's User ID
            Debug.Log("FB id is " + aToken.UserId);
            // Print current access token's granted permissions
            foreach (string perm in aToken.Permissions) {
                Debug.Log(perm);
            }
            string logMessage = string.Format(
                "OnInitCompleteCalled IsLoggedIn='{0}' IsInitialized='{1}' with AccessToken '{2}'",
                FB.IsLoggedIn,
                FB.IsInitialized, aToken.TokenString);
        } else {
            //bring up login failed window
            Debug.Log("User cancelled login");
        }
    }
    

    private void LoginStatusCallback(ILoginStatusResult result) {
        if (!string.IsNullOrEmpty(result.Error)) {
            Debug.Log("Error: " + result.Error);
            FreshLogin();
        } else if (result.Failed) {
            Debug.Log("Failure: Access Token could not be retrieved");
            FreshLogin();
        } else {
            // Successfully logged user in
            // A popup notification will appear that says "Logged in as <User Name>"
            Debug.Log("Success: " + result.AccessToken.UserId);
        }
    }

    private void InitCallback ()
    {
        if (FB.IsInitialized) {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            // ...
        } else {
            Debug.LogError("Failed to Initialize the Facebook SDK");
            //singnal for user to check connection then close the app if user exits
        }
    }

    private void OnHideUnity (bool isGameShown)
    {
        if (!isGameShown) {
            // Pause the game - we will need to hide
            Time.timeScale = 0;
        } else {
            // Resume the game - we're getting focus again
            Time.timeScale = 1;
        }
    }
}
