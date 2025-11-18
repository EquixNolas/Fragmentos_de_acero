using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeChanger : MonoBehaviour
{
    float slowdown = 0.05f;
    public float slowdownTime = 1.5f;

    bool activeSlowdown = false;
    public bool onSlowMo;
    public bool normalTime;

    [SerializeField] GameObject[] objects; 
    [SerializeField] GameObject canvas;
    TimeChanger timeChanger;
    PlayerMovement  PlayerMovement;
    private void Awake()
    {
        activeSlowdown = false;
        PlayerMovement = GameObject.Find("Player").GetComponent<PlayerMovement>();
        timeChanger = GameObject.Find("TimeSlow").GetComponent<TimeChanger>();
        onSlowMo = false;
        normalTime = false;
        canvas.SetActive(false);
    }
    private void Update()
    {
        normalTime = timeChanger.normalTime;
        SlowDownOn();
        DestroyObjects();
        if (activeSlowdown)
        {
            TimeRecover();
        }

        //TimeRecover();
        //Debug.Log(PlayerMovement.pulsarBoton);
    }

    void TimeRecover()
    {
        Time.timeScale += (1f / timeChanger.slowdownTime) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
    }
    void SlowDownOn()
    {
        if (activeSlowdown)
        {
            canvas.SetActive (true);
            Time.timeScale = slowdown;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

    /*
    void TimeRecover()
    {
        Time.timeScale += (1f / slowdownTime) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
    }*/

    void DestroyObjects()
    {
        if (normalTime && !onSlowMo || PlayerMovement.pulsarBoton)
        {
            Destroy(objects[0]);
            Destroy(objects[1]);
            Time.timeScale = 1f;
            activeSlowdown = false;
            PlayerMovement.ConsumeJumpOrButton();
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        onSlowMo=true;
        normalTime = false;
        activeSlowdown = true;
        //Debug.Log("Activa SlowMo");
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        onSlowMo=false;
        normalTime = true;
        activeSlowdown = false;
        //Debug.Log("DesactivaSlowMo");
    }


}
