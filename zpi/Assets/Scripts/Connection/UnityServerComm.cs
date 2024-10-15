using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnityServerComm : MonoBehaviour
{
    public HttpCommunication communication;
    public string serverUri = "http://127.0.0.1:8002/";

    void Start()
    {
        RegisterTrain(7,200,80000,100,1000);
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


public class TrainRegister
{
    public int trainId;
    public int lenghtMeters;
    public int weightKilos;
    public int maxSpeedMps;
    public int breakWeight;
}