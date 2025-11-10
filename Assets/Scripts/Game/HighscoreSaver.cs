using UnityEngine;

/// Fachada mínima por si quieres llamar SaveScore desde otros lados.
/// En este proyecto el guardado se hace en GameManager.GameOver().
public class HighscoreSaver : MonoBehaviour
{
    public async void SaveScore(string name, int score)
    {
        await FirebaseService.WaitUntilReady();
        await ScoreRepository.AddAsync(name, score);
        Debug.Log("Score saved by HighscoreSaver.");
    }
}
