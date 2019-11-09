using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Zoom : MonoBehaviour
{
    public GameObject ZoomX1;
    public GameObject ZoomX2;

    public static bool isAreaSelected;
    public DesktopCamera deskCamera;
    public MobileCamera mobileCamera;

    public AreasManager SectorManager;

    void Start()
    {

    }

    void Update()
    {

        if (deskCamera.curDistance < 80 && AreasManager.currentArea == 0)
        {
            switch (AreasManager.currentArea)
            {
                case 0:
                    ZoomX2.transform.GetChild(0).gameObject.SetActive(true);
                    ZoomX2.transform.GetChild(0).localPosition=new Vector3 (0,SectorManager.stationPosition[0],0);
                    break;
            }
            //Debug.Log("entroo");
        }
        else
        {
            ZoomX1.SetActive(true);
            ZoomX2.transform.GetChild(0).gameObject.SetActive(false);
            ZoomX2.transform.GetChild(1).gameObject.SetActive(false);
            ZoomX2.transform.GetChild(2).gameObject.SetActive(false);
            workDone = false;
        }

        if (deskCamera.curDistance <= deskCamera.maxDistance)
        {
            switch (AreasManager.currentArea)
            {
                case 1:
                    ZoomX2.transform.GetChild(1).gameObject.SetActive(true);
                    ZoomX2.transform.GetChild(1).localPosition=new Vector3 (0,SectorManager.stationPosition[1]-20,0) * -1;
                    break;
                case 2:
                    ZoomX2.transform.GetChild(2).gameObject.SetActive(true);
                    ZoomX2.transform.GetChild(2).localPosition=new Vector3 (0,SectorManager.stationPosition[2]-20,0) * -1;
                    break;
            }
            //Debug.Log("entroo");
        }

        if (ZoomX2.transform.GetChild(0).gameObject.activeSelf && !workDone)
        {
            StartCoroutine(disableAreas());
            //ZoomX1.SetActive(false);
        }
    }

    public bool workDone = false;
    IEnumerator disableAreas()
    {
        yield return new WaitForSeconds(0.81f);
        ZoomX1.SetActive(false);
        deskCamera.maxDistance = 130;
        workDone = true;
    }

    /*private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.name == "Zoom Threshold")
        {
            if (isAreaSelected) {
                backAreaButton.SetActive(true);
                ZoomX2.SetActive(true);
                ZoomX1.SetActive(false);
            }
        }

        Debug.Log("estado: " + isAreaSelected);
    }*/

    /*private void OnTriggerExit(Collider other)
    {

        if (other.gameObject.name == "Zoom Threshold")
        {
            isAreaSelected = false;
            ZoomX1.SetActive(true);
            ZoomX2.SetActive(false);
        }
    }*/

    public void exitStations()
    {

        deskCamera.maxDistance = 130f;
        mobileCamera.maxDistance = 130f;

        isAreaSelected = false;

        ZoomX1.SetActive(true);
        ZoomX2.SetActive(false);
    }

}
