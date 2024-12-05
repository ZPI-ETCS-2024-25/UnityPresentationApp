using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
//using UnityEditor.SceneManagement;
using UnityEngine;

public class UnityETCSComm:CoroutineComm
{
    //public HttpCommunication communication;
    public string serverUri = "http://127.0.0.1:8000/name";

    void Start()
    {
        //Debug.Log("sending starting data");
        //SendBaliseInfo(50,2,50,10,2);
    }

    public void SendBaliseInfo(float kilometer,int number,int numberOfBalises,string trackNumber, int lineNumber,string messageType)
    {
        EnqueueRequest(serverUri, new BaliseInfo()
        {
            kilometer = Convert.ToString(kilometer),
            number = number,
            numberOfBalises = numberOfBalises,
            trackNumber = trackNumber,
            lineNumber = lineNumber,
            messageType = messageType
        });
        /*StartCoroutine(communication
            .POSTRequest(serverUri, new BaliseInfo()
            {
                kilometer = Convert.ToString(kilometer),
                number = number,
                numberOfBalises = numberOfBalises,
                trackNumber = trackNumber,
                lineNumber = lineNumber,
                messageType = messageType
            }
            , response => { Debug.Log(response); })
        );*/
    }

    public void SendBaliseInfo(BaliseInfo info)
    {
        Debug.Log($"Track:{info.trackNumber}  Line:{info.lineNumber}  Kilometer:{info.kilometer}  Number:{info.number}   Message:{info.messageType}");
        EnqueueRequest(serverUri, info);
        /*StartCoroutine(communication
            .POSTRequest(serverUri, info
            , response => { Debug.Log(response); })
        );*/
    }

    public void SendSpeedInfo(float speed)
    {
        EnqueueRequest(serverUri, new SpeedInfo()
        {
            messageType = "NS",
            NewSpeed = speed
        });
        /*StartCoroutine(communication
            .POSTRequest(serverUri, new SpeedInfo()
            {
                messageType = "NS",
                NewSpeed = speed
            }
            , response => { Debug.Log(response); })
        );*/
        Debug.Log(speed);
    }

    public void SendIsAliveInfo()
    {
        EnqueueRequest(serverUri, new AliveInfo()
        {
            messageType = "isAlive",
            isAlive = true
        });
        Debug.Log("entered");
        /*StartCoroutine(communication
            .POSTRequest(serverUri, new AliveInfo()
            {
                messageType = "isAlive",
                isAlive = true
            }
            , response => { Debug.Log(response); })
        );*/
    }
}


public class BaliseInfo:UnityMessage
{   
    //wszystko z duzych
    public string kilometer; //kilometry nie resetuja sie pomiedzy liniami, float lub double, przy wysylaniu string
    public int number; // numer balisy w grupia
    public int numberOfBalises; // nie odwoluje sie do ilosci balis w tracku czy line, szamiast tego balisy sa w grupach 
    public string trackNumber; //na string, numer pasu ulicy
    public int lineNumber; //sub track / ulica
    public string messageType;// typ ¿¹dania string
}

public class SpeedInfo:UnityMessage
{
    public string messageType;
    public float NewSpeed;
}

public class AliveInfo : UnityMessage
{
    public string messageType;
    public bool isAlive;
}

public class UnityMessage
{
    public string source = "UNITY";
}