using UnityEngine;

public class LayerSpawner : MonoBehaviour
{
    public GameObject[] prefabs; // Layer prefabs
    [Range(3, 8)] public int numActiveLayers;
    [Range(0f, 200f)] public float speed; // speed in cm/sec
    public float maxStartHeight; // initial position of the highest layer
    public float spacing; // Adjust spacing between layers
    public GameObject camera; // camera object

    private int lastPrefab = -1; // prevent identical consecutive layers
    private GameObject[] activeLayers;
    private int highestIndex = 0; // The current element of activeLayers highest in world space

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        activeLayers = new GameObject[numActiveLayers];
        for(int i = 0; i < activeLayers.Length; i++)
        {         
            activeLayers[i] = SpawnLayer(i);
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < activeLayers.Length; i++)
        {
            activeLayers[i].transform.position += new Vector3(0f, speed * Time.deltaTime / 100, 0f);
        }
        // Once the top layer makes it to the camera, replace it with a new layer at the bottom
        // 2 * spacing is to avoid jarring disappearance of cast shadows
        if(activeLayers[highestIndex].transform.position.y > camera.transform.position.y + 2 * spacing)
        {
            Destroy(activeLayers[highestIndex]);
            activeLayers[highestIndex] = SpawnLayer(activeLayers.Length - 1);
            highestIndex = (highestIndex + 1) % activeLayers.Length;
        }
    }

    GameObject SpawnLayer(int i)
    {
        int layerIndex = -1;
        do
        {
            layerIndex = Random.Range(0, prefabs.Length);
        } while (layerIndex == lastPrefab);
        lastPrefab = layerIndex;
        return Instantiate(prefabs[layerIndex], new Vector3(0f, maxStartHeight + (2 - i) * spacing, 0f), Quaternion.identity);
    }
}
