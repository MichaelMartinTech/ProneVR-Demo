using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject[] creatures;
    int creatureCount;
    GameObject current;

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
        if(current.transform.position.x > 20f)
        {
            Destroy(current);
            Spawn();
        }
        current.transform.Translate(Vector3.forward * 1f * Time.deltaTime);
    }

    public void Spawn()
    {
        current = Instantiate(creatures[Random.Range(0, creatureCount)]);
        current.transform.Rotate(0f, 90f, 0f);
        current.transform.position = start_pos;
    }
}
