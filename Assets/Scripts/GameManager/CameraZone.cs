using Unity.Cinemachine;
using UnityEngine;

public class CameraZone : MonoBehaviour
{
    //THIS CODE CHANGE THE PRIORITY OF THE CINEMACHINE CAMS

    //Getting the cinemachineCam
    private CinemachineCamera cineCam;

    //Variables of control
    public int activePriority = 20;
    public int inactivePriority = 0;

    private void Awake()
    {
        cineCam = GetComponentInChildren<CinemachineCamera>();
        cineCam.Priority = inactivePriority;
    }

    //When you enter on the trigger the cam is activated
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cineCam.Priority = activePriority;
        }
    }

    //When you enter on the trigger the cam is desactivated
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            cineCam.Priority = inactivePriority;
        }
    }
}
