using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineComm : MonoBehaviour
{
    public HttpCommunication communication;
    private Queue<System.Action> requestQueue = new Queue<System.Action>();
    private bool isProcessing = false;
    public bool debugResponseQueu = true;

    public void EnqueueRequest(string serverUri, object info)
    {
        if (debugResponseQueu)
        {
            requestQueue.Enqueue(() =>
            StartCoroutine(communication.POSTRequest(serverUri, info, response =>
            {
                Debug.Log(response);
                ProcessNextRequest();
            }))
        );
        }
        else{
            requestQueue.Enqueue(() =>
            StartCoroutine(communication.POSTRequest(serverUri, info, response =>
            {
                ProcessNextRequest();
            }))
        );
        }
        

        if (!isProcessing)
        {
            ProcessNextRequest();
        }
    }

    private void ProcessNextRequest()
    {
        if (requestQueue.Count > 0)
        {
            isProcessing = true;
            var request = requestQueue.Dequeue();
            request?.Invoke(); 
        }
        else
        {
            isProcessing = false;
        }
    }
}
