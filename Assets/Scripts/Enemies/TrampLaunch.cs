using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TrampLaunch : MonoBehaviour
{
    [SerializeField] GameObject tramp;
    [SerializeField] GameObject trampDetection;
 
    [SerializeField] Rigidbody2D rbTramp;
    [SerializeField] Animator trampAnimator;

    [SerializeField] Vector2 trampPosition;
    //[SerializeField] bool activatedTramp = false; 

    PlayerMovement playerMovement;
    // Start is called before the first frame update
    void Awake()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        rbTramp.simulated = false; // Desactiva la simulaci�n del Rigidbody2D al inicio
        trampPosition = tramp.transform.position; // Guarda la posici�n inicial de la trampa
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

        if (collision.CompareTag("Player") && playerMovement.alive) // Aseg�rate de que solo reaccione al jugador
        {
           
            StartCoroutine(SoltarTramp()); // Llama a la corrutina para soltar la trampa
            Debug.Log("Detectado");
        }
        
        if(collision.gameObject.layer == LayerMask.NameToLayer("Ground")) // Verifica si colisiona con el suelo
        {
            rbTramp.simulated = false; // Desactiva la simulaci�n del Rigidbody2D al colisionar con el suelo
            Debug.Log("Colision con el suelo");
        }

        
    }
    IEnumerator SoltarTramp()
    {
        trampAnimator.SetTrigger("Launch"); // Activa la animaci�n de lanzamiento de la trampa
        yield return new WaitForSeconds(.1f); // Espera 0.2 segundos antes de lanzar la trampa
        rbTramp.simulated = true; // Activa la simulaci�n del Rigidbody2D
        trampDetection.SetActive(false); // Desactiva el GameObject de detecci�n de trampas
    }
    IEnumerator ResetTramp()
    {
        yield return new WaitForSeconds(.2f); // Espera 1 segundo antes de resetear la trampa
        rbTramp.simulated = false; // Desactiva la simulaci�n del Rigidbody2D
        tramp.transform.position = trampPosition; // Resetea la posici�n de la trampa al inicio
        rbTramp.linearVelocity = Vector2.zero; // Resetea la velocidad del Rigidbody2D
        trampDetection.SetActive(true); // Reactiva el GameObject de detecci�n de trampas
    }
}
