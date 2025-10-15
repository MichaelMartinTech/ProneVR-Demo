/*
Name: M.R. Martin
Sep 15, 2025
Description: This code samples the environment around its position by casting rays to 
detect how much of the space is blocked or open, estimating acoustic properties like muffling, 
reverb, and outdoor exposure. It updates a global **AudioContext** with these smoothed values
and allows other audio components to adapt their effects based on the current environment.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct AudioContext
{
	public float muffling;     // 0 clear, 1 blocked
	public float reverbLevel;  // baseline reflections
	public float outdoorFactor;
}

public class AudioProbe : MonoBehaviour
{
	public static AudioProbe Instance;
	public static AudioContext CurrentContext;

	[Header("Probe Settings")]
	public int rayCount = 64;
	public float probeDistance = 30f;
	public LayerMask geometryMask;
	public float checkInterval = 0.25f;
	public bool drawDebug = true;
	public float smoothing = 0.2f;

	private float timer;
	private float smoothedMuffling = 0f;
	private float smoothedReverb = 0f;
	private float smoothedOutdoor = 0f;

	void Awake()
	{
		Instance = this;
	}

	void Update()
	{
		timer += Time.deltaTime;
		if (timer >= checkInterval)
		{
			timer = 0f;
			SampleEnvironment();
		}
	}

	void SampleEnvironment()
	{
		int blocked = 0;
		int escaped = 0;

		for (int i = 0; i < rayCount; i++)
		{
			Vector3 dir = Random.onUnitSphere;
			Vector3 rayOrigin = transform.position + dir * 0.05f;
			Ray ray = new Ray(rayOrigin, dir);
			RaycastHit hit;

			if (Physics.Raycast(ray, out hit, probeDistance, geometryMask))
			{
				blocked++;
				if (drawDebug)
					Debug.DrawRay(ray.origin, ray.direction * hit.distance, Color.red, checkInterval);
			}
			else
			{
				escaped++;
				if (drawDebug)
					Debug.DrawRay(ray.origin, ray.direction * probeDistance, Color.green, checkInterval);
			}
		}

		float rawMuffling = (float)blocked / rayCount;
		float outdoorFactor = (float)escaped / rayCount;

		smoothedMuffling = Mathf.Lerp(smoothedMuffling, rawMuffling, smoothing);
		smoothedReverb = Mathf.Lerp(smoothedReverb, 1f - rawMuffling, smoothing);
		smoothedOutdoor = Mathf.Lerp(smoothedOutdoor, outdoorFactor, smoothing);

		CurrentContext = new AudioContext
		{
			muffling = smoothedMuffling,
			reverbLevel = smoothedReverb,
			outdoorFactor = smoothedOutdoor
		};
	}
}