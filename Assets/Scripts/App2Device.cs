using Azure.Functions;
using Azure.AppServices;
using RESTClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class App2Device : MonoBehaviour {
  [Header("Azure Functions")]
  public string Account;
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

  // Use this for initialization
  internal void Initialize(AzureFunctionClient _functionClient) {
    //this method is called from Authoo after successful login
    if (string.IsNullOrEmpty(Account)) {
      Debug.LogError("Azure Functions Account name required.");
      return;
    }
    // if (Authorizer == null){
    //   Authorizer = GameObject.FindObjectOfType<Authoo>();  //get reference
    // }
    if (uiManager == null){
      uiManager = GameObject.FindObjectOfType<UiManager>();  //get reference

    }

    // client = AzureFunctionClient.Create(Account);
    azureFunction = new AzureFunction(FunctionName, _functionClient, FunctionCode);
  }

  public void TappedGet() {
    QueryParams queryParams = new QueryParams();
    queryParams.AddParam("MethodName", MethodName);
    queryParams.AddParam("Payload", payload);

    Debug.Log("GET: " + MethodName + " url:" + azureFunction.ApiUrl());
    StartCoroutine(azureFunction.Get<string>(CommandCompleted, route, queryParams));
  }

  public void InvokeCommandToCloud(UiSlideControl _slideControl) {  //tapped post method
  // will be called from ui controller
    QueryParams queryParams = new QueryParams();
    MethodName = _slideControl.methodName;
    payload = _slideControl.methodPayload;
    deviceName = _slideControl.deviceName;
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
      //simple put iconsback to original mode, dont show error
      // uiManager.SwapIconPositions();
      return;
    }
    Debug.Log("Completed: " + response.Content);
    //change ui behaviour to indicate success here
  }

  private string TrimQuotes(string message) {
    return message.TrimStart('"').TrimEnd('"');
  }

  // Update is called once per frame
}
