using System.Collections;
using UnityEngine;
public class SkillControl : MonoBehaviour
{
    [Header("Script Declaration")]
    [SerializeField] GameManager gameManager;
    [SerializeField] PlayerMovement pm;
    [SerializeField] TimeChanger timeChanger; // Cambiado el nombre para evitar conflictos con 'time' de C#

    [Header("GameObjects")]
    [SerializeField] private GameObject[] skillUnlocker;
    [SerializeField] private GameObject[] timeSlower;
    [SerializeField] private Animator animatorButtons;

    private bool skillProcessed = false;
    public bool isPlayerInside;

    private void Awake()
    {
        
        gameManager = GameObject.Find("10_GAMEMANAGER").GetComponent<GameManager>();
        pm = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>();
        timeChanger = GameObject.FindGameObjectWithTag("TimeSlowMo").GetComponent<TimeChanger>();
        isPlayerInside = gameManager.playerInsideSlowMo;
    }

    private void Update()
    {
        if (isPlayerInside)
        {
            if (gameManager.skillsCount > 0)
            {
                //ReturnToNormalTime();
                Debug.Log("Debe Funcionar");
            }
        }

        Debug.Log("Habilidades: "+ gameManager.skillsCount);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !skillProcessed)
        {
            skillProcessed = true; // Bloqueamos futuras ejecuciones inmediatas
            isPlayerInside = true;
            gameManager.playerInsideSlowMo = isPlayerInside;

            if (gameManager.skillsCount == 0)
            {
                // 1. Mostramos el texto/UI
                animatorButtons.SetInteger("ButtonNext", gameManager.skillsCount);
                skillUnlocker[0].SetActive(true);

                // 2. Activamos el SlowMo (TimeChanger se encarga del TimeScale)
                timeChanger.activeSlowdown = true;
                timeChanger.normalTime = false;

                // 3. Desbloqueo de habilidad
                StartCoroutine(WaitTime(0.05f));
                pm.totalJumps = 2;
                pm.availableJumps = 1;
                Debug.Log("Double Jump unlocked!");
            }

            else if(gameManager.skillsCount == 1)
            {
                skillUnlocker[0].SetActive(true);
                animatorButtons.SetInteger("ButtonNext", gameManager.skillsCount);
                Debug.Log("Dash Desbloqueado");
                pm.dashUnlock = true;
            }
            gameManager.skillsCount++;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            ReturnToNormalTime();
            skillProcessed = false;
        }
    }

    void ReturnToNormalTime()
    {
        DesactivateUI();
        isPlayerInside = false;
        gameManager.playerInsideSlowMo = isPlayerInside;
        timeChanger.normalTime = true;
        timeChanger.activeSlowdown = false;
    }

    void DesactivateUI()
    {
        // Usamos un bucle para apagar todo lo que esté encendido
        foreach (GameObject ui in skillUnlocker) ui.SetActive(false);
        foreach (GameObject slow in timeSlower) slow.SetActive(false);
    }

    IEnumerator WaitTime(float waitTime)
    {
        //WaitForSecondsRealtime porque el Time.timeScale es muy bajo
        yield return new WaitForSecondsRealtime(1.5f);
        DesactivateUI();
    }
}
