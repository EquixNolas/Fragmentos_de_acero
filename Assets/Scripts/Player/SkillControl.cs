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

    }
    private void Update()
    {
        if (!time.activeSlowdown || pm.isDoubleJumping)
        {
            if (gameManager.skillsCount != 0)
            {
                skillUnlocker[gameManager.skillsCount - 1].SetActive(false);
                timeSlower[gameManager.skillsCount - 1].SetActive(false);

            }
            //activeSlowMo.SetActive(false);
        }
    }
    private void OnTriggerEnter2D(Collider2D other) 
    { 
        if (other.CompareTag("Player")) 
        { 
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null && gameManager.skillsCount == 0) {
                
                skillUnlocker[gameManager.skillsCount].SetActive(true);
                Debug.Log(skillUnlocker[gameManager.skillsCount]);

                pm.totalJumps = 2; //Desbloquea la habilidad
                pm.availableJumps = 1;

                gameManager.skillsCount++;
                //Debug.Log("Double Jump unlocked!"); 
            }
        } 
    }
    
}