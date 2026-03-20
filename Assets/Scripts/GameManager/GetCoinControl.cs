using Unity.VisualScripting;
using UnityEngine;

public class GetCoinControl : MonoBehaviour
{
    [SerializeField] SpriteRenderer coinRenderer;
    PlayerMovement player;
    GameManager gameManager;
    Color color;
    bool coinReach;
    
    private void Awake()
    {
        gameManager = GameObject.Find("10_GAMEMANAGER").GetComponent<GameManager>();
        player = GameObject.Find("Player").GetComponent<PlayerMovement>();
        coinRenderer = GetComponent<SpriteRenderer>();
        color = coinRenderer.color;
        coinReach = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            color.a = 0.06f;
            coinRenderer.color = color;
            coinReach = true;
            //Debug.Log("Core");
        }
    }
    private void Update()
    {
        GetCoin();
    }
    void GetCoin()
    {
        if (player.IsGrounded() && coinReach)
        {
            gameManager.coins++;
            coinReach = false;
            GameObject padreGO = transform.parent.gameObject;
            padreGO.SetActive(false);
        }
    }
}
