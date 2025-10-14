/*
Author: M.R. Martin
Created: 10/3/2025
Description: Algorithmic AI for fish avoidance behavior 
Makes a fish dodge away from the camera when it gets too close, while still following its spline path.
 */
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineAnimate))]
public class FishAvoidCamera : MonoBehaviour
{
    //[Header("Camera to avoid")]
    private Transform targetCamera;
    private GameObject proxy;

    [Header("Dodge settings")]
    public float distanceFromCamera = 6f;   // start moving away when camera is this close
    public float maxAvoidDistance = 1f;     // how far fish offset away from path

    private SplineAnimate ani;
    private Vector3 offset_T;
    private Vector3 velocity;
    private int dodgeSide = 0;               // -1 = left, +1 = right

    private Vector3 prevPos;
    private Quaternion offset_R;

    void Awake()
    {
        // SplineAnimate check (should always be present) - Needed to get base position and forward
        ani = GetComponent<SplineAnimate>();
        if (ani == null)
        {
            Debug.LogError("FishAvoidCamera requires a SplineAnimate component on the same GameObject.");
            enabled = false;
            return;
        }

    }

    void Start() // run after all Awake() calls have finished.
    {
        targetCamera = CenterCameraManager.CenterCam;
        if (transform.childCount != 1)
        {
            Debug.Log(transform.name + " has a FishAvoidCamera component but does not have exactly 1 child.");
            return;
        }
        proxy = transform.GetChild(0).gameObject;
    }

    void LateUpdate()
    {
        if (targetCamera == null)
            return;

        // Position and forward from spline
        Vector3 basePos = transform.position;
        Vector3 forward = transform.forward;

        // Distance to camera
        float dist = Vector3.Distance(basePos, targetCamera.position);

        // Check if within avoidance distance, then apply dodging
        if (dist < distanceFromCamera)
        {
            if (dodgeSide == 0)
            {
                // Cross product to see if cam is to left or right of fish
                Vector3 toCam = (targetCamera.position - basePos).normalized;
                float side = Vector3.Dot(Vector3.Cross(forward, Vector3.up), toCam);
                dodgeSide = side > 0 ? 1 : -1; // pick left or right consistently
            }

            // Sideways direction (right or left relative to forward)
            Vector3 sideways = Vector3.Cross(Vector3.up, forward).normalized * dodgeSide;

            // Scale offset based on how close the camera is
            float strength = ease((distanceFromCamera - dist) / distanceFromCamera);
            offset_T = sideways * (maxAvoidDistance * strength);
        }
        else
        {
            // Reset dodge side when safe again
            dodgeSide = 0;
            // Smoothly return to no offset
            offset_T = Vector3.zero;
        }

        // final position
        proxy.transform.position = transform.position + offset_T;

        proxy.transform.rotation = transform.rotation;
        offset_R = Quaternion.identity;
        if (dist < distanceFromCamera)
        {
            // Rotate into combined forward + avoidance direction
            Vector3 direction = (proxy.transform.position - prevPos).normalized;
            offset_R = Quaternion.FromToRotation(forward, direction);
            proxy.transform.rotation = transform.rotation * offset_R;
            prevPos = proxy.transform.position;
        }
}

    // Cubic ease in / ease out
    private float ease(float x)
    {
        if(x < 0) return 0;
        else if(x < 0.5) return 4 * Mathf.Pow(x, 3);
        else return Mathf.Min(1 - Mathf.Pow(-2 * x + 2, 3) / 2, 1);
    }

    // Returns the difference in rotation between frames
    public Quaternion getOffset_R()
    {
        return offset_R;
    }
}