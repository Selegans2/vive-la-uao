using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class StationManager : MonoBehaviour
{
    #region Stations Pins Methods
    public GetPins pinsManager;
    public AreasManager areasManager;
    public GameObject target;
    public bool isTargetFocus;

    [Space(5)]
    public GameObject buttonsContainer;
    public GameObject stationButton;
    public GameObject targetButton;
    public GameObject sectorPanel;
    public Text sectorName;

    void Update()
    {
        if (pinsManager.currentPlace != AreasManager.currentArea.ToString())
        {
            switch (pinsManager.currentPlace)
            {
                case "0":
                    sectorName.text = "Campus UAO";
                    break;

                case "1":
                    sectorName.text = "UAO Labs 1";
                    break;

                case "2":
                    sectorName.text = "UAO Labs 2";
                    break;


            }
            sectorPanel.SetActive(true);
        }
        else
        {
            sectorPanel.SetActive(false);
        }
    }

    public void closeCameraStation()
    {
        isTargetFocus = true;

        stationButton.SetActive(false);
        targetButton.SetActive(true);

        if (pinsManager.currentPlace == AreasManager.currentArea.ToString())
        {
            StartCoroutine(focusCamera());
        }
        else
        {
            StartCoroutine(focusCameraOutSector());
        }
    }

    private IEnumerator focusCamera()
    {

        //disablePin();
        float elapsed = 0.0f;
        float duration = 2.2f;

        buttonsContainer.SetActive(false);
        //Changue smooth the Zoom of the camera
        Camera.main.GetComponent<DesktopCamera>().maxDistance = 28;
        Camera.main.GetComponent<MobileCamera>().maxDistance = 28;

        Vector3 finalPos = Vector3.zero;
        switch (pinsManager.currentPlace)
        {
            case "0":
                finalPos = new Vector3(pinsManager.currentPin.transform.position.x, pinsManager.currentPin.transform.position.y * 3.2f, pinsManager.currentPin.transform.position.z);
                break;

            default:
                finalPos = new Vector3(pinsManager.currentPin.transform.position.x, pinsManager.currentPin.transform.position.y * 1.5f, pinsManager.currentPin.transform.position.z);
                break;
        }
       

        while (elapsed < duration)
        {
            target.transform.position = Vector3.Lerp(target.transform.position, finalPos, elapsed / 8.0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        buttonsContainer.SetActive(true);
        target.transform.position = finalPos;
        Camera.main.GetComponent<DesktopCamera>().maxDistance = 130;
        Camera.main.GetComponent<MobileCamera>().maxDistance = 130;

    }


    private IEnumerator focusCameraOutSector()
    {

        //disablePin();
        float elapsed = 0.0f;
        float duration = 2.2f;

        buttonsContainer.SetActive(false);
        areasManager.changueSector(int.Parse(pinsManager.currentPlace));
        yield return new WaitForSeconds(4);

        //Changue smooth the Zoom of the camera
        Camera.main.GetComponent<DesktopCamera>().maxDistance = 28;
        Camera.main.GetComponent<MobileCamera>().maxDistance = 28;


        Vector3 finalPos = Vector3.zero;
        switch (pinsManager.currentPlace)
        {
            case "0":
                finalPos = new Vector3(pinsManager.currentPin.transform.position.x, pinsManager.currentPin.transform.position.y * 3.2f, pinsManager.currentPin.transform.position.z);
                break;

            default:
                finalPos = new Vector3(pinsManager.currentPin.transform.position.x, pinsManager.currentPin.transform.position.y * 1.5f, pinsManager.currentPin.transform.position.z);
                break;
        }

        while (elapsed < duration)
        {
            target.transform.position = Vector3.Lerp(target.transform.position, finalPos, elapsed / 8.0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        buttonsContainer.SetActive(true);
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
