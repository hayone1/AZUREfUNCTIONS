using Azure.Functions;
using Azure.AppServices;
using RESTClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class App2Device : MonoBehaviour {
  [Header("Azure Functions")]
  public string Account = Messsages.TelemetryFunctions;
  public string FunctionName = "HttpTrigger3";
  public string FunctionCode = "";
  private string route = "";

  // [Header("Sample Values")]
  private string MethodName = "ControlDevice";
  private string payload = "5";
  private string deviceName = "lightSensor1";


  // private AzureFunctionClient client;
  private AzureFunction azureFunction;
  // Authoo Authorizer;
  // string currentMethod; //string that hold the curent device method to be invoked
  UiManager uiManager;
  IUIControl currentControlSelectable;  //for manipulating the ui selectable controls

  private void Awake() {
    if (uiManager == null){
      uiManager = GameObject.FindObjectOfType<UiManager>();  //get reference

    }
  }

  // Use this for initialization of HTTP trigger 3
  internal void Initialize(AzureFunctionClient _functionClient) {
    //this method is called from Authoo after successful login
    if (string.IsNullOrEmpty(Account)) {
      Debug.LogError("Azure Functions Account name required.");
      return;
    }
    //azure function HTTP trrigger3
    azureFunction = new AzureFunction(FunctionName, _functionClient, FunctionCode);
    //get back to Authoo for logic continuation
  }

  public void TappedGet() {
    QueryParams queryParams = new QueryParams();
    queryParams.AddParam("MethodName", MethodName);
    queryParams.AddParam("Payload", payload);

    Debug.Log("GET: " + MethodName + " url:" + azureFunction.ApiUrl());
    StartCoroutine(azureFunction.Get<string>(CommandCompleted, route, queryParams));
  }

  public void InvokeCommandToCloud(IUIControl _controlObject) {  //tapped post method
  // will be called from ui controller
  currentControlSelectable = _controlObject;
    MethodName = _controlObject.methodName;
    payload = _controlObject.methodPayload;
    deviceName = _controlObject.deviceName;
    QueryParams queryParams = new QueryParams();
    queryParams.AddParam("MethodName", MethodName); //add arguement
    queryParams.AddParam("Payload", payload); //add arguement
    queryParams.AddParam("deviceName", deviceName); //add arguement

    Debug.Log("POST: " + MethodName + " url:" + azureFunction.ApiUrl());
    //alongside invoking device methd, it also swaps the UI
    StartCoroutine(azureFunction.Post<string>(CommandCompleted, route, queryParams));
  }




  private void CommandCompleted(IRestResponse<string> response) {
    if (response.IsError) {
      Debug.LogError("Request error: " + response.StatusCode);
      uiManager.SwapIconPositions(currentControlSelectable);  //swap back to original position to indicate failure
      return;
    }
    if (response.Content.Contains("400")){
      Debug.LogError("invalid request: " + response.StatusCode);
       uiManager.SwapIconPositions(currentControlSelectable);  //swap back to original position to indicate failure
      return;
    }
    Debug.Log("Completed: " + response.Content);
    if (response.Content.Contains("200")){
      //we bless God
    }
    //change ui behaviour to indicate success here
  }

  private string TrimQuotes(string message) {
    return message.TrimStart('"').TrimEnd('"');
  }

  // Update is called once per frame
}
