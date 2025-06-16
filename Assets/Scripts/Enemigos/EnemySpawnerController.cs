using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public class SpawnPhase
{
    public float spawnInterval;
    public float endPhase;
    public List<GameObject> enemies;
}

[Serializable]
public class BossSpawn
{
    public GameObject prefabBoss;
    public float distanciaAparicion;
}

public class tipoEnemigo
{
    public string nombre;
    public int max, min;

    public tipoEnemigo(string name, int ma, int mi)
    {
        nombre = name;
        max = ma;
        min = mi;
    }
}

public class EnemySpawnerController : MonoBehaviour
{
    [Header("Enemy spawn")]
    public List<SpawnPhase> spawnPhases;
    private float endCurrentPhase;
    private int indexPhase;
    private Coroutine spawnRoutine;
    public List<BossSpawn> bosses;
    private BossSpawn nextBoss;
    private int bossesIndex;

    private List<GameObject> enemiesInGame = new List<GameObject>();
    private List<tipoEnemigo> enemigosPermitidos = new List<tipoEnemigo>();

    [Header("Zonas de spawn")]
    public List<SpawnAreaController> spawnZones;

    [Header("Distance Tracker")]
    public TextMeshProUGUI DistanciaUI;
    public float distanciaRecorrida = 0f;
    public float velocidad = 10f;

    private void Awake()
    {
        Time.timeScale = 1f;
        generarEnemigos();
    }

    private void Start()
    {
        indexPhase = 0;
        bossesIndex = 0;
        agregarMasEnemigos();
        spawnRoutine = StartCoroutine(SpawnEnemiesRoutine());
    }

    private void Update()
    {
        distanciaRecorrida += velocidad * Time.deltaTime;
        DistanciaUI.text = Mathf.RoundToInt(distanciaRecorrida).ToString() + "KM";

        if (indexPhase < spawnPhases.Count-1 &&
            distanciaRecorrida > endCurrentPhase)
        {
            indexPhase++;
            agregarMasEnemigos();
        }

        if (bossesIndex < bosses.Count && distanciaRecorrida > bosses[bossesIndex].distanciaAparicion)
        {
            Instantiate(bosses[bossesIndex].prefabBoss, Vector3.zero, Quaternion.identity);
            if (bossesIndex < bosses.Count)
            {
                bossesIndex++;
            }

        }
    }

    private void generarEnemigos()
    {
        foreach (var phase in spawnPhases)
        {
            foreach (var tipeEnemy in phase.enemies)
            {
                for (int i = 1; i <= 50; i++)
                {
                    GameObject newEnemy = Instantiate(tipeEnemy, 
                                                    Vector3.zero + new Vector3(0, -25, 0),
                                                    Quaternion.identity);
                    newEnemy.transform.parent = transform;
                    enemiesInGame.Add(newEnemy);
                    newEnemy.SetActive(false);
                }
            }
        }
    }

    private void agregarMasEnemigos()
    {
        foreach (var enemy in spawnPhases[indexPhase].enemies)
        {
            string nombreEnemigo = enemy.GetComponent<ControladorEnemigos>().nombre;
            int max = enemy.GetComponent<ControladorEnemigos>().cantMaxSpawn;
            int min = enemy.GetComponent<ControladorEnemigos>().cantMinSpawn;
            tipoEnemigo nuevoEnemigo = new tipoEnemigo(nombreEnemigo,max,min);
            if (!enemigosPermitidos.Contains(nuevoEnemigo))
            {
                enemigosPermitidos.Add(nuevoEnemigo);
            }
        }
        endCurrentPhase = spawnPhases[indexPhase].endPhase;
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
        SpawnAreaController spawnArea = spawnZones[UnityEngine.Random.Range(0, spawnZones.Count)];

        tipoEnemigo selectedEnemy = enemigosPermitidos[UnityEngine.Random.Range(0, enemigosPermitidos.Count)];

        int cantidad = UnityEngine.Random.Range(selectedEnemy.min, selectedEnemy.max);

        List<Vector3> posiciones = spawnArea.recibirPuntosDeSpawn(cantidad);

        for (int i = 0; i < cantidad; i++)
        {
            GameObject objetoElegido = enemiesInGame.FirstOrDefault(x => !x.gameObject.activeInHierarchy &&
                                        x.GetComponent<ControladorEnemigos>().nombre == selectedEnemy.nombre);
            if (objetoElegido != null)
            {
                objetoElegido.GetComponent<ControladorEnemigos>().ActivarEnemigo(posiciones[i]);
            }
            else
            {
                Debug.Log("No hay más enemigos para spawnear");
                return;
            }
        }
    }
}
