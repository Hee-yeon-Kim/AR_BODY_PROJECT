using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;
using UnityEngine.UI;

public class ScanScene2 : MonoBehaviour
{

    // Use this for initialization
    BluetoothHelper bluetoothHelper;
    BluetoothHelper bluetoothHelper2;

    public Text text;
    public Text text2;
    public GameObject sphere;

    string received_message;

    void Start()
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



            bluetoothHelper.ScanNearbyDevices();



            text.text = "start scan";

        }
        catch (BluetoothHelper.BlueToothNotEnabledException ex)
        {
            sphere.GetComponent<Renderer>().material.color = Color.yellow;
            Debug.Log(ex.ToString());
            text.text = ex.Message + "음1";
        }
    }

    IEnumerator blinkSphere()
    {
        sphere.GetComponent<Renderer>().material.color = Color.cyan;
        yield return new WaitForSeconds(0.5f);
        sphere.GetComponent<Renderer>().material.color = Color.green;
    }

    //Asynchronous method to receive messages
    void OnMessageReceived()
    {
        //StartCoroutine(blinkSphere());
        received_message = bluetoothHelper.Read();
        text.text = received_message;
        Debug.Log(System.DateTime.Now.Second);
        //Debug.Log(received_message);
    }
    void OnMessageReceived2()
    {
        //StartCoroutine(blinkSphere());
        received_message = bluetoothHelper2.Read();
        text2.text = received_message;
        Debug.Log(System.DateTime.Now.Second);
        //Debug.Log(received_message);
    }
    void OnScanEnded(LinkedList<BluetoothDevice> nearbyDevices)
    {
        text.text = "  scan1";

        if (nearbyDevices.Count == 0)
        {
            text.text = "scan fail1";
            bluetoothHelper.ScanNearbyDevices();
            return;
        }


        foreach (BluetoothDevice device in nearbyDevices)
        {
            if (device.DeviceName == "LowerLeg")
                text.text = "FOUND1!!";
        }

        bluetoothHelper.setDeviceName("LowerLeg");
        // bluetoothHelper.setDeviceAddress("00:21:13:02:16:B1");
        bluetoothHelper.Connect();
        bluetoothHelper.isDevicePaired();

    }

    void OnScanEnded2(LinkedList<BluetoothDevice> nearbyDevices)
    {
        text2.text = "scan2";
        if (nearbyDevices.Count == 0)
        {
            text2.text = "scan fail2";
            bluetoothHelper2.ScanNearbyDevices();
            return;
        }


        foreach (BluetoothDevice device in nearbyDevices)
        {
            Debug.Log(device.DeviceName);
            if (device.DeviceName == "UpperLeg")
                text2.text = "Found2!!";
        }


        bluetoothHelper2.setDeviceName("UpperLeg");
        bluetoothHelper2.Connect();
        bluetoothHelper2.isDevicePaired();
    }

    void Update()
    {
        //Debug.Log(bluetoothHelper.IsBluetoothEnabled());
        if (!bluetoothHelper.IsBluetoothEnabled())
        {
            bluetoothHelper.EnableBluetooth(true);
        }
    }

    void OnConnected()
    {
        sphere.GetComponent<Renderer>().material.color = Color.green;
        try
        {
            bluetoothHelper.StartListening();
            bluetoothHelper2.ScanNearbyDevices();
        }
        catch (Exception ex)
        {
            text.text = ex.Message;
            Debug.Log(ex.Message);
        }

    }

    void OnConnected2()
    {
        text2.text = "Device 2 connected";
        bluetoothHelper2.StartListening();
    }

    void OnConnectionFailed()
    {
        sphere.GetComponent<Renderer>().material.color = Color.red;
        text.text = "Connection Failed";
    }

    void OnConnectionFailed2()
    {
        sphere.GetComponent<Renderer>().material.color = Color.red;
        text2.text = "Connection Failed 2";
    }

    //Call this function to emulate message receiving from bluetooth while debugging on your PC.
    void OnGUI()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.DrawGUI();
        else
            return;

        if (bluetoothHelper.isConnected())
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height - 2 * Screen.height / 10, Screen.width / 5, Screen.height / 10), "Disconnect"))
            {
                bluetoothHelper.Disconnect();
                sphere.GetComponent<Renderer>().material.color = Color.blue;
            }

        if (bluetoothHelper.isConnected())
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height / 10, Screen.width / 5, Screen.height / 10), "Send text"))
            {
                //bluetoothHelper.SendData(new byte[] { 0, 1, 2, 3, 4 });
                bluetoothHelper.SendData("Hi From unity");
            }
    }

    void OnDestroy()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
        if (bluetoothHelper2 != null)
            bluetoothHelper2.Disconnect();
    }
}