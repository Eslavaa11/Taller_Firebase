using UnityEngine;

/// Hace que el objeto caiga usando física 2D (rb.velocity) y
/// reporta "esquivado" si sale por abajo.
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class FallingObject : MonoBehaviour
{
    public float fallSpeed = 4f;
    public bool isGood = false;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // Forzamos configuración segura
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;

        var col = GetComponent<BoxCollider2D>();
        col.isTrigger = false; // los que caen no son trigger
    }

    private void OnEnable()
    {
        rb.linearVelocity = Vector2.down * fallSpeed; // caer siempre
    }

    void Update()
{
    transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);

    if (transform.position.y < -6f)
    {
        // Si NO quieres puntos gratis por nada, no llames OnDodged.
        // Si quieres puntuar solo por bad, deja la condición:
        // if (!isGood) GameManager.I.OnDodged();
        Destroy(gameObject);
    }
}

}
