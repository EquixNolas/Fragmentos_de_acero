using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrampLaunch : MonoBehaviour
{
    [SerializeField] GameObject trampDetection;
    [SerializeField] GameObject tramp;
    [SerializeField] Rigidbody2D rbTramp;
    [SerializeField] Vector2 trampPosition;

    PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Awake()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        rbTramp.simulated = false; // Desactiva la simulación del Rigidbody2D al inicio
        trampPosition = tramp.transform.position; // Guarda la posición inicial de la trampa
    }
    private void Update()
    {
        if(playerMovement.alive == false)
        {
           StartCoroutine(ResetTramp());
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.CompareTag("Player") && collision.gameObject.layer != LayerMask.NameToLayer("Ground"))
            return;

        if (collision.CompareTag("Player") && playerMovement.alive) // Asegúrate de que solo reaccione al jugador
        {
            rbTramp.simulated = true; // Activa la simulación del Rigidbody2D
            trampDetection.SetActive(false); // Desactiva el GameObject de detección de trampas
            Debug.Log("Detectado");
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground")) // Verifica si colisiona con el suelo
        {
            rbTramp.simulated = false; // Desactiva la simulación del Rigidbody2D al colisionar con el suelo
            Debug.Log("Colision con el suelo");
        }
    }

    IEnumerator ResetTramp()
    {
        yield return new WaitForSeconds(.2f); // Espera 1 segundo antes de resetear la trampa
        rbTramp.simulated = false; // Desactiva la simulación del Rigidbody2D
        tramp.transform.position = trampPosition; // Resetea la posición de la trampa al inicio
        rbTramp.velocity = Vector2.zero; // Resetea la velocidad del Rigidbody2D
        trampDetection.SetActive(true); // Reactiva el GameObject de detección de trampas
    }
}
