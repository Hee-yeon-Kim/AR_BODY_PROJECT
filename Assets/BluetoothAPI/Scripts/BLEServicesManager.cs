
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

using ArduinoBluetoothAPI;
using System;

public class BLEServicesManager : MonoBehaviour
{
    private BluetoothHelper BTHelper;
    private float timer;
    public Text text1;
    public Text text2;

    void Start()
    {
        timer = 0;
        try
        {
            Debug.Log("HI");
            BluetoothHelper.BLE = true;  //use Bluetooth Low Energy Technology
            BTHelper = BluetoothHelper.GetInstance();
            
            BTHelper.OnConnected += OnBluetoothConnected; //OnBluetoothConnected is a function defined later on
            BTHelper.OnDataReceived += OnMessageReceived;
             
            BTHelper.OnConnectionFailed += OnConnectionFailed;

            BTHelper.OnScanEnded += OnScanEnded;

            BTHelper.setTerminatorBasedStream("\n");

            if (!BTHelper.ScanNearbyDevices())
            {
                text1.text += "cannot start scan";
 
                // bluetoothHelper.setDeviceAddress("00:21:13:02:16:B1");
                BTHelper.setDeviceName("LowerLeg");
                BTHelper.Connect();
            }
            else
            {
                text1.text += "start scan";
                // sphere.GetComponent<Renderer>().material.color = Color.green;
            }
        }
        catch (BluetoothHelper.BlueToothNotEnabledException ex)
        {
             text1.text += ex.Message;
        }
    }
    void OnMessageReceived()
    {
        //StartCoroutine(blinkSphere());
         
        try
        {
            string msg;
            string xx = BTHelper.Read();
            string[] array = xx.Split(',');
            if (array.Length == 3)
            {
                msg = " x축: " + array[0] + " y축: " + array[1] + " z축: " + array[2];
                text2.text = msg;
            }



        }
        catch (Exception ex)
        {
            text1.text += ex.Message;
        }
    }
    void OnScanEnded(LinkedList<BluetoothDevice> nearbyDevices)
    {
        text1.text = "Found " + nearbyDevices.Count + " devices";
        if (nearbyDevices.Count == 0)
        {
            BTHelper.ScanNearbyDevices();
            text1.text += "오";
            return;
        }


        foreach (BluetoothDevice device in nearbyDevices)
        {
            text1.text += " ㅎ ";
            if (device.DeviceName == "LowerLeg")
                text1.text="FOUND!!";
        }

        
        BTHelper.setDeviceName("LowerLeg");
        // bluetoothHelper.setDeviceAddress("00:21:13:02:16:B1");
        BTHelper.Connect();
        BTHelper.isDevicePaired();
    }

    void OnBluetoothConnected()
    {
        try
        {
            BTHelper.StartListening();


        }
        catch (Exception ex)
        {
            text1.text += ex.ToString();
            Debug.Log(ex.Message);
        }

    }
    void OnConnectionFailed()
    {
        text1.text+="Connection Failed";
    }


    void OnGUI()
    {

        if (BTHelper == null)
            return;


        BTHelper.DrawGUI();

        if (!BTHelper.isConnected())
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height / 10, Screen.width / 5, Screen.height / 10), "Connect"))
            {
                if (BTHelper.isDevicePaired())
                    BTHelper.Connect(); // tries to connect
            }

        if (BTHelper.isConnected())
            if (GUI.Button(new Rect(Screen.width / 2 - Screen.width / 10, Screen.height - 2 * Screen.height / 10, Screen.width / 5, Screen.height / 10), "Disconnect"))
            {
                BTHelper.Disconnect();
            }
    }

    void OnDestroy()
    {
        if (BTHelper != null)
            BTHelper.Disconnect();
    }

}

/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ArduinoBluetoothAPI;
using System;

public class BLEServicesManager : MonoBehaviour
{
    private BluetoothHelper bluetoothHelper;
    private float timer;
    void Start()
    {
        timer = 0;
        try{
            Debug.Log("HI");
            BluetoothHelper.BLE = true;  //use Bluetooth Low Energy Technology
            bluetoothHelper = BluetoothHelper.GetInstance("TEST");
            Debug.Log(bluetoothHelper.getDeviceName());
            bluetoothHelper.OnConnected += () => {
                Debug.Log("Connected");
                sendData();
            };
            bluetoothHelper.OnConnectionFailed += ()=>{
                Debug.Log("Connection failed");
            };
            bluetoothHelper.OnScanEnded += OnScanEnded;
            bluetoothHelper.OnServiceNotFound += (serviceName) =>
            {
                Debug.Log(serviceName);
            };
            bluetoothHelper.OnCharacteristicNotFound += (serviceName, characteristicName) =>
            {
                Debug.Log(characteristicName);
            };
            bluetoothHelper.OnCharacteristicChanged += (value, characteristic) =>
            {
                Debug.Log(characteristic.getName());
                Debug.Log(System.Text.Encoding.ASCII.GetString(value));
            };

            // BluetoothHelperService service = new BluetoothHelperService("FFE0");
            // service.addCharacteristic(new BluetoothHelperCharacteristic("FFE1"));
            // BluetoothHelperService service2 = new BluetoothHelperService("180A");
            // service.addCharacteristic(new BluetoothHelperCharacteristic("2A24"));
            // bluetoothHelper.Subscribe(service);
            // bluetoothHelper.Subscribe(service2);
            // bluetoothHelper.ScanNearbyDevices();

            BluetoothHelperService service = new BluetoothHelperService("19B10000-E8F2-537E-4F6C-D104768A1214");
            service.addCharacteristic(new BluetoothHelperCharacteristic("19B10001-E8F2-537E-4F6C-D104768A1214"));
            //BluetoothHelperService service2 = new BluetoothHelperService("180A");
            //service.addCharacteristic(new BluetoothHelperCharacteristic("2A24"));
            bluetoothHelper.Subscribe(service);
            //bluetoothHelper.Subscribe(service2);
            bluetoothHelper.ScanNearbyDevices();

        }catch(Exception ex){
            Debug.Log(ex.Message);
        }
    }

    private void OnScanEnded(LinkedList<BluetoothDevice> devices){
        Debug.Log("FOund " + devices.Count);
        if(devices.Count == 0){
            bluetoothHelper.ScanNearbyDevices();
            return;
        }
            
        try
        {
            bluetoothHelper.setDeviceName("KURV_Master");
            // bluetoothHelper.setDeviceName("HC-08");
            bluetoothHelper.Connect();
            Debug.Log("Connecting");
        }catch(Exception ex)
        {
            Debug.Log(ex.Message);
        }

    }

    void OnDestroy()
    {
        if (bluetoothHelper != null)
            bluetoothHelper.Disconnect();
    }

    void Update(){
        if(bluetoothHelper == null)
            return;
        if(!bluetoothHelper.isConnected())
            return;
        timer += Time.deltaTime;

        if(timer < 5)
            return;
        timer = 0;
        sendData();
    }

    void sendData(){
        // Debug.Log("Sending");
        // BluetoothHelperCharacteristic ch = new BluetoothHelperCharacteristic("FFE1");
        // ch.setService("FFE0"); //this line is mandatory!!!
        // bluetoothHelper.WriteCharacteristic(ch, new byte[]{0x44, 0x55, 0xff});

        Debug.Log("Sending");
        BluetoothHelperCharacteristic ch = new BluetoothHelperCharacteristic("19B10001-E8F2-537E-4F6C-D104768A1214");
        ch.setService("19B10000-E8F2-537E-4F6C-D104768A1214"); //this line is mandatory!!!
        bluetoothHelper.WriteCharacteristic(ch, "10001000"); //string: 10001000 is this binary? no, as string.
    }

    void read(){
        BluetoothHelperCharacteristic ch = new BluetoothHelperCharacteristic("2A24");
        ch.setService("180A");//this line is mandatory!!!

        bluetoothHelper.ReadCharacteristic(ch);
        //Debug.Log(System.Text.Encoding.ASCII.GetString(x));
    }

}*/
