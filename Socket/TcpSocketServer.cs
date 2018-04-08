using UnityEngine;
using System.Collections;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Collections.Generic;

/// <summary>
/// 최종 수정일 : 2016.09.10
/// </summary>

/**
 * 2015.03 우동희 create
 * 
 * Queue<string> recieveQueue = new Queue<string>();
 * 
 * ser = GetComponent<TcpSocketServer>();
 * ser.serverOpen = ServerOpen;
 * ser.serverClose = ServerClose;
 * ser.clientConnect = ClientConnect;
 * ser.clientDisconnect = ClientDisconnect;
 * ser.receiveMsg = ReceiveMsg;
 * 
 * ser.SetMessageDelimiter("\n");
 * ser.Open(3333);
 * 
 * void ServerOpen(){}
 * void ServerClose(){}
 * void ClientConnect(string _ip, string _port){}
 * void ClientDisconnect(string _ip, string _port){}
 * void ReceiveMsg(string _ip, string _port, string _msg)
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
 * ser.SendMsg("data msg");
 * ser.SendMsg("192.168.0.2", "7654", "data msg");
 * //소켓서버 종료
 * ser.Close();
 */

public class ClientObject
{
    public Socket client = null;
    public string ip = "";
    public string port = "";
    public byte[] receiveBytes = new byte[256];
    public string receiveDatas = "";
}

public class TcpSocketServer : MonoBehaviour {

    //string messageDelimiter = "\n";
    string sendDelimiter = "";
    string receiveDelimiter = "";
    int delLeng = 1;
    int portNum = 3333;
    bool listening = false;

    int MAX_MEMBER = 10;
    Socket listenSocket = null;
    List<ClientObject> clientList = null;

    public delegate void ReceiveMsg(string _ip, string _port, string _msg);
    public ReceiveMsg receiveMsg;

    public delegate void ServerOpen();
    public ServerOpen serverOpen;

    public delegate void ServerClose();
    public ServerClose serverClose;

    public delegate void ClientConnect(string _ip, string _port);
    public ClientConnect clientConnect;

    public delegate void ClientDisconnect(string _ip, string _port);
    public ClientDisconnect clientDisconnect;

    //-------------------------------------------------------------------------------------------------------
    // Functions
    //-------------------------------------------------------------------------------------------------------
    public void SetMessageDelimiter(string delim)
    {
        //messageDelimiter = delim;
        //delLeng = messageDelimiter.Length;
        //Debug.Log("delLeng : " + delLeng);

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

    public bool Open(int _port)
    {
        portNum = _port;

        return StartServer();
    }

    public void Close()
    {
        StopServer();
    }

    public void SendMsg(string _msg)
    {
        //SendData(_msg + messageDelimiter);
        SendData(_msg + sendDelimiter);
    }

    public void SendMsg(string _ip, string _port, string _msg)
    {
        //SendData(_ip, _port, _msg + messageDelimiter);
        SendData(_ip, _port, _msg + sendDelimiter);
    }

    //-------------------------------------------------------------------------------------------------------
    // Socket
    //-------------------------------------------------------------------------------------------------------
    bool StartServer()
    {
        if (listenSocket != null) return false;

        if (clientList == null)
        {
            clientList = new List<ClientObject>();
        }

        try
        {
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.IP);
            listenSocket.Bind(new IPEndPoint(IPAddress.Any, portNum));
            listenSocket.Listen(MAX_MEMBER);
        }
        catch (Exception e)
        {
            Debug.Log("serverSocket Open Fail : " + e.Message);
            return false;
        }

        serverOpen();
        listening = true;
        WaitingAccept();
        
        return true;
    }

    void WaitingAccept()
    {
        try
        {
            if (listening)
            {
                listenSocket.BeginAccept(new AsyncCallback(AcceptHandler), listenSocket);
            }
        }
        catch (Exception e)
        {
            Debug.Log("serverSocket BeginAccept Error : " + e.Message);
        }
    }

    void AcceptHandler(IAsyncResult iar)
    {
        Socket listener = (Socket)iar.AsyncState;
        Socket handler = listener.EndAccept(iar);

        try
        {
            ClientObject clientObj = new ClientObject();
            clientObj.client = handler;
            IPEndPoint iep = (IPEndPoint)handler.RemoteEndPoint;
            clientObj.ip = iep.Address.ToString();
            clientObj.port = iep.Port.ToString();
            clientList.Add(clientObj);

            handler.BeginReceive(clientObj.receiveBytes, 0, clientObj.receiveBytes.Length, 0, new AsyncCallback(ReceiveComplete), clientObj);

            clientConnect(clientObj.ip, clientObj.port);
        }
        catch (Exception e)
        {
            IPEndPoint iep = (IPEndPoint)handler.RemoteEndPoint;
            string eip = iep.Address.ToString();
            string eport = iep.Port.ToString();

            Debug.Log("serverSocket Disconnect Client : " + eip + " (" + eport + ") : " + e.Message);
        }

        WaitingAccept();
    }

    void ReceiveComplete(IAsyncResult iar)
    {
        ClientObject clientObj = (ClientObject)iar.AsyncState;
        Socket client = clientObj.client;

        if (!client.Connected) return;

        try
        {
            int len = client.EndReceive(iar);
            //Debug.Log("len : " + len.ToString());

            if (len == 0)
            {
                Shutdown(clientObj);
            }
            else
            {
                //if (messageDelimiter == "")
                if (receiveDelimiter == "")
                {
                    string currMsg = Encoding.UTF8.GetString(clientObj.receiveBytes, 0, len);
                    ProcessData(currMsg, clientObj.ip, clientObj.port);
                }
                else
                {
                    clientObj.receiveDatas += Encoding.UTF8.GetString(clientObj.receiveBytes, 0, len);
                    ProcessData(clientObj);
                }
                
                client.BeginReceive(clientObj.receiveBytes, 0, clientObj.receiveBytes.Length, 0, new AsyncCallback(ReceiveComplete), clientObj);
            }
        }
        catch (Exception e)
        {
            Debug.Log("serverSocket receive exception : " + clientObj.ip + " (" + clientObj.port + ") : " + e.Message);
            Shutdown(clientObj);
        }
    }

    void ProcessData(ClientObject _clientObj)
    {
        string datas = _clientObj.receiveDatas;
        //int idx = datas.IndexOf(messageDelimiter);
        int idx = datas.IndexOf(receiveDelimiter);
        //Debug.Log("idx : " + idx.ToString());

        while (idx != -1)
        {
            string msg = datas.Substring(0, idx);
            receiveMsg(_clientObj.ip, _clientObj.port, msg);

            _clientObj.receiveDatas = _clientObj.receiveDatas.Substring(idx + delLeng); //1);
            datas = _clientObj.receiveDatas;
            //idx = datas.IndexOf(messageDelimiter);
            idx = datas.IndexOf(receiveDelimiter);
            //Debug.Log("idx : " + idx.ToString());
        }
    }

    void ProcessData(String _clientMsg, string _ip, string _port)
    {
        receiveMsg(_ip, _port, _clientMsg);
    }

    void SendData(string _data)
    {
        byte[] sendBytes = Encoding.UTF8.GetBytes(_data);

        foreach (ClientObject clientObj in clientList)
        {
            Socket client = clientObj.client;

            if (client != null && client.Connected)
            {
                try
                {
                    client.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, new AsyncCallback(SendComplete), clientObj);
                }
                catch (Exception e)
                {
                    Debug.Log("serverSocket sendAll exception : " + clientObj.ip + " (" + clientObj.port + ") : " + e.Message);
                    //Shutdown(clientObj);
                }
            }
        }
    }

    void SendData(string _ip, string _port, string _data)
    {
        byte[] sendBytes = Encoding.UTF8.GetBytes(_data);

        foreach (ClientObject clientObj in clientList)
        {
            if (clientObj.ip == _ip && clientObj.port == _port)
            {
                Socket client = clientObj.client;

                if (client != null && client.Connected)
                {
                    try
                    {
                        client.BeginSend(sendBytes, 0, sendBytes.Length, SocketFlags.None, new AsyncCallback(SendComplete), clientObj);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("serverSocket send exception : " + clientObj.ip + " (" + clientObj.port + ") : " + e.Message);
                        //Shutdown(clientObj);
                    }
                }

                break;
            }
        }
    }

    void SendComplete(IAsyncResult iar)
    {
        ClientObject clientObj = (ClientObject)iar.AsyncState;
        Socket client = clientObj.client;

        try
        {
            int len = client.EndSend(iar);
            //Debug.Log("len : " + len.ToString());

            if (len == 1)
            {
                Debug.Log("serverSocket send success : " + clientObj.ip + " (" + clientObj.port + ")");
            }
        }
        catch (Exception e)
        {
            Debug.Log("serverSocket sendcomp Error : " + clientObj.ip + " (" + clientObj.port + ") : " + e.Message);
            Shutdown(clientObj);
        }
    }

    void Shutdown(ClientObject _clientObj)
    {
        clientList.Remove(_clientObj);
        clientDisconnect(_clientObj.ip, _clientObj.port);

        Socket client = _clientObj.client;

        if (client != null)
        {
            try
            {
                if (client.Connected) client.Shutdown(SocketShutdown.Both);
                client.Close();
            }
            catch (System.Exception) { }

            client = null;
        }

        _clientObj.client = null;
        _clientObj.ip = null;
        _clientObj.receiveBytes = null;
        _clientObj.receiveDatas = null;
        _clientObj = null;
    }

    void StopServer()
    {
        listening = false;

        if (clientList != null)
        {
            while (clientList.Count > 0)
            {
                Shutdown(clientList[0]);
            }

            clientList.Clear();
            clientList = null;
        }

        if (listenSocket != null)
        {
            if (listenSocket.Connected) listenSocket.Shutdown(SocketShutdown.Both);
            listenSocket.Close();
            listenSocket = null;
        }

        serverClose();
    }

    void OnApplicationQuit()
    {
        StopServer();
    }

}
