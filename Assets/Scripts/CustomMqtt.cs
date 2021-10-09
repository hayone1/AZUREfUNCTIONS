using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Cysharp.Threading.Tasks;
using M2MqttUnity;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Exceptions;
using uPLibrary.Networking.M2Mqtt.Messages;
using uPLibrary.Networking.M2Mqtt.Session;
using uPLibrary.Networking.M2Mqtt.Utility;
using uPLibrary.Networking.M2Mqtt.Internal;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;
using System.Text;
using System.Net;
using UnityEngine.UI;

public class CustomMqtt : M2MqttUnity.M2MqttUnityClient
{
    // Start is called before the first frame update
    internal MainManager mainManager;    //the app main manager
    private List<string> eventMessages = new List<string>();
    private bool updateUI = false;
    public string GetDeviceIDTopic = "device/startup/broadcast";    //to get the device ID, device is the publisher
    public string IDRequestControlTopic = "Rpi/Request/AuthControl"; //to request that device gives over control to phone 
    // internal Dictionary<string, TelemetryData> mainManager.telemetryDevicesDict = new Dictionary<string, TelemetryData>();

    [SerializeField] private UiManager uiManager;   //set in editor

    protected override void Awake() {
        base.Awake();
        if (mainManager == null){
            mainManager = GameObject.FindObjectOfType<MainManager>();
        }
    }

    protected override void Start() //this is where new user back-end code really starts
    {
        if (PlayerPrefs.GetInt(Messsages.NewUser, 1) == 0){ //if its not a new user
            //get the existing data of devices from file
            string _storedDevicesInfoString = PlayerPrefs.GetString(Messsages.devicesInfoString);
            mainManager.telemetryDevicesDict = JsonConvert.DeserializeObject<Dictionary<string, TelemetryDataPoint<dynamic>>>(_storedDevicesInfoString);
            Debug.Log("stored dict looks partition like: " + mainManager.telemetryDevicesDict[Messsages.myard1].RowKey);
            // the main thing needed from this class here really is the stored Devicesinfo
            //if its a new user, the Uicontroller automatically directs user to get started Page

        }
    }
    public void SetBroker(string _brokerAddress = null, string _brokerPort = null)
    {
        //in case app is allowe to do this manually
        if (brokerAddress != null || brokerAddress != "")
        {
            this.brokerAddress = _brokerAddress;
        }
        if (_brokerPort != null || _brokerPort != "")
        {
            int.TryParse(_brokerPort, out this.brokerPort);
        }
    }
    public void SetEncrypted(bool isEncrypted)
    {//encrypt connection
        this.isEncrypted = isEncrypted;
    }
    protected override void OnConnecting()
    {
        base.OnConnecting();
        Debug.Log("Connecting to broker on " + brokerAddress + ":" + brokerPort.ToString() + "...\n");
        //indicate connecting to network in ui
    }
    protected override void OnConnected()
    {
        base.OnConnected();
        Debug.LogFormat("Connected to {0}:{1}...\n", brokerAddress, brokerPort.ToString());

        // SubscribeTopics();  //subscribe to topics afterwards
        // ConnectionSucceeded?.inv
        //indicate searching for device on the ui
        //send message to device?


    }
    public void RequestDeviceControl() 
     //called from ui in device discovery page, hmm... but shouldnt this indicate the device discovered? Oh well
    //request control of the device by sending my client_ID
    //this page wont appear until after user is loggen in
    {
        if (Application.platform == RuntimePlatform.WindowsEditor){
            //send user email and phone number
            string testID = $"ID:{mainManager.profileDetailsManager.Email.text};{mainManager.profileDetailsManager.PhoneNumber.text}";
            client.Publish(IDRequestControlTopic, System.Text.Encoding.UTF8.GetBytes(testID), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("client ID sent to device: " + testID);
        }
        else if (mainManager.aToken != null){
            //send user ID(gotten from facebook) and phone number the rpi knows how to interpret this
            //the user id could be the unique one gotten from the authorization token to ensure random acounts cannot access the device
            // string _ID = $"ID:{mainManager.aToken.UserId};{mainManager.profileDetailsManager.PhoneNumber.text}";

            //user id could also be the user provided email which is what I went with
            string _ID = $"ID|{mainManager.aToken.UserId}:{mainManager.profileDetailsManager.Email.text};{mainManager.profileDetailsManager.PhoneNumber.text}";
            client.Publish(IDRequestControlTopic, System.Text.Encoding.UTF8.GetBytes(_ID), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("client ID sent to device: " + _ID);
            //after this the device sends the (connectionestablished)telemetry data points dictionary for each device
            // and DecodeMessage looks for Etag in the received telemetry to confirm receiving telemetry
        }
        else{
            Debug.LogError("Facebook token not yet assigned, please sign in");
            //show error modal

        }
        //the device will thud proceed to send the phone the edited classes injected with client id
    }

    protected override void SubscribeTopics()   //automatically done when connected
    {
        //subscribe to topics is called from base.OnConnected afte phone conects to broker
        client.Subscribe(new string[] { GetDeviceIDTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
        // client.Subscribe(new string[] { IDRequestControlTopic }, new byte[] { MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE });
    }

    protected override void UnsubscribeTopics()
    {
        client.Unsubscribe(new string[] { GetDeviceIDTopic });
    }

    protected override void DecodeMessage(string topic, byte[] message)
    //decode is called after messages are recieved in the foreground or background in update
    //when message is received it is put at he frnt of the queue
    //then at the back for processing while another message enters the queue
    {
        //decode message
        string msg = System.Text.Encoding.UTF8.GetString(message);
        Debug.Log("Received: " + msg);
        if (msg.Contains("Etag"))
        {  //if the message received is the telemetry devices dictionary information class
        try{
            // Dictionary<string, object> ok = JsonConvert.DeserializeObject<Dictionary<string, object>>(msg);
            Debug.Log("the object dictionary deserialized");
            mainManager.telemetryDevicesDict = JsonConvert.DeserializeObject<Dictionary<string, TelemetryDataPoint<dynamic>>>(msg);
            //now I've gotten the dictionary
        }
        catch (Exception e){
            Debug.LogError("wrong deserialization occured: " + e.Message);
        }

            //save info on devices
            // string json = JsonConvert.SerializeObject(mainManager.telemetryDevicesDict, Formatting.Indented);
            //hmmm very smart way to store the device info rather than using things like writeallfiles
            PlayerPrefs.SetString(Messsages.devicesInfoString, msg);
            //when the client receives the messages with QOS1, the device is notified
            //then sends ControlAccessGranted string which this devices is listening for and will respond to in this if-else sectino
        }
        else if (msg.Contains("deviceType:"))   //initial broadcast by device
        { //if it is just the device ID
          //visually show device found based on device ID
            //allow user click the device image and bring up dialog of adding device(prsent continuous)
            if (msg.Contains(Messsages.RpiDeviceFound)){    //if it is a raspberry device that is found
                uiManager.AddDeviceUI(Messsages.RpiDeviceFound);    //show the ui in the discover devices
                //control falls the UI for the user to click on the found device to call RequestDeviceControl() here;
            }
            else if (msg.Contains(Messsages.CameraDeviceFound)){
                uiManager.AddDeviceUI(Messsages.CameraDeviceFound);    //show the ui

            }
            //request deice control is called from ui when add device button is tapped
        }
        else if (msg.Contains("ControlAccessGranted")){
            //show that connection is successful this should happen after completely receiving edited device telemetry 
            if (mainManager.telemetryDevicesDict != null){  //that is we authenticated without error
                uiManager.RemoveDeviceUI();
                uiManager.MovetoFocus(uiManager.DeviceAddedSuccessfully);  //indicate to user a successful set-up of device

                PlayerPrefs.SetInt(Messsages.NewUser, 0);   //user is no longer new important pience of code

                //after this point, device store info on its devices and begins to send telemetry to cloud
                //take user to main menu after delay...no return value s needed for fire and forget{.Forget()}
                // StartCoroutine(GenericExecuteAfterDelay<RectTransform>(uiManager.MovetoFocus, uiManager.generalUiDelay, uiManager.HomePage));
                GenericExecuteAfterDelay<RectTransform>(uiManager.MovetoFocus, uiManager.generalUiDelay, uiManager.HomePage).Forget();
                //at this point the logic merges to the main app logic again and client program can start requesting telemetry
                // Invoke(uiManager.MoveToFocus(Messsages.PageHomePage), uiManager.generalUiDelay);//take user to home page
            }
            else{
                //indicate unknown error in ui
                Debug.LogError("Unknown error occured while connecting, please contact support");
            }
        }
        else{
            //do nothing
        }


    }





    protected override void OnConnectionFailed(string errorMessage)
    {
        base.OnConnectionFailed(errorMessage);

        // Debug.Log("CONNECTION FAILED! " + errorMessage);
    }
    protected override void OnDisconnected()
    {
        Debug.Log("disconnected");
        //indicate this in ui
    }

    protected override void OnConnectionLost()
    {
        Debug.Log("disconnected");
        //indicate this in ui

    }
    private async UniTaskVoid GenericExecuteAfterDelay<T>(Action<T> rev, float delay, T arg){
        //fire and forget
        // yield return new WaitForSeconds(delay);
        await UniTask.Delay(TimeSpan.FromSeconds(delay));
        rev(arg);
    }
    // void Start()
    // {

    // }

    // // Update is called once per frame
    // void Update()
    // {

    // }
}
