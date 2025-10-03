using UnityEngine;

public class SharkPrefab : MonoBehaviour
{
    Vector3 start_pos = new Vector3(-20f, 0f, 0f);

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // transform.Rotate(0f, 90f, 0f);
        // transform.position = start_pos;
    }

    // Update is called once per frame
    void Update()
    {
        // if(transform.position.x > 20f)
        // {
        //     transform.position = start_pos;
        // }
        // transform.Translate(Vector3.forward * 1f * Time.deltaTime);
    }
}
