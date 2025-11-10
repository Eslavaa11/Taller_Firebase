using UnityEngine;
using System.Collections;

/// Spawnea muchos más "Bad" que "Good" y acelera con el tiempo.
/// Además, a veces lanza "ráfagas" de Bad para subir dificultad.
public class Spawner : MonoBehaviour
{
    [Header("Prefabs")]
    public GameObject goodPrefab;
    public GameObject badPrefab;

    [Header("Frecuencia")]
    [Tooltip("Tiempo entre spawns al comenzar (seg)")]
    public float rate = 0.9f;
    [Tooltip("Límite inferior de intervalo (más rápido no baja)")]
    public float minRate = 0.30f;
    [Tooltip("Cada 5s: rate *= rampFactor (ej: 0.92 = 8% más rápido)")]
    public float rampFactor = 0.92f;

    [Header("Probabilidades")]
    [Range(0f, 1f)]
    [Tooltip("Probabilidad de spawnear un GOOD. (El resto será BAD)")]
    public float goodChance = 0.18f; // 18% good → 82% bad

    [Header("Ráfagas de BAD")]
    [Range(0f, 1f)]
    [Tooltip("Posibilidad por tick de soltar una ráfaga extra de BADs")]
    public float badBurstChance = 0.35f;   // 35% de ticks tienen ráfaga
    [Tooltip("Cuántos BADs adicionales (además del spawn normal)")]
    public Vector2Int badBurstCount = new Vector2Int(1, 3); // entre 1 y 3 extra

    [Header("Rango horizontal")]
    public float xRange = 3.5f;

    private void Start()
    {
        InvokeRepeating(nameof(SpawnTick), 0.5f, rate);
        StartCoroutine(RampDifficulty());
    }

    private IEnumerator RampDifficulty()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);
            rate = Mathf.Max(minRate, rate * rampFactor);
            CancelInvoke(nameof(SpawnTick));
            InvokeRepeating(nameof(SpawnTick), rate, rate);
        }
    }

    private void SpawnTick()
    {
        // 1) Spawn normal (según goodChance)
        SpawnOne();

        // 2) Ráfaga de BAD (opcional)
        if (Random.value < badBurstChance)
        {
            int extra = Random.Range(badBurstCount.x, badBurstCount.y + 1);
            for (int i = 0; i < extra; i++)
                SpawnBad();
        }
    }

    private void SpawnOne()
    {
        bool spawnGood = Random.value < goodChance;
        if (spawnGood) SpawnGood();
        else          SpawnBad();
    }

    private void SpawnGood()
    {
        var pos = new Vector3(Random.Range(-xRange, xRange), 6f, 0f);
        Instantiate(goodPrefab, pos, Quaternion.identity);
    }

    private void SpawnBad()
    {
        var pos = new Vector3(Random.Range(-xRange, xRange), 6f, 0f);
        Instantiate(badPrefab, pos, Quaternion.identity);
    }
}
