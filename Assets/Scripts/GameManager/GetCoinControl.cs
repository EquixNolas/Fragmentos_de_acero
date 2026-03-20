using Unity.VisualScripting;
using UnityEngine;

public class GetCoinControl : MonoBehaviour
{
    [SerializeField] SpriteRenderer coinRenderer;
    PlayerMovement player;
    GameManager manager;
    Color color;
    bool coinReach;
    
    private void Awake()
    {
        manager = GameObject.Find("10_GAMEMANAGER").GetComponent<GameManager>();
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
            Debug.Log("Core");
        }
    }
    private void Update()
    {
        GetCoin();
        Debug.Log("Tus puntos son: " + manager.coins);
    }
    void GetCoin()
    {
        if (player.IsGrounded() && coinReach)
        {
            manager.coins++;
            coinReach = false;
            GameObject padreGO = transform.parent.gameObject;
            padreGO.SetActive(false);
            Debug.Log("Consigues el coin");
        }
    }
}
