using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;

public static class ScoreRepository
{
    private static FirebaseFirestore DB =>
        (FirebaseService.Instance != null && FirebaseService.Instance.IsReady)
            ? FirebaseService.Instance.DB
            : FirebaseFirestore.DefaultInstance;

    private static CollectionReference Col => DB.Collection("highscores");

    // Guarda un único doc por jugador (sólo si mejora)
    public static async Task AddAsync(string name, int score)
    {
        await FirebaseService.WaitUntilReady();

        DocumentReference doc = Col.Document(name);
        var snap = await doc.GetSnapshotAsync();

        int current = (snap.Exists && snap.ContainsField("score"))
                      ? (int)snap.GetValue<long>("score")
                      : 0;

        if (score <= current) return;

        var data = new Dictionary<string, object>
        {
            { "name", name },
            { "score", score },
            { "updatedAt", FieldValue.ServerTimestamp }
        };

        await doc.SetAsync(data, SetOptions.Overwrite);
    }

    // Top N
    public static async Task<List<(string name, int score)>> GetTopAsync(int limit = 10)
    {
        await FirebaseService.WaitUntilReady();
        var list = new List<(string, int)>();

        var snap = await Col.OrderByDescending("score").Limit(limit).GetSnapshotAsync();
        foreach (var d in snap.Documents)
        {
            string n = d.GetValue<string>("name");
            int s = (int)d.GetValue<long>("score");
            list.Add((n, s));
        }
        return list;
    }

    // === NUEVO: lee el puntaje de un jugador ===
    public static async Task<(bool exists, int score)> GetPlayerScoreAsync(string name)
    {
        await FirebaseService.WaitUntilReady();
        var snap = await Col.Document(name).GetSnapshotAsync();
        if (!snap.Exists) return (false, 0);
        int s = (int)snap.GetValue<long>("score");
        return (true, s);
    }

    // === NUEVO: cuenta cuántos tienen score mayor (para calcular el rank) ===
    // Nota: para una tarea de clase está perfecto; en bases muy grandes usarías
    // agregaciones/count del SDK más nuevo.
    public static async Task<int> CountHigherThanAsync(int score)
    {
        await FirebaseService.WaitUntilReady();
        var q = Col.WhereGreaterThan("score", score);
        var snap = await q.GetSnapshotAsync();
        return snap.Count;
    }
}
