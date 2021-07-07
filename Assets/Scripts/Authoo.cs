using Azure.Functions;
using Azure.AppServices;
using RESTClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using RaspberryDevices;
public class Authoo : MonoBehaviour
{
  //used for logging in and requesting device telemetry
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
        internal AzureFunctionClient functionClient;

        [Header("Query Parameters")]
        //these 2 properties partition and row identify each device uniquely to get its data
        public string deviceType = "doorsensor";
        public string DeviceUniqueID = "doorsens123";

        [SerializeField] private UiManager uiManager;
        [SerializeField] private CustomMqtt customMqtt;
        [SerializeField] private App2Device app2Device;
    // Start is called before the first frame update
    void Start()
    { 
      if (uiManager == null){
          uiManager = GameObject.FindObjectOfType<UiManager>();
      }
      if (customMqtt == null){
          customMqtt = GameObject.FindObjectOfType<CustomMqtt>();
      }
      if (app2Device == null){
          app2Device = GameObject.FindObjectOfType<App2Device>();
      }
        
        // serviceClient = new AppServiceClient(url);  //iniialize
    }

    public void Initialize(string _accessToken = null)
    {
        //firstly get facebook token
        functionClient = AzureFunctionClient.Create(Account);
        if (Application.platform == RuntimePlatform.WindowsEditor) {  //use the Fake accessToken
            Debug.Log("I used thefake access token");
            StartCoroutine(functionClient.LoginWithFacebook(access_token, OnInitializeCompleted));
        }
        else if (string.IsNullOrEmpty(_accessToken) || _accessToken == ""){  //use the real accessToken
            Debug.Log("I used the real access token" + _accessToken);
            StartCoroutine(functionClient.LoginWithFacebook(_accessToken, OnInitializeCompleted));
        }
        else {
          Debug.LogError("unable to initialize azure function connection, token is empty or invalid");
        }
        Debug.Log("GET: " + id_token + " url:" + functionClient.Url);
    }
    public void GetDeviceData(string telemetryData)
    {

        QueryParams queryParams = new QueryParams();
        //add specific device info
        queryParams.AddParam("partition", customMqtt.telemetryDevicesDict[telemetryData].PartitionKey); 
        queryParams.AddParam("row", customMqtt.telemetryDevicesDict[telemetryData].RowKey);


        StartCoroutine(azureFunction.Post<string>(OnTelemetryReceived, route, queryParams));


    }

    private void OnTelemetryReceived(IRestResponse<string> response) {
      if (response.IsError) {
        Debug.LogError("Request error: " + response.StatusCode);
        return;
      }
      Debug.Log("Telemetry receive Completed content: " + response.Content);
      Debug.Log("Telemetry receive Completed Data: " + response.Data);
      //this is what is received when the table is queried
      TelemetryDataPoint receivedTelemetry = JsonConvert.DeserializeObject<TelemetryDataPoint>(response.Data as string);
    // DisplayName.text = TrimQuotes(response.Content);
  }

    

    private void OnInitializeCompleted(IRestResponse<AuthenticatedUser> authUser) {
        if (authUser.IsError) {
        Debug.LogError("Request error: " + authUser.StatusCode);
        //show error modal window
        return;
        }
        Debug.Log("Completed: " + authUser.Content);
        Debug.Log("Completed url is : " + authUser.Url);
        Debug.Log("Completed auth token is : " + authUser.Data.authenticationToken);
        Debug.Log("Completed Data is : " + authUser.ToString());

        //initialize the azurefunction after user is loggen in
        azureFunction = new AzureFunction(FunctionName, functionClient, FunctionCode);
        app2Device.Initialize(functionClient);
        //take user to home page
        if (PlayerPrefs.GetInt(Messsages.NewUser, 1) == 1){ //if its a new user
          //take the user to discover devices
          customMqtt.Connect(); //start connection to broker on network
          uiManager.MovetoFocus(uiManager.ActiveDevices); 
        }
        else{ //if its an existing user
          //take user to home page
        uiManager.MovetoFocus(uiManager.HomePage);

        }
        
        // DisplayName.text = TrimQuotes(authUser.Content);
    }
  }
