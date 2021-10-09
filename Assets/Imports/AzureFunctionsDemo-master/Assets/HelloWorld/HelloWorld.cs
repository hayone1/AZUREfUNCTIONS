using Azure.Functions;
using Azure.AppServices;
using RESTClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HelloWorld : MonoBehaviour {
  [Header("Azure Functions")]
  public string Account = "TelemetryFunctions";
  public string FunctionName = "HttpTrigger2";
  public string FunctionCode = "";
  public string route = "/.auth/login/facebook"; //the route 

  [Header("Sample Values")]
  public string partition = "partitioninTable";
  public string row = "rowinTable";
  
  // public string id_token = "482385619494176";
  public string id_token = "use yout id token, it looks like the comment above";
  // public string access_token = "GGQVlZASXk5T052a3gxYzZAfTWpmTm82XzdHVi1LZAmJkTDZAxZAXFXWHk1OWdyX1ZAWd2lfbG1uQUhDTk1xdEExZAUVrUF9vU0JyeVc3dVc3dDcyZAVpYUnhSX0xHck9rUEFCMnJ4clJBcEZAxaktxdUtJZA3pXSEQ0MXlxLXhfQk9hcG5jR0dDUQZDZD";
  public string access_token = "use your own access token, it looks like the comment above";

  [Header("Unity objects")]
  public TextMesh DisplayName;

  private AzureFunctionClient client;
  private AzureFunction service;

  // Use this for initialization
  void Start() {
    if (string.IsNullOrEmpty(Account)) {
      Debug.LogError("Azure Functions Account name required.");
      return;
    }

    if (DisplayName == null) {
      Debug.LogError("Expected TextMesh game object to be set.");
      return;
    }

    client = AzureFunctionClient.Create(Account);
    service = new AzureFunction(FunctionName, client, FunctionCode);
  }

  public void TappedGet() {
    QueryParams queryParams = new QueryParams();
    queryParams.AddParam("partition", partition);
    queryParams.AddParam("row", row);
    //add authentication tokens
    queryParams.AddParam("id_token", id_token);
    queryParams.AddParam("access_token", access_token);


    Debug.Log("GET: " + partition + " url:" + service.ApiUrl());
    DisplayName.text = "GET";
    StartCoroutine(service.Get<string>(SayHelloCompleted, route, queryParams));
  }

  public void TappedPost() {
    QueryParams queryParams = new QueryParams();
    queryParams.AddParam("name", partition);

    Debug.Log("POST: " + partition + " url:" + service.ApiUrl());
    DisplayName.text = "POST";
    StartCoroutine(service.Post<string>(SayHelloCompleted, route, queryParams));
  }

  public void TappedPut() {
    QueryParams queryParams = new QueryParams();
    queryParams.AddParam("name", partition);

    Debug.Log("PUT: " + partition + " url:" + service.ApiUrl());
    DisplayName.text = "PUT";
    StartCoroutine(service.Put<string>(SayHelloCompleted, route, queryParams));
  }

  public void TappedDelete() {
    QueryParams queryParams = new QueryParams();
    queryParams.AddParam("name", partition);

    Debug.Log("DELETE: " + partition + " url:" + service.ApiUrl());
    DisplayName.text = "DELETE";
    StartCoroutine(service.Delete<string>(SayHelloCompleted, route, queryParams));
  }

  public void TappedPatch() {
    QueryParams queryParams = new QueryParams();
    queryParams.AddParam("name", partition);

    Debug.Log("PATCH: " + partition + " url:" + service.ApiUrl());
    DisplayName.text = "PATCH";
    StartCoroutine(service.Patch<string>(SayHelloCompleted, route, queryParams));
  }

  private void SayHelloCompleted(IRestResponse<string> response) {
    if (response.IsError) {
      Debug.LogError("Request error: " + response.StatusCode);
      return;
    }
    Debug.Log("Completed: " + response.Content);
    DisplayName.text = TrimQuotes(response.Content);
  }

  private string TrimQuotes(string message) {
    return message.TrimStart('"').TrimEnd('"');
  }

  // Update is called once per frame
  void Update() {

  }
}
