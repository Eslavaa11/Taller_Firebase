using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    public float speed = 8f;
    public float clampX = 3.5f;

    // --- Ajustes del detector de recogida ---
    [Header("Pickup Overlap (garantizado)")]
    [Tooltip("Escala el tamaño del área de detección respecto al BoxCollider2D del Player")]
    public Vector2 pickupScale = new Vector2(1.05f, 1.05f); // un pelín más grande que tu collider
    [Tooltip("Filtro de capas opcional (déjalo en -1 para usar todas)")]
    public LayerMask pickupMask = ~0; // todas

    private BoxCollider2D _col;

    private void Awake()
    {
        _col = GetComponent<BoxCollider2D>();

        // Config segura para movimiento
        var rb = GetComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0f;

        // Puedes tener Trigger ON u OFF; no depende de eso
        // _col.isTrigger = true; // opcional
    }

    void Update()
    {
        // Movimiento lateral
        float move = Input.GetAxisRaw("Horizontal") * speed * Time.deltaTime;
        var pos = transform.position;
        pos.x = Mathf.Clamp(pos.x + move, -clampX, clampX);
        transform.position = pos;

        // Detector GARANTIZADO de "goods"
        CheckGoodPickup();
    }

    private void CheckGoodPickup()
    {
        // Tamaño del área basado en tu BoxCollider2D
        var size = _col.bounds.size;
        size = new Vector2(size.x * pickupScale.x, size.y * pickupScale.y);

        // Busca TODO lo que se superponga con el Player
        var hits = Physics2D.OverlapBoxAll(transform.position, size, 0f, pickupMask);

        for (int i = 0; i < hits.Length; i++)
        {
            // ¿Es un FallingObject?
            if (!hits[i].TryGetComponent<FallingObject>(out var falling)) continue;

            // Si es GOOD → recogida
            if (falling.isGood)
            {
                // Puntos + popup (llama al GameManager)
                GameManager.I.OnGoodPickup(falling.transform.position, 5);

                // Destruye el cubo azul para que no se repita
                Destroy(falling.gameObject);

                // Como ya recogimos uno en este frame, podemos salir (opcional)
                break;
            }
        }
    }

    // Mantengo la ruta de colisión para el ROJO (= game over)
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.TryGetComponent<FallingObject>(out var falling) && !falling.isGood)
        {
            GameManager.I.GameOver();
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.TryGetComponent<FallingObject>(out var falling) && !falling.isGood)
        {
            GameManager.I.GameOver();
            Destroy(other.collider.gameObject);
        }
    }

    // Gizmo para ver el área de pickup en escena/juego
    private void OnDrawGizmosSelected()
    {
        if (_col == null) _col = GetComponent<BoxCollider2D>();
        var size = _col.bounds.size;
        size = new Vector2(size.x * pickupScale.x, size.y * pickupScale.y);
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(transform.position, size);
    }
}
