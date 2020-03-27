using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour {
    public static VideoController Instance { get; set; }

    [SerializeField] private YoutubePlayer.YoutubePlayer youtubePlayer;
    private bool isPlaying = false;
    public bool IsPlaying { get => isPlaying && YoutubePlayer.VideoPlayer.isPlaying; private set => isPlaying = value; }
    public YoutubePlayer.YoutubePlayer YoutubePlayer { get => youtubePlayer; set => youtubePlayer = value; }

    private void OnEnable () {
        YoutubePlayer.VideoPlayer.started += OnVideoStarted;
    }
    private void OnDisable () {
        YoutubePlayer.VideoPlayer.started -= OnVideoStarted;
    }
    private void Awake () {
        if (Instance != null)
            Destroy (this);
        Instance = this;
    }

    public async void PlayVideo (string url) {
        if (string.IsNullOrEmpty (url)) {
            return;
        }
        IsPlaying = true;
        StartLoadingBar ();
        YoutubePlayer.youtubeUrl = url;
        await YoutubePlayer.PlayVideoAsync (url);
    }

    public void StopVideo () {
        YoutubePlayer.VideoPlayer.Stop ();
        IsPlaying = false;
        UIEngine.Show<UICamera> ();
        Screen.autorotateToPortrait = false;
        Screen.autorotateToPortraitUpsideDown = false;
        Screen.autorotateToLandscapeLeft = false;
        Screen.autorotateToLandscapeRight = false;
        Screen.orientation = ScreenOrientation.Portrait;
    }

    private void StartLoadingBar () {
        UIEngine.Get<UILoadingBar> ().Show ();
    }

    private void StopLoadingBar () {
        UIEngine.Get<UILoadingBar> ().Hide ();
    }

    private void OnVideoStarted (VideoPlayer player) {
        Screen.autorotateToPortrait = true;
        Screen.autorotateToPortraitUpsideDown = true;
        Screen.autorotateToLandscapeLeft = true;
        Screen.autorotateToLandscapeRight = true;
        StopLoadingBar ();
        UIEngine.Show<UIVideo> ();
    }

}