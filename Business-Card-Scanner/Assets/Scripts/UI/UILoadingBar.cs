using UnityEngine;

public class UILoadingBar : UIBase {
    [SerializeField] private GameObject loadingImage;
    public void ShowLoadingBar () {
        loadingImage.SetActive (true);
    }

    public void HideLoadingBar () {
        loadingImage.SetActive (false);
    }
    private void Awake () {
        UIEngine.Set (this);
    }
}