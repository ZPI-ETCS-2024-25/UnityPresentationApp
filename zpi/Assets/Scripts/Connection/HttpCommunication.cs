using System.Collections;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;

public class Connection : MonoBehaviour
{
    public bool shutUp;

    void Start()
    {
        if (!shutUp)
        {
            StartCoroutine(GETRequest("http://127.0.0.1:8000/name/", result =>
            {
                Debug.Log(result);
            }
            ));
            StartCoroutine(POSTRequest("http://127.0.0.1:8000/name/", new PostDATA() { name = "test"},result =>
            {
                Debug.Log(result);
            }
            ));
            //StartCoroutine(MakeRequest());"http://127.0.0.1:8000/name/"
        }
    }

    
    /*private IEnumerator GETRequest(string uri)
    {
        var getRequest = CreateReqest(uri);
        yield return getRequest.SendWebRequest();
        var deserializedGetData = JsonUtility.FromJson<JsonResponse>(getRequest.downloadHandler.text);
        //deserializedGetData.message;
    }*/


    private IEnumerator GETRequest(string uri, System.Action<string> response)
    {
        var getRequest = CreateReqest(uri);
        yield return getRequest.SendWebRequest();

        if(getRequest.result == UnityWebRequest.Result.Success)
        {
            //var deserializedGetData = JsonUtility.FromJson<JsonResponse>(getRequest.downloadHandler.text);
            response?.Invoke(getRequest.downloadHandler.text);
        }
        yield break;
        
        //deserializedGetData.message;
    }

    /*private async Task<string> GETRequestAsync(string uri)
    {
        using (UnityWebRequest getRequest = CreateReqest(uri))
        {
            var webRequest = getRequest.SendWebRequest();


            while (!webRequest.isDone)
            {
                await Task.Yield(); 
            }

            if(getRequest.result == UnityWebRequest.Result.Success)
            {
                var deserializedGetData = JsonUtility.FromJson<JsonResponse>(getRequest.downloadHandler.text);
                return deserializedGetData.message;
            }
            else
            {
                return null;
            }   
        }
    }*/


    private IEnumerator POSTRequest(string uri,object postData, System.Action<string> response)
    {
        var postRequest = CreateReqest(uri,RequestType.POST,postData);
        yield return postRequest.SendWebRequest();

        if (postRequest.result == UnityWebRequest.Result.Success)
        {
            //var deserializedPostData = JsonUtility.FromJson<JsonResponse>(getRequest.downloadHandler.text);
            response?.Invoke(postRequest.downloadHandler.text);
        }
        yield break;
        //var getRequest = CreateReqest(uri);
        //yield return getRequest.SendWebRequest();
        //var deserializedGetData = JsonUtility.FromJson<JsonResponse>(getRequest.downloadHandler.text);
        //changeText(deserializedGetData.message);
    }


    /*private IEnumerator MakeRequest()
    {  
        //POST DO SERVERAETCS
        var dataToPost = new PostDATA() { name = "POST from Unity to ETCS Server" };
        var postRequest = CreateReqest("http://127.0.0.1:8000/name/",RequestType.POST,dataToPost);
        yield return postRequest.SendWebRequest();
        string s = postRequest.downloadHandler.text;
        var deserializedPostData = JsonUtility.FromJson<JsonResponse>(postRequest.downloadHandler.text);
        //changeText(deserializedPostData.message);

        //POST DO DRIVERETCS
        dataToPost = new PostDATA() { name = "POST from Unity to Driver" };
        postRequest = CreateReqest("http://127.0.0.1:8002/", RequestType.POST, dataToPost);
        yield return postRequest.SendWebRequest();
        s = postRequest.downloadHandler.text;
        //changeText(s);

    }
    */

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