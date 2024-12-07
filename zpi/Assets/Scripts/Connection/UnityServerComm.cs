using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityServerComm : CoroutineComm
{
    //public HttpCommunication communication;
    public string serverUri = "http://127.0.0.1:8002/";
    private string unityEndPoint = "unity/";
    //unity/crossingState
    void Start()
    {
        //RegisterTrain(7,200,80000,100,1000);
    }

    private void SendPOSTRequest(string endPoint, object postData)
    {
        //EnqueueRequest(serverUri + unityEndPoint + endPoint, postData);
        StartCoroutine(communication.POSTRequest(serverUri+unityEndPoint+endPoint, postData, response => { Debug.Log(response); }));
    }

    public void SendCrossingState(int crossingId, bool damaged)
    {
        /*EnqueueRequest(serverUri + unityEndPoint + "crossingState/", new CrossingState()
        {
            CrossingId = crossingId,
            IsFunctional = damaged
        });*/
        SendPOSTRequest("crossingState/", new CrossingState()
        {
            CrossingId = crossingId,
            IsFunctional = damaged
        }) ;
    }

    public void SendSemaphoreSignal(int semaphoreId, bool shouldGo)
    {
        /*EnqueueRequest(serverUri + unityEndPoint + "semaphoreState/", new SemaphoreState()
        {
            SemaphoreId = semaphoreId,
            Go = shouldGo
        });*/
        SendPOSTRequest("semaphoreState/", new SemaphoreState()
        {
            SemaphoreId = semaphoreId,
            Go = shouldGo
        });
    }

    public void SendJunctionState(int junctionId, bool straight)
    {
        /*EnqueueRequest( serverUri +unityEndPoint + "JunctionState/", new JunctionState()
        {
            JunctionId = junctionId,
            Straight = straight
        });*/
        SendPOSTRequest("JunctionState/", new JunctionState()
        {
            JunctionId = junctionId,
            Straight = straight
        });
    }


    private void RegisterTrain(int trainId, int lenghtMeters, int weightKilos, int maxSpeedMps, int breakWeight)
    {
        /*EnqueueRequest(serverUri, new TrainRegister()
        {
            trainId = trainId,
            lenghtMeters = lenghtMeters,
            weightKilos = weightKilos,
            maxSpeedMps = maxSpeedMps,
            breakWeight = breakWeight
        });*/
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
    public int CrossingId;
    public bool IsFunctional;
}

public class SemaphoreState
{
    public int SemaphoreId;
    public bool Go;
}

public class JunctionState
{
    public int JunctionId;
    public bool Straight;
}

public class TrainRegister
{
    public int trainId;
    public int lenghtMeters;
    public int weightKilos;
    public int maxSpeedMps;
    public int breakWeight;
}