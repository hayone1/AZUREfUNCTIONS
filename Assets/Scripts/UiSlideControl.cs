using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Serialization;
using UnityEngine.EventSystems;

public class UiSlideControl : Button
{
    // Start is called before the first frame update
    public string methodName;   //thhe method to be invoked by this selectable
    public string methodPayload;    //the method arguement
    public string deviceName = "RaspberryPi";   //the device(controller) on which the method is invoked

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);

    }

}
