using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Camera : MonoBehaviour
{
    public Camera currentCam;

    InputAction zoomAction = new InputAction(binding: "<Mouse>/scroll");
    InputAction rotateAction = new InputAction(binding: "<Mouse>/rightbutton");

    private static GameObject target;
    private static float minDist;
    private static float maxDist;
    private static float currDistance;
    private static float front;
    private static float rightRange;
    private static float leftRange;
    private static float upRange;
    private static float downRange;
    private static float currentXRotation;
    private static float currentYRotation;
    private static bool rmbHeld;

    private void Start()
    {
        zoomAction.performed += ctx =>
        {
            currDistance -= Mouse.current.scroll.ReadValue().y / 300;
            currDistance = Mathf.Clamp(currDistance, minDist, maxDist);
        };
        rotateAction.started += ctx =>
        {
            rmbHeld = true;        
        };
        rotateAction.canceled += ctx =>
        {
            rmbHeld = false;
            currentXRotation = 0;
            currentYRotation = 0;
        };

        zoomAction.Enable();
        rotateAction.Enable();

        SetTarget("DoorLock", -180);
        SetDistance(1, 3);
        SetRange(40, 160, 120, 120);
    }

    void Update()
    {
        if (rmbHeld)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            currentXRotation = -delta.y / 2;
            currentYRotation = delta.x / 2;
        }
        if (target)
        {
            currentCam.transform.position = target.transform.position;
            Quaternion rot = currentCam.transform.rotation;
            Debug.Log(rot.x);
            if (rot.y > leftRange)
            {
                Debug.Log("Too far left");
                Debug.Log(rot.y + " X: Right- " + (rightRange) + ", Left- " + (leftRange));
                currentCam.transform.rotation = new Quaternion(rot.x, leftRange, rot.z, rot.w);
            }
            if(rot.y < rightRange)
            {
                Debug.Log("Too far right");
                Debug.Log(rot.y + " X: Right- " + (rightRange) + ", Left- " + (leftRange));
                currentCam.transform.rotation = new Quaternion(rot.x, rightRange, rot.z, rot.w);
            }
            if (rot.x < -downRange)
            {
                Debug.Log("Too far down");
                Debug.Log(rot.x + " Y: Up- " + (upRange) + ", Down- " + -downRange);
                currentCam.transform.rotation = new Quaternion(-downRange, rot.y, rot.z, rot.w);
            }
            if (rot.x > upRange)
            {
                Debug.Log("Too far up");
                Debug.Log(rot.x + " Y: Up- " + (upRange) + ", Down- " + -downRange);
                currentCam.transform.rotation = new Quaternion(upRange, rot.y, rot.z, rot.w);
            }
            currentCam.transform.Rotate(currentXRotation, currentYRotation, 0);

            currentCam.transform.position -= currentCam.transform.forward * currDistance;
            currentCam.transform.LookAt(target.transform.position);
        }
    }

    public static void SetTarget(string t, float frt)
    {
        GameObject newTarget = GameObject.Find(t);
        if (newTarget)
        {
            Debug.Log("Camera target set to " + t);
            target = newTarget;
            front = frt / 180;
            currentXRotation = 0;
            currentYRotation = 0;
        }
    }

    public static void SetDistance(float min, float max)
    {
        minDist = min;
        maxDist = max;
        currDistance = (min + max) / 2;
    }

    public static void SetRange(float r, float l, float u, float d)
    {
        rightRange = front - (r / 180);
        if (rightRange > 1)
        {
            rightRange = 1 - (rightRange - 1);
        }
        if (rightRange < -1)
        {
            rightRange = -1 - (rightRange + 1);
        }
        leftRange = front + (l / 180);
        if (leftRange > 1)
        {
            leftRange -= 1 - (leftRange - 1);
        }
        if (leftRange < -1)
        {
            leftRange = -1 - (leftRange + 1);
        }
        upRange = u / 180;
        downRange = d / 180;
    }

    public static Vector2 GetCurrentRotation()
    {
        return new Vector2(currentXRotation,currentYRotation);
    }
}
