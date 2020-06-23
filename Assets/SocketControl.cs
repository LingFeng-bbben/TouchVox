using System.Collections;
using System.Collections.Generic;
using System.Net.WebSockets;
using UnityEngine;
using Newtonsoft.Json;
using System.Threading;
using System.Text;
using System;
using System.Threading.Tasks;

public class SocketControl : MonoBehaviour
{
    int id = 0;
    
    private void Start()
    {
        instance = this;
        SendCycle();
        print("?");
    }

    public static SocketControl instance;

    ClientWebSocket ws;
    CancellationToken ct;
    public async Task<bool> Connect(string uri)
    {
        try
        {
            if (ws != null)
            {
                if (ws.State == WebSocketState.Connecting) return false;
                ws.Dispose();
            }
            Debug.Log("try Reconnect");
            ws = new ClientWebSocket();
            ct = new CancellationToken();
            Uri url = new Uri(uri);
            await ws.ConnectAsync(url, ct);
            Debug.Log("Reconnect Success");
            return true;
            //await SendRequest(new Request("info", "launcher"));
        }
        catch (Exception ex)
        {
            Debug.LogError(ex.Message);
            return false;
        }
    }
    List<Request> requestsPendingList = new List<Request>();
    public void SendRequest(Request req)
    {
        req.id = id;
        id++;
        requestsPendingList.Add(req);
    }

    public async void SendCycle()
    {
        while (true)
        {
            if (ws == null || ws.State != WebSocketState.Open)
            {
                Debug.LogWarning("ws not connected");
                await Connect(PlayerPrefs.GetString("IP"));
            }
            if (requestsPendingList.Count > 0)
            {
                string js = JsonConvert.SerializeObject(requestsPendingList[0]);
                js = js.Replace("theparams", "params");
                Debug.Log("Sending: " + js);
                await ws.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(js)), WebSocketMessageType.Binary, true, ct); //发送数据
                /*var result = new byte[1024];
                await ws.ReceiveAsync(new ArraySegment<byte>(result), new CancellationToken());//接受数据
                var str = Encoding.UTF8.GetString(result, 0, result.Length);
                Debug.Log(str);
                */
                requestsPendingList.RemoveAt(0);
            }
            await Task.Delay(1);
        }
    }

    private void OnApplicationQuit()
    {
        ws.Dispose();
    }
}

public class Request
{
    public int id;
    public string module;
    public string function;
    public object[] theparams = new object[0];
    public Request(string _module, string _function, object[] para = null)
    {
        module = _module;
        function = _function;
        if (para != null)
            theparams = para;
    }
}
