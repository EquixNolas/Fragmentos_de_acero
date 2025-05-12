using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    PlayerMovement playerMovement;
    [SerializeField] private Transform respawnPoint;
    Collider2D coll;
    private void Awake()
    {
        playerMovement = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        coll = GetComponent<Collider2D>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            playerMovement.UdpateCheckpoint(respawnPoint.position);
            Debug.Log("Checkpoint reached!");
            coll.enabled = false; // Disable the checkpoint collider after being activated
        }
    }
}
