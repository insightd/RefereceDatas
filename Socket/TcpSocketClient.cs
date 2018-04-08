using UnityEngine;
using System.Collections;

using System;
using System.Net.Sockets;
using System.Text;

/// <summary>
/// 최종 수정일 : 2016.09.10
/// </summary>

/**
 * 2015.03 우동희 create
 * 
 * Queue<string> recieveQueue = new Queue<string>();
 * 
 * soc = GetComponent<TcpSocketClient>();
 * soc.socketConnect = SocketConnect;
 * soc.socketDisconnect = SocketDisconnect;
 * soc.receiveMsg = ReceiveMsg;
 * 
 * soc.SetMessageDelimiter("\n");
 * soc.Connect(3333, "127.0.0.1");
 * 
 * void SocketConnect(){}
 * void SocketDisconnect(){}
 * void ReceiveMsg(string _msg)
 * {
 *      lock (recieveQueue)
 *      {
 *          recieveQueue.Enqueue(_msg);
 *      }
 * }
 * 
 * void Update () 
 * {
 *      lock (recieveQueue)
 *      {
 *          while (recieveQueue.Count > 0)
 *          {
 *              string data = recieveQueue.Dequeue();
 *              //do what you want
 *          }
 *      }
 * }
 * 
 * //메시지 전송
 * soc.SendMsg("data msg");
 * //소켓 종료
 * soc.Disconnect();
 *
    //디버깅 창
    //----------------------------------------------------------------------------------------------
    // GUI LOG
    //----------------------------------------------------------------------------------------------
    Vector2 scrollPosition;
    string logMsg = "";

    void AddLog(string _addMsg)
    {
        logMsg += _addMsg + "\n";
        scrollPosition.y = Mathf.Infinity;
    }

    void OnGUI()
    {
        //return;
        if (PublicValue.isDebug != "true") return;

        GUILayout.BeginArea(new Rect(10f, 10f, 600, 100f));
        scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUI.skin.box, GUILayout.Width(600f), GUILayout.Height(100f));
        GUILayout.Label(logMsg);
        GUILayout.EndScrollView();
        GUILayout.EndArea();

        GUILayout.BeginArea(new Rect(615f, 10f, 60, 22f));
        if (GUILayout.Button("Clear", GUI.skin.button))
            logMsg = "";
        GUILayout.EndArea();
    }
 *
 */

public class TcpSocketClient : MonoBehaviour {

    //string messageDelimiter = "\n";
    string sendDelimiter = "";
    string receiveDelimiter = "";
    int delLeng = 1;
    string ipAddress = "127.0.0.1";
    int portNum = 3333;
    
    Socket clientSocket = null;
    byte[] receiveBytes = new byte[256];
    string receiveDatas = "";

    public delegate void ReceiveMsg(string _msg);
    public ReceiveMsg receiveMsg;

    public delegate void SocketConnect();
    public SocketConnect socketConnect;

    public delegate void SocketDisconnect();
    public SocketDisconnect socketDisconnect;

    //-------------------------------------------------------------------------------------------------------
    // Functions
    //-------------------------------------------------------------------------------------------------------
    public void SetMessageDelimiter(string delim)
    {
        //messageDelimiter = delim;
        //delLeng = messageDelimiter.Length;

        sendDelimiter = delim;
        receiveDelimiter = delim;
        delLeng = receiveDelimiter.Length;
    }

    public void SetReceiveDelimiter(string delim)
    {
        receiveDelimiter = delim;
        delLeng = receiveDelimiter.Length;
    }

    public void SetSendDelimiter(string delim)
    {
        sendDelimiter = delim;
    }

    public bool Connected()
    {
        return clientSocket.Connected;
    }

    public void Connect(int _port, string _ip = "127.0.0.1")
    {
        portNum = _port;
        ipAddress = _ip;

        StartCoroutine("ConnectSocket");
    }

    public void Disconnect()
    {
        StopCoroutine("ConnectSocket");
        Shutdown();
    }

    public bool SendMsg(string _msg)
    {
        //return SendData(_msg + messageDelimiter);
        return SendData(_msg + sendDelimiter);
    }

    //-------------------------------------------------------------------------------------------------------
    // Socket
    //-------------------------------------------------------------------------------------------------------
    IEnumerator ConnectSocket()
    {
        yield return new WaitForSeconds(5.0f);

        if (clientSocket == null)
        {
            clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        }

        if (!clientSocket.Connected)
        {
            clientSocket.BeginConnect(ipAddress, portNum, new AsyncCallback(ConnectCallback), clientSocket);
        }

        StartCoroutine("ConnectSocket");
    }

    void ConnectCallback(IAsyncResult iar)
    {
        try
        {
            Socket client = (Socket)iar.AsyncState;
            client.EndConnect(iar);

            clientSocket.BeginReceive(receiveBytes, 0, receiveBytes.Length, SocketFlags.None, new AsyncCallback(ReceiveComplete), clientSocket);

            socketConnect();
        }
        catch (Exception e)
        {
            Debug.Log("clientSocket Connect Fail : " + e.Message);
            //Shutdown();
        }
    }

    void ReceiveComplete(IAsyncResult iar)
    {
        if (clientSocket == null) return;

        try
        {
            int len = clientSocket.EndReceive(iar);
            //Debug.Log("len : " + len.ToString());

            if (len == 0)
            {
                Shutdown();
            }
            else
            {
                //if (messageDelimiter == "")
                if (receiveDelimiter == "")
                {
                    string currMsg = Encoding.UTF8.GetString(receiveBytes, 0, len);
                    ProcessData(currMsg);
                }
                else
                {
                    receiveDatas += Encoding.UTF8.GetString(receiveBytes, 0, len);
                    ProcessData();
                }

                clientSocket.BeginReceive(receiveBytes, 0, receiveBytes.Length, SocketFlags.None, new AsyncCallback(ReceiveComplete), null);
            }
        }
        catch (Exception e)
        {
            Debug.Log("clientSocket receive exception : " + e.Message);
            Shutdown();
        }
    }

    void ProcessData()
    {
        //int idx = receiveDatas.IndexOf(messageDelimiter);
        int idx = receiveDatas.IndexOf(receiveDelimiter);
        //Debug.Log("idx : " + idx.ToString());

        while (idx != -1)
        {
            string msg = receiveDatas.Substring(0, idx);
            receiveMsg(msg);

            receiveDatas = receiveDatas.Substring(idx + delLeng);
            //idx = receiveDatas.IndexOf(messageDelimiter);
            idx = receiveDatas.IndexOf(receiveDelimiter);
        }
    }

    void ProcessData(String _recMsg)
    {
        receiveMsg(_recMsg);
    }

    bool SendData(string _data)
    {
        byte[] sendBytes = Encoding.UTF8.GetBytes(_data);

        if (clientSocket != null && clientSocket.Connected)
        {
            try
            {
                clientSocket.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, new AsyncCallback(SendComplete), null);
            }
            catch (Exception e)
            {
                Debug.Log("clientSocket send exception : " + e.Message);
                Shutdown();
                return false;
            }
        }

        return true;
    }

    void SendComplete(IAsyncResult iar)
    {
        if (clientSocket == null) return;

        try
        {
            int len = clientSocket.EndSend(iar);

            if (len == 1)
            {
                Debug.Log("clientSocket send success");
            }
        }
        catch (Exception e)
        {
            Debug.Log("clientSocket send Error : " + e.Message);
            Shutdown();
        }
    }

    void Shutdown()
    {
        if (clientSocket != null)
        {
            try
            {
                if (clientSocket.Connected) clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
            catch (System.Exception) { }

            clientSocket = null;
        }

        socketDisconnect();
    }

    void OnApplicationQuit()
    {
        StopCoroutine("ConnectSocket");
        Shutdown();
    }

}
