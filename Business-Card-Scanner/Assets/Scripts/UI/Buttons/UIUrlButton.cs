using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIUrlButton : MonoBehaviour {
    public Image Icon;
    public string url;

    public void OpenUrl () {
        Application.OpenURL (url);
    }
}