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

    public string methodPayload => _methodPayload;    //the method arguement just use one for the sake of passing arguement
    public string deviceName => _deviceName;   //the device(controller) on which the method is invoked
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
        myDefaultPosition = myCurrentPosition = this.image.rectTransform.anchoredPosition;
        myTwinDefaultPosition = myTwinCurrentPosition = myTwinIcon.image.rectTransform.anchoredPosition;
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        //uiManager.OnMove += OnPointerMove;  //uimanager detects movement

    }
    public override void OnPointerUp(PointerEventData eventData)
    {
        base.OnPointerUp(eventData);
        // uiManager.OnMove -= OnPointerMove;  //unsubscribe
            deviceMethodCaller.InvokeCommandToCloud(this);
            ToggleUI();   //this will be swapped back if command it fails
        // if (this.FindSelectableOnDown() == myTwinIcon || this.FindSelectableOnUp() == myTwinIcon){ 
        //     //if user moved icon to where the twin icon is
        //     Debug.Log("I found my twin: ");
        //     return;
        // }
        // this.image.rectTransform.anchoredPosition = myCurrentPosition; //else go back to stable position
        // Debug.LogWarning("I didnt  find my twin: ");
        //may be swapped with twin icon and myCurrentPosition is altered

    }
    public void OnPointerMove(object sender, Vector2 _newPos)
    {
        // this.image.rectTransform.position = _newPos;
        // Vector2 anchorPos = _newPos - new Vector2(this.image.rectTransform.position.x, this.image.rectTransform.position.y);
        // anchorPos = new Vector2(anchorPos.x / this.image.rectTransform.lossyScale.x, anchorPos.y / this.image.rectTransform.lossyScale.y);
        // // Vector2 normalizedPos = Rect.PointToNormalized(this.image.rectTransform.rect, anchorPos);
        // // this.image.rectTransform.anchoredPosition = transform.InverseTransformPoint(_newPos.x, _newPos.y, 0f);
        // this.image.rectTransform.anchoredPosition = anchorPos;
        Vector2 anchorPos = transform.InverseTransformPoint(_newPos);
        this.image.rectTransform.anchoredPosition = anchorPos;

        Debug.Log("moving slide control to" + anchorPos);
    }

    public void ToggleUI()
    {
        this.image.rectTransform.anchoredPosition = myTwinCurrentPosition;
        myTwinIcon.image.rectTransform.anchoredPosition = myCurrentPosition;

        myCurrentPosition = this.image.rectTransform.anchoredPosition;
        myTwinCurrentPosition = myTwinIcon.image.rectTransform.anchoredPosition;
        Debug.LogWarning("switch ui called");
    }

    //need to implement default states to aloow for correct true false checks
    public void SetUI(bool _toggleState)
    {
            Debug.LogWarning("set ui called");
        switch (_toggleState)
        {   //the ifs are to account for if my tween has already moved me
            case true:
                if (myCurrentPosition != myDefaultPosition){myCurrentPosition = this.image.rectTransform.anchoredPosition = myDefaultPosition;}
                if (myTwinCurrentPosition != myTwinDefaultPosition){myTwinCurrentPosition = myTwinIcon.image.rectTransform.anchoredPosition = myTwinDefaultPosition;}
                
                break;
            case false:
                if (myCurrentPosition != myTwinCurrentPosition){myCurrentPosition = this.image.rectTransform.anchoredPosition = myTwinDefaultPosition;}
                if (myTwinCurrentPosition != myTwinDefaultPosition){myTwinCurrentPosition = myTwinIcon.image.rectTransform.anchoredPosition = myDefaultPosition;}
                
                
                break;
        }
    }

}
