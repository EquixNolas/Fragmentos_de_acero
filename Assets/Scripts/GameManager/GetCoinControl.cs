using UnityEngine;

public class GetCoinControl : MonoBehaviour
{
    [SerializeField] SpriteRenderer coinRenderer;
    Color color;
    private void Awake()
    {
        coinRenderer = GetComponent<SpriteRenderer>();
        color = coinRenderer.color;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            color.a = 0.06f;
            coinRenderer.color = color;
            Debug.Log("Core");
        }
    }
}
