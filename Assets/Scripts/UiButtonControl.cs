using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
public class UiButtonControl : Selectable, IUIControl
{
    // Start is called before the first frame update
    public string _deviceID;   //the device ID associated with the control, there should be a more flexible way to do this

    public string _methodName;   //thhe method to be invoked by this selectable
    public string _methodPayload = "1";   //the method arguement just use one for the sake of passing arguement
    public string _deviceName = "RaspberryPi";   //the device(controller) on which the method is invoked
        public string methodName{get{return _methodName;} set{_methodName = value;}}   //thhe method to be invoked by this selectable
    public string deviceID{get{return _deviceID;} set{_deviceID = value;}}   //thhe method to be invoked by this selectable

    public string methodPayload => _methodPayload;    //the method arguement just use one for the sake of passing arguement
    public string deviceName => _deviceName;   //the device(controller) on which the method is invoked
    public Color myOnColor = Color.white;
    public Text myIndicatorText;
    public string OnText;
    public string OFFText;
    [SerializeField] private UiManager uiManager;
    [SerializeField] private App2Device deviceMethodCaller;
    protected override void Start() {
        if (uiManager == null){
            GameObject.FindObjectOfType<UiManager>();
        }
        if (deviceMethodCaller == null){
            deviceMethodCaller = GameObject.FindObjectOfType<App2Device>();
        }
        this.transition = Transition.None;  //so the color changes dont interfere
        // this.on.AddListener(ToggleUI);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        deviceMethodCaller.InvokeCommandToCloud(this);
        ToggleUI();

    }

    public void ToggleUI()
    {
        if (this.image.color == Color.white){   //if it is on
            this.image.color = Color.black;
            myIndicatorText.text = OFFText;
        }
        else{
            this.image.color = Color.white;
            myIndicatorText.text = OnText;
        }
    }
    public void SetUI(bool _toggleState)
    {
        switch (_toggleState)
        {
            case true:
                this.image.color = Color.white;
                myIndicatorText.text = OnText;
                break;
            case false:
                this.image.color = Color.black;
                myIndicatorText.text = OFFText;
                break;
        }
    }
}
