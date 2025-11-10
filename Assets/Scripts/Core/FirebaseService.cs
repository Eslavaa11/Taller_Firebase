using System.Threading.Tasks;
using Firebase;
using Firebase.Extensions;
using Firebase.Firestore;
using UnityEngine;

public class FirebaseService : MonoBehaviour
{
    public static FirebaseService Instance { get; private set; }
    public FirebaseFirestore DB { get; private set; }
    public bool IsReady { get; private set; }

    // ⬇️ Crea el objeto automáticamente al cargar el juego si no existe
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (Instance == null && FindObjectOfType<FirebaseService>() == null)
        {
            var go = new GameObject("FirebaseService");
            go.AddComponent<FirebaseService>();
        }
    }

    private async void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        await InitAsync();
    }

    private async Task InitAsync()
    {
        var status = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (status == DependencyStatus.Available)
        {
            DB = FirebaseFirestore.DefaultInstance;
            IsReady = true;
            Debug.Log("[Firebase] Ready");
        }
        else
        {
            Debug.LogError($"[Firebase] Dependencies: {status}");
        }
    }

    public static async Task WaitUntilReady()
    {
        // Evita espera infinita: si no hay Instance, créala ya.
        if (Instance == null) Bootstrap();
        while (Instance == null || !Instance.IsReady)
            await Task.Yield();
    }
}
