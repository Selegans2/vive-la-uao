using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AreasManager : MonoBehaviour
{
    [Header("Sky Objects")]
    public GameObject sunnySky;
    public GameObject nightSky;
    public float duration = 2.3f;
    [Space(10)]

    [Header("Areas")]
    public Camera mainCamera;
    public Vector3 lookPosition;
    public Vector3 lookRotation;
    [Space(5)]

    public List<Button> stationsButtons = new List<Button>();
    public List<Button> YincanaStationsButtons = new List<Button>();

    public List<float> stationPosition;
    public List<GameObject> areas = new List<GameObject>();

    public GameObject areasObject;
    public static int currentArea = 0;
    public int previousArea;


    // Start is called before the first frame update
    void Start()
    {
        //changueSector(2);
    }


    #region Custom Methods
    public void selectStation(int stationIndex) {

        if (stationIndex != currentArea)
        {
            changueSector(stationIndex);
        }
        
    }

    private IEnumerator setNightSky()
    {

        Color tempcolor = sunnySky.GetComponent<Renderer>().material.color;
        float elapsed = 0.0f;
        float speed = 1.5f;

        while (elapsed < speed)
        {
            tempcolor.a = Mathf.Lerp(1, 0.0f, elapsed / speed);
            sunnySky.GetComponent<Renderer>().material.color = tempcolor;
            elapsed += Time.deltaTime;

            if (elapsed >= (speed / 2))
            {
            }

            yield return null;
        }
    }

    private IEnumerator setDaySky()
    {

        Color tempcolor = sunnySky.GetComponent<Renderer>().material.color;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            tempcolor.a = Mathf.Lerp(0, 1.0f, elapsed / duration);
            sunnySky.GetComponent<Renderer>().material.color = tempcolor;
            elapsed += Time.deltaTime;

            if (elapsed >= (duration / 2))
            {
            }

            yield return null;
        }
    }


    private IEnumerator focusCamera() {
        float elapsed = 0.0f;
        float speed = 10.5f;

        while (elapsed < duration)
        {
            mainCamera.transform.localPosition = Vector3.Lerp(mainCamera.transform.position, lookPosition, elapsed / speed);
            mainCamera.transform.localRotation = Quaternion.Lerp(mainCamera.transform.localRotation, Quaternion.Euler(lookRotation), elapsed / speed);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCamera.transform.localPosition = lookPosition;
        mainCamera.transform.localRotation = Quaternion.Euler(lookRotation);
    }

    private IEnumerator moveToArea(int area) {
        Vector3 curPos = areasObject.transform.localPosition;

        /*float elapsed = 0.0f;
        float speed = 1.0f;

        while (elapsed < duration){

            areasObject.transform.localPosition = Vector3.Lerp(curPos, new Vector3(curPos.x, stationPosition[area], curPos.z), (elapsed / speed));

            elapsed += Time.deltaTime;
            yield return null;
        }

        areasObject.transform.localPosition = new Vector3(curPos.x, stationPosition[area], curPos.z);*/

        float elapsedTime = 0;
        float waitTime = 1.2f;

        while (elapsedTime < waitTime)
        {
            areasObject.transform.localPosition = Vector3.Lerp(curPos, new Vector3(curPos.x, stationPosition[area], curPos.z), (elapsedTime / waitTime));
            elapsedTime += Time.deltaTime;

            // Yield here
            yield return null;
        }
        // Make sure we got there
        areasObject.transform.localPosition = new Vector3(curPos.x, stationPosition[area], curPos.z);
        yield return null;
    }


    public void changueSector(int area)
    {
        StartCoroutine(changeStationSmooth(area));
    }

    IEnumerator changeStationSmooth(int area)
    {

        mainCamera.GetComponent<MobileCamera>().enabled = false;
        mainCamera.GetComponent<DesktopCamera>().enabled = false;
        StartCoroutine(focusCamera());     

        yield return new WaitForSeconds(0.8f);
        if (currentArea == 0)
        {
            StartCoroutine(setNightSky());
        }
        else if (currentArea > 0 && area == 0)
        {
            StartCoroutine(setDaySky());
        }
        

        yield return new WaitForSeconds(0.8f);
        StartCoroutine(moveToArea(area));

        yield return new WaitForSeconds(0.5f);
        areas[currentArea].SetActive(false);
        yield return new WaitForSeconds(0.5f);
        areas[area].SetActive(true);

        yield return new WaitForSeconds(0.95f);
        #if UNITY_IOS || UNITY_ANDROID
                mainCamera.GetComponent<MobileCamera>().enabled = true;
                mainCamera.GetComponent<DesktopCamera>().enabled = false;
        #else
                mainCamera.GetComponent<MobileCamera>().enabled = false;
                mainCamera.GetComponent<DesktopCamera>().enabled = true;
        #endif

        currentArea = area;
    }


#endregion

}
