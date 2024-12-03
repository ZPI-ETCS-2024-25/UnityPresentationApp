//using Palmmedia.ReportGenerator.Core.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class Server : MonoBehaviour
{
    public UnityETCSComm comm;
    public bool turnOff = false;
    private HttpListener listener;
    private Thread listenerThread;
    private bool _isRunning = false;
    public string unityServerUri = "http://127.0.0.1:8001/";
    public event EventHandler<bool> BreakCommand;


    private void Start()
    {
        if (!turnOff)
        {
            StartServer(unityServerUri);
        }
        //StartServer(unityServerUri);      
    }

    void OnApplicationQuit()
    {
        StopServer();
    }


    protected virtual void OnBreakCommand(bool startOrFinish)
    {
        BreakCommand?.Invoke(this,startOrFinish);
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

        if(listener != null)
        {
            listener.Stop();
            listener.Close();
        }

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
                        try
                        {
                            JObject json = JObject.Parse(receivedMessage);
                            string messageType = json["messageType"]?.Value<string>() ?? "";
                            //bool breakCommand = json["BreakCommand"]?.Value<bool>() ?? false;
                            if (messageType == "brake")
                            {
                                bool breakCommand = json["BreakCommand"]?.Value<bool>() ?? false;
                                OnBreakCommand(breakCommand);
                                Debug.Log("success");
                            }
                            if(messageType == "alive")
                            {
                                string source = json["SOURCE"]?.Value<string>() ?? "";
                                if(source == "DRIVER")
                                {
                                    Debug.Log("succces");
                                    comm.SendIsAliveInfo();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            Debug.Log(e.ToString());
                        }

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
}
