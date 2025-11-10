using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;   // para Button
using TMPro;            // para TMP

public class StartUI : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button playButton;
    [SerializeField] private TMP_Text warningText; // opcional (ej: "Write your name")

    private const int MaxLen = 16;

    private void Awake()
    {
        // Estado inicial
        ValidateName(nameInput != null ? nameInput.text : "");

        // Revalida cada vez que se escribe
        if (nameInput != null)
            nameInput.onValueChanged.AddListener(ValidateName);
    }

    private void Update()
    {
        // Permitir Enter solo si es válido
        if (Input.GetKeyDown(KeyCode.Return) && IsValid(Current()))
            OnPlay();
    }

    public void OnPlay()
    {
        string n = Current();

        // Doble validación por seguridad
        if (!IsValid(n))
        {
            // feedback visual
            if (warningText) warningText.gameObject.SetActive(true);
            if (playButton) playButton.interactable = false;
            return;
        }

        PlayerPrefs.SetString("player_name", n);
        SceneManager.LoadScene("Game");
    }

    // ===== helpers =====
    private string Current()
    {
        string raw = (nameInput != null) ? nameInput.text : "";
        raw = string.IsNullOrWhiteSpace(raw) ? "" : raw.Trim();
        if (raw.Length > MaxLen) raw = raw.Substring(0, MaxLen);
        return raw;
    }

    private bool IsValid(string s) => !string.IsNullOrEmpty(s) && s.Length >= 2;

    private void ValidateName(string _)
    {
        string n = Current();
        bool ok = IsValid(n);

        if (playButton) playButton.interactable = ok;
        if (warningText) warningText.gameObject.SetActive(!ok);
    }
}
