using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    TimeChanger TimeChanger;
    bool normalTime = false;
    private void Awake()
    {
        TimeChanger = GameObject.Find("TimeSlow").GetComponent<TimeChanger>();
        Debug.Log(TimeChanger);
    }
    // Update is called once per frame
    void Update()
    {
        normalTime = TimeChanger.normalTime;
     
        TimeRecover();
    }

    void TimeRecover()
    {
        Time.timeScale += (1f /TimeChanger.slowdownTime) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
    }
}
