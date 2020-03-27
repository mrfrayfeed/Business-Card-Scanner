using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UILogger : MonoBehaviour {
    public static UILogger Instance { get; set; }

    [SerializeField] private bool debug = true;
    [SerializeField] private Text LogText;

    private void Awake () {
        if (Instance != null)
            Destroy (this);
        Instance = this;
        if (!debug) {
            LogText.gameObject.SetActive (false);
        }
    }

    public void Log (string text) {
        Debug.Log (text);
        if (!debug) {
            return;
        }
        if (LogText.text.Length > 250) {
            LogText.text = "";
        }
        LogText.text += text;
    }

}