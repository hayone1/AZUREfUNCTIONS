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

  [Header("Sample Values")]
  public string MethodName = "ControlDevice";
  public string payload = "5";


  // private AzureFunctionClient client;
  private AzureFunction azureFunction;
  Authoo Authorizer;

  // Use this for initialization
  void Start() {
    if (string.IsNullOrEmpty(Account)) {
      Debug.LogError("Azure Functions Account name required.");
      return;
    }
    if (Authorizer == null){
      GameObject.FindObjectOfType<Authoo>();  //get reference
    }


    // client = AzureFunctionClient.Create(Account);
    azureFunction = new AzureFunction(FunctionName, Authorizer.functionClient, FunctionCode);
  }

  public void TappedGet() {
    QueryParams queryParams = new QueryParams();
    queryParams.AddParam("MethodName", MethodName);
    queryParams.AddParam("Payload", payload);

    Debug.Log("GET: " + MethodName + " url:" + azureFunction.ApiUrl());
    StartCoroutine(azureFunction.Get<string>(CommandCompleted, route, queryParams));
  }

  public void InvokeCommandToCloud(string _methodName) {  //tapped post method
    QueryParams queryParams = new QueryParams();
    queryParams.AddParam("name", MethodName); //add arguement

    Debug.Log("POST: " + MethodName + " url:" + azureFunction.ApiUrl());
    StartCoroutine(azureFunction.Post<string>(CommandCompleted, route, queryParams));
  }




  private void CommandCompleted(IRestResponse<string> response) {
    if (response.IsError) {
      Debug.LogError("Request error: " + response.StatusCode);
      //simple put iconsback to original mode, dont show error
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
