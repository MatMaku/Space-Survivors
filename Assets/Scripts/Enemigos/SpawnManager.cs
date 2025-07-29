using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public enum EventType
{
    SpawnWave,
    SpawnBoss,
    EnableEnemies,
    DisableEnemies,
    ChangeSpawnRate    // nuevo tipo de evento
}

[Serializable]
public class SpawnRateOverride
{
    public GameObject prefab;         // el prefab cuyo rate vamos a cambiar
    public float newInterval;         // nuevo intervalo de spawn (segundos)
}

[Serializable]
public class EventAction
{
    public EventType actionType;

    [Header("Para SpawnWave")]
    public int spawnCount;
    public List<GameObject> spawnPrefabs;

    [Header("Para SpawnBoss")]
    public GameObject bossPrefab;

    [Header("Para Enable/Disable Prefabs")]
    public List<GameObject> prefabsToToggle;

    [Header("Para ChangeSpawnRate")]
    public List<SpawnRateOverride> rateOverrides;
}

[Serializable]
public class GameEvent
{
    [Tooltip("Distancia recorrida al disparar este evento")]
    public float triggerDistance;

    [HideInInspector]
    public bool triggered = false;

    public List<EventAction> actions = new List<EventAction>();
}

public class SpawnManager : MonoBehaviour
{
    [Header("Lista de Prefabs de Enemigos")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();

    [Header("Pooling")]
    public int defaultPoolSize = 10;
    private Dictionary<string, Queue<GameObject>> pool = new Dictionary<string, Queue<GameObject>>();

    [Header("Zonas de Spawn")]
    public List<SpawnAreaController> spawnZones = new List<SpawnAreaController>();

    [Header("Eventos")]
    public List<GameEvent> events = new List<GameEvent>();

    [Header("Distancia y Nivel")]
    public float distanciaRecorrida = 0f;
    public float velocidad = 10f;
    public float distancePerLevel = 100f;
    public TextMeshProUGUI distanciaUI;
    public int level = 1;

    [Header("Spawn Continuo Por Enemigo")]
    public bool useIndividualSpawn = true;
    public float defaultSpawnInterval = 2f;

    // control interno de coroutines e intervalos
    private Dictionary<GameObject, float> spawnIntervals = new Dictionary<GameObject, float>();
    private Dictionary<GameObject, Coroutine> spawnCoroutines = new Dictionary<GameObject, Coroutine>();
    private List<GameObject> activeEnemyPrefabs;

    private void Awake()
    {
        Time.timeScale = 1f;
        InitializePool();

        // inicializamos intervalos y estado activo
        activeEnemyPrefabs = new List<GameObject>(enemyPrefabs);
        foreach (var prefab in enemyPrefabs)
        {
            spawnIntervals[prefab] = defaultSpawnInterval;
            if (useIndividualSpawn)
                spawnCoroutines[prefab] = StartCoroutine(SpawnRoutine(prefab));
        }
    }

    private void Update()
    {
        AdvanceDistance();
        HandleEvents();
    }

    // --- POOLING ------------------------------------------------------------
    private void InitializePool()
    {
        foreach (var prefab in enemyPrefabs)
        {
            var q = new Queue<GameObject>();
            for (int i = 0; i < defaultPoolSize; i++)
            {
                var go = Instantiate(prefab, transform);
                go.SetActive(false);
                q.Enqueue(go);
            }
            pool[prefab.name] = q;
        }
    }

    private GameObject GetPooledOrInstantiate(GameObject prefab)
    {
        if (pool.TryGetValue(prefab.name, out var q) && q.Count > 0)
            return q.Dequeue();

        // pool vacío: instanciamos de todas formas
        var go = Instantiate(prefab, transform);
        go.SetActive(false);
        return go;
    }

    public void ReturnToPool(GameObject go)
    {
        go.SetActive(false);
        if (pool.ContainsKey(go.name))
            pool[go.name].Enqueue(go);
        else
            Destroy(go);
    }

    // --- DISTANCIA Y NIVEL -------------------------------------------------
    private void AdvanceDistance()
    {
        distanciaRecorrida += velocidad * Time.deltaTime;
        level = Mathf.FloorToInt(distanciaRecorrida / distancePerLevel) + 1;

        if (distanciaUI != null)
            distanciaUI.text = $"{Mathf.RoundToInt(distanciaRecorrida)} KM";
    }

    // --- SPAWN ROUTINE POR PREFAB ------------------------------------------
    private IEnumerator SpawnRoutine(GameObject prefab)
    {
        while (true)
        {
            // si ya no está activo, detenemos esta rutina
            if (!activeEnemyPrefabs.Contains(prefab))
                yield break;

            yield return new WaitForSeconds(spawnIntervals[prefab]);
            SpawnOne(prefab);
        }
    }

    private void SpawnOne(GameObject prefab)
    {
        if (spawnZones.Count == 0) return;
        var zone = spawnZones[UnityEngine.Random.Range(0, spawnZones.Count)];
        var pos = zone.recibirPuntosDeSpawn(1)[0];

        var go = GetPooledOrInstantiate(prefab);
        go.transform.position = pos;
        go.SetActive(true);
        go.GetComponent<ControladorEnemigos>()
          .ActivarEnemigo(pos, level);
    }

    // --- GESTIÓN DE EVENTOS ------------------------------------------------
    private void HandleEvents()
    {
        foreach (var ev in events.Where(e => !e.triggered && distanciaRecorrida >= e.triggerDistance))
        {
            ev.triggered = true;
            foreach (var act in ev.actions)
                ExecuteAction(act);
        }
    }

    private void ExecuteAction(EventAction act)
    {
        switch (act.actionType)
        {
            case EventType.SpawnWave:
                SpawnWave(act.spawnCount, act.spawnPrefabs);
                break;

            case EventType.SpawnBoss:
                if (act.bossPrefab != null)
                    SpawnBoss(act.bossPrefab);
                break;

            case EventType.EnableEnemies:
                TogglePrefabs(act.prefabsToToggle, true);
                break;

            case EventType.DisableEnemies:
                TogglePrefabs(act.prefabsToToggle, false);
                break;

            case EventType.ChangeSpawnRate:
                foreach (var o in act.rateOverrides)
                {
                    if (spawnIntervals.ContainsKey(o.prefab))
                    {
                        spawnIntervals[o.prefab] = o.newInterval;
                        RestartSpawnRoutine(o.prefab);
                    }
                }
                break;
        }
    }

    // oleada puntual: ignora rates, spawnea X de cada uno
    private void SpawnWave(int count, List<GameObject> prefabs)
    {
        if (count <= 0 || spawnZones.Count == 0) return;

        var zone = spawnZones[UnityEngine.Random.Range(0, spawnZones.Count)];
        var points = zone.recibirPuntosDeSpawn(count);

        var source = (prefabs != null && prefabs.Count > 0)
            ? prefabs
            : activeEnemyPrefabs;

        for (int i = 0; i < points.Count; i++)
        {
            var prefab = source[UnityEngine.Random.Range(0, source.Count)];
            var go = GetPooledOrInstantiate(prefab);
            go.transform.position = points[i];
            go.SetActive(true);
            go.GetComponent<ControladorEnemigos>()
              .ActivarEnemigo(points[i], level);
        }
    }

    private void SpawnBoss(GameObject bossPrefab)
    {
        if (spawnZones.Count == 0) return;
        var zone = spawnZones[UnityEngine.Random.Range(0, spawnZones.Count)];
        var pos = zone.recibirPuntosDeSpawn(1)[0];
        var boss = Instantiate(bossPrefab, pos, Quaternion.identity);
        boss.GetComponent<ControladorEnemigos>()
            .ActivarEnemigo(pos, level);
    }

    private void TogglePrefabs(List<GameObject> list, bool enable)
    {
        if (list == null) return;

        foreach (var prefab in list)
        {
            if (enable)
            {
                if (!activeEnemyPrefabs.Contains(prefab) && enemyPrefabs.Contains(prefab))
                {
                    activeEnemyPrefabs.Add(prefab);
                    if (useIndividualSpawn && !spawnCoroutines.ContainsKey(prefab))
                        spawnCoroutines[prefab] = StartCoroutine(SpawnRoutine(prefab));
                }
            }
            else
            {
                if (activeEnemyPrefabs.Remove(prefab))
                    StopSpawnRoutine(prefab);
            }
        }
    }

    private void RestartSpawnRoutine(GameObject prefab)
    {
        StopSpawnRoutine(prefab);
        if (useIndividualSpawn && activeEnemyPrefabs.Contains(prefab))
            spawnCoroutines[prefab] = StartCoroutine(SpawnRoutine(prefab));
    }

    private void StopSpawnRoutine(GameObject prefab)
    {
        if (spawnCoroutines.TryGetValue(prefab, out var co))
        {
            StopCoroutine(co);
            spawnCoroutines.Remove(prefab);
        }
    }
}