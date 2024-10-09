using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Connection : MonoBehaviour
{
    public TMP_Text tmp;
    public TMP_Text receivedText;
    public bool shutUp;

    private HttpListener listener;
    private Thread listenerThread;
    private bool _isRunning = false;
    private string unityUri = "http://127.0.0.1:8001/";

    void Start()
    {
        StartServer(unityUri);

        if (!shutUp)
        {
            StartCoroutine(MakeRequest());
        }
    }
    void OnApplicationQuit()
    {
        StopServer();
    }

    private IEnumerator MakeRequest()
    {
        
        ////GET
        //var getRequest = CreateReqest("http://127.0.0.1:8000/name/");
        //yield return getRequest.SendWebRequest();
        //var deserializedGetData = JsonUtility.FromJson<JsonResponse>(getRequest.downloadHandler.text);
        //changeText(deserializedGetData.message);
        

        
        //POST DO SERVERAETCS
        var dataToPost = new PostDATA() { name = "POST from Unity to ETCS Server" };
        var postRequest = CreateReqest("http://127.0.0.1:8000/name/",RequestType.POST,dataToPost);
        yield return postRequest.SendWebRequest();
        string s = postRequest.downloadHandler.text;
        var deserializedPostData = JsonUtility.FromJson<JsonResponse>(postRequest.downloadHandler.text);
        changeText(deserializedPostData.message);

        //POST DO DRIVERETCS
        dataToPost = new PostDATA() { name = "POST from Unity to Driver" };
        postRequest = CreateReqest("http://127.0.0.1:8002/", RequestType.POST, dataToPost);
        yield return postRequest.SendWebRequest();
        s = postRequest.downloadHandler.text;
        changeText(s);

    }


    private UnityWebRequest CreateReqest(string path, RequestType type = RequestType.GET,object data = null)
    {
        var request = new UnityWebRequest(path,type.ToString());

        if(data != null)
        {
            var bodyRaw = Encoding.UTF8.GetBytes(JsonUtility.ToJson(data));
            request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        }

        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        return request;
    }

    public void StartServer(string uriPrefix)
    {
        listener = new HttpListener();
        listener.Prefixes.Add(uriPrefix);
        listener.Start();
        _isRunning = true;
        Debug.Log("Unity HTTP Server started, listening on " + uriPrefix);

        // Start a background thread to listen for incoming requests
        listenerThread = new Thread(HandleIncomingConnection);
        listenerThread.Start();
    }
    public void StopServer()
    {
        _isRunning = false;
        listener.Stop();
        listener.Close();

        if (listenerThread != null && listenerThread.IsAlive)
        {
            listenerThread.Abort();
        }
        Debug.Log("Server stopped.");
    }

    private void HandleIncomingConnection()
    {
        while (_isRunning)
        {
            try
            {
                // Wait for a request
                HttpListenerContext context = listener.GetContext();
                HttpListenerRequest request = context.Request;
                HttpListenerResponse response = context.Response;

                if (request.HttpMethod == "POST")
                {
                    using (var reader = new System.IO.StreamReader(request.InputStream, request.ContentEncoding))
                    {
                        string receivedMessage = reader.ReadToEnd();
                        Debug.Log("Message received from client: " + receivedMessage);
                    }

                    string responseMessage = "Unity received your POST message!";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseMessage);
                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.OutputStream.Close();
                }
                if (request.HttpMethod == "GET")
                {
                    Debug.Log("Received GET request");
                    string responseText = "Unity received your GET request!";
                    byte[] buffer = Encoding.UTF8.GetBytes(responseText);

                    response.ContentLength64 = buffer.Length;
                    response.OutputStream.Write(buffer, 0, buffer.Length);
                    response.OutputStream.Close();
                }
            }
            catch (Exception e)
            {
                Debug.Log("Error in HTTP listener: " + e.Message);
            }
        }
    }
    public void changeText(string change)
    {
        //Debug.Log("changing");
        string text = tmp.text;
        //Debug.Log($"current {text}");
        text = change;
        Debug.Log($"new {change}");
        //Debug.Log("changed");
        tmp.SetText(change);
    }
}


public enum RequestType
{
    GET = 0,
    POST = 1
}

public class JsonResponse
{
    public string message;
}

public class PostDATA
{
    public string name;
}