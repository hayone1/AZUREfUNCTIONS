using Azure.Functions;
using Azure.AppServices;
using RESTClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class Authoo : MonoBehaviour
{
      public string id_token = "482385619494176";
  // public string access_token = "GGQVlZASXk5T052a3gxYzZAfTWpmTm82XzdHVi1LZAmJkTDZAxZAXFXWHk1OWdyX1ZAWd2lfbG1uQUhDTk1xdEExZAUVrUF9vU0JyeVc3dVc3dDcyZAVpYUnhSX0xHck9rUEFCMnJ4clJBcEZAxaktxdUtJZA3pXSEQ0MXlxLXhfQk9hcG5jR0dDUQZDZD";
  public string access_token = "EAAEfUTvYrMcBAGJiEcmCxgFo3WSxA7EKv6Mw5v7IHpOIuA9Ij8rhVpw0D93Ws5QtKRWAhCiRbgza9cxMOsOZAHQn0KuTz2shCwFLeTEO8BRqRUK4z0rZB1h75H8x2RNlFJ7pKZC2irvTrR0DUjDKH4DCx0h1ZCTFrZBeG6cAo9lSVbjjhqu56GjDT82QZA0JW5WXaZAHFw8RWKWUawhdglw";
          public string Account = "TelemetryFunctions";
        public string FunctionName = "HttpTrigger2";
        public string FunctionCode = "";
        public string url = "https://telemetryfunctions.azurewebsites.net/.auth/login/facebook";
         public string route = "/.auth/login/facebook"; //the route 
        //  private AzureFunctionClient client;
        private AzureFunction azureFunction;
        private AppServiceClient serviceClient;
        private AzureFunctionClient functionClient;

        [Header("Query Parameters")]
        //these 2 properties partition and row identify each device uniquely to get its data
        public string deviceType = "doorsensor";
        public string DeviceUniqueID = "doorsens123";
    // Start is called before the first frame update
    void Start()
    {
        
        // serviceClient = new AppServiceClient(url);  //iniialize
    }

    public void Initialize(string _accessToken = null)
    {
        //firstly get facebook token
        functionClient = AzureFunctionClient.Create(Account);
        // if (string.IsNullOrEmpty(_accessToken)){  //use the real accessToken
        //     Debug.Log("I used the real access token" + _accessToken);
        //     StartCoroutine(functionClient.LoginWithFacebook(_accessToken, OnInitializeCompleted));
        // }
        // else {  //use the Fake accessToken
            Debug.Log("I usedthefake access token");
            StartCoroutine(functionClient.LoginWithFacebook(access_token, OnInitializeCompleted));
        // }
        Debug.Log("GET: " + id_token + " url:" + functionClient.Url);
    }
    public void GetDeviceData()
    {

        QueryParams queryParams = new QueryParams();
        //add specific device info
        queryParams.AddParam("partition", deviceType);
        queryParams.AddParam("row", DeviceUniqueID);


        StartCoroutine(azureFunction.Post<string>(SayHelloCompleted, route, queryParams));


    }

    private void SayHelloCompleted(IRestResponse<string> response) {
    if (response.IsError) {
      Debug.LogError("Request error: " + response.StatusCode);
      return;
    }
    Debug.Log("Completed: " + response.Content);
    // DisplayName.text = TrimQuotes(response.Content);
  }

    

    private void OnInitializeCompleted(IRestResponse<AuthenticatedUser> authUser) {
        if (authUser.IsError) {
        Debug.LogError("Request error: " + authUser.StatusCode);
        return;
        }
        Debug.Log("Completed: " + authUser.Content);
        Debug.Log("Completed url is : " + authUser.Url);
        Debug.Log("Completed auth token is : " + authUser.Data.authenticationToken);
        Debug.Log("Completed Data is : " + authUser.ToString());

        //initialize the azurefunction after user is loggen in
        azureFunction = new AzureFunction(FunctionName, functionClient, FunctionCode);
        
        // DisplayName.text = TrimQuotes(authUser.Content);
    }
  }
