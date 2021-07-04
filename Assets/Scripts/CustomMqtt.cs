using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
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
using RaspberryDevices;
using UnityEngine.UI;

public class CustomMqtt : M2MqttUnity.M2MqttUnityClient
{
    // Start is called before the first frame update
    MainManager mainManager;    //the app main manager
    private List<string> eventMessages = new List<string>();
    private bool updateUI = false;
    public string GetDeviceIDTopic = "Rpi/DeviceID";    //to get the device ID, device is the publisher
    public string IDRequestControlTopic = "Rpi/Request/Control"; //to request that device gives over control to phone 
    internal Dictionary<string, TelemetryDataPoint> telemetryDevicesDict = new Dictionary<string, TelemetryDataPoint>();

    [SerializeField] private UiManager uiManager;   //set in editor


    //a public broker address fiel exists in base cass 
    //the topic where the rpi device ID is sent
    // public string _brokerAddress
    // {
    //     get {return brokerAddress;}
    //     set {brokerAddress = value;}
    // }
    // public string _brokerPort
    // {
    //     get {return brokerAddress;}
    //     set {brokerAddress = value;}
    // }
    // public bool _isEncrypted
    // {
    //     get {return isEncrypted;}
    //     set {isEncrypted = value;}
    // }
    protected override void Start()
    {
        if (PlayerPrefs.HasKey(Messsages.NewUser) && PlayerPrefs.GetInt(Messsages.NewUser, 1) == 0){
            //get the existing data of devices from file
            telemetryDevicesDict = JsonConvert.DeserializeObject<Dictionary<string, TelemetryDataPoint>>((PlayerPrefs.GetString(Messsages.devicesInfoString)) as string);
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
    public void RequestDeviceControl()  //called from ui
    //request control of the device by sending my client_ID
    //this page wont appear until after user is loggen in
    {
        if (Application.platform == RuntimePlatform.WindowsEditor){
            string testID = "myTestID";
            client.Publish(IDRequestControlTopic, System.Text.Encoding.UTF8.GetBytes(testID), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("client ID sent to device: " + testID);
        }
        else if (mainManager.aToken != null){
            client.Publish(IDRequestControlTopic, System.Text.Encoding.UTF8.GetBytes(mainManager.aToken.UserId), MqttMsgBase.QOS_LEVEL_EXACTLY_ONCE, false);
            Debug.Log("client ID sent to device: " + mainManager.aToken.UserId);
            //after this the device sends the (connectionestablished)telemetry data points for each device
        }
        else{
            Debug.LogError("Facebook token not yet assigned, please sign in");
            //show error modal

        }
        //the device will thud proceed to send the phone the edited classes injected with client id
    }

    protected override void SubscribeTopics()
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
        {  //if the message received is the telemetry devices information class
            var telemetryDevice = JsonConvert.DeserializeObject<TelemetryDataPoint>(msg);
            if (telemetryDevicesDict.ContainsKey(telemetryDevice.deviceId))
            {
                telemetryDevicesDict[telemetryDevice.deviceId] = telemetryDevice;
            }
            else
            {
                telemetryDevicesDict.Add(telemetryDevice.deviceId, telemetryDevice);
            }

            //save info on devices
            string json = JsonConvert.SerializeObject(telemetryDevicesDict, Formatting.Indented);
            //hmmm very smart way to store the device info rather than using things like writeallfiles
            PlayerPrefs.SetString(Messsages.devicesInfoString, json);
        }
        else if (msg.Contains("deviceType:"))
        { //if it is just the device ID
          //visually show device found based on device ID
            //allow user click the device image and bring up dialog of adding device(prsent continuous)
            if (msg.Contains(Messsages.RpiDeviceFound)){    //if it is a raspberry device that is found
                uiManager.AddDeviceUI(Messsages.RpiDeviceFound);    //show the ui
                //the next step is for the user to click on the found device to callRequestDeviceControl();
            }
            else if (msg.Contains(Messsages.CameraDeviceFound)){
                uiManager.AddDeviceUI(Messsages.CameraDeviceFound);    //show the ui

            }
            //request deice control is called from ui when add device button is tapped
        }
        else if (msg.Contains("ControlAccessGranted")){
            //show that connection is successful this should happen after completely receiving edited device telemetry 
            if (telemetryDevicesDict != null){
                uiManager.RemoveDeviceUI();
                uiManager.MovetoFocus(uiManager.DeviceAddedSuccessfully);  //indicate to user a successful set-up of device
                PlayerPrefs.SetInt(Messsages.NewUser, 0);   //user is no longer new
                //after this point, device store info on its devices and begins to send telemetry to cloud
                //take user to main menu after delay
                StartCoroutine(GenericExecuteAfterDelay<RectTransform>(uiManager.MovetoFocus, uiManager.generalUiDelay, uiManager.HomePage));
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
    private IEnumerator GenericExecuteAfterDelay<T>(Action<T> rev, float delay, T arg){
        yield return new WaitForSeconds(delay);
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
