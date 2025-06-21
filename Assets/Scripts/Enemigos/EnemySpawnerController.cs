using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public class SpawnEnemyConfig
{
    public GameObject enemyPrefab;
    public bool enabledInPhase = true;
    public int minSpawn = 1;
    public int maxSpawn = 3;
    public int spawnMaximo = 10;
}

[Serializable]
public class SpawnPhase
{
    public float spawnInterval;
    public float endPhase;
    public List<SpawnEnemyConfig> enemies;
}

public class EnemySpawnerController : MonoBehaviour
{
    [Header("Enemy spawn")]
    public List<SpawnPhase> spawnPhases;
    private float endCurrentPhase;
    private int indexPhase;
    private Coroutine spawnRoutine;

    private List<GameObject> enemiesInGame = new List<GameObject>();

    [Header("Zonas de spawn")]
    public List<SpawnAreaController> spawnZones;

    [Header("Distance Tracker")]
    public TextMeshProUGUI DistanciaUI;
    public float distanciaRecorrida = 0f;
    public float velocidad = 10f;

    private void Awake()
    {
        Time.timeScale = 1f;
        PreloadEnemies();
    }

    private void Start()
    {
        indexPhase = 0;
        endCurrentPhase = spawnPhases[indexPhase].endPhase;
        spawnRoutine = StartCoroutine(SpawnEnemiesRoutine());
    }

    private void Update()
    {
        distanciaRecorrida += velocidad * Time.deltaTime;
        DistanciaUI.text = Mathf.RoundToInt(distanciaRecorrida).ToString() + "KM";

        if (indexPhase < spawnPhases.Count - 1 && distanciaRecorrida > endCurrentPhase)
        {
            indexPhase++;
            endCurrentPhase = spawnPhases[indexPhase].endPhase;
        }
    }

    private void PreloadEnemies()
    {
        HashSet<GameObject> uniquePrefabs = new HashSet<GameObject>();

        foreach (var phase in spawnPhases)
        {
            foreach (var enemyConfig in phase.enemies)
            {
                if (!uniquePrefabs.Contains(enemyConfig.enemyPrefab))
                {
                    uniquePrefabs.Add(enemyConfig.enemyPrefab);
                    for (int i = 0; i < enemyConfig.spawnMaximo; i++)
                    {
                        GameObject newEnemy = Instantiate(enemyConfig.enemyPrefab, new Vector3(0, -25, 0), Quaternion.identity);
                        newEnemy.transform.parent = transform;
                        newEnemy.SetActive(false);
                        enemiesInGame.Add(newEnemy);
                    }
                }
            }
        }
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        while (true)
        {
            TrySpawnEnemy();
            yield return new WaitForSeconds(spawnPhases[indexPhase].spawnInterval);
        }
    }

    private void TrySpawnEnemy()
    {
        var activeEnemies = spawnPhases[indexPhase].enemies.Where(e => e.enabledInPhase).ToList();
        if (activeEnemies.Count == 0) return;

        SpawnAreaController spawnArea = spawnZones[UnityEngine.Random.Range(0, spawnZones.Count)];
        SpawnEnemyConfig selectedEnemy = activeEnemies[UnityEngine.Random.Range(0, activeEnemies.Count)];

        int cantidad = UnityEngine.Random.Range(selectedEnemy.minSpawn, selectedEnemy.maxSpawn + 1);
        List<Vector3> posiciones = spawnArea.recibirPuntosDeSpawn(cantidad);

        for (int i = 0; i < cantidad; i++)
        {
            GameObject objetoElegido = enemiesInGame.FirstOrDefault(x =>
                !x.activeInHierarchy &&
                x.name.Contains(selectedEnemy.enemyPrefab.name));

            if (objetoElegido != null)
            {
                objetoElegido.GetComponent<ControladorEnemigos>().ActivarEnemigo(posiciones[i]);
            }
            else
            {
                Debug.LogWarning("No hay más instancias disponibles para: " + selectedEnemy.enemyPrefab.name);
            }
        }
    }
}

