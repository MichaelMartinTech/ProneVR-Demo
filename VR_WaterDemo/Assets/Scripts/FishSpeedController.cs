/*
Author: M.R. Martin
Revised from: 'spawner.cs' - Authors M.R. Martin and Garrick Chan
Created: 10/3/2025
Description: Controls fish speed along a spline with acceleration and deceleration profile.
This script was derived and adapted from 'spawner.cs' authored by M.R. Martin and Garrick Chan.
 */

// Note - Requires easing set to 'none' in SplineAnimate for full effect
using UnityEngine;
using UnityEngine.Splines;

[RequireComponent(typeof(SplineAnimate))]
public class FishSpeedController : MonoBehaviour
{
    // Speed control
    public float speedMultiplier = 1f;
    public bool isLooping = true;
    // Smoothening
    [Range(0f, 1f)] public float smoothenVal = 0.5f;
    private float smoothVel;
    private SplineAnimate ani;

    // Manual spline progress
    private float progress; // 0–1 spline progress

    // Debug (Read only)
    [SerializeField] private float currentSpeed;   // visible in Inspector
    [SerializeField] private float splineLength;   // current spline length
    [SerializeField] private float normalizedTime; // Ani.NormalizedTime for debug

    void Awake()
    {
        ani = GetComponent<SplineAnimate>();
        if (ani == null)
        {
            Debug.LogError("FishAvoidCamera requires a SplineAnimate component on the same GameObject.");
            enabled = false;
            return;
        }
        ani.PlayOnAwake = false;
        ani.NormalizedTime = 0f;
        progress = 0f;
    }

    void Update()
    {
        if (ani.Container == null) return;

        // spline length (so world speed can be normalized to 0–1 progress)
        splineLength = ani.Container.CalculateLength();
        currentSpeed = calcSpeed(progress);

        // world speed --> normal progress increments
        float deltaProgress = (currentSpeed * Time.deltaTime) / splineLength;
        progress += deltaProgress;

        //progress = Mathf.Clamp01(progress);
        // wrap progress instead of clamping -- for looping
        //progress = Mathf.Repeat(progress, 1f);
        if (isLooping == true)
        {
            // wrap progress for looping
            progress = Mathf.Repeat(progress, 1f);
        }
        else
        {
            // clamp progress for non-looping
            progress = Mathf.Clamp01(progress);
        }

        // cyclic smoothing using angles (0–360 instead of 0–1)
        float currentAngle = ani.NormalizedTime * 360f;
        float targetAngle = progress * 360f;

        float smoothedAngle = Mathf.LerpAngle(
            currentAngle,
            targetAngle,
            Time.deltaTime / (1f - smoothenVal + 0.0001f) // avoid div/0
        );

        ani.NormalizedTime = smoothedAngle / 360f;

        normalizedTime = ani.NormalizedTime;
    }

    float calcSpeed(float t)
    {
        // polynomial profile (accelerate, cruise, decelerate)
        float baseCurve = 213.333f * Mathf.Pow(t, 4f)
                        - 426.667f * Mathf.Pow(t, 3f)
                        + 274.667f * Mathf.Pow(t, 2f)
                        - 61.333f * t
                        + 6f;

        float speed = baseCurve * speedMultiplier;
        return Mathf.Max(0.5f, speed); // Do n ot reverse/stall
    }
}