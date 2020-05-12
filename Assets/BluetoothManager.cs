using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using UnityEngine.UI;

public class BluetoothManager : MonoBehaviour
{
    private Toggle bttog;
    private Image btcondition;
    private Image btcondition2;
    private Text imumonitor;
    private Text imumonitor2;
    private float offset_roll1;
    private float offset_pitch1;
    private float offset_roll2;
    private float offset_pitch2;

    BluetoothHelper bluetoothHelper;
    BluetoothHelper bluetoothHelper2;
    [HideInInspector] public float[] lowerleg_array;
    [HideInInspector] public float[] upperleg_array;
    [HideInInspector] public bool isnew1 = false;
    [HideInInspector] public bool isnew2 = false;
    [HideInInspector] public bool connect1 = false;
    [HideInInspector] public bool connect2 = false;
    private Button btcalli;


    // Start is called before the first frame update
    void Awake()
    {
        bttog = GetComponent<Toggle>();
        btcondition = GameObject.FindGameObjectWithTag("btdone").GetComponent<Image>();
        btcondition.enabled = false;
        btcondition2 = GameObject.FindGameObjectWithTag("btdone2").GetComponent<Image>();
        btcondition2.enabled = false;
        imumonitor = GameObject.FindGameObjectWithTag("imumonitor").GetComponent<Text>();
        
        imumonitor2 = GameObject.FindGameObjectWithTag("imumonitor2").GetComponent<Text>();
        lowerleg_array= new float[3] { 0.0f, 0.0f, 0.0f };
        upperleg_array = new float[3] { 0.0f, 0.0f, 0.0f };
        offset_roll1 = 0.0f;
        offset_pitch1 = 0.0f;
        offset_roll2 = 0.0f;
        offset_pitch2 = 0.0f;
    }
    private void Start()
    {
        bttog.onValueChanged.AddListener((bool val) => SettingBT(val));
        btcalli = GameObject.FindGameObjectWithTag("BTinit").GetComponent<Button>();
        btcalli.onClick.AddListener(btcallibration);
        btcalli.interactable = false;
        InitBT();
    }
    void btcallibration()
    {
        if (btcalli.IsInteractable())
        {
            offset_roll1 = upperleg_array[2];
            offset_pitch1 = upperleg_array[1];
            offset_roll2 = lowerleg_array[2];
            offset_pitch2 = lowerleg_array[1];
        }
    }
    void InitBT()
    {
        try
        {
            BluetoothHelper.BLE = true;  //use Bluetooth Low Energy Technology
            bluetoothHelper = BluetoothHelper.GetInstance();

            bluetoothHelper.OnConnected += OnConnected;
            bluetoothHelper.OnConnectionFailed += OnConnectionFailed;
            bluetoothHelper.OnDataReceived += OnMessageReceived; //read the data
            bluetoothHelper.OnScanEnded += OnScanEnded;

            bluetoothHelper.setTerminatorBasedStream("\n");

            bluetoothHelper2 = BluetoothHelper.GetNewInstance();
            bluetoothHelper2.OnConnected += OnConnected2;
            bluetoothHelper2.OnConnectionFailed += OnConnectionFailed2;
            bluetoothHelper2.OnScanEnded += OnScanEnded2;
            bluetoothHelper.OnDataReceived += OnMessageReceived2; //read the data

            bluetoothHelper2.setTerminatorBasedStream("\n");

        }
        catch (BluetoothHelper.BlueToothNotEnabledException ex)
        {
            Debug.Log(ex.ToString());
        }
    }
    void SettingBT(bool val)
    {
        if (bluetoothHelper == null)
            return;
        if (bttog.isOn)
        {
            if (!bluetoothHelper.isConnected())
            {
                //upperleg 시작
                bluetoothHelper.ScanNearbyDevices();
                imumonitor.text = "start scan";

            }
        }
        else
        {
            if (bluetoothHelper.isConnected())
                bluetoothHelper.Disconnect();
            if (bluetoothHelper2.isConnected())
                bluetoothHelper2.Disconnect();
            btcalli.interactable = false;
            btcondition.enabled = false;
            btcondition2.enabled = false;
            isnew1 = false;
            isnew2 = false;
            connect1 = false;
            connect2 = false;
            imumonitor.text = "";
            imumonitor2.text = "";
            
        }
    }

    void OnMessageReceived()
    {
        //StartCoroutine(blinkSphere());
        string msg = bluetoothHelper.Read();
        string[] array = msg.Split(',');
        if(array.Length==3)
        {
            imumonitor.text = msg;
            try
            {
                upperleg_array[0] = float.Parse(array[0]);
                upperleg_array[1] = float.Parse(array[1]) - offset_pitch1;
                upperleg_array[2] = float.Parse(array[2])-offset_roll1;

                isnew1 = true;
            }
            catch(Exception e)
            {
               
                isnew1 = false;
                return;
            }
        }
        Debug.Log(System.DateTime.Now.Second);
        //Debug.Log(received_message);
    }
    void OnMessageReceived2()
    {
        //StartCoroutine(blinkSphere());
        string msg = bluetoothHelper2.Read();
        
        string[] array = msg.Split(',');
        if (array.Length == 3)
        {
            imumonitor2.text = msg;
            try
            {
                lowerleg_array[0] = float.Parse(array[0]);
                lowerleg_array[1] = float.Parse(array[1]) - offset_pitch2;
                lowerleg_array[2] = float.Parse(array[2])-offset_roll2;
                isnew2 = true;
            }
            catch (Exception e)
            {
                isnew2 = false;
                return;
            }
        }
        //Debug.Log(received_message);
    }
    void OnScanEnded(LinkedList<BluetoothDevice> nearbyDevices)
    {


        if (nearbyDevices.Count == 0)
        {
            imumonitor.text = "scanning...";
            bluetoothHelper.ScanNearbyDevices();
            return;
        }


        foreach (BluetoothDevice device in nearbyDevices)
        {
            if (device.DeviceName == "UpperLeg")
                imumonitor.text = "IMU1 Found";
        }

        bluetoothHelper.setDeviceName("UpperLeg");
        // bluetoothHelper.setDeviceAddress("00:21:13:02:16:B1");
        bluetoothHelper.Connect();
        bluetoothHelper.isDevicePaired();

    }

    void OnScanEnded2(LinkedList<BluetoothDevice> nearbyDevices)
    {
        if (nearbyDevices.Count == 0)
        {
            imumonitor2.text = "scanning...";
            bluetoothHelper2.ScanNearbyDevices();
            return;
        }


        foreach (BluetoothDevice device in nearbyDevices)
        {
            Debug.Log(device.DeviceName);
            if (device.DeviceName == "LowerLeg")
                imumonitor2.text = "IMU2 Found";
        }


        bluetoothHelper2.setDeviceName("LowerLeg");
        bluetoothHelper2.Connect();
        bluetoothHelper2.isDevicePaired();
    }
    void OnConnected()
    {
        try
        {
            connect1 = true;
            btcondition.enabled = true;
            imumonitor.text = "IMU1 connected";
            bluetoothHelper.StartListening();
            bluetoothHelper2.ScanNearbyDevices();
            imumonitor2.enabled = true;
        }
        catch (Exception ex)
        {
            imumonitor.text = ex.Message;
            Debug.Log(ex.Message);
        }

    }

    void OnConnected2()
    {
        try
        {
            connect2 = true;
            btcalli.interactable = true;
            btcondition2.enabled = true;
            imumonitor2.text = "IMU2 connected";
            bluetoothHelper2.StartListening();
        }
        catch (Exception ex)
        {
            imumonitor2.text = ex.Message;
            Debug.Log(ex.Message);
        }
    }

    void OnConnectionFailed()
    {
        imumonitor.text = "Connection Failed";
        btcondition.enabled = false;
        isnew1 = false;
        connect1 = false;         
    }

    void OnConnectionFailed2()
    {
        imumonitor2.text = "Connection Failed 2";
        btcalli.interactable = false;
        btcondition2.enabled = false;
        connect2 = false;
        isnew2 = false;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log(bluetoothHelper.IsBluetoothEnabled());
        if (!bluetoothHelper.IsBluetoothEnabled())
        {
            bluetoothHelper.EnableBluetooth(true);
        }
    }
    private void OnDestroy()
    {

        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
        if (bluetoothHelper2 != null)
            bluetoothHelper2.Disconnect();
    }
}
