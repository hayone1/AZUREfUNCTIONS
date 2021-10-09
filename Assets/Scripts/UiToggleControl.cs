using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using DG.Tweening;
public class UiToggleControl : Selectable, IUIControl
{
    #region for toggle buttons
    public string _deviceID;   //the device ID associated with the control, there should be a more flexible way to do this

    public string _methodName;   //thhe method to be invoked by this selectable
    public string _methodPayload = "1";   //the method arguement just use one for the sake of passing arguement
    public string _deviceName = "RaspberryPi";   //the device(controller) on which the method is invoked
    public string methodName{get{return _methodName;} set{_methodName = value;}}   //thhe method to be invoked by this selectable
    public string deviceID{get{return _deviceID;} set{_deviceID = value;}}   //thhe method to be invoked by this selectable

    public string methodPayload => _methodPayload;    //the method arguement just use one for the sake of passing arguement
    public string deviceName => _deviceName;   //the device(controller) on which the method is invoked
    [SerializeField] UiManager uiManager;
    [SerializeField] private Text indicatorText;
    [SerializeField] private Image indicator;  //for icons with indicators
    public Color indicatorOnColor = Color.blue;
    [SerializeField] private RectTransform indicatorPos1;  //for icons with indicators
    [SerializeField] private RectTransform indicatorPos2;  //for icons with indicators
    [SerializeField] private App2Device deviceMethodCaller;
    #endregion
    // Start is called before the first frame update

    protected override void Start() {
        if (uiManager == null){
            GameObject.FindObjectOfType<UiManager>();
        }
        if (deviceMethodCaller == null){
            deviceMethodCaller = GameObject.FindObjectOfType<App2Device>();
        }
        // this.onClick.AddListener(OnButtonClick);
    }

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        deviceMethodCaller.InvokeCommandToCloud(this);
        ToggleUI();

    }
    public void ToggleUI()  //toggle to on of off state
    {
        
        if (Mathf.Approximately(indicator.rectTransform.anchoredPosition.x, indicatorPos1.anchoredPosition.x)){
            indicator.rectTransform.DOAnchorPosX(indicatorPos2.anchoredPosition.x, uiManager.generalUiDelay/4)
            .OnComplete(() => indicator.color = indicatorOnColor);
            // indicator.rectTransform.position = indicatorPos2.position;
            indicatorText.text = "ON";
            Debug.Log("UI toggle button toggled to" + indicatorPos1.position.x);


        }
        else{
            indicator.rectTransform.DOAnchorPosX(indicatorPos1.anchoredPosition.x, uiManager.generalUiDelay/4)
            .OnComplete(() => indicator.color = Color.white);   //move back to original pos
            indicatorText.text = "OFF";
            Debug.Log("UI toggle button toggled to" + indicatorPos2.position.x);
        }
    }
    public void SetUI(bool _toggleState)
    {
        switch (_toggleState)
        {
            case true:
                indicator.rectTransform.DOAnchorPosX(indicatorPos2.anchoredPosition.x, uiManager.generalUiDelay/4)
                .OnComplete(() => indicator.color = indicatorOnColor);
                indicatorText.text = "ON";
                Debug.Log("from case: UI toggle button toggled to" + indicatorPos2.anchoredPosition.x);
                break;
            case false:
                indicator.rectTransform.DOAnchorPosX(indicatorPos1.anchoredPosition.x, uiManager.generalUiDelay/4)
                .OnComplete(() => indicator.color = Color.white);   //move back to original pos
                indicatorText.text = "OFF";
                Debug.Log("from case: UI toggle button toggled to" + indicatorPos1.anchoredPosition.x);
                break;
        }
    }

    // Update is called once per frame
}
