using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SkillControl : MonoBehaviour 
{
    [Header ("Script Declaration")]
    [SerializeField] GameManager gameManager;
    [SerializeField] PlayerMovement pm;
    [SerializeField] TimeChanger time;

    [Header ("GameObjects")]
    [SerializeField] GameObject[] skillUnlocker; 
    [SerializeField] GameObject[] timeSlower;
    private void Awake() 
    {
        gameManager = GameObject.Find("10_GAMEMANAGER").GetComponent<GameManager>();
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        time = GameObject.FindGameObjectWithTag("TimeSlowMo").GetComponent<TimeChanger>();
        Debug.Log(gameManager.skillsCount);

    }
    private void Update()
    {
        if (!time.activeSlowdown || pm.isDoubleJumping)
        {
           
            time.normalTime = true;
            time.activeSlowdown = false;
            Debug.Log("Doble Salto: " + pm.isDoubleJumping);
            Debug.Log("El SlowDown Está: " + time.activeSlowdown);
            if(gameManager.skillsCount > 0)
            {
               // skillUnlocker[0].SetActive(false);
                //timeSlower[0].SetActive(false);
                Debug.Log(gameManager.skillsCount);
            }
        }
    }
    private void OnTriggerEnter2D(Collider2D other) 
    { 
        if (other.CompareTag("Player")) 
        { 
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null && gameManager.skillsCount == 0) {
                
                skillUnlocker[gameManager.skillsCount].SetActive(true);

                StartCoroutine(WaitTime(0.05f));

                gameManager.skillsCount++;
                Debug.Log("Double Jump unlocked!"); 
            }
        } 
    }

    IEnumerator WaitTime(float waitTime)
    {
        //Debug.Log("Dentro del WaitTime");
        yield return new WaitForSeconds(waitTime);
        //Debug.Log("Despues del WaitTime");
        
            pm.totalJumps = 2; //Desbloquea la habilidad
            pm.availableJumps = 1;
        
    }
    
}