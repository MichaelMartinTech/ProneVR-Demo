using UnityEngine;
using UnityEngine.Splines;

public class Spawner : MonoBehaviour
{
    public GameObject[] creatures;
    public GameObject[] splines;
    int creatureCount;
    GameObject currentCreature;
    GameObject currentSpline;
    SplineAnimate ani;

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
        ani.MaxSpeed = calcSpeed(ani.NormalizedTime);

        if(ani.NormalizedTime >= 1.0f)
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
    }

    float calcSpeed(float t)
    {
        float speed = 213.333f * Mathf.Pow(t, 4f) - 426.667f * Mathf.Pow(t, 3f) + 274.667f * Mathf.Pow(t, 2f) - 61.333f * t + 6f;
        // Debug.Log(speed);
        // float speed = 0;
        // if(ani.NormalizedTime < 0.3 || ani.NormalizedTime > 0.7)
        // {
        //     speed = 6;
        // }
        // else
        // {
        //     speed = 2;
        // }
        return speed;
    }
}
