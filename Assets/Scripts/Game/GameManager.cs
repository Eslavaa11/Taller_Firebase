using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // TextMeshPro

/// Orquesta puntaje, analítica mínima y fin de partida.
/// Ahora incluye popups de "+5 pts" cuando recoges un "good".
public class GameManager : MonoBehaviour
{
    public static GameManager I;

    [Header("UI")]
    [SerializeField] private TMP_Text scoreText;           // Texto grande del score en HUD
    [SerializeField] private RectTransform popupRoot;      // Contenedor UI (RectTransform) donde van los popups
    [SerializeField] private TMP_Text popupPrefab;         // Prefab de TMP_Text para mostrar "+5 pts"

    [Header("Score & Stats")]
    public string PlayerName { get; private set; }
    public int Score { get; private set; }
    public int Caught { get; private set; }   // si quieres contar "good" recogidos
    public int Dodged { get; private set; }   // "bad" esquivados (salen por abajo)
    public int Attempts { get; private set; }

    private float _timeAlive;
    private bool _running = false;
    private bool _ended = false;              // evita doble GameOver

    private void Awake() { I = this; }

    private IEnumerator Start()
    {
        // Asegura que Firebase esté listo antes de posibles guardados
        yield return FirebaseService.WaitUntilReady();

        PlayerName = PlayerPrefs.GetString("player_name", "Player");
        Attempts   = PlayerPrefs.GetInt("attempts", 0) + 1;
        PlayerPrefs.SetInt("attempts", Attempts);

        _running = true;
        StartCoroutine(ScoreLoop());
    }

    private IEnumerator ScoreLoop()
    {
        while (_running)
        {
            Score += 1;               // +1 por segundo vivo
            _timeAlive += 1f;
            if (scoreText) scoreText.text = Score.ToString();
            yield return new WaitForSeconds(1f);
        }
    }

    /// Llamado cuando recoges un "good" (azul).
    /// Suma puntos, actualiza UI y muestra un popup en pantalla.
    public void OnGoodPickup(Vector3 worldPos, int pts = 5)
    {
        Caught++;                     // estadístico opcional
        Score += pts;                 // suma extra
        if (scoreText) scoreText.text = Score.ToString();
        SpawnPopup(worldPos, $"+{pts} pts", new Color(0.2f, 1f, 0.4f)); // verde
    }

    /// Llamado cuando esquivas (un "bad" sale por la parte baja).
    public void OnDodged()
    {
        Dodged++;
        Score += 5;                   // bonus por esquivar (si así lo deseas)
        if (scoreText) scoreText.text = Score.ToString();
    }

    /// Termina la partida (colisión con "bad").
    public async void GameOver()
    {
        if (_ended) return;           // anti-doble llamada
        _ended = true;

        _running = false;
        StopAllCoroutines();

        // Congelar controles para evitar movimientos posteriores
        var player = FindObjectOfType<PlayerController>();
        if (player) player.enabled = false;

        // Guarda score y analítica
        await ScoreRepository.AddAsync(PlayerName, Score);
        await AnalyticsRepository.AddSessionAsync(PlayerName, _timeAlive, Caught, Dodged, Attempts);

        PlayerPrefs.SetInt("last_score", Score);
        SceneManager.LoadScene("Results");
    }

    /// Instancia un popup en pantalla (sobre la posición del mundo del ítem).
    private void SpawnPopup(Vector3 worldPos, string text, Color color)
    {
        // Requiere: popupRoot (RectTransform) y popupPrefab (TMP_Text) asignados en el inspector.
        if (popupRoot == null || popupPrefab == null) return;

        // Convertir posición mundo → pantalla para colocar el popup a la altura del objeto
        Vector3 screen = Camera.main != null
            ? Camera.main.WorldToScreenPoint(worldPos)
            : worldPos;

        // Instanciar el TMP_Text y ubicarlo en la posición de pantalla
        TMP_Text t = Instantiate(popupPrefab, popupRoot);
        var rt = t.GetComponent<RectTransform>();
        rt.position = screen;
        t.alignment = TextAlignmentOptions.Center;
        t.fontSize = 28f;

        // Añadir o usar FloatingText para animar y autodestruir
        var ft = t.GetComponent<FloatingText>();
        if (ft == null) ft = t.gameObject.AddComponent<FloatingText>();
        ft.Set(text, color);
    }
}
