using System;


    [System.Serializable]
    public class TelemetryDataPoint<T>: TelemetryData
    {
        //RaspberryPiUWP.cl
        public T property2 { get; set; }   //corresponds to RowKey

        public TelemetryDataPoint(string s_partitionKey, string s_rowKey, string s_myDeviceId, string label1, string label2, double s_property1, T s_property2, string s_misc = null)
        : base(s_partitionKey, s_rowKey, s_myDeviceId, label1, label2, s_property1)
        {
            property2 = s_property2;
        }
    }

   
