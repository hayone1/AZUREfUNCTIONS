using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Facebook.Unity;
//most overal app scripts are attached to th scene's event system
public class Initializer : MonoBehaviour
{
    void Awake ()
    {
        DontDestroyOnLoad(this.gameObject); //peserve acrossscenes
        Initialize();   //initialize sdk internet connection with facebook, this is not the same as logging in
        //that is done by main manager
        
    }
    public void Initialize() {  //nintialize the FB SDK
        {
            if (!FB.IsInitialized) {
            // Initialize the Facebook SDK
            FB.Init(InitCallback, OnHideUnity);
            } else {
                // Already initialized, signal an app activation App Event
                FB.ActivateApp();
                SceneManager.LoadScene(Messsages.MainSceneName);
            }
        }
    }
     private void InitCallback ()
    {
        if (FB.IsInitialized) {
            // Signal an app activation App Event
            FB.ActivateApp();
            // Continue with Facebook SDK
            SceneManager.LoadScene(Messsages.MainSceneName);
            // ...
        } else {
            Debug.LogError("Failed to Initialize the Facebook SDK");
            //singnal for user to check connection then close the app if user exits
            //needs modal window
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
