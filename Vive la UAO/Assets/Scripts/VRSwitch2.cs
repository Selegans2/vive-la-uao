using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
public class VRSwitch2 : MonoBehaviour{
	private const float DRAG_RATE = .2f;
	int portraitField=85;
	int VRField=60;
	int temp=0;
	float dragYawDegrees;
    public GameObject gvrEventSystem;
    public GameObject unityEventSystem;
    public GameObject test;
    //public GameObject hideMenu1;
    //public GameObject hideMenu2;
    //public GameObject hideMenu3;
    // Use this for initialization
    void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (temp==0) {
			StartCoroutine(LoadDevice(""));
			Input.gyro.enabled = true;
			XRSettings.enabled = false;
            if (Screen.orientation == ScreenOrientation.Portrait)
            {
                Camera.main.fieldOfView = portraitField;
            }
            else if (Screen.orientation == ScreenOrientation.LandscapeLeft || Screen.orientation == ScreenOrientation.LandscapeRight)
            {
                Camera.main.fieldOfView = VRField;
            }
            //Stability
            ResetCameras ();
			// Update `dragYawDegrees` based on user touch.
			CheckDrag ();

			Camera.main.transform.localRotation =
			// Allow user to drag left/right to adjust direction they're facing.
				Quaternion.Euler (0f, -dragYawDegrees, 0f) *

				// Neutral position is phone held upright, not flat on a table.
			Quaternion.Euler (90f, 0f, 0f) *

				// Sensor reading, assum ing default `Input.compensateSensors == true`.
			Input.gyro.attitude *

				// So image is not upside down.
			Quaternion.Euler (0f, 0f, 180f);
			return;
		} else if (temp == 1) {
            StartCoroutine(activate());
            if (Input.GetKeyDown (KeyCode.Escape)) {
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                temp = 0;
			}
			return;
		}
	}

	void CheckDrag () {
		if (Input.touchCount != 1) {
			return;
		}

		Touch touch = Input.GetTouch (0);
		if (touch.phase != TouchPhase.Moved) {
			return;
		}

		dragYawDegrees += touch.deltaPosition.x * DRAG_RATE;
	}

	IEnumerator SwitchTo2D() {
		// Empty string loads the "None" device.
		XRSettings.LoadDeviceByName("");

		// Must wait one frame after calling `XRSettings.LoadDeviceByName()`.
		yield return null;

		// Not needed, since loading the None (`""`) device takes care of this.
		// XRSettings.enabled = false;

		// Restore 2D camera settings.

		ResetCameras();

		//Camera.main.ResetAspect ();
		//Camera.main.GetComponent<Transform> ().localRotation = InputTracking.GetLocalRotation (XRNode.CenterEye);
	}

	// Resets camera transform and settings on all enabled eye cameras.
	void ResetCameras() {
		// Camera looping logic copied from GvrEditorEmulator.cs
		for (int i = 0; i < Camera.allCameras.Length; i++) {
			Camera cam = Camera.allCameras[i];
			if (cam.enabled && cam.stereoTargetEye != StereoTargetEyeMask.None) {

				// Reset local position.
				// Only required if you change the camera's local position while in 2D mode.
				cam.transform.localPosition = Vector3.zero;

				// Reset local rotation.
				// Only required if you change the camera's local rotation while in 2D mode.
				cam.transform.localRotation = Quaternion.identity;

				// No longer needed, see issue github.com/googlevr/gvr-unity-sdk/issues/628.
				// cam.ResetAspect();

				// No need to reset `fieldOfView`, since it's reset automatically.
			}
		}
	}

	// Exit "None-2d Mode" and enthers Cardboard mode
	IEnumerator LoadDevice(string newDevice)
	{
		XRSettings.LoadDeviceByName(newDevice);
		yield return null;
	}
	IEnumerator activate(){
        yield return null;      
        XRSettings.enabled = true;
		//Camera.main.ResetAspect ();
		Camera.main.GetComponent<Transform> ().localRotation = InputTracking.GetLocalRotation (XRNode.CenterEye);
        ResetCameras();
    }
	public void switchMode(){
		//if was deactivated, do:
		if (temp==0){
            unityEventSystem.SetActive(false);
            test.SetActive(true);
            gvrEventSystem.SetActive(true);

            //DontDestroyOnLoad(this.gvrEventSystem);
            //DontDestroyOnLoad(this.unityEventSystem);
            temp = 1;
			//Input.gyro.enabled = false;
			StartCoroutine(LoadDevice("cardboard"));
			return;
		}
		//if was activated, do:
		else if (temp==1){          
            temp = 0;
			//StartCoroutine(LoadDevice(""));
            //XRSettings.enabled = false;
            return;
		}
	}
}


