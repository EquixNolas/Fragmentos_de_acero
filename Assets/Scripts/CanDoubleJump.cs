using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanDoubleJump : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement pm = other.GetComponent<PlayerMovement>();
            if (pm != null)
            {
                pm.canDoubleJump = true; //Desbloquea la habilidad
                Debug.Log("Double Jump unlocked!");
            }
            Destroy(gameObject); // Elimina el pickup tras recogerlo
        }
    }
}

