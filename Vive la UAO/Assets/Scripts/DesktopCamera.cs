using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class DesktopCamera : MonoBehaviour
{
    private static readonly float[] BoundsX = new float[] { -10, 60 };
    private static readonly float[] BoundsZ = new float[] { -20, 20 };

    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = .6f;
    public float xSpeed = 200.0f;
    public float ySpeed = 200.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public int zoomRate = 40;
    public float panSpeed = 0.08f;
    public float zoomDampening = 5.0f;
    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;
    void Start() { Init(); }
    void OnEnable() { Init(); }
    RaycastHit hit;
    public void Init()
    {
        //If there is no target, create a temporary target at 'distance' from the cameras current viewpoint
        if (!target)
        {
            GameObject go = new GameObject("Cam Target");
            go.transform.position = transform.position + (transform.forward * distance);
            target = go.transform;
        }
        distance = Vector3.Distance(transform.position, target.position);
        currentDistance = distance;
        desiredDistance = distance;
        //be sure to grab the current rotations as starting points.
        position = transform.position;
        rotation = transform.rotation;
        currentRotation = transform.rotation;
        desiredRotation = transform.rotation;
        xDeg = Vector3.Angle(Vector3.right, transform.right);
        yDeg = Vector3.Angle(Vector3.up, transform.up);
        StartCoroutine(SetZoomDampening());
    }
    private IEnumerator SetZoomDampening()
    {
        yield return new WaitForSeconds(1.5f);
        zoomDampening = 5f;
        maxDistance = 130f;
    }
    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled.
     */

    private Vector3 touchStart;
    float groundZ = 0;

    [Space(10)]
    [Header("Camera Zoom")]
    public float distanceFocus = 1;
    public float curDistance;
    Vector3 direction;
    void LateUpdate()
    {
        // If Control and Alt and Middle button? ZOOM!
        if (Input.GetMouseButton(2))
        {
            desiredDistance -= Input.GetAxis("Mouse Y") * Time.deltaTime * zoomRate * 0.125f * Mathf.Abs(desiredDistance);
        }
        // If middle mouse and left alt are selected? ORBIT
        if (Input.GetMouseButton(1))
        {
            xDeg += Input.GetAxis("Mouse X") * xSpeed * 0.02f;
            yDeg -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            ////////OrbitAngle
            //Clamp the vertical axis for the orbit
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
            // set camera rotation
            desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
            currentRotation = transform.rotation;
            rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening);
            transform.rotation = rotation;
        }

        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        if (Input.GetMouseButtonDown(0))
        {
            touchStart = GetWorldPosition(groundZ);
        }
        //&& hitColliderName == "Campus UAO"
        // otherwise if middle mouse is selected, we pan by way of transforming the target in screenspace
        /* if (Input.GetMouseButton(0) && hitColliderName != null)
        {
            target.rotation = transform.rotation;
            direction = touchStart - GetWorldPosition(groundZ);
            target.transform.position += direction * panSpeed * 0.5f;

            // Ensure the camera remains within bounds.
            Vector3 pos = target.transform.position;
            pos.x = Mathf.Clamp(target.transform.position.x, BoundsX[0], BoundsX[1]);
            pos.z = Mathf.Clamp(target.transform.position.z, BoundsZ[0], BoundsZ[1]);
            target.transform.position = pos;
        } */
        if (Input.GetMouseButton(0))
        {
            if (hitColliderName != null)
            {
                target.rotation = transform.rotation;
                direction = touchStart - GetWorldPosition(groundZ);
                target.transform.position += direction * panSpeed * 0.5f;

                // Ensure the camera remains within bounds.
                Vector3 pos = target.transform.position;
                pos.x = Mathf.Clamp(target.transform.position.x, BoundsX[0], BoundsX[1]);
                pos.z = Mathf.Clamp(target.transform.position.z, BoundsZ[0], BoundsZ[1]);
                target.transform.position = pos;
                flag = true;
            }
            else if (hitColliderName == null && flag)
            {
                target.rotation = transform.rotation;
                direction = touchStart - GetWorldPosition(groundZ);
                target.transform.position += direction * panSpeed * 0.5f;

                // Ensure the camera remains within bounds.
                Vector3 pos = target.transform.position;
                pos.x = Mathf.Clamp(target.transform.position.x, BoundsX[0], BoundsX[1]);
                pos.z = Mathf.Clamp(target.transform.position.z, BoundsZ[0], BoundsZ[1]);
                target.transform.position = pos;
            }
        }
        else
        {
            flag = false;
        }
        ////////Orbit Position
        // affect the desired Zoom distance if we roll the scrollwheel
        desiredDistance -= Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * zoomRate * Mathf.Abs(desiredDistance);
        //clamp the zoom min/max
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        // For smoothing of the zoom, lerp distance

        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening);
        curDistance = currentDistance;

        // calculate position based on the new currentDistance
        position = target.position - (rotation * Vector3.forward * currentDistance + targetOffset);
        transform.position = position;
    }

    private static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
    string hitColliderName;
    bool flag = false;
    private Vector3 GetWorldPosition(float z)
    {
        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.down, new Vector3(0, 0, z));
        float distance;
        ground.Raycast(mousePos, out distance);

        if (Physics.Raycast(mousePos, out hit))
        {
            hitColliderName = hit.collider.name;
            //Debug.Log(hitColliderName);
        }
        else if (Physics.Raycast(mousePos, out hit) == false)
        {
            hitColliderName = null;
        }
        return mousePos.GetPoint(distance);

    }
}