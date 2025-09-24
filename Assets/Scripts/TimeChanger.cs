using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TimeChanger : MonoBehaviour
{
    float slowdown = 0.05f;
    float slowdownTime = 1.5f;
    bool activeSlowdown;
    public bool destryActive;
    [SerializeField] GameObject[] destruir; 
    [SerializeField] GameObject canvas;
    private void Awake()
    {
        destryActive = false;
        canvas.SetActive(false);
    }
    private void Update()
    { 
        Time.timeScale += (1f / slowdownTime) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f , 1f);
        if (destryActive)
        {
            foreach(GameObject destroy in destruir)
            {
                Destroy(gameObject);
            }
        }    
        SlowDownOn();
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

    private void OnTriggerExit2D(Collider2D collision)
    {
        activeSlowdown = false;
        Debug.Log("DesactivaSlowMo");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        activeSlowdown = true;
        Debug.Log("Activa SlowMo");
    }

}
