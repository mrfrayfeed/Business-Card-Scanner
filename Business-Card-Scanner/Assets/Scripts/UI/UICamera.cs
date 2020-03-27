using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UICamera : UIBase {
    public RawImage CameraImage;
    private void Awake () {
        UIEngine.Set (this);
    }
}