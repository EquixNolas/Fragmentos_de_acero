using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampLaunch : MonoBehaviour
{
    [SerializeField] GameObject trampDetection;
    [SerializeField] GameObject tramp;
    [SerializeField] Rigidbody2D rbTramp;
    PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Awake()
    {
        rbTramp.gravityScale = 0; // Desactiva la gravedad al inicio
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Player")) // Asegúrate de que solo reaccione al jugador
        {
            rbTramp.gravityScale = 4; // Reactiva la gravedad al entrar en el trigger
            Debug.Log("Detectado");
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player")) // Asegúrate de que solo reaccione al jugador
        {
          //playerMovement.alive = false; // Desactiva el movimiento del jugador
        }
    }
}
