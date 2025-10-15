/*
Name: M.R. Martin
Sep 15, 2025
Description: This code imulates realistic 3D audio in Unity by casting sound rays 
from the audio source to the listener, taking in environmental geometry to determine 
how sound should be filtered. 
Based on these raycast results, it dynamically adjusts audio filters such as low-pass, 
reverb, and echo to reflect the acoustic properties of the environment.
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AudioLowPassFilter))]
[RequireComponent(typeof(AudioReverbFilter))]
[RequireComponent(typeof(AudioEchoFilter))]
public class AudioSource3D : MonoBehaviour
{
	private AudioLowPassFilter lp;
	private AudioReverbFilter reverb;
	private AudioEchoFilter echo;

	[Header("Source Ray Settings")] // raycast parameters
	public int rayCount = 32;
	public int maxBounces = 3;
	public float rayDistance = 30f;
	public float checkInterval = 0.25f;
	public float catchRadius = 0.5f;
	public LayerMask geometryMask;
	public bool drawDebug = true;

	[Header("Audio Settings")]
	public float minCutoff = 800f;
	public float maxCutoff = 22000f;
	public float smoothSpeed = 5f;

	private float smoothedCutoff;
	private float timer;
	private float lastRatioHeard;
	private float avgPathLength;   // store bounce distance for echo
	private float avgBounceCount;  // average bounces per ray that reached listener
	private float objectHitFactor; // more objects in room = more absorption

	void Start()
	{
		lp = GetComponent<AudioLowPassFilter>();
		reverb = GetComponent<AudioReverbFilter>();
		echo = GetComponent<AudioEchoFilter>();
		smoothedCutoff = maxCutoff;
	}

	void Update()
	{
		timer += Time.deltaTime;
		if (timer >= checkInterval)
		{
			timer = 0f;
			lastRatioHeard = ShootSoundRays();
		}
		// apply filters based on last raycast results
		ApplyFilters(lastRatioHeard);
	}

	/*
	Function: ShootSoundRays
	Parameters: None
	Return Type: float
	Description: This function simulates the emission of sound rays from the audio source. 
		It shoots a specified number of rays in random directions, checks for intersections 
		with geometry, and calculates how many rays reach the listener. It also gathers 
		statistics about the paths taken by the rays, such as average path length and 
		bounce count, which are used for audio filtering.
	*/
	float ShootSoundRays()
	{
		// get listener position (main camera)
		Transform listener = Camera.main.transform;
		int raysHitListener = 0;
		float totalPath = 0f;
		float totalBounces = 0f;

		// track unique objects hit for absorption calculation
		HashSet<Collider> uniqueObjects = new HashSet<Collider>();
		float totalSegmentLength = 0f;
		int segmentCount = 0;

		// shoot rays for spatial audio simulation and environmental context
		for (int i = 0; i < rayCount; i++)
		{
			// random direction
			Vector3 dir = Random.onUnitSphere;
			Ray ray = new Ray(transform.position, dir);
			RaycastHit hit;
			// reset per-ray data
			Vector3 origin = transform.position;
			float traveled = 0f;
			int bounces = 0;
			bool hitListener = false;

			for (int b = 0; b < maxBounces; b++)
			{
				// --- Check if ray directly reaches listener mid-air ---
				Vector3 toListener = listener.position - origin;
				float distToListener = toListener.magnitude;

				if (!Physics.Raycast(origin, toListener.normalized, distToListener, geometryMask))
				{
					raysHitListener++;
					totalPath += traveled + distToListener;
					totalBounces += bounces;

					if (drawDebug)
					{
						Debug.DrawRay(origin, toListener, b == 0 ? Color.green : Color.blue, checkInterval);
						DrawDebugCircle(listener.position, catchRadius, Color.cyan, checkInterval);
					}

					hitListener = true;
					// break;
				}


				// --- Otherwise continue with geometry checks ---
				if (Physics.Raycast(ray, out hit, rayDistance, geometryMask))
				{
					traveled += hit.distance;
					bounces++;

					// track object hit
					uniqueObjects.Add(hit.collider);

					// track path segment length
					totalSegmentLength += hit.distance;
					segmentCount++;

					if (drawDebug) Debug.DrawRay(origin, ray.direction * hit.distance, Color.red, checkInterval);

					// reflect
					Vector3 reflectDir = Vector3.Reflect(ray.direction, hit.normal);
					origin = hit.point + reflectDir * 0.01f;
					ray = new Ray(origin, reflectDir);
				}
				else
				{
					// escape outdoors
					if (drawDebug) Debug.DrawRay(origin, ray.direction * rayDistance, Color.yellow, checkInterval);
					break;
				}
			}

			if (!hitListener) continue;
		}
		// calculate averages
		avgPathLength = raysHitListener > 0 ? totalPath / raysHitListener : 0f;
		avgBounceCount = raysHitListener > 0 ? totalBounces / raysHitListener : 0f;

		// absorption factors
		int objectCount = uniqueObjects.Count;
		float avgSegmentLength = (segmentCount > 0) ? totalSegmentLength / segmentCount : rayDistance;

		// normalize
		float clutterFactor = Mathf.Clamp01(objectCount / 20f); // 20+ objects = max clutter
		float densityFactor = Mathf.Clamp01(1f - (avgSegmentLength / rayDistance));

		// combine (more clutter + shorter free paths → stronger absorption)
		objectHitFactor = Mathf.Clamp01((clutterFactor + densityFactor) * 0.5f);

		return (float)raysHitListener / rayCount;
	}


	/*
	Function: ApplyFilters
	Parameters: float ratioHeard - The ratio of rays that reached the listener.
	Return Type: void
	Description: This function applies audio filters based on the ratio of rays that 
		reached the listener. It adjusts the low-pass filter cutoff frequency, reverb 
		settings, and echo settings based on the environmental context and the 
		calculated absorption factor.
	*/
	void ApplyFilters(float ratioHeard)
	{
		AudioContext ctx = AudioProbe.CurrentContext;

		// --- Low-pass muffling ---
		float targetCutoff = Mathf.Lerp(maxCutoff, minCutoff, 1f - ratioHeard);
		smoothedCutoff = Mathf.Lerp(smoothedCutoff, targetCutoff, Time.deltaTime * smoothSpeed);
		lp.cutoffFrequency = smoothedCutoff;

		// Calculate absorption (fewer objects = stronger reverb, more objects = weaker)
		float absorption = 1f - objectHitFactor;
		float indoorFactor = 1f - ctx.outdoorFactor;

		// --- Reverb ambience ---
		reverb.decayTime = Mathf.Lerp(0.5f, 2.5f, ctx.reverbLevel * indoorFactor * absorption);
		reverb.reverbLevel = Mathf.Lerp(-10000f, -2000f, ctx.reverbLevel * indoorFactor * absorption);
		reverb.reflectionsLevel = Mathf.Lerp(-10000f, -5000f, ctx.reverbLevel * indoorFactor * absorption);

		// --- Echo (early reflections) ---
		// only apply if we have valid path data
		if (avgPathLength > 0f)
		{
			// delay based on average path length (speed of sound = 343 m/s)
			float delayMs = (avgPathLength / 343f) * 1000f;
			float safeDelay = Mathf.Clamp(delayMs, 20f, 250f);

			float decay = Mathf.Clamp01(avgBounceCount / maxBounces);
			float safeDecay = Mathf.Clamp(decay * absorption, 0.05f, 0.5f);
			// wet mix based on indoor factor and absorption
			float wetMix = Mathf.Lerp(0.2f, 0.7f, indoorFactor * absorption);

			// apply
			echo.delay = safeDelay;
			echo.decayRatio = safeDecay;
			echo.wetMix = wetMix;
		}
		else
		{
			// reset
			echo.delay = 50f;
			echo.decayRatio = 0.05f;
			echo.wetMix = 0f;
		}
	}

	/*
	Function: DrawDebugCircle
	Parameters: Vector3 center - The center position of the circle.
	            float radius - The radius of the circle.
	            Color color - The color of the circle lines.
	            float duration - How long the debug lines should last.
	            int segments - Number of line segments to use for the circle.
	Return Type: void
	Description: This function draws a debug circle in the scene view using Debug.DrawLine.
		It approximates the circle by drawing multiple line segments around the specified 
		center.
	*/
	void DrawDebugCircle(Vector3 center, float radius, Color color, float duration, int segments = 20)
	{
		// Calculate points around the circle
		Vector3 up = Vector3.up;
		float angleStep = 360f / segments;
		Vector3 prevPoint = center + Quaternion.AngleAxis(0, up) * Vector3.forward * radius;

		// Draw the circle
		for (int i = 1; i <= segments; i++)
		{
			Vector3 nextPoint = center + Quaternion.AngleAxis(angleStep * i, up) * Vector3.forward * radius;
			Debug.DrawLine(prevPoint, nextPoint, color, duration);
			prevPoint = nextPoint;
		}
	}
}