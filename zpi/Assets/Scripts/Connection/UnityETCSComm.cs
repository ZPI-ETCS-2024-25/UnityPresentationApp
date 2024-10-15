using System.Collections;
using System.Collections.Generic;
using UnityEditor.SceneManagement;
using UnityEngine;

public class UnityETCSComm : MonoBehaviour
{
    public HttpCommunication communication;
    public string serverUri = "http://127.0.0.1:8000/name";

    void Start()
    {
        //Debug.Log("sending starting data");
        //SendBaliseInfo(50,2,50,10,2);
    }

    public void SendBaliseInfo(int kilometer,int number,int groupSize,int track,int line)
    {
        StartCoroutine(communication
            .POSTRequest(serverUri, new BaliseInfo()
            {
                kilometer = kilometer,
                number = number,
                groupSize = groupSize,
                track = track,
                line = line
            }
            , response => { Debug.Log(response); })
        );
    }

    public void SendBaliseInfo(BaliseInfo info)
    {
        StartCoroutine(communication
            .POSTRequest(serverUri, info
            , response => { Debug.Log(response); })
        );
    }
}


public class BaliseInfo
{
    public int kilometer;
    public int number;
    public int groupSize;
    public int track;
    public int line;
}
