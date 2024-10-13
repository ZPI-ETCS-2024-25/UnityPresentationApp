using System;
using System.Net;
using System.Text;
using System.Threading;
using UnityEngine;

public class Server : MonoBehaviour
{
    private HttpListener listener;
    private Thread listenerThread;
    private bool _isRunning = false;
    private string unityServerUri = "http://127.0.0.1:8001/";


    private void Start()
    {
        StartServer(unityServerUri);      
    }

    void OnApplicationQuit()
    {
        StopServer();
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
}
