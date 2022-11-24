using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowMotion : MonoBehaviour
{
    [SerializeField]
    float slowMotionTimescale;
    float startTimescale;
    float startFixedDeltaTime;


    // Start is called before the first frame update
    void Start()
    {
        startTimescale = Time.timeScale;
        startFixedDeltaTime = Time.fixedDeltaTime;
    }
    

    public void StartSlowMotion()
    {
        Time.timeScale = slowMotionTimescale;
        Time.fixedDeltaTime = startFixedDeltaTime * slowMotionTimescale;
    }

    public void StopSlowMotion()
    {
        Time.timeScale = startTimescale;
        Time.fixedDeltaTime = startFixedDeltaTime;
    }
}
