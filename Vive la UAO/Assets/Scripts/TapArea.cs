using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class TapArea : MonoBehaviour, IPointerClickHandler
{
    public Vector3 offset;
    public GameObject target;
    public Vector3 finalPos;

    public void OnPointerClick(PointerEventData eventData)
    {
        target = GameObject.Find("Target");
        finalPos = gameObject.transform.position;
        Zoom.isAreaSelected = true;

        closeCamera();
    }

    void Update()
    {
        target = GameObject.Find("Target");
        finalPos = gameObject.transform.position;
    }

    public void closeCamera()
    {
        StartCoroutine(focusCamera());
    }

    public void disablePin()
    {
        GameObject areaParent = this.transform.parent.gameObject;
        areaParent.transform.GetChild(0).gameObject.SetActive(false);

        areaParent.transform.GetChild(1).gameObject.GetComponent<MeshRenderer>().enabled = false;
        areaParent.transform.GetChild(1).gameObject.GetComponent<MeshCollider>().enabled = false;
        areaParent.transform.GetChild(1).gameObject.GetComponent<BoxCollider>().enabled = false;

    }

    private IEnumerator focusCamera() {

        //disablePin();
        float elapsed = 0.0f;
        float duration = 2.8f;
        

        //Changue smooth the Zoom of the camera
        Camera.main.GetComponent<DesktopCamera>().maxDistance = 40;
        Camera.main.GetComponent<MobileCamera>().maxDistance = 40;


        //Camera.main.gameObject.GetComponent<DesktopCamera>().enabled = false;
        finalPos = gameObject.transform.position;

        while (elapsed < duration) {
            target.transform.position = Vector3.Lerp(target.transform.position, finalPos, elapsed / 8.0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        target.transform.position = finalPos;
        Camera.main.GetComponent<DesktopCamera>().maxDistance = 130;
        Camera.main.GetComponent<MobileCamera>().maxDistance = 130;

    }
}
