using System;
using TMPro;
using TMPro.EditorUtilities;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("PAUSE MENU")]//Titulo de la sección
    [SerializeField] GameObject pauseCanvas;
    public GameObject firstSelectedButton;
    string currentSceneName;
    public bool pausa =  false;
    public bool nowCanMove = true;

    [Header("HUD MENU")]//Titulo de la sección
    public int coins = 0;
    [SerializeField] TextMeshProUGUI coinCount;

    [Header("ABILITIES UNLOCKER")]//Titulo de la sección
    public GameObject[] skillUnlockers;
    public int skillsCount;

    private void Awake()
    {
        skillsCount = 0;
        currentSceneName = SceneManager.GetActiveScene().name;
        pauseCanvas.SetActive(false);
         nowCanMove = true;
}
    // Update is called once per frame
    void Update()
    {

        Debug.Log("Pausa es: "+pausa);
        CountCoins();

    }
    private void CountCoins()
    {
        coinCount.text = coins.ToString();
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
        Debug.Log("Quitar Pausa");
        if (pausa && pauseCanvas == true)
        {
            pauseCanvas.SetActive(false);
            pausa = false;
            Time.timeScale = 1f;
        }
    }

    public void Reiniciar()
    {
        SceneManager.LoadScene(currentSceneName);
        Time.timeScale = 1f;
    }
}
