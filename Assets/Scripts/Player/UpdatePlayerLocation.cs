using System;
using Unity.VisualScripting;
using UnityEngine;

public class UpdatePlayerLocation : MonoBehaviour
{
    [SerializeField] Transform player;
    [SerializeField] Transform teleport;

    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Transform>();
        teleport = transform.Find("PointTeleport");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.position = teleport.position;
            Debug.Log("El player esta en el teleport");
        }
    }
}
