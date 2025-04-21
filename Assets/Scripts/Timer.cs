using System.Collections;
using UnityEngine;

public class Timer:MonoBehaviour
{
    void Start()
    {
        StartCoroutine(TimerCoroutine());
    }

    IEnumerator TimerCoroutine()
    {
        Debug.Log("Timer started");
        yield return new WaitForSeconds(5);
        Debug.Log("5 seconds have passed");
    }

}
