// CenterCameraManager.cs
using UnityEngine;

public class CenterCameraManager : MonoBehaviour
{
    public static Transform CenterCam;

    void Awake()
    {
        if (CenterCam == null)
        {
            CenterCam = transform;        // store reference
            DontDestroyOnLoad(gameObject); // keep across scenes
        }
        else
        {
            Destroy(gameObject); // prevent duplicates
        }
    }
}