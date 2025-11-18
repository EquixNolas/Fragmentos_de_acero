using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class GameManager : MonoBehaviour
{
    
    [SerializeField] GameObject pauseCanvas;
    public GameObject firstSelectedButton;
    public bool pausa =  false;
    public bool nowCanMove = true;
    bool normalTime = false;
    private void Awake()
    {
        pauseCanvas.SetActive(false);
         nowCanMove = true;
}
    // Update is called once per frame
    void Update()
    {

        Debug.Log("Pausa es: "+pausa);

    }


   

    public void Pausa(InputAction.CallbackContext context)
    {
        if (!pausa && context.started)
        {
            pauseCanvas.SetActive(true);
            pausa = true;
            nowCanMove = false;
            Time.timeScale = 0.001f;
            EventSystem.current.SetSelectedGameObject(null); // limpiar selección
            EventSystem.current.SetSelectedGameObject(firstSelectedButton); // seleccionar botón
        }

        else if (pausa && context.started) 
        {
            pauseCanvas.SetActive(false) ;
            pausa = false ;
            Time.timeScale = 1f;

        }   

    }

    public void Reanudar()
    {
        if (pausa && pauseCanvas == true)
        {
            pauseCanvas.SetActive(false);
            pausa = false;
            Time.timeScale = 1f;
        }
    }


}
