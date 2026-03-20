using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanDoubleJump : MonoBehaviour
{
    GameManager gameManager;
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.totalJumps = 2; //Desbloquea la habilidad
                pm.availableJumps = 1;
                gameManager.skillUnlockers.SetValue(false, gameManager.skillsCount);
                gameManager.skillsCount++;
                gameManager.skillUnlockers.SetValue(true, gameManager.skillsCount);
                Debug.Log("Double Jump unlocked!");
            }
            //Destroy(gameObject); // Elimina el pickup tras recogerlo
        }
    }
}

