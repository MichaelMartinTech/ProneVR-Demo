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

    [Header("Dodge settings")]
    public float distanceFromCamera = 4f;    // start moving away when camera is this close
    public float maxAvoidDistance = 5f;    // how far fish offset away from path
    public float smoothTime = 0.5f;          // smoothing for movement
    public float turnSpeed = 2f;             // how fast the fish turns when dodging

    private SplineAnimate ani;
    private Vector3 avoidanceOffset;
    private Vector3 velocity;
    private int dodgeSide = 0;               // -1 = left, +1 = right

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
                dodgeSide = side > 0 ? -1 : 1; // pick left or right consistently
            }

            // Sideways direction (right or left relative to forward)
            Vector3 sideways = Vector3.Cross(Vector3.up, forward).normalized * dodgeSide;

            // Scale offset based on how close the camera is
            float strength = Mathf.InverseLerp(distanceFromCamera, 0f, dist);
            Vector3 desiredOffset = sideways * (maxAvoidDistance * strength);

            // Smooth move toward desired offset
            avoidanceOffset = Vector3.SmoothDamp(avoidanceOffset, desiredOffset, ref velocity, smoothTime);
        }
        else
        {
            // Reset dodge side when safe again
            dodgeSide = 0;
            // Smoothly return to no offset
            avoidanceOffset = Vector3.SmoothDamp(avoidanceOffset, Vector3.zero, ref velocity, smoothTime);
        }

        // final position
        transform.position = basePos + avoidanceOffset;

        // Rotate into combined forward + avoidance direction
        Vector3 moveDir = (forward + avoidanceOffset.normalized * 0.5f).normalized;
        if (moveDir.sqrMagnitude > 0.001f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, Time.deltaTime * turnSpeed);
        }
    }
}