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
    public string _methodName;   //thhe method to be invoked by this selectable
    public string _methodPayload = "1";   //the method arguement just use one for the sake of passing arguement
    public string _deviceName = "RaspberryPi";   //the device(controller) on which the method is invoked
    public string methodName{get{return _methodName;} set{_methodName = value;}}   //thhe method to be invoked by this selectable
    public string methodPayload{get; set;} = "1";    //the method arguement just use one for the sake of passing arguement
    public string deviceName{get; set;} = "RaspberryPi";   //the device(controller) on which the method is invoked
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
        if (Mathf.Approximately(indicator.rectTransform.position.x, indicatorPos1.position.x)){
            indicator.rectTransform.DOAnchorPosX(indicatorPos2.position.x, uiManager.generalUiDelay/4)
            .OnComplete(() => indicator.color = indicatorOnColor);
            indicatorText.text = "ON";


        }
        else{
            indicator.rectTransform.DOAnchorPosX(indicatorPos1.position.x, uiManager.generalUiDelay/4)
            .OnComplete(() => indicator.color = Color.white);   //move back to original pos
            indicatorText.text = "OFF";
        }
    }
    public void SetUI(bool _toggleState)
    {
        switch (_toggleState)
        {
            case true:
                indicator.rectTransform.DOAnchorPosX(indicatorPos2.position.x, uiManager.generalUiDelay/4)
                .OnComplete(() => indicator.color = indicatorOnColor);
                indicatorText.text = "ON";
                break;
            case false:
                indicator.rectTransform.DOAnchorPosX(indicatorPos1.position.x, uiManager.generalUiDelay/4)
                .OnComplete(() => indicator.color = Color.white);   //move back to original pos
                indicatorText.text = "OFF";
                break;
        }
    }

    // Update is called once per frame
}
