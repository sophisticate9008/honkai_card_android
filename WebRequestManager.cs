
using System.Collections;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class WebRequestManager : MonoBehaviour
{
    public int timeout = 120; // 设置默认超时时间为 10 秒

    // 发送 GET 请求
    public IEnumerator SendGetRequest(string url, System.Action<string> onSuccess, System.Action<string> onFailure)
    {
        using (UnityWebRequest webRequest = UnityWebRequest.Get(url))
        {
            webRequest.timeout = timeout; // 设置超时时间

            // 请求并等待服务器响应
            yield return webRequest.SendWebRequest();

            // 检查是否有错误
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                // 如果有错误，调用失败回调
                onFailure?.Invoke(webRequest.error);
            }
            else
            {
                // 如果成功，调用成功回调
                onSuccess?.Invoke(webRequest.downloadHandler.text);
            }
        }
    }

    // 发送 POST 请求
    public IEnumerator SendPostRequest(string url, string jsonData, System.Action<MyData> onSuccess, System.Action<string> onFailure)
    {
        using (UnityWebRequest webRequest = new UnityWebRequest(url, "POST"))
        {
            // 将JSON字符串转换为字节
            byte[] jsonToSend = new UTF8Encoding().GetBytes(jsonData);
            webRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            webRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();

            // 设置请求头部为application/json
            webRequest.SetRequestHeader("Content-Type", "application/json");

            // 发送请求并等待服务器响应
            yield return webRequest.SendWebRequest();

            // 检查是否有错误
            if (webRequest.result != UnityWebRequest.Result.Success)
            {
                // 如果有错误，调用失败回调
                onFailure?.Invoke(webRequest.error);
            }
            else
            {
                // 尝试解析JSON
                try
                {
                    string responseJson = webRequest.downloadHandler.text;
                    Debug.Log(responseJson);
                    MyData data = JsonUtility.FromJson<MyData>(responseJson);
                    onSuccess?.Invoke(data);
                }
                catch (System.Exception e)
                {
                    // 如果解析失败，调用失败回调
                    onFailure?.Invoke(e.Message);
                }
            }
        }
    }

}

[System.Serializable]
public class MyData
{
    public string nickname;
    public string card_str;
}