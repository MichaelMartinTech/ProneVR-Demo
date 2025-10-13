using UnityEngine;

public class FishStateController : MonoBehaviour
{
    private Animator anim;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        float rotY = transform.localEulerAngles.y;
        if (rotY > 180f)
        {
            // Force angle to be between -180 and 180 degrees
            rotY -= 360f;
        }
        if (rotY < -3f)
        {
            anim.SetInteger("Swim", -1);
        }
        else if (rotY > 3f)
        {
            anim.SetInteger("Swim", 1);
        }
        else
        {
            anim.SetInteger("Swim", 0);
        }
    }
}
