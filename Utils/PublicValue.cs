using UnityEngine;
using System.Collections;

public class PublicValue : MonoBehaviour {

    TxtSettings setting;

    public static string isDebug = "false";
    public static string kioskID = "0";
    public static string serverIP = "127.0.0.1";
    public static int serverPORT = 8000;
    public static string dataFolder = "";
    public static int timeOut = 60;

    void Awake()
    {
        setting = gameObject.AddComponent<TxtSettings>();

        if (setting.LoadSetting("config.txt"))
        {
            isDebug = setting.GetSetting("debug");
            kioskID = setting.GetSetting("kioskID");
            serverIP = setting.GetSetting("serverIP");
            serverPORT = int.Parse(setting.GetSetting("serverPORT"));
            dataFolder = setting.GetSetting("dataFolder");
            timeOut = int.Parse(setting.GetSetting("timeOut"));

            Debug.Log("isDebug : " + isDebug);
            Debug.Log("kioskID : " + kioskID);
            Debug.Log("serverIP : " + serverIP);
            Debug.Log("serverPORT : " + serverPORT.ToString());
            Debug.Log("dataFolder : " + dataFolder);
            Debug.Log("timeOut : " + timeOut);
        }
        else
        {
            Debug.Log("config load fail");
        }

        if (isDebug == "false")
        {
            Cursor.visible = false;
        }

        Destroy(GetComponent<TxtSettings>());
    }
}