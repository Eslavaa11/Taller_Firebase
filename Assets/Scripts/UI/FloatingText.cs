using TMPro;
using UnityEngine;

/// Anima un TMP_Text: sube, se desvanece y se destruye al finalizar.
/// Úsalo en el prefab de popup o el script lo agrega automáticamente.
[RequireComponent(typeof(TMP_Text))]
public class FloatingText : MonoBehaviour
{
    [Tooltip("Duración total del popup (s)")]
    public float duration = 0.8f;

    [Tooltip("Velocidad vertical en píxeles/segundo")]
    public float speed = 40f;

    private float _t;
    private TMP_Text _txt;
    private CanvasGroup _cg;
    private RectTransform _rt;

    private void Awake()
    {
        _txt = GetComponent<TMP_Text>();
        _rt  = GetComponent<RectTransform>();

        // CanvasGroup para manejar la opacidad sin pelear con TMP
        _cg = gameObject.GetComponent<CanvasGroup>();
        if (_cg == null) _cg = gameObject.AddComponent<CanvasGroup>();
        _cg.alpha = 1f;
    }

    private void Update()
    {
        _t += Time.deltaTime;

        // Mover hacia arriba en el espacio de la UI
        if (_rt) _rt.anchoredPosition += Vector2.up * speed * Time.deltaTime;

        // Fade-out lineal
        if (_cg) _cg.alpha = 1f - Mathf.Clamp01(_t / duration);

        // Destruir al terminar
        if (_t >= duration) Destroy(gameObject);
    }

    /// Configura el texto y su color inicial.
    public void Set(string text, Color col)
    {
        if (_txt == null) _txt = GetComponent<TMP_Text>();
        _txt.text = text;
        _txt.color = col;
    }
}
