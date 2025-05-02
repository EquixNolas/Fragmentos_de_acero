using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class PlayerMovement : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;

    //Checking ground
    [Header("Ground Check")]//Titulo de la sección
    [SerializeField] TrailRenderer dashTrail; // Prefab del trail del dash
    [SerializeField] Transform groundCheck; //Empty object que verifica si estas en el suelo
    [SerializeField] LayerMask groundLayer; //Layer del Suelo
    //[SerializeField] GameObject dashGhostPrefab;

    SpriteRenderer spriteRenderer; //Renderizador del sprite

    float move; //Variable para el movimiento
    [Header("Movement")] //sección de movimiento
    [SerializeField]float speed = 5f; //variable para la velocidad
    bool lookDch = true; //Variable para mirar a la derecha

    int availableJumps; //saltos disponibles
    [Header("Jump")] //sección de salto
    [SerializeField]float jumpForce = 10f; //Fuerza del salto
    [SerializeField] int totalExtraJumps = 2; //Saltos extra 
    [SerializeField]float fallMultiplier = 1.2f; //Multiplicador de gravedad para caer más rápido
    public bool canDoubleJump = false; //Variable para el doble salto
    float normalGravityScale; //Gravedad normal del objeto

    [Header("Propulsion")] //sección de propulsión
    [SerializeField]float planeoSpeed = 2f; //Velocidad de planeo
    bool isPropulsing = false; //Variable para saber si se está planeando
    bool canPropulse = false; //Variable para saber si se puede planear

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
        animator = GetComponent<Animator>(); //
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        vecGravity = new Vector2(0, -Physics2D.gravity.y);
        normalGravityScale = rb.gravityScale;
        availableJumps = totalExtraJumps;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isDashing)
        {
            Mover();
            Propulsion();
        }
            fall();
      
    }

    bool isGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(groundCheck.position, Vector2.down, 0.1f, groundLayer);
        return hit.collider != null;
    }
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundCheck.position, groundCheck.position + Vector3.down * 0.1f);
    }

    //Inputs Actions
    public void Move(InputAction.CallbackContext context)
    {
        move = context.ReadValue<Vector2>().x;
        animator.SetFloat("move", Mathf.Abs(move));

        dashInput = context.ReadValue<Vector2>();
    }



    public void Jump(InputAction.CallbackContext context) 
    {
        if(context.started)
        {
            DoJump();

        }
        
        if (context.performed)
        {
            isPropulsing = true;

        }

        if (context.canceled)
        {
            isPropulsing = false;

        }

        

    }



    //ACCIONES DE PERSONAJE
    void Mover()
    {
        rb.velocity = new Vector2(move * speed, rb.velocity.y);

        Girar();
    }

    void Girar()
    {
        if (move < -0.1 && lookDch)
        {
            flip();
        }

        if (move > 0.1 && !lookDch)
        {
            flip();
        }
    }

    void flip()
    {
        lookDch = !lookDch;
        /*
        if (lookDch == true)
            spriteRenderer.flipX = false;
        else
            spriteRenderer.flipX = true;
        */

        Vector3 escala = transform.localScale;
        escala.x *= -1;
        transform.localScale = escala;
    }

    void DoJump()
    {
        if (isGrounded())
        {
            rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset vertical velocity
            rb.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);

            animator.SetBool("saltar", true);
            animator.SetBool("fall", false);


            availableJumps = totalExtraJumps;
        }

        else if (availableJumps > 0 && canDoubleJump)
        {
            animator.SetBool("fall", true);

            float force = jumpForce;
            if (Mathf.Abs(move) > 0.1f) force *= 0.7f;
            rb.velocity = new Vector2(rb.velocity.x, 0f); // Reset vertical velocity
            rb.AddForce(Vector3.up * force/1.2f, ForceMode2D.Impulse);
            

            animator.SetBool("saltar", true);
            
            availableJumps--;
        }
    }
    void fall()
    {

        if (!isGrounded () && rb.velocity.y < -0.1f  /* && !isPropulsing*/)
        {
            animator.SetBool("fall", true);     
            rb.velocity -= vecGravity * fallMultiplier * Time.deltaTime;

        }

        else
        {
            animator.SetBool("fall", false);

        }
    }

    void Propulsion()
    {
        if (isPropulsing && !isGrounded() && rb.velocity.y < 0 && canPropulse)
        {
            rb.velocity = new Vector2(rb.velocity.x, Mathf.Max(rb.velocity.y, -planeoSpeed));
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
            rb.gravityScale = normalGravityScale;
        }
    }


    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started && canDash)
        {
            Vector2 direcion = dashInput;

            if (direcion == Vector2.zero)
            {
                direcion = lookDch ? Vector2.right : Vector2.left;
            }

            StartCoroutine(DoDash(direcion.normalized));
            //StartDash();
        }
    }

    IEnumerator DoDash(Vector2 direction)
    {
        isDashing = true;
        canDash = false;
        float elapsedTime = 0f;

        rb.gravityScale = 0f;
        rb.velocity = direction * dashForce;

        animator.SetTrigger("dash");

        while (elapsedTime < dashTime)
        {/*
            if (dashGhostPrefab)
            {
                Instantiate(dashGhostPrefab, transform.position, Quaternion.identity);
            }
         */
            dashTrail.emitting = true;
            elapsedTime += 0.05f;
            yield return new WaitForSeconds(0.05f);
        }

        rb.gravityScale = normalGravityScale;
        dashTrail.emitting = false;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }




    //COLLISIONES
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            animator.SetBool("saltar",false);
            animator.SetBool("fall", false);

            isPropulsing = false;

            //availableJumps = totalExtraJumps;
            canDash = true; // <- Opcional: recarga dash al tocar el suelo
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        //canJump=false;
    }

    
}
