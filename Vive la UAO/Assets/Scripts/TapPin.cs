using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TapPin : MonoBehaviour
{
    private string stationName;
    private string areaName;
    public static Station StationTapped = new Station();
    private GameObject AreaModal;
    private GameObject StationModal;

    // Start is called before the first frame update
    void Start()
    {
        try
        {
            StationModal = GameObject.FindGameObjectWithTag("station modal");
            AreaModal = GameObject.FindGameObjectWithTag("area modal");
        }
        catch{}
    }

    // Update is called once per frame
    void Update()
    {

    }

    //Show a modal after a tap in the current station has been done
    public void ShowStationModal()
    {
        stationName = transform.parent.parent.gameObject.name;
        StationTapped = GetPins.stationList.Find((st) => st.name == stationName);
        StationModal.SetActive(true);
        StationModal.transform.GetChild(0).gameObject.SetActive(true);
        var stationTextName = StationModal.transform.GetChild(0).GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>();
        stationTextName.text = StationTapped.name;
        var stationTextDescription = StationModal.transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>();
        stationTextDescription.text = StationTapped.description;
    }

    //Show a modal after a tap in the area has been done
    public void ShowAreaModal()
    {
        AreaModal.SetActive(true);
    }
}
