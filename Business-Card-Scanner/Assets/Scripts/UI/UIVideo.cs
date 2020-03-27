using UnityEngine;

public class UIVideo : UIBase {
    private void Awake () {
        UIEngine.Set (this);
    }
    public override void Hide () {
        base.Hide ();
        UIEngine.Get<UIDetails> ().Hide ();
    }
    public void ShowDetails () {
        UIEngine.Get<UIDetails> ().Show ();
    }

    public void Close () {
        VideoController.Instance.StopVideo ();
    }
}