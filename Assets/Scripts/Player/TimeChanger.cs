using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeChanger : MonoBehaviour
{
    float slowdown = 0.05f;
    float targetScale;

    [Header("TIME VARIABLES")]//Titulo de la sección
    public bool normalTime;
    public float slowDownTimer = 0.5f;
    public bool activeSlowdown = false;

    [Header("CANVAS OBJECTS")]//Titulo de la sección
    [SerializeField] GameObject canva;
    TimeChanger timeChanger;
    PlayerMovement  pm;

    private void Awake()
    {
        activeSlowdown = false;
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
        timeChanger = gameObject.GetComponent<TimeChanger>();    
        //onSlowMo = false;
        normalTime = false;
    }

    private void Update()
    {
        normalTime = timeChanger.normalTime;
        SlowDownOn();
    }
   
    void SlowDownOn()
    {
        targetScale = activeSlowdown ? slowdown : 1f;

        // Solo actualizamos si el valor ha cambiado
        if (Time.timeScale != targetScale)
        {
            Time.timeScale = targetScale;
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
        }
    }
    /*
    void DestroyObjects()
    {
        if (normalTime || pm.pulsarBoton)
        {
            Destroy(objects[0]);
            Destroy(objects[1]);
            Time.timeScale = 1f;
            activeSlowdown = false;
            pm.ConsumeJumpOrButton();
        }
    }*/
    private void OnTriggerEnter2D(Collider2D collision)
    {
        normalTime = false;
        activeSlowdown = true;
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        normalTime = true;
        activeSlowdown = false;
    }


}
