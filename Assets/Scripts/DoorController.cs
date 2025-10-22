using UnityEngine;

public class DoorController : MonoBehaviour
{
    /*
    [SerializeField] Transform Door;
    [SerializeField] Transform DoorObjective;
    [SerializeField] float move = 2f;
    [SerializeField] float tiempo = 0f;
    [SerializeField] bool puertaActivada = false;
    */

    [SerializeField] Animator doorAnim;
    [SerializeField] AudioSource doorClip;
    bool puertaActivada,played = false;

    private void Awake()
    {
        doorAnim.speed = 0;
        puertaActivada = false ;
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    private void Update()
    {
        if (puertaActivada && !played)
        {
            doorClip.Play();
            doorAnim.speed = 1;
            played = true;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        puertaActivada = true;


    }

}
