using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 namespace RaspberryPiDevices
 {
     public class TelemetryData
    {
        //RaspberryPiUWP.cl
        public string PartitionKey { get; set; }    //type of device
        public string RowKey { get; set; }  //serial number given to device from I2C etc.
        public string deviceId { get; set; }    //name of device
        //the label of the properties eg temperature, humidity
        public string propertyLabel1 { get; set; }  //usually affirms connection
        public string propertyLabel2 { get; set; }  //most important parameter here
        public double property1 { get; set; }   //corresponds to PartitionKey
        // public T property2 { get; set; }   //corresponds to RowKey
        public string Etag { get; set; }
        public string Misc { get; set; }    //for any redundant data needed
        public TelemetryData() {}

        public TelemetryData(string s_partitionKey, string s_rowKey, string s_myDeviceId, string label1, string label2, double s_property1, string s_misc = null)
        {
            PartitionKey = s_partitionKey;
            RowKey = s_rowKey;
            deviceId = s_myDeviceId;
            propertyLabel1 = label1;
            propertyLabel2 = label2;
            property1 = s_property1;
            // property2 = s_property2;
            Etag = this.RowKey + this.PartitionKey; //let thus be the E_tag
            Misc = s_misc;
        }
    }
 }
 [System.Serializable]
    public class TelemetryData
    {
        //RaspberryPiUWP.cl
        public string PartitionKey { get; set; }    //type of device
        public string RowKey { get; set; }  //serial number given to device from I2C etc.
        public string deviceId { get; set; }    //name of device
        //the label of the properties eg temperature, humidity
        public string propertyLabel1 { get; set; }  //usually affirms connection
        public string propertyLabel2 { get; set; }  //most important parameter here
        public double property1 { get; set; }   //corresponds to PartitionKey
        // public T property2 { get; set; }   //corresponds to RowKey
        public string Etag { get; set; }
        public string Misc { get; set; }    //for any redundant data needed
        public TelemetryData() {}

        public TelemetryData(string s_partitionKey, string s_rowKey, string s_myDeviceId, string label1, string label2, double s_property1, string s_misc = null)
        {
            PartitionKey = s_partitionKey;
            RowKey = s_rowKey;
            deviceId = s_myDeviceId;
            propertyLabel1 = label1;
            propertyLabel2 = label2;
            property1 = s_property1;
            // property2 = s_property2;
            Etag = this.RowKey + this.PartitionKey; //let thus be the E_tag
            Misc = s_misc;
        }
    }
