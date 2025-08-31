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

    Vector3 start_pos = new Vector3(-20f, 0f, 0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        creatureCount = creatures.Length;
        Spawn();
    }

    // Update is called once per frame
    void Update()
    {
        // if(current.transform.position.x > 20f)
        // {
        //     Destroy(currentCreature);
        //     Spawn();
        // }
        // current.transform.Translate(Vector3.forward * 1f * Time.deltaTime);
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
        Debug.Log(index1);
        currentCreature = Instantiate(creatures[index1]);
        int index2 = Random.Range(0, creatureCount);
        Debug.Log(index2);
        currentSpline = Instantiate(splines[index2]);
        ani = currentCreature.GetComponent<SplineAnimate>();
        ani.Container = currentSpline.GetComponent<SplineContainer>();
        ani.Restart(true);
        // current.transform.Rotate(0f, 90f, 0f);
        // current.transform.position = start_pos;
    }
}
