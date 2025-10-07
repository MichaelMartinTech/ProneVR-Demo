using UnityEngine;

public class Canvas : MonoBehaviour
{
    private enum Mode
    {
        Idle, FadeIn, FadeOut
    }
    [SerializeField] private Mode mode = Mode.Idle;
    [SerializeField] private float fadeSpeed = 1f;
    private CanvasGroup cg;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cg = GetComponent<CanvasGroup>();

        // Initialize these values for testing
        cg.alpha = 0;
        mode = Mode.FadeIn;
    }

    // Update is called once per frame
    void Update()
    {
        if(mode == Mode.FadeIn && cg.alpha < 1f)
        {
            cg.alpha = Mathf.Min(cg.alpha + fadeSpeed * Time.deltaTime, 1f);
            if(cg.alpha >= 1f) { mode = Mode.FadeOut; }     // delete this later
        } 
        else if(mode == Mode.FadeOut && cg.alpha > 0)
        {
            cg.alpha = Mathf.Max(cg.alpha - fadeSpeed * Time.deltaTime, 0f);
            if(cg.alpha <= 0) { mode = Mode.FadeIn; }       // delete this later
        }
    }
}
