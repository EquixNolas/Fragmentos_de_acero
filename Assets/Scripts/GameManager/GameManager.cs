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

    [Header("TIME CONTROL")]
    [SerializeField] PlayerMovement pm;
    [SerializeField]TimeChanger time;

    private void Awake()
    {
        pm = GameObject.Find("Player").GetComponent<PlayerMovement>();
        time= GameObject.Find("TimeSlow").GetComponent<TimeChanger>();
        currentSceneName = SceneManager.GetActiveScene().name;
        pauseCanvas.SetActive(false);
        nowCanMove = true;
        skillsCount = 0;
}
    // Update is called once per frame
    void Update()
    {

        CountCoins();
        if (!time.activeSlowdown || pm.isDoubleJumping)
        {
           TimeRecover();
        }

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
    public void TimeRecover()
    {
        Time.timeScale += (1f / time.slowdownTime) * Time.unscaledDeltaTime;
        Time.timeScale = Mathf.Clamp(Time.timeScale, 0f, 1f);
    }
}
