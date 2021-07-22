using System;
using Azure.Functions;
using Azure.AppServices;
using RESTClient;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
public class Authoo : MonoBehaviour
{
  //used for logging in and requesting device telemetry
  //dont concern yourself with this guy until you get to the login page 
      public string id_token = "482385619494176";
  // public string access_token = "GGQVlZASXk5T052a3gxYzZAfTWpmTm82XzdHVi1LZAmJkTDZAxZAXFXWHk1OWdyX1ZAWd2lfbG1uQUhDTk1xdEExZAUVrUF9vU0JyeVc3dVc3dDcyZAVpYUnhSX0xHck9rUEFCMnJ4clJBcEZAxaktxdUtJZA3pXSEQ0MXlxLXhfQk9hcG5jR0dDUQZDZD";
  public string access_token = "EAAEfUTvYrMcBAL1CDLeMakebckOmSt8Efdn8eWPXU5LyloskynCtz9ruEDiYOAt5h3Sj5TVy42ZCTZBHRXcVsZBs6rjyY9cpQHMBKIuF98hiZAqbml1TCe0WI7ZC7tHXi2pPZBfsPsSWMizkmUboQG4NTv78CZBkmjESSulCfEEfqAbPv9yLuIzHmYQf8VoDVZCjENPnCeUN5okcpj5jFrdZA7dx21Cl96AZBLQLChmmIRbMl4kGbd7F8T";
          public string Account = Messsages.TelemetryFunctions;
        public string FunctionName = "HttpTrigger2";  //couldbe alteredin editor
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
        [SerializeField] internal App2Device app2Device;
        internal IRestResponse<AuthenticatedUser> currentAuthUser;
    // Start is called before the first frame update
    void Awake()
    {   //this is not where initializatin takes place
      if (uiManager == null){
          uiManager = GameObject.FindObjectOfType<UiManager>();
      }
      if (customMqtt == null){
          customMqtt = GameObject.FindObjectOfType<CustomMqtt>();
      }
      if (app2Device == null){
          app2Device = GameObject.FindObjectOfType<App2Device>();
      }
      
      functionClient = AzureFunctionClient.Create(Account);
        // serviceClient = new AppServiceClient(url);  //iniialize
    }

    public void Initialize(string _accessToken = null)  //after user logs in, authorize with azure functions
    {
        //firstly get facebook token
        // functionClient = AzureFunctionClient.Create(Account);
        // if (Application.platform == RuntimePlatform.WindowsEditor) {  //use the Fake accessToken
        //     Debug.Log("I used thefake access token"); //which needs to be renewed from the graph api site for testing
        //     StartCoroutine(functionClient.LoginWithFacebook(access_token, OnInitializeCompleted));
        // }
        if (!string.IsNullOrEmpty(_accessToken) || _accessToken != ""){  //use the real accessToken
            Debug.Log("I used the real access token" + _accessToken);
            StartCoroutine(functionClient.LoginWithFacebook(_accessToken, OnInitializeCompleted));
        }
        else {
          Debug.LogError("unable to initialize azure function connection, token is empty or invalid");
        }
        Debug.Log("GET: " + id_token + " url:" + functionClient.Url);
    }
    public void GetSetDeviceData(string deviceID)
    {

        QueryParams queryParams = new QueryParams();
        //add specific device info
        TelemetryDataPoint<dynamic> _telemetry = customMqtt.mainManager.telemetryDevicesDict[deviceID];
        queryParams.AddParam("partition", customMqtt.mainManager.telemetryDevicesDict[deviceID].PartitionKey); 
        queryParams.AddParam("row", customMqtt.mainManager.telemetryDevicesDict[deviceID].RowKey);
        // queryParams.AddParam("partition", "doorsensor"); 
        // queryParams.AddParam("row", "doorsens123");
        // Debug.Log($" receivedID is {deviceID} \n stored ard1 dict looks row like: {customMqtt.mainManager.telemetryDevicesDict[Messsages.myard1].RowKey}\n but app is showing {customMqtt.mainManager.telemetryDevicesDict[deviceID].RowKey}");
        Debug.Log($"query params are {queryParams.ToString()}");

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
      try{
        
        // TelemetryDataPoint<dynamic> receivedTelemetry = new TelemetryDataPoint<dynamic>();
        var jsonResult = JsonConvert.DeserializeObject(response.Content).ToString();
        TelemetryDataPoint<dynamic> receivedTelemetry = JsonConvert.DeserializeObject<TelemetryDataPoint<dynamic>>(jsonResult);
        // var receivedTelemetry = JsonConvert.DeserializeObject<TelemetryDataPoint<dynamic>>(response.Content);
        customMqtt.mainManager.telemetryDevicesDict[receivedTelemetry.deviceId] = receivedTelemetry;
      }
      catch (Exception e){
        Debug.LogWarning("unexpected message received " + e.Message);
      }
      
      //very crucial code

      //update the devices info in Ui
    // DisplayName.text = TrimQuotes(response.Content);
  }

    

    private void OnInitializeCompleted(IRestResponse<AuthenticatedUser> authUser) {
        if (authUser.IsError) {
        Debug.LogError("Azure Function Request error: " + authUser.StatusCode);
        //show error modal window
        return;
        }
        Debug.Log("Completed: " + authUser.Content);
        Debug.Log("Completed url is : " + authUser.Url);
        Debug.Log("Completed auth token is : " + authUser.Data.authenticationToken);
        Debug.Log("Completed Data is : " + authUser.ToString());

        //initialize the azurefunctions after user is loggen in
        //this is not the only azure function used in the whole app, this is for HTTP trigger 2: RequestTelemetry
        azureFunction = new AzureFunction(FunctionName, functionClient, FunctionCode);
        app2Device.Initialize(functionClient);  //initialize HTTPtrigger3 app2device commands capability
        currentAuthUser = authUser; //allow mainmanager to check this to allow update to run

        if (PlayerPrefs.GetInt(Messsages.NewUser, 1) == 1){ //if its a new user
          //take the user to discover devices page and
          uiManager.MovetoFocus(uiManager.ActiveDevices);
          customMqtt.Connect(); //start connection to broker on network
          //what happens after this point? 
        }
        else{ //if its an existing user
          //take user to home page, this is the final aim of all this login
        uiManager.MovetoFocus(uiManager.HomePage);
        //after this point, the client app can start requesting telemetry

        }
        
        // DisplayName.text = TrimQuotes(authUser.Content);
    }
  }
