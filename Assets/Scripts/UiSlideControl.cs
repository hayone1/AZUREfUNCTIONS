using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;

public class UiSlideControl : Selectable, IUIControl
{
    // Start is called before the first frame update
    public string _deviceID;   //the device ID associated with the control, there should be a more flexible way to do this
    public string _methodName;   //thhe method to be invoked by this selectable
    public string _methodPayload = "1";   //the method arguement just use one for the sake of passing arguement
    public string _deviceName = "RaspberryPi";   //the device(controller) on which the method is invoked
    public string methodName{get{return _methodName;} set{_methodName = value;}}   //thhe method to be invoked by this selectable
    public string deviceID{get{return _deviceID;} set{_deviceID = value;}}   //thhe method to be invoked by this selectable
    public string methodPayload{get; set;} = "1";    //the method arguement just use one for the sake of passing arguement
    public string deviceName{get; set;} = "RaspberryPi";   //the device(controller) on which the method is invoked
    [SerializeField] private UiManager uiManager;
    [SerializeField] private UiSlideControl myTwinIcon;
    internal Vector3 myCurrentPosition;    //can be altered by UiController during swap
    internal Vector3 myTwinCurrentPosition;    //can be altered by UiController during swap
    //set at the start of app so each icon has its default position
    internal Vector3 myDefaultPosition;    
    internal Vector3 myTwinDefaultPosition; 
    [SerializeField] private App2Device deviceMethodCaller;
    

    

    protected override void Awake()
    {
        if (uiManager == null){
            uiManager = GameObject.FindObjectOfType<UiManager>();
        }
        if (deviceMethodCaller == null){
            deviceMethodCaller = GameObject.FindObjectOfType<App2Device>();
        }
    }

    protected override void Start() 
    {
        //set default and current
        myDefaultPosition = myCurrentPosition = this.transform.position;
        myTwinDefaultPosition = myTwinCurrentPosition = myTwinIcon.transform.position;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        uiManager.OnMove += OnPointerMove;  //uimanager detects movement

    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        uiManager.OnMove -= OnPointerMove;  //unsubscribe
        if (this.FindSelectableOnDown() == myTwinIcon || this.FindSelectableOnUp() == myTwinIcon){ 
            //if user moved icon to where the twin icon is
            deviceMethodCaller.InvokeCommandToCloud(this);
            ToggleUI();   //this will be swapped back if command it fails
            return;
        }
        this.transform.position = myCurrentPosition; //go back to stable position
        //may be swapped with twin icon and myCurrentPosition is altered

    }
    public void OnPointerMove(object sender, Vector2 _newPos)
    {
        this.image.rectTransform.position = _newPos;
    }

    public void ToggleUI()
    {
        this.transform.position = myTwinCurrentPosition;
        myTwinIcon.transform.position = myCurrentPosition;

        myCurrentPosition = this.transform.position;
        myTwinCurrentPosition = myTwinIcon.transform.position;
    }

    //need to implement default states to aloow for correct true false checks
    public void SetUI(bool _toggleState)
    {
        switch (_toggleState)
        {   //the ifs are to account for if my tween has already moved me
            case true:
                if (myCurrentPosition != myDefaultPosition){myCurrentPosition = this.transform.position = myDefaultPosition;}
                if (myTwinCurrentPosition != myTwinDefaultPosition){myTwinCurrentPosition = myTwinIcon.transform.position = myTwinDefaultPosition;}
                
                break;
            case false:
                if (myCurrentPosition != myTwinCurrentPosition){myCurrentPosition = this.transform.position = myTwinDefaultPosition;}
                if (myTwinCurrentPosition != myTwinDefaultPosition){myTwinCurrentPosition = myTwinIcon.transform.position = myDefaultPosition;}
                
                
                break;
        }
    }

}
