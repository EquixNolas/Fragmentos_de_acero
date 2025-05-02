using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;

    //CHECKING GROUND
    [Header("Ground Check")]//Titulo de la sección
    [SerializeField] TrailRenderer dashTrail; // Prefab del trail del dash
    [SerializeField] Transform groundCheck; //Empty object que verifica si estas en el suelo
    [SerializeField] LayerMask groundLayer; //Layer del Suelo
    //[SerializeField] GameObject dashGhostPrefab;

    SpriteRenderer spriteRenderer; //Renderizador del sprite

    //MOVIMIENTO
    float move; //Variable para el movimiento
    [Header("Movement")] //sección de movimiento
    [SerializeField]float speed = 5f; //variable para la velocidad
    bool lookDch = true; //Variable para mirar a la derecha

    //SALTO
    int availableJumps; //saltos disponibles
    [Header("Jump")] //sección de salto
    public bool canDoubleJump = false; //Variable para el doble salto
    [SerializeField]float jumpForce = 10f; //Fuerza del salto
    [SerializeField] int totalExtraJumps = 2; //Saltos extra 
    [SerializeField]float fallMultiplier = 1.2f; //Multiplicador de gravedad para caer más rápido
    [SerializeField] float wallSlideSpeed = 2f; //Velocidad de deslizamiento por la pared
    [SerializeField] Transform wallCheck; //Empty object que verifica si estas en la pared
    [SerializeField] LayerMask wallLayer; //Layer de la pared
    bool isWallSliding; //Variable para saber si se está deslizándose por la pared
    float normalGravityScale; //Gravedad normal del objeto

    //PROPULSION
    [Header("Propulsion")] //sección de propulsión
    [SerializeField]float planeoSpeed = 2f; //Velocidad de planeo
    bool isPropulsing = false; //Variable para saber si se está planeando
    bool canPropulse = false; //Variable para saber si se puede planear

    //DASHEO
    [Header("Dash")] //sección de dash
    [SerializeField] float dashForce = 15f;       // Fuerza del dash
    [SerializeField] float dashTime = 0.2f;   // Duración del dash
    [SerializeField] float dashCooldown = 0.5f;   // Tiempo entre dashes

    bool isDashing = false; //Variable para saber si se está haciendo dash
    bool canDash = true; //Variable para saber si se puede hacer dash

    Vector2 dashInput; // Variable para la dirección del dash
    Vector2 vecGravity; //Vector2 de gravedad

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>(); //Se obtiene el componente Animator
        rb = GetComponent<Rigidbody2D>(); //Se obtiene el componente Rigidbody2D
        spriteRenderer = GetComponent<SpriteRenderer>(); //Se obtiene el componente SpriteRenderer

        vecGravity = new Vector2(0, -Physics2D.gravity.y); //Se obtine la gravedad del objeto
        normalGravityScale = rb.gravityScale;   //Se guarda la gravedad normal del objeto
        availableJumps = totalExtraJumps; //Se inicializan los saltos disponibles

    }

    // Update is called once per frame
    void Update()
    {
        if (!isDashing)
        {
            Mover(); //Se llama la función de movimiento
            Propulsion(); //Se llama la función de propulsión
        }

        WallSlide(); //Se llama la función de deslizamiento por la pared
        fall(); //Se llama la función de caída

    }

    bool IsGrounded() //Verifica si el Player está en el suelo
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer); //Se lanza un rayo hacia abajo desde el objeto
        return hit.collider != null; //Si el rayo colisiona con algo, devuelve true
    }

    bool IsWalled() //Verifica si el Player está en la pared
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayer); //Se lanza un círculo desde el objeto
    }

    private void WallSlide() //Se verifica si el Player está deslizándose por la pared
    {
        //Si el Player está en la pared y no está en el suelo y se está moviendo
        if (IsWalled() && !IsGrounded() && move != 0f) 
        {
            isWallSliding = true;//Se activa la variable de deslizamiento por la pared
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Clamp(rb.velocity.y , -wallSlideSpeed, float.MaxValue)); //Se limita la velocidad vertical del objeto
        }

        else
        {
            isWallSliding = false; //Se desactiva la variable de deslizamiento por la pared
        }
    }

    //Dibuja el Gizmo de la verificación del suelo
    void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * 0.1f);
    }

    //INPUTS ACTIONS
    // INPUTS  ACTIONS DE MOVIMIENTO
    public void Move(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>().x; //Se obtiene el valor del movimiento
        animator.SetFloat("move", Mathf.Abs(move)); //Se asigna el valor del movimiento al animator

        dashInput = context.ReadValue<Vector2>(); //Se obtiene el valor del dashInput
    }


    //INPUT ACTION DE SALTO
    public void Jump(InputAction.CallbackContext context) 
    {
        //Se verifica si se presiona el botón de salto
        if (context.started)  
        {
            DoJump(); //Se llama a la función de salto
        }

        //Se verifica si se mantiene el botón de salto
        if (context.performed)
        {
            isPropulsing = true; //Se activa la variable de propulsión
        }

        //Se verifica si se suelta el botón de salto
        if (context.canceled)
        {
            isPropulsing = false; //Se desactiva la variable de propulsión
        }
    }

    //INPUT ACTION DE DASHEO
    public void Dash(InputAction.CallbackContext context)
    {
        //Se verifica si se presiona el botón de dash y si se puede hacer dash
        if (context.started && canDash)
        {
            Vector2 direcion = dashInput; //Se obtiene la dirección del dash
            //Se verifica si la dirección del dash es cero
            if (direcion == Vector2.zero)
            {
                direcion = lookDch ? Vector2.right : Vector2.left; //Se asigna la dirección del dash a la derecha o izquierda
            }

            StartCoroutine(DoDash(direcion.normalized)); //Se inicia la corrutina del dash
        }
    }

    //ACCIONES DE PERSONAJE
    //MOVIMIENTO
    void Mover()
    {
        rb.velocity = new Vector2(move * speed, rb.velocity.y); //Se asigna la velocidad del objeto

        Girar(); //Se llama a la función de giro
    }

    //Función para girar el Player
    void Girar()
    {
        //Si el movimiento es menor a -0.1 y el Player está mirando a la derecha
        if (move < -0.1 && lookDch)
        {
            Flip(); //Se llama a la función de giro
        }

        //Si el movimiento es mayor a 0.1 y el Player está mirando a la izquierda
        if (move > 0.1 && !lookDch)
        {
            Flip(); //Se llama a la función de giro
        }
    }

    //Función para girar el Player
    void Flip()
    {
        lookDch = !lookDch; //Se cambia la dirección de la variable de giro
        /*
        if (lookDch == true)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;
        */

        Vector3 escala = transform.localScale; //Se obtiene la escala del objeto
        escala.x *= -1; //Se invierte la escala en el eje X
        transform.localScale = escala; //Se asigna la nueva escala al objeto
    }

    //Función para hacer el salto
    void DoJump()
    {
        //Si el Player está en el suelo
        if (IsGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset vertical velocity
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse); // Se aplica la fuerza del salto

            animator.SetBool("saltar", true); //Se activa la animación de salto
            animator.SetBool("fall", false); //Se desactiva la animación de caída


            availableJumps = totalExtraJumps; //Se inicializan los saltos disponibles
        }

        //Si el Player tiene saltos disponibles y puede hacer doble salto
        else if (availableJumps > 0 && canDoubleJump)
        {
            animator.SetBool("fall", true); //Se activa la animación de caída

            float force = jumpForce; //Se asigna la fuerza del salto
            if (Mathf.Abs(move) > 0.1f) force *= 0.7f; //Si el movimiento es mayor a 0.1, se reduce la fuerza del salto
            rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset vertical velocity
            rb.AddForce(Vector3.up * force/1.2f, ForceMode2D.Impulse);// Se aplica la fuerza del salto


            animator.SetBool("saltar", true); //Se activa la animación de salto

            availableJumps--; //Se reduce el número de saltos disponibles
        }
    }

    //Función para hacer la caída
    void fall()
    {
        //Si el Player está en el suelo y está cayendo
        if (!IsGrounded () && rb.velocity.y < -0.1f  /* && !isPropulsing*/)
        {
            animator.SetBool("fall", true);//Se activa la animación de caída
            rb.velocity -= vecGravity * fallMultiplier * Time.deltaTime; //Se aplica la gravedad al objeto

        }

        else
        {
            animator.SetBool("fall", false);//Se desactiva la animación de caída

        }
    }

    //Función para hacer la propulsión
    void Propulsion()
    {
        if (isPropulsing && !IsGrounded() && rb.velocity.y < 0 && canPropulse)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -planeoSpeed));// Se limita la velocidad vertical del objeto
            /*
            rb.gravityScale = 0f;

            if(rb.velocity.y < maxPropulsionSpeed)
            {
                rb.AddForce(Vector2.up * propulsionForce, ForceMode2D.Force);
            }
           
            /*
            rb.AddForce(Vector2.up * 500f * Time.deltaTime, ForceMode2D.Force);
            Debug.Log("Propulsando");
            */
        }

        else
        {
            rb.gravityScale = normalGravityScale;//Se asigna la gravedad normal al objeto
        }
    }

    //Corrutina para hacer el dash
    IEnumerator DoDash(Vector2 direction)
    {
        isDashing = true; //Se activa la variable de dash
        canDash = false; //Se desactiva la variable de dash
        float elapsedTime = 0f; //Se inicializa el tiempo transcurrido

        rb.gravityScale = 0f; //Se desactiva la gravedad del objeto
        rb.velocity = direction * dashForce; //Se aplica la fuerza del dash

        animator.SetTrigger("dash"); //Se activa la animación de dash

        // Se activa el trail del dash u otro efecto visual
        while (elapsedTime < dashTime)
        {/*
            if (dashGhostPrefab)
            {
                Instantiate(dashGhostPrefab, transform.position, Quaternion.identity);
            }
         */
            dashTrail.emitting = true; // Se activa el trail del dash
            elapsedTime += 0.05f; // Se incrementa el tiempo transcurrido
            yield return new WaitForSeconds(0.05f); //Se espera un tiempo
        }

        rb.gravityScale = normalGravityScale; //Se asigna la gravedad normal al objeto
        dashTrail.emitting = false; // Se desactiva el trail del dash
        isDashing = false; //Se desactiva la variable de dash

        yield return new WaitForSeconds(dashCooldown); //Se espera un tiempo para CoolDown
        canDash = true; //Se activa la variable de poder dashear
    }




    //COLLISIONES
    private void OnCollisionEnter2D(Collision2D collision)
    {
        //Si el Player colisiona con el suelo
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            animator.SetBool("saltar",false);//Se desactiva la animación de salto
            animator.SetBool("fall", false);//Se desactiva la animación de caída

            isPropulsing = false;//Se desactiva la variable de propulsión

            //availableJumps = totalExtraJumps;
            canDash = true; //Se activa la variable de poder dashear
        }

    }

}
