using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityServerComm : MonoBehaviour
{
    public HttpCommunication communication;
    public string serverUri = "http://127.0.0.1:8002/";
    private string unityEndPoint = "unity/";
    //unity/crossingState
    void Start()
    {
        //RegisterTrain(7,200,80000,100,1000);
    }

    private void SendPOSTRequest(string endPoint, object postData)
    {
        StartCoroutine(communication.POSTRequest(serverUri+unityEndPoint+endPoint, postData, response => { Debug.Log(response); }));
    }

    public void SendCrossingState(int crossingId, bool damaged)
    {
        SendPOSTRequest("crossingState/", new CrossingState()
        {
            crossingId = crossingId,
            damaged = damaged
        }) ;
    }

    public void SendSemaphoreSignal(int semaphoreId)
    {

    }

    private void RegisterTrain(int trainId, int lenghtMeters, int weightKilos, int maxSpeedMps, int breakWeight)
    {
        communication
            .POSTRequest(serverUri,new TrainRegister() { trainId = trainId,
                                                        lenghtMeters = lenghtMeters,
                                                        weightKilos = weightKilos,
                                                        maxSpeedMps = maxSpeedMps,
                                                        breakWeight = breakWeight}
            ,response => { Debug.Log(response); });
    } 
}

public class CrossingState
{
    public int crossingId;
    public bool damaged;
}

public class TrainRegister
{
    public int trainId;
    public int lenghtMeters;
    public int weightKilos;
    public int maxSpeedMps;
    public int breakWeight;
}