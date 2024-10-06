using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Connection : MonoBehaviour
{
    public TMP_Text tmp;
    public bool shutUp;

    void Start()
    {
        if (!shutUp)
        {
            StartCoroutine(MakeRequest());
        }
    }

    private IEnumerator MakeRequest()
    {
        
        //GET
        var getRequest = CreateReqest("http://127.0.0.1:8000/name/");
        yield return getRequest.SendWebRequest();
        var deserializedGetData = JsonUtility.FromJson<JsonResponse>(getRequest.downloadHandler.text);
        changeText(deserializedGetData.message);
        

        
        //POST
        var dataToPost = new PostDATA() { name = "posting" };
        var postRequest = CreateReqest("http://127.0.0.1:8000/name/",RequestType.POST,dataToPost);
        yield return postRequest.SendWebRequest();
        string s = postRequest.downloadHandler.text;
        var deserializedPostData = JsonUtility.FromJson<JsonResponse>(postRequest.downloadHandler.text);
        changeText(deserializedPostData.message);
        
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

    public void changeText(string change)
    {
        Debug.Log("changing");
        string text = tmp.text;
        Debug.Log($"current {text}");
        text = change;
        Debug.Log($"new {change}");
        Debug.Log("changed");
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