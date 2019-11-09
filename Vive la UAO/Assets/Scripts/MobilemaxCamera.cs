using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MobilemaxCamera : MonoBehaviour
{

    private static readonly float[] BoundsX = new float[] { 0, 80 };
    private static readonly float[] BoundsZ = new float[] { -40f, 40 };
    public Transform target;
    public Vector3 targetOffset;
    public float distance = 5.0f;
    public float maxDistance = 20;
    public float minDistance = 0.6f;
    public float xSpeed = 5.0f;
    public float ySpeed = 5.0f;
    public int yMinLimit = -80;
    public int yMaxLimit = 80;
    public float zoomRate = 10.0f;
    public float panSpeed = 5.0f;
    public float zoomDampening = 10.0f;

    private float xDeg = 0.0f;
    private float yDeg = 0.0f;
    private float currentDistance;
    private float desiredDistance;
    private Quaternion currentRotation;
    private Quaternion desiredRotation;
    private Quaternion rotation;
    private Vector3 position;

    private Vector3 FirstPosition;
    private Vector3 SecondPosition;
    private Vector3 delta;
    private Vector3 lastOffset;
    private Vector3 lastOffsettemp;
    //private Vector3 CameraPosition;
    //private Vector3 Targetposition;
    //private Vector3 MoveDistance;
    public Text debugText;
    public Text debugText2;

    Camera cam;

    public GameObject Campus;

    void Start() { Init(); }
    void OnEnable() { Init(); }

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

        //Defines orthographic camera
        cam = this.GetComponent<Camera>();
      StartCoroutine(SetZoomDampening());
    }
    private IEnumerator SetZoomDampening()
    {
        yield return new WaitForSeconds(3f);
        zoomDampening = 10f;
        maxDistance = 130f;
    }

    /*
     * Camera logic on LateUpdate to only update after all character movement logic has been handled.
     */
    private Vector3 touchStart;
    float groundZ = 0;
    void LateUpdate()
    {
        
        // If Control and Alt and Middle button? ZOOM!
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);

            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPreviousPosition = touchZero.position - touchZero.deltaPosition;

            Vector2 touchOnePreviousPosition = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPreviousPosition - touchOnePreviousPosition).magnitude;

            float TouchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float deltaMagDiff = prevTouchDeltaMag - TouchDeltaMag;

            if (cam.orthographic)
            {
                cam.orthographicSize += deltaMagDiff * Time.deltaTime * (zoomRate / 10);
                if (cam.orthographicSize >= 90)
                    cam.orthographicSize = 90;
                if (cam.orthographicSize <= 25)
                    cam.orthographicSize = 25;
            }
            if (Input.GetTouch(0).phase == TouchPhase.Moved && Input.GetTouch(1).phase == TouchPhase.Moved)
                desiredDistance += deltaMagDiff * Time.deltaTime * zoomRate * 0.005f * Mathf.Abs(desiredDistance);

        }
        
        // If middle mouse and left alt are selected? ORBIT
        if (Input.touchCount == 1)
        {
            Vector2 touchposition = Input.GetTouch(0).deltaPosition;
            xDeg += touchposition.x * xSpeed * 0.03f;
            yDeg -= touchposition.y * ySpeed * 0.03f;
            yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);

        }
        /* 
        if (Input.touchCount == 2)
        {

            if (Input.GetTouch(0).deltaPosition.y < Input.GetTouch(1).deltaPosition.y)
            {
                 
                if (Input.GetTouch(0).phase == TouchPhase.Moved)
                {
                    Vector2 touchposition = Input.GetTouch(0).deltaPosition;
                    //xDeg += touchposition.x * xSpeed * 0.008f;
                    xDeg -= touchposition.x * xSpeed * 0.085f ;
                    yDeg -= touchposition.y * ySpeed * 0.035f;
                    yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
                }
                if (Input.GetTouch(1).phase == TouchPhase.Moved)
                {
                    Vector2 touchposition = Input.GetTouch(1).deltaPosition;
                    //xDeg += touchposition.x * xSpeed * 0.008f;
                    xDeg += touchposition.x * xSpeed * 0.085f;
                    yDeg -= touchposition.y * ySpeed * 0.035f;
                    yDeg = ClampAngle(yDeg, yMinLimit, yMaxLimit);
                }
            }
        }

        ////-------------------------------------
        /* 
       if (Input.GetTouch (0).deltaPosition.x > Input.GetTouch (1).deltaPosition.y) {
           if (Input.GetTouch (0).phase == TouchPhase.Moved) {
               Vector2 touchposition = Input.GetTouch (0).deltaPosition;
               xDeg = touchposition.x * Time.deltaTime * 10f;
               Campus.transform.Rotate (0f, xDeg, 0f, Space.Self);
               //transform.Rotate(Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * speed);
           }
           if (Input.GetTouch (1).phase == TouchPhase.Moved) {
               Vector2 touchposition = Input.GetTouch (1).deltaPosition;
               xDeg = touchposition.x * Time.deltaTime * 10f;
               Campus.transform.Rotate (0f, -xDeg, 0f, Space.Self);
               //transform.Rotate(Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * speed);
           }
       }
       if (Input.GetTouch (0).deltaPosition.x < Input.GetTouch (1).deltaPosition.x) {
           if (Input.GetTouch (0).phase == TouchPhase.Moved) {
               Vector2 touchposition = Input.GetTouch (0).deltaPosition;
               xDeg = touchposition.x * Time.deltaTime * 10f;
               Campus.transform.Rotate (0f, xDeg, 0f, Space.Self);
               //transform.Rotate(Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * speed);
           }
           if (Input.GetTouch (1).phase == TouchPhase.Moved) {
               Vector2 touchposition = Input.GetTouch (1).deltaPosition;
               xDeg = touchposition.x * Time.deltaTime * 10f;
               Campus.transform.Rotate (0f, -xDeg, 0f, Space.Self);
               //transform.Rotate(Vector3(Input.GetAxis("Mouse Y"), Input.GetAxis("Mouse X"), 0) * Time.deltaTime * speed);
           }
       }*/


        if (Input.GetMouseButtonDown(0))
        {
            touchStart = GetWorldPosition(groundZ);
        }
        if (Input.touchCount == 3 && (Input.GetTouch(0).phase == TouchPhase.Moved || Input.GetTouch(1).phase == TouchPhase.Moved || Input.GetTouch(2).phase == TouchPhase.Moved))
        {
            //grab the rotation of the camera so we can move in a psuedo local XY space
            target.rotation = transform.rotation;
            Vector3 direction = touchStart - GetWorldPosition(groundZ);
            target.transform.position += direction * panSpeed * 0.04f;

            Vector3 pos = target.transform.position;
            pos.x = Mathf.Clamp(target.transform.position.x, BoundsX[0], BoundsX[1]);
            pos.z = Mathf.Clamp(target.transform.position.z, BoundsZ[0], BoundsZ[1]);
            target.transform.position = pos;
        }
        desiredRotation = Quaternion.Euler(yDeg, xDeg, 0);
        currentRotation = transform.rotation;
        rotation = Quaternion.Lerp(currentRotation, desiredRotation, Time.deltaTime * zoomDampening/2);
        transform.rotation = rotation;

        if (Input.GetMouseButtonDown(1))
        {
            FirstPosition = Input.mousePosition;
            lastOffset = targetOffset;
        }
/* 
        if (Input.GetMouseButton(1))
        {
            SecondPosition = Input.mousePosition;
            delta = SecondPosition - FirstPosition;
            targetOffset = lastOffset + transform.right * delta.x * 0.003f + transform.up * delta.y * 0.003f;

        }
*/
        ////////Orbit Position

        // affect the desired Zoom distance if we roll the scrollwheel
        desiredDistance = Mathf.Clamp(desiredDistance, minDistance, maxDistance);
        currentDistance = Mathf.Lerp(currentDistance, desiredDistance, Time.deltaTime * zoomDampening/2);

        position = target.position - (rotation * Vector3.forward * currentDistance);

        position = position - targetOffset;

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
    private Vector3 GetWorldPosition(float z)
    {
        Ray mousePos = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane ground = new Plane(Vector3.down, new Vector3(0, 0, z));
        float distance;
        ground.Raycast(mousePos, out distance);
        return mousePos.GetPoint(distance);
    }
}