using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StationManager : MonoBehaviour
{
    #region Stations Pins Methods
    public GetPins pinsManager;
    public GameObject target;
    public bool isTargetFocus;

    [Space(5)]
    public GameObject stationButton;
    public GameObject targetButton;

    public void closeCameraStation()
    {
        isTargetFocus = true;

        stationButton.SetActive(false);
        targetButton.SetActive(true);

        StartCoroutine(focusCamera());
    }

    private IEnumerator focusCamera()
    {

        //disablePin();
        float elapsed = 0.0f;
        float duration = 2.8f;


        //Changue smooth the Zoom of the camera
        Camera.main.GetComponent<DesktopCamera>().maxDistance = 28;
        Camera.main.GetComponent<MobileCamera>().maxDistance = 28;


        Vector3 finalPos = new Vector3(pinsManager.currentPin.transform.position.x, pinsManager.currentPin.transform.position.y * 3, pinsManager.currentPin.transform.position.z);

        while (elapsed < duration)
        {
            target.transform.position = Vector3.Lerp(target.transform.position, finalPos, elapsed / 8.0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.transform.position = finalPos;
        Camera.main.GetComponent<DesktopCamera>().maxDistance = 130;
        Camera.main.GetComponent<MobileCamera>().maxDistance = 130;

    }


    public void centerCamera()
    {
        isTargetFocus = false;

        stationButton.SetActive(true);
        targetButton.SetActive(false);

        StartCoroutine(centerCamera_Smooth());
    }

    private IEnumerator centerCamera_Smooth()
    {

        //disablePin();
        float elapsed = 0.0f;
        float duration = 2.8f;


        //Changue smooth the Zoom of the camera
        Camera.main.GetComponent<DesktopCamera>().minDistance = 75;
        Camera.main.GetComponent<MobileCamera>().minDistance = 75;


        Vector3 finalPos = Vector3.zero;

        while (elapsed < duration)
        {
            target.transform.position = Vector3.Lerp(target.transform.position, finalPos, elapsed / 8.0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.transform.position = finalPos;
        Camera.main.GetComponent<DesktopCamera>().minDistance = 0.6f;
        Camera.main.GetComponent<MobileCamera>().minDistance = 0.6f;

    }
    #endregion
}
