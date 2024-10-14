using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityETCSComm : MonoBehaviour
{
    private HttpCommunication communication = new HttpCommunication();
    public string serverUri = "http://127.0.0.1:8000/";

    void Start()
    {
        SendBaliseInfo(50,2,50,10,2);
    }

    private void SendBaliseInfo(int kilometer,int number,int groupSize,int track,int line)
    {
        communication
            .POSTRequest(serverUri, new BaliseInfo()
            {
                kilometer = kilometer,
                number = number,
                groupSize = groupSize,
                track = track,
                line = line
            }
            , response => { Debug.Log(response); });
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
