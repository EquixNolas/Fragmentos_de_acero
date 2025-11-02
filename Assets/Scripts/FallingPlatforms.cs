using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FallingPlatforms : MonoBehaviour
{
    Rigidbody2D rigidbody2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rigidbody2 = GetComponent<Rigidbody2D>();
        rigidbody2.bodyType = RigidbodyType2D.Kinematic;
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        Debug.Log("Estas Encima de la plataforma");

        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(transform);
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.transform.SetParent(null);
        }
        StartCoroutine(Fall());
    }

    IEnumerator Fall()
    {
        yield return new WaitForSeconds(.1f);

        rigidbody2.bodyType = RigidbodyType2D.Dynamic;

        Destroy(gameObject, 3f);
    }
}
