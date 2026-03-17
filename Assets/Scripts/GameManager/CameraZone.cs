using Unity.Cinemachine;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    private CinemachineCamera cineCam;

    public int activePriority = 20;
    public int inactivePriority = 0;

    private void Awake()
    {
        cineCam = GetComponentInChildren<CinemachineCamera>();
        cineCam.Priority = inactivePriority;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cineCam.Priority = activePriority;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cineCam.Priority = inactivePriority;
        }
    }
}
