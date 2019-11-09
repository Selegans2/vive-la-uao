using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoomYincanaHandler : MonoBehaviour
{
    
    public GameObject ZoomPrefab;
    public GameObject ZoomX3;

    public AreasManager SectorManager;
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetString("Yincana").Length != 0)
        {
            ZoomPrefab.SetActive(false);
        }
        else
        {
            ZoomPrefab.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

        switch (AreasManager.currentArea)
        {
            case 0:
                ZoomX3.transform.GetChild(0).gameObject.SetActive(true);
                ZoomX3.transform.GetChild(1).gameObject.SetActive(false);
                ZoomX3.transform.GetChild(2).gameObject.SetActive(false);
                ZoomX3.transform.GetChild(0).localPosition = new Vector3(0, SectorManager.stationPosition[1] - 20, 0) * -1;
                break;
            case 1:
                ZoomX3.transform.GetChild(1).gameObject.SetActive(true);
                ZoomX3.transform.GetChild(0).gameObject.SetActive(false);
                ZoomX3.transform.GetChild(2).gameObject.SetActive(false);
                ZoomX3.transform.GetChild(1).localPosition = new Vector3(0, SectorManager.stationPosition[1] - 20, 0) * -1;
                break;
            case 2:
                ZoomX3.transform.GetChild(2).gameObject.SetActive(true);
                ZoomX3.transform.GetChild(1).gameObject.SetActive(false);
                ZoomX3.transform.GetChild(0).gameObject.SetActive(false);
                ZoomX3.transform.GetChild(2).localPosition = new Vector3(0, SectorManager.stationPosition[2] - 20, 0) * -1;
                break;
        }

    }
}
