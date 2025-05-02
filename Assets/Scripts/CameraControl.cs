using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControls : MonoBehaviour
{
    public Transform player;
    public float smoothSpeed = 5f;
    public Vector2 offset;

    public float deadZoneY = 1.5f; // Tamaño de la zona muerta vertical

    public Vector2 minLimits;
    public Vector2 maxLimits;

    private float initialY;

    void Start()
    {
        if (player != null)
        {
            initialY = player.position.y;
        }
    }

    void LateUpdate()
    {
        if (player == null) return;

        Vector3 targetPosition = transform.position;

        // Movimiento lateral siempre
        targetPosition.x = player.position.x + offset.x;

        // Movimiento vertical solo si el jugador sale de la zona muerta
        float distanceY = player.position.y - initialY;

        if (Mathf.Abs(distanceY) > deadZoneY)
        {
            initialY = player.position.y - Mathf.Sign(distanceY) * deadZoneY;
        }

        targetPosition.y = Mathf.Lerp(transform.position.y, initialY + offset.y, smoothSpeed * Time.deltaTime);

        targetPosition.x = Mathf.Clamp(targetPosition.x, minLimits.x, maxLimits.x);
        targetPosition.y = Mathf.Clamp(targetPosition.y, minLimits.y, maxLimits.y);

        transform.position = targetPosition;
    }
}

