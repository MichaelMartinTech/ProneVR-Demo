using UnityEngine;
using UnityEngine.Splines;

public class Spawner : MonoBehaviour
{
    // Prefabs
    public GameObject[] creatures;
    public GameObject[] splines;
    public float speedMultiplier = 1f;
    
    // Smoothening
    [Range(0f, 1f)] public float smoothenVal = 0.5f;
    private float smoothVel;

    int creatureCount;
    GameObject currentCreature;
    GameObject currentSpline;
    SplineAnimate ani;
    float progress; // manual 0–1 spline progress - Fix to 'jitter'

    // Debug (Read only)
    [SerializeField] private float currentSpeed;   // visible in Inspector
    [SerializeField] private float splineLength;   // current spline length

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        creatureCount = creatures.Length;
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        // Parametric speed
        //ani.MaxSpeed = calcSpeed(ani.NormalizedTime);

        // spline length (so world speed can be normalized to 0–1 progress)
        splineLength = ani.Container.CalculateLength();
        // calculate speed from progress
        currentSpeed = calcSpeed(progress);

        // convert world speed to normalized progress increments
        float deltaProgress = (currentSpeed * Time.deltaTime) / splineLength;
        progress += deltaProgress;
        progress = Mathf.Clamp01(progress); // Added clamping
        // move along spline
        //ani.NormalizedTime = progress;
        ani.NormalizedTime = Mathf.SmoothDamp(
            ani.NormalizedTime,
            progress,
            ref smoothVel,
            1f - smoothenVal
        );

        //if(ani.NormalizedTime >= 10.0f)
        // End of progress --> spawn new one
        if (progress >= 1.0f)
        {
            Destroy(currentCreature);
            Destroy(currentSpline);
            Spawn();
        }
    }

    public void Spawn()
    {
        int index1 = Random.Range(0, creatureCount);
        currentCreature = Instantiate(creatures[index1]);
        int index2 = Random.Range(0, creatureCount);
        currentSpline = Instantiate(splines[index2]);
        ani = currentCreature.GetComponent<SplineAnimate>();
        // ani.AnimationMethod = SplineAnimate.Method.Speed;
        ani.Container = currentSpline.GetComponent<SplineContainer>();
        ani.Restart(true);
        progress = 0f;     // reset manual progress
        currentSpeed = 0f; // reset debug
    }

    float calcSpeed(float t)
    {
        //float speed = 213.333f * Mathf.Pow(t, 4f) - 426.667f * Mathf.Pow(t, 3f) + 274.667f * Mathf.Pow(t, 2f) - 61.333f * t + 6f;
        //Debug.Log(speed);
        //float speed = 0;
        /*
        if(ani.NormalizedTime < 0.3 || ani.NormalizedTime > 0.7)
        {
            speed = 6;
        }
        else
        {
            speed = 2;
        }
        */
        // poly profile (accelerate, cruise, decelerate)
        float baseCurve = 213.333f * Mathf.Pow(t, 4f)
                    - 426.667f * Mathf.Pow(t, 3f)
                    + 274.667f * Mathf.Pow(t, 2f)
                    - 61.333f * t
                    + 6f;
        float speed = baseCurve * speedMultiplier; // Pass curve with multiplier to speed
        //return speed;
        return Mathf.Max(0.5f, speed); // never stall/reverse
    }
}