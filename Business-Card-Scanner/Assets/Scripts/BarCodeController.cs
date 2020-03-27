using System;
using System.Collections;
using BarcodeScanner;
using BarcodeScanner.Scanner;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Video;

public class BarCodeController : MonoBehaviour {
	public static BarCodeController Instance { get; set; }
	public Action<string, string> OnCodeFounded;

	private IScanner BarcodeScanner;
	private float RestartTime;
	private VideoPlayer VideoPlayer;

	private void Awake () {
		if (Instance != null)
			Destroy (this);
		Instance = this;
	}

	private IEnumerator Start () {
		// Create a basic scanner
		yield return Application.RequestUserAuthorization (UserAuthorization.WebCam);

		if (!Application.HasUserAuthorization (UserAuthorization.WebCam)) {
			throw new Exception ("This Webcam library can't work without the webcam authorization");
		}
		ScannerSettings scannerSettings = new ScannerSettings ();
		scannerSettings.WebcamRequestedWidth = Screen.width;
		scannerSettings.WebcamRequestedHeight = Screen.height;

		var image = UIEngine.Get<UICamera> ().CameraImage;
		image.rectTransform.sizeDelta = new Vector2 (Screen.width, Screen.height);

		BarcodeScanner = new Scanner (scannerSettings);
		BarcodeScanner.Camera.Play ();

		// Display the camera texture through a RawImage
		BarcodeScanner.OnReady += (sender, arg) => {
			Log ("BarcodeScanner.OnReady");
			// Set Orientation & Texture
			image.transform.localEulerAngles = BarcodeScanner.Camera.GetEulerAngles ();
			image.transform.localScale = BarcodeScanner.Camera.GetScale ();
			image.texture = BarcodeScanner.Camera.Texture;
			image.color = Color.white;
			// Keep Image Aspect Ratio
			var rect = image.GetComponent<RectTransform> ();
			var newHeight = rect.sizeDelta.x * BarcodeScanner.Camera.Height / BarcodeScanner.Camera.Width;
			rect.sizeDelta = new Vector2 (rect.sizeDelta.x, newHeight);

			RestartTime = Time.realtimeSinceStartup;
		};
	}

	/// <summary>
	/// Start a scan and wait for the callback (wait 1s after a scan success to avoid scanning multiple time the same element)
	/// </summary>
	private void StartScanner () {
		Log ("StartScan: ");
		BarcodeScanner.Scan ((barCodeType, barCodeValue) => {
			BarcodeScanner.Stop ();
			Log ("Found: " + barCodeType + " / " + barCodeValue + "\n");
			OnCodeFounded?.Invoke (barCodeType, barCodeValue);
			RestartTime += Time.realtimeSinceStartup + 1f;
		});
	}

	public void Log (string text) {
		UILogger.Instance.Log (text);
	}

	/// <summary>
	/// The Update method from unity need to be propagated
	/// </summary>
	private void Update () {
		if (BarcodeScanner != null) {
			BarcodeScanner.Update ();
		}

		// Check if the Scanner need to be started or restarted
		if (RestartTime != 0 && RestartTime < Time.realtimeSinceStartup && !VideoController.Instance.IsPlaying) {
			StartScanner ();
			RestartTime = 0;
		}
	}

	#region UI Buttons

	/// <summary>
	/// This coroutine is used because of a bug with unity (http://forum.unity3d.com/threads/closing-scene-with-active-webcamtexture-crashes-on-android-solved.363566/)
	/// Trying to stop the camera in OnDestroy provoke random crash on Android
	/// </summary>
	/// <param name="callback"></param>
	/// <returns></returns>
	public IEnumerator StopCamera (Action callback) {
		// Stop Scanning
		UIEngine.Get<UICamera> ().CameraImage = null;
		BarcodeScanner.Destroy ();
		BarcodeScanner = null;

		// Wait a bit
		yield return new WaitForSeconds (0.1f);

		callback.Invoke ();
	}

	#endregion
}