using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

public class DataController : MonoBehaviour {
    public const string HOST_API = "https://site.com/api/";
    public const string HOST = "https://site.com/";
    public const string ID_QUERY_PARAM = "?id=";
    public static DataController Instance { get; set; }

    [SerializeField] private VideoController videoController;

    private Dictionary<string, CachedData> localBuffer = new Dictionary<string, CachedData> ();
    private int localInvokeCountBeforeUpdate = 3;

    private void Awake () {
        if (Instance != null)
            Destroy (this);
        Instance = this;

        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;

        // Enable vsync for the samples (avoid running mobile device at 300fps)
        QualitySettings.vSyncCount = 1;
    }

    private void Start () {
        BarCodeController.Instance.OnCodeFounded += OnCodeFounded;
#if UNITY_EDITOR
        OnCodeFounded ("", HOST + ID_QUERY_PARAM + 1);
#endif  

        // CardData test = new CardData () {
        //     id = "1",
        //     type = 0,
        //     url = "https://www.youtube.com/watch?v=gincn1XNs14",
        //     detailsData = new Details () {
        //     header = "Unity Technologies",
        //     description = "Unity’s real-time 3D development platform lets artists, designers and developers work together to create amazing immersive and interactive experiences. (Available for Windows, Mac, and Linux.)",
        //     image = "https://unity3d.com/files/images/ogimg.jpg",
        //     links = new string[] { "https://twitter.com/unity3d/" }
        //     }
        // };
        // Debug.Log (JsonConvert.SerializeObject (test));
    }

    private void OnCodeFounded (string codeType, string codeValue) {
        Log ($"OnCodeFounded: CodeType {codeType}, codeValue {codeValue}");
        if (!IsCorrectUrl (codeValue)) {
            Log ($"BAD URL {codeValue}");
            return;
        }
        if (videoController.IsPlaying) {
            Log ("Video already playing");
            return;
        }
        CachedData cachedData = null;
        string id = codeValue.Replace (HOST + ID_QUERY_PARAM, string.Empty);

        if (localBuffer.TryGetValue (id, out cachedData)) {
            if (cachedData.usageCount < localInvokeCountBeforeUpdate) {
                cachedData.usageCount++;
                videoController.PlayVideo (cachedData.data.url);
                return;
            }
        }
        StartCoroutine (WWWData (HOST_API + ID_QUERY_PARAM + id));
    }

    private IEnumerator WWWData (string url) {
        UnityWebRequest webRequest = UnityWebRequest.Get (url);
        yield return webRequest.SendWebRequest ();
        if (!webRequest.isNetworkError) {
            ParseResult (webRequest);
        } else {
            Log ("WWW Error: " + webRequest.error);
        }
    }

    private void ParseResult (UnityWebRequest webRequest) {
        CardData result = JsonConvert.DeserializeObject<CardData> (webRequest.downloadHandler.text);

        if (result == null) {
            Log ($"Error Result: {result}");
            return;
        }
        if (!string.IsNullOrEmpty (result.error)) {
            Log ($"Parse error: {result.error}");
            return;
        }
        CachedData cachedData = null;
        if (localBuffer.TryGetValue (result.id, out cachedData)) {
            cachedData.usageCount = 0;
        } else {
            cachedData = new CachedData () { data = result };
            localBuffer.Add (result.id, cachedData);
        }
        UIEngine.Get<UIDetails> ().SetData (result.details);
        videoController.PlayVideo (cachedData.data.url);
        Log ("Text: " + webRequest.downloadHandler.text);
    }

    private bool IsCorrectUrl (string url) {
        return url.Contains (HOST + ID_QUERY_PARAM);
    }

    private void Log (string text) {
        UILogger.Instance.Log (text);
    }
}

[Serializable]
public class CachedData {
    public CardData data;
    public int usageCount = 0;
}

[Serializable]
public class CardData {
    public string id;
    public int type;
    public string url;
    public Details details;
    public string error;
}

[Serializable]
public class Details {
    public string header;
    public string description;
    public string image;
    public string[] links;

    public Details () { }
}