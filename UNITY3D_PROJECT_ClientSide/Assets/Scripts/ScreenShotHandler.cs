using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ScreenShotHandler : MonoBehaviour {
    // Start is called before the first frame update
    private static ScreenShotHandler instance;
    private Camera myCamera;
    private bool take_screenshot_on_next_frame;
    private int screenShotNumber = 0; //keep track of and assign the number of screenshots
    public int Width = 1080;
    public int Height = 2160;
    void Awake () {
        instance = this;
        myCamera = gameObject.GetComponent<Camera> ();
    }
    
    private void Update() {
        if (Keyboard.current.sKey.isPressed){
            // takeScreenshot(Width, Height);
            takeScreenshot(Screen.width, Screen.height);
        }
    }

    private void OnPostRender () {
        if (take_screenshot_on_next_frame) {
            take_screenshot_on_next_frame = false;
            RenderTexture _renderTexture = myCamera.targetTexture;

            Texture2D renderResult = new Texture2D (_renderTexture.width, _renderTexture.height, TextureFormat.ARGB32, false);
            Rect _rect = new Rect (0, 0, _renderTexture.width, _renderTexture.height);
            renderResult.ReadPixels (_rect, 0, 0);

            byte[] byteArray = renderResult.EncodeToPNG ();
            string filePath = string.Format ("Assets/Screenshots/screenShot{0}.png ", screenShotNumber);
            System.IO.File.WriteAllBytes (filePath, byteArray);
            Debug.Log ("saved camera screenShot ");

            RenderTexture.ReleaseTemporary (_renderTexture);
            myCamera.targetTexture = null;

            screenShotNumber++; //increment for next screen shot

        }
    }

    // Update is called once per frame
    void TakeShot (int width, int height) {
        myCamera = gameObject.GetComponent<Camera> ();
        myCamera.targetTexture = RenderTexture.GetTemporary (width, height, 16);
        take_screenshot_on_next_frame = true;
    }

    public static void takeScreenshot (int width, int height) {
        instance.TakeShot (width, height);
    }
}