using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIDetails : UIBase {
    private const string FAVICON_HOST = "http://www.google.com/s2/favicons?domain=";
    [SerializeField] private TextMeshProUGUI headerText;
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Transform contentButtons;
    [SerializeField] private GameObject contentButtonPrefab;

    private void Awake () {
        UIEngine.Set (this);
    }

    public void SetData (Details data) {
        if (!string.IsNullOrEmpty (data.image)) {
            StartCoroutine (LoadImage (data.image, image));
        }
        headerText.text = data.header;
        descriptionText.text = data.description;
        foreach (Transform child in contentButtons) {
            GameObject.Destroy (child.gameObject);
        }
        foreach (var link in data.links) {
            var obj = Instantiate (contentButtonPrefab, Vector3.zero, Quaternion.identity, contentButtons);
            if (obj.TryGetComponent<UIUrlButton> (out UIUrlButton button)) {
                button.url = link;
                Uri uri = new Uri (link);
                StartCoroutine (LoadImage (FAVICON_HOST + uri.Host, button.Icon));
            }
        }
    }

    IEnumerator LoadImage (string url, Image img) {
        WWW www = new WWW (url);
        yield return www;
        img.sprite = Sprite.Create (www.texture, new Rect (0, 0, www.texture.width, www.texture.height), new Vector2 (0, 0));
    }
}