using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIControl
{
    public string deviceID{get; set;}   //the device ID associated with the control, there should be a more flexible way to do this
    public string methodName{get; set;}   //thhe method to be invoked by this selectable
    public string methodPayload{get; set;}    //the method arguement
    public string deviceName{get; set;}   //the device(controller) on which the method is invoked

    public void ToggleUI();
    public void SetUI(bool _toggleState);
}