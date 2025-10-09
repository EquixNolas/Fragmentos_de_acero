using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb;
    Animator animator;

    //Respawn and Die
    [Header("Respawn")] //sección de respawn
    public bool alive = true; //Variable para saber si el Player está vivo
    public Vector2 respawnPosition; //Posición de respawn
    //CHECKING GROUND
    [Header("Ground Check")]//Titulo de la sección
    [SerializeField] TrailRenderer dashTrail; // Prefab del trail del dash
    //[SerializeField] GameObject dashEffect;
    [SerializeField] Transform groundCheck; //Empty object que verifica si estas en el suelo
    [SerializeField] Transform groundCheck2; //Empty object que verifica si estas en el suelo
    [SerializeField] Transform jumpReset; //Empty object que verifica si estas en el suelo
    [SerializeField] GameObject jumpResetObject;
    [SerializeField] LayerMask groundLayer; //Layer del Suelo
    //[SerializeField] GameObject dashGhostPrefab;

    SpriteRenderer spriteRenderer; //Renderizador del sprite

    //MOVIMIENTO
    float horizontalMove; //Variable para el movimiento
    [Header("Movement")] //sección de movimiento
    [SerializeField] float speed = 6f; //variable para la velocidad
    [SerializeField] bool lookDch = true; //Variable para mirar a la derecha
    [SerializeField] ParticleSystem dustParticle; //Partícula de polvo al caminar

    //SALTO
    [Header("Jump")] //sección de salto
    //public bool canDoubleJump = false; //Variable para el doble salto
    [SerializeField] bool isDoubleJumping = false; //Variable para saber si se está haciendo doble salto
    [SerializeField] float jumpForce = 10f; //Fuerza del salto
    public int totalJumps = 1; //Saltos extra 
    [SerializeField] bool jumpQueued = false;
    [SerializeField] int availableJumps; //saltos disponibles
    [SerializeField] float coyoteTime = 0.17f; //Tiempo de salto antes de caer
    [SerializeField] float coyoteCounter = 0.2f; //Contador del tiempo de salto antes de caer
    [SerializeField] float fallMultiplier = 1.2f; //Multiplicador de gravedad para caer más rápido
    public bool pulsarBoton;

    //WALL JUMP
    [Header("Wall Jump")] //sección de salto de la pared
    [SerializeField]float wallJumpTime = 0.2f; //Tiempo del salto de la pared
    [SerializeField]Vector2 wallJumpPower = new Vector2(5f, 8f); //Fuerza del salto de la pared
    [SerializeField] float wallSlideSpeed = 2f; //Velocidad de deslizamiento por la pared
    [SerializeField] Transform wallCheck; //Empty object que verifica si estas en la pared
    [SerializeField] LayerMask wallLayer; //Layer de la pared
    [SerializeField]float wallJumpDirection; //Dirección del salto de la pared
    float wallJumpTimer; //Contador del salto de la pared
    
    //WALL SLIDE
    bool isWallSliding; //Variable para saber si se está deslizándose por la pared
    bool isWallJumping; //Variable para saber si se está saltando de la pared
    [SerializeField] private float maxWallSlideTime = 0.5f; //Tiempo máximo de deslizamiento por la pared
    private float wallSlideTimer = 0f; //Contador del tiempo de deslizamiento por la pared


    float normalGravityScale; //Gravedad normal del objeto
    Vector2 vecGravity; //Vector2 de gravedad

    //PROPULSION
    [Header("Propulsion")] //sección de propulsión
    [SerializeField]float planeoSpeed = 2f; //Velocidad de planeo
    bool isPropulsing = false; //Variable para saber si se está planeando
    bool canPropulse = false; //Variable para saber si se puede planear

    //DASHEO
    [Header("Dash")] //sección de dash
    [SerializeField] bool dashUnlock = false;
    [SerializeField] float dashForce = 30f; // Fuerza del dash
    [SerializeField] float dashTime = 0.1f; // Duración del dash
    [SerializeField] float dashCooldown = 0.5f; // Tiempo entre dashes

    bool isDashing = false; //Variable para saber si se está haciendo dash
    bool canDash = true; //Variable para saber si se puede hacer dash

    TimeChanger timeChanger;
    bool onSlowMo;
    Vector2 dashInput; // Variable para la dirección del dash

    Vector3 PlayerlocalScale; //Escala del objeto

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>(); //Se obtiene el componente Animator
        rb = GetComponent<Rigidbody2D>(); //Se obtiene el componente Rigidbody2D
        spriteRenderer = GetComponent<SpriteRenderer>(); //Se obtiene el componente SpriteRenderer
        timeChanger = GameObject.Find("TimeSlow").GetComponent<TimeChanger>();
        //dashEffect.SetActive(false); //Se desactiva el efecto del dash
        vecGravity = new Vector2(0, -Physics2D.gravity.y); //Se obtine la gravedad del objeto
        normalGravityScale = rb.gravityScale;   //Se guarda la gravedad normal del objeto

        availableJumps = totalJumps; //Se inicializan los saltos disponibles
        
        PlayerlocalScale = GetComponent<Transform>().localScale; //Se obtiene la escala del objeto
        respawnPosition = transform.position; //Se obtiene la posición de respawn

        pulsarBoton = false; 
        lookDch = true; //Se inicializa la dirección de giro a la derecha
        alive = true; //Se activa la variable de vida

        jumpResetObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //CoyoteTimeControl(); //Se llama a la función de control del tiempo de salto antes de caer
        if (alive)
        {
            onSlowMo = timeChanger.onSlowMo;
            Flip(); //Se llama a la función de giro
            WallSlide(); //Se llama la función de deslizamiento por la pared
            ProccessWallJump(); //Se llama a la función de salto de la pared
        
            if (!isWallJumping) { fall(); } //Se llama la función de caída

            if (!isDashing && !isWallJumping)
            {
                Mover(); //Se llama la función de movimiento
                Propulsion(); //Se llama la función de propulsión
            }
            
            if (IsGrounded())
            {
                availableJumps = totalJumps;
                jumpResetObject.SetActive(false);
            }

            if (jumpQueued && availableJumps > 0) 
            {
                DoJump();
                jumpQueued = false; 
            }
        }
        Debug.Log(pulsarBoton);

        ActualizarAnimaciones(); //Se llama a la función de actualización de animaciones
    }
    public void ConsumeJumpOrButton()
    {
        pulsarBoton = false;
    }
    bool IsGrounded() //Verifica si el Player está en el suelo
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer); //Se lanza un rayo hacia abajo desde el objeto
        RaycastHit2D hit2 = Physics2D.Raycast(groundCheck2.position, Vector2.down, 0.1f, groundLayer); //Se lanza un rayo hacia abajo desde el objeto
        RaycastHit2D hit3 = Physics2D.Raycast(jumpReset.position, Vector2.down, 0.1f, groundLayer); //Se lanza un rayo hacia abajo desde el objeto
        return hit.collider != null || hit2.collider != null; //Si el rayo colisiona con algo, devuelve true
    }

    bool WallCheck() //Verifica si el Player está en la pared
    {
        return Physics2D.OverlapCircle(wallCheck.position, 0.1f, wallLayer); //Se lanza un círculo desde el objeto
    }

    //Dibuja el Gizmo de la verificación del suelo
    void OnDrawGizmos() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * 0.1f);
        Gizmos.DrawLine(groundCheck2.position, groundCheck2.position + Vector3.down * 0.1f);
        Gizmos.DrawLine(jumpReset.position, jumpReset.position + Vector3.down * 0.1f);
        Gizmos.DrawWireSphere(wallCheck.position, 0.1f); //Dibuja un círculo en la posición del wallCheck
    }

    //INPUTS ACTIONS
    // INPUTS  ACTIONS DE MOVIMIENTO

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMove = context.ReadValue<Vector2>().x; //Se obtiene el valor del movimiento
        dashInput = context.ReadValue<Vector2>(); //Se obtiene el valor del dashInput
    }

    void ActualizarAnimaciones()
    {
        if (!alive)
        {
            // Desactivar todas las animaciones en estado inactivo
            animator.SetFloat("move", 0f);
            animator.SetBool("saltar", false);
            animator.SetBool("fall", false);
            animator.SetBool("sliding", false);
            animator.SetBool("dash", false);
            return;
        }

        // Movimiento en el suelo
        animator.SetFloat("move", IsGrounded() ? Mathf.Abs(horizontalMove) : 0f);

        // Animaciones verticales
        float verticalVelocity = rb.linearVelocity.y;

        if (verticalVelocity > 0.1f)
        {
            // Saltando
            animator.SetBool("saltar", true);
            if (isDoubleJumping)
            {
                animator.SetBool("doublejump", isDoubleJumping);
            }
            animator.SetBool("fall", false);
        }
        else if (verticalVelocity < -0.1f)
        {
            // Cayendo
            animator.SetBool("saltar", false);
            animator.SetBool("doublejump", false);
            animator.SetBool("fall", true);
        }
        else if (IsGrounded())
        {
            // En el suelo
            animator.SetBool("saltar", false);
            animator.SetBool("doublejump", false);
            animator.SetBool("fall", false);
        }

        // Deslizamiento en pared
        animator.SetBool("sliding", isWallSliding);
    }

    //INPUT ACTION DE SALTO
    public void Jump(InputAction.CallbackContext context) 
    {
        if (!alive) return; //Si el Player no está vivo, no se hace nada
        //Se verifica si se presiona el botón de salto
        if (context.started && !isWallJumping)  
        {
            //DoJump(); //se llama a la función de salto
            if(availableJumps >0)
            jumpQueued = true;
        }
        if(context.started && wallJumpTimer > 0f)
        {
            DoWallJump(); //Se llama a la función de salto de la pared si el timer es mayor a 0
        }

        //Se verifica si se mantiene el botón de salto
        if (context.performed)
        {
            isPropulsing = true; //Se activa la variable de propulsión
        }

        //Se verifica si se suelta el botón de salto
        else if (context.canceled)
        {
            isPropulsing = false; //Se desactiva la variable de propulsión
        }
    }

    //INPUT ACTION DE DASHEO
    public void Dash(InputAction.CallbackContext context)
    {
        if (alive && dashUnlock)
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

                if(WallCheck() && !IsGrounded()) //Se verifica si el Player está en la pared y no está en el suelo
                {
                    direcion = lookDch ? Vector2.left : Vector2.right; //Se asigna la dirección del dash a la derecha o izquierda
                    lookDch = !lookDch; //Se cambia la dirección de la variable de giro
                    Vector3 escala = transform.localScale; //Se obtiene la escala del objeto
                    escala.x *= -1; //Se invierte la escala en el eje X
                    transform.localScale = escala; //Se asigna la nueva escala al objeto
                }


                StartCoroutine(DoDash(direcion.normalized)); //Se inicia la corrutina del dash
            }

        }
      
       
    }

    //ACCIONES DE PERSONAJE
    //MOVIMIENTO
    void Mover()
    {
        rb.linearVelocity = new Vector2(horizontalMove * speed, rb.linearVelocity.y); //Se asigna la velocidad del objeto
    }
   
    //Función para girar el Player
    void Flip()
    {
        if (lookDch && horizontalMove < 0 || !lookDch && horizontalMove > 0) 
        {
            lookDch = !lookDch; //Se cambia la dirección de la variable de giro
            Vector3 escala = transform.localScale; //Se obtiene la escala del objeto
            escala.x *= -1; //Se invierte la escala en el eje X
            transform.localScale = escala; //Se asigna la nueva escala al objeto
        }

    }
    void CoyoteTimeControl()
    {
        if (IsGrounded() || WallCheck())
        {
            coyoteCounter = coyoteTime; //Se reinicia el contador del tiempo de salto antes de caer

            if(availableJumps <= 0)
            {
                availableJumps = totalJumps; //Se reinicia el número de saltos disponibles
                isDoubleJumping = false; //Se desactiva la variable de doble salto
            }
        }

        else
        {
            coyoteCounter -= Time.deltaTime; //Se reduce el contador del tiempo de salto antes de caer
        }
    }

    //Función para hacer el salto
    void DoJump()
    {
        float force = jumpForce; //Se asigna la fuerza del salto
        //Si el Player está en el suelo
        if (availableJumps > 0)
        {
            if (totalJumps == 2 && availableJumps == 1)
            {
                isDoubleJumping = true; //Se activa la variable de doble salto
                if (onSlowMo && !pulsarBoton)
                {
                    pulsarBoton = true;
                }
            }

            if (Mathf.Abs(horizontalMove) > 0.1f || isDoubleJumping)
            {
               force *= 0.80f; //Si el movimiento es mayor a 0.1, se reduce la fuerza del salto
            }

            StartCoroutine(Dust()); //Se inicia la corrutina de polvo al caminar

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f); // Reset vertical velocity
            rb.AddForce(Vector3.up * force, ForceMode2D.Impulse); // Se aplica la fuerza del salto

            coyoteCounter = 0f; //Se reinicia el contador del tiempo de salto antes de caer
            
            availableJumps--;
        }

        StartCoroutine(JumpResetActive());
    }
    IEnumerator JumpResetActive()
    {
        yield return new WaitForSeconds(0.2f);
        jumpResetObject.SetActive(true);
    }
    IEnumerator Dust()
    {
        dustParticle.Play(); //Se reproduce la partícula de polvo al caminar
        yield return new WaitForSeconds(0.1f); //Se espera un tiempo
        dustParticle.Stop(); //Se detiene la partícula de polvo al caminar
    }

    void DoWallJump()
    {
        if(isWallJumping) return; //Si ya se está saltando de la pared, no se hace nada

        float jumpDir = -wallJumpDirection; //Se asigna la dirección del salto de la pared

        isWallJumping = true; //Se activa la variable de salto de la pared

        rb.linearVelocity = new Vector2(jumpDir * wallJumpPower.x, wallJumpPower.y); //Se aplica la fuerza del salto de la pared
        

        wallJumpTimer = 0f; //Se reinicia el contador del tiempo de salto de la pared

        if (Mathf.Sign(transform.localScale.x) != Mathf.Sign(jumpDir))
        {
            lookDch = !lookDch;
            Vector3 escala = transform.localScale;
            escala.x *= -1;
            transform.localScale = escala;
        }

        Invoke(nameof(CancelWallJump), wallJumpTime + 0.1f); //Se invoca la función de detener el salto de la pared
    }


    private void WallSlide() //Se verifica si el Player está deslizándose por la pared
    {
        //Si el Player está en la pared y no está en el suelo y se está moviendo
        if (WallCheck() && !IsGrounded() && horizontalMove != 0f)
        {
            if(wallSlideTimer < maxWallSlideTime)
            {
                isWallSliding = true;//Se activa la variable de deslizamiento por la pared
                wallSlideTimer += Time.deltaTime; //Se incrementa el contador del tiempo de deslizamiento por la pared
                
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed)); //Se limita la velocidad vertical del objeto
            }

            else
            {
                isWallSliding = false; //Se desactiva la variable de deslizamiento por la pared
            }

        }

        else
        {
            isWallSliding = false; //Se desactiva la variable de deslizamiento por la pared
            wallSlideTimer = 0f; //Se reinicia el contador del tiempo de deslizamiento por la pared
        }

    }

    private void ProccessWallJump()
    {
        if (isWallSliding)
        {
            //isWallJumping = false;
            wallJumpDirection = transform.localScale.x; //Se asigna la dirección del salto de la pared
            wallJumpTimer = wallJumpTime; //Se asigna el tiempo del salto de la pared

            //CancelInvoke(nameof(CancelWallJump)); //Se cancela la invocación de la función de detener el salto de la pared 
        }

        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime; //Se reduce el tiempo del salto de la pared
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false; //Se desactiva la variable de salto de la pared
    }


    //Función para hacer la caída
    void fall()
    {
        //Si el Player no está en el suelo y está cayendo
        if (!IsGrounded () && rb.linearVelocity.y < -0.1f  /* && !isPropulsing*/)
        {
            rb.linearVelocity -= vecGravity * fallMultiplier * Time.deltaTime; //Se aplica la gravedad al objeto
            isDoubleJumping = false; //Se desactiva la variable de doble salto
        }

    }
   
    //Función para hacer la propulsión
    void Propulsion()
    {
        if (isPropulsing && !IsGrounded() && rb.linearVelocity.y < 0 && canPropulse)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -planeoSpeed));// Se limita la velocidad vertical del objeto
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
        rb.linearVelocity = direction * dashForce; //Se aplica la fuerza del dash

        animator.SetBool("dash",true); //Se activa la animación de dash

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
        animator.SetBool("dash", false); //Se Desactiva la animación de dash
        dashTrail.emitting = false; // Se desactiva el trail del dash
        //dashEffect.SetActive(false); // Se desactiva el efecto del dash
        isDashing = false; //Se desactiva la variable de dash

        yield return new WaitForSeconds(dashCooldown); //Se espera un tiempo para CoolDown
        canDash = true; //Se activa la variable de poder dashear
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("_Respawn"))
        {
            //Die(); //Se llama a la función de muerte
        }
        if (alive)
        {
            if(collision.gameObject.layer == LayerMask.NameToLayer("Reset Zone"))
            {
                rb.linearVelocity = Vector2.zero; //Se detiene el movimiento del objetoº
                rb.simulated = false; //Se desactiva la simulación del objeto

                //Debug.Log("Estas Muerto!"); //Se imprime un mensaje en la consola
            }

            if(collision.gameObject.CompareTag("Die"))
            {   
           
                animator.SetTrigger("death"); //Se activa la animación de muerte
                alive = false; //Se desactiva la variable de vida
                Die(); //Se llama a la función de muerte
            }
        }
    }

    public void UdpateCheckpoint(Vector2 pos)
    {
        respawnPosition = pos; //Se asigna la posición de respawn al objeto
    }

    void Die()
    {
        StartCoroutine(Respawn(0.5f)); //Se inicia la corrutina de respawn
    }

    IEnumerator Respawn(float waitTime)
    {
        rb.linearVelocity = Vector2.zero; //Se detiene el movimiento del objeto
        animator.SetFloat("move", 0f); //Se asigna el valor del movimiento al animator

        yield return new WaitForSeconds(2f); //Se espera un tiempo
        rb.simulated = false; //Se desactiva la simulación del objeto
        transform.localScale = new Vector3(0, 0, 0); //Se asigna la escala del objeto a cero

        yield return new WaitForSeconds(waitTime); //Se espera un tiempo
        rb.simulated = true; //Se activa la simulación del objeto
        transform.position = respawnPosition; //Se asigna la posición de respawn al objeto
        lookDch = true; //Se reinicia la dirección de giro
        rb.linearVelocity = Vector2.zero; //Se detiene el movimiento del objeto
        animator.Play("RobotIdle", 0, 0f); //Se reproduce la animación de idle del robot
        animator.Update(0f); //Se actualiza el animator para que la animación se reproduzca correctamente
        transform.localScale = PlayerlocalScale; //Se asigna la escala del objeto

        yield return new WaitForSeconds(0.3f); //Se espera un tiempo
        alive = true; //Se activa la variable de vida
    }
}
