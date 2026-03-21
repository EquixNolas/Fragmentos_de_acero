using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeChanger : MonoBehaviour
{
    float slowdown = 0.05f;
    public float slowDownTimer = 0.5f;

    public bool activeSlowdown = false;
   // public bool onSlowMo;
    public bool normalTime;

    [SerializeField] GameObject[] objects; 
    [SerializeField] GameObject canvas;
    TimeChanger timeChanger;
    PlayerMovement  pm;
    GameManager gameManager;

    private void Awake()
    {
        activeSlowdown = false;
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
        timeChanger = gameObject.GetComponent<TimeChanger>();    
        //onSlowMo = false;
        normalTime = false;
        //canvas.SetActive(false);
    }
    private void Update()
    {
        normalTime = timeChanger.normalTime;
        SlowDownOn();
       // DestroyObjects();
        
    }

   
    void SlowDownOn()
    {
        if (activeSlowdown)
        {
            //canvas.SetActive (true);
            Time.timeScale = slowdown;
            Time.fixedDeltaTime = Time.timeScale * 0.02f;
        }
    }

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
    }
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
