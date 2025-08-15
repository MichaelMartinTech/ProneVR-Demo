using UnityEngine;
using Adobe.Substance.Runtime;

public class CausticController : MonoBehaviour
{
    public SubstanceRuntimeGraph graph;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // if (graph != null)
        // {
        //     float current = graph.GetInputFloat("phase");
        //     float newValue = current + Time.deltaTime;
        //     if (newValue > 1)
        //     {
        //         newValue -= 1;
        //     }
        //     graph.SetInputFloat("phase", newValue);
        //     graph.RenderAsync();
        // }
    }
}
