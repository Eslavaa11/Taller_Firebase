using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class ResultsUI : MonoBehaviour
{
    [SerializeField] private TMP_Text lastScoreText;
    [SerializeField] private TMP_Text rankingText;

    private async void Start()
    {
        if (lastScoreText) lastScoreText.text = $"Your score: {PlayerPrefs.GetInt("last_score", 0)}";
        await FirebaseService.WaitUntilReady();
        await ShowTop10();
    }

private async Task ShowTop10()
{
    var meName  = PlayerPrefs.GetString("player_name", "Player");

    // 1) Trae Top-10
    var rows = await ScoreRepository.GetTopAsync(10);

    // 2) ¿Estoy en el Top-10?
    bool imInTop = false;
    foreach (var r in rows) if (r.name == meName) { imInTop = true; break; }

    // 3) Construir listado Top-10 (resaltando mi nombre si aparece)
    var sb = new System.Text.StringBuilder();
    int i = 1;
    foreach (var (name, score) in rows)
    {
        string line = $"{i}. {name} - {score}";
        if (name == meName)
            line = $"<b><color=#FFD166>{line}</color></b>";
        sb.AppendLine(line);
        i++;
    }

    // 4) Si NO estoy en Top-10, calculo mi RANK y lo agrego al final
    if (!imInTop)
    {
        var (exists, myBest) = await ScoreRepository.GetPlayerScoreAsync(meName);

        if (exists)
        {
            int higher = await ScoreRepository.CountHigherThanAsync(myBest);
            int myRank = higher + 1;

            string line = $"{myRank}. {meName} - {myBest}";
            line = $"<b><color=#FFD166>{line}</color></b>";  // resalta mi fila
            sb.AppendLine(line);
        }
        else
        {
            // Si aún no tiene documento (primera partida sin mejorar), muestro el de la sesión
            int sessionScore = PlayerPrefs.GetInt("last_score", 0);
            string line = $"—. {meName} - {sessionScore}";
            line = $"<b><color=#FFD166>{line}</color></b>";
            sb.AppendLine(line);
        }
    }

    if (rankingText) rankingText.text = sb.ToString();
}



    public void OnRetry() => SceneManager.LoadScene("Game");
    public void OnHome()  => SceneManager.LoadScene("Start");
}
