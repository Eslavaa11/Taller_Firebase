using System.Collections.Generic;
using System.Threading.Tasks;
using Firebase.Firestore;

public static class AnalyticsRepository
{
    private static FirebaseFirestore DB =>
        (FirebaseService.Instance != null && FirebaseService.Instance.IsReady)
            ? FirebaseService.Instance.DB
            : FirebaseFirestore.DefaultInstance;

    private static CollectionReference Col => DB.Collection("sessions");

    public static async Task AddSessionAsync(string name, float totalTime, int caught, int dodged, int attempts)
    {
        await FirebaseService.WaitUntilReady();

        var data = new Dictionary<string, object>
        {
            { "name", name },
            { "totalTime", totalTime },
            { "caught", caught },
            { "dodged", dodged },
            { "attempts", attempts },
            { "endedAt", FieldValue.ServerTimestamp } // fecha del servidor
        };

        await Col.AddAsync(data);
    }
}
