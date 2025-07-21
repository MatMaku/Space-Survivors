using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

[Serializable]
public class EnemyConfig
{
    public GameObject enemyPrefab;
    public int minSpawn, maxSpawn;
    public int maximunAmount;
    public bool enableNow = true;
}

public enum spawnState
{
    Normal,
    EnPausa,
    TestingEnemigos,
    TestingBosses
}

public class SpawnerEnemies : MonoBehaviour
{
    public List<EnemyConfig> enemies = new List<EnemyConfig>();
    private List<GameObject> enemiesInGame = new List<GameObject>();

    public spawnState estado;

    #region Control de Distancia Recorrida
    [Header("Zonas de Spawn")]
    public List<SpawnAreaController> spawnZones;

    [Header("Control de distancia")]
    public TextMeshProUGUI DistanciaUI;
    public float distanciaRecorrida = 0f;
    public float velocidad = 10f;
    #endregion

    #region Control de Fases del juego
    [Header("Control de Fase")]
    public float finDeFase;
    [SerializeField] private float distanciaEntreFases; //Vuelve a 0 cada vez que empieza una nueva fase
    [Range(0,10)]
    public int minDuracionFase, maxDuracionFase; //Lo que va a durar cada fase
    public int minLevel, maxLevel; //Se tienen que actualizar con cada fase
    private int indexEnemies;
    public float spawnTime;
    private Coroutine spawnRoutine;
    #endregion

    #region Control Oleadas Sorpresas y Jefes
    [Header("Oleada Sorpresa")]
    public float waveProbability = 0.1f;
    public int minWaveAmount, maxWaveAmount;

    [Header("Control spawn Jefe")]
    public float bossSpawnDistance; //La distancia en la que se activa el boss
    [SerializeField]private float bossDistanceCounter; //Cuenta la distancia que se recorre antes del boss
    public int bossLevel; //Se tiene que actualizar despues de cada boss con el numberOfBossesEliminated 
    public int numberOfBossesEliminated; // Cantidad de jefes eliminados
    #endregion
   
    private void Awake()
    {
        Time.timeScale = 1f;
        preloadEnemies();
    }

    private void Start()
    {
        finDeFase = 1000f;
        distanciaEntreFases = 0f;
        indexEnemies = 0;
        changeEnemies();
        spawnRoutine = StartCoroutine(spawnEnemiesRoutine());
        bossDistanceCounter = 0f;
        numberOfBossesEliminated = 0;
        bossLevel = (numberOfBossesEliminated + 1) * 10;
    }

    private void Update()
    {
        float avanceEfectuado = velocidad * Time.deltaTime;
        distanciaRecorrida += avanceEfectuado;
        DistanciaUI.text = Mathf.RoundToInt(distanciaRecorrida).ToString() + "KM";
        
        if (estado == spawnState.Normal)
        {
            distanciaEntreFases += avanceEfectuado;
            bossDistanceCounter += avanceEfectuado;
            if (distanciaEntreFases >= finDeFase)
            {
                distanciaEntreFases = 0;
                finDeFase = UnityEngine.Random.Range(minDuracionFase, maxDuracionFase) * 100;
                minLevel += 3;
                maxLevel += 3;
                if (maxLevel > 50)
                {
                    maxLevel = 50;
                    minLevel = 50 - 5;
                }
                if (indexEnemies < enemies.Count - 1)
                    indexEnemies++;

                changeEnemies();
            }
        }else if (estado == spawnState.TestingBosses)
        {
            bossDistanceCounter += avanceEfectuado;
        }
    }

    private void preloadEnemies()
    {
        foreach (EnemyConfig datoEnemigo in enemies)
        {
            for (int i = 0; i < datoEnemigo.maximunAmount; i++)
            {
                GameObject newEnemy = Instantiate(datoEnemigo.enemyPrefab, new Vector3(0,-25,0), Quaternion.identity);
                newEnemy.transform.parent = transform;
                newEnemy.SetActive(false);
                enemiesInGame.Add(newEnemy);
            }
        }
    }

    IEnumerator spawnEnemiesRoutine()
    {
        while (true)
        {
            spawnerController();
            yield return new WaitForSeconds(spawnTime);
        }
    }

    private void spawnerController()
    {
        if (estado == spawnState.Normal || estado == spawnState.TestingEnemigos)
        {
            bool esOleadaSorpresa = UnityEngine.Random.value < waveProbability;
            SpawnAreaController spawnArea = spawnZones[UnityEngine.Random.Range(0, spawnZones.Count)];
            var posibleEnemigos = enemies.Where(e => e.enableNow).ToList();
            if (esOleadaSorpresa)
            {
                int cantidadTotal = UnityEngine.Random.Range(minWaveAmount, maxWaveAmount + 1);
                List<Vector3> posiciones = spawnArea.recibirPuntosDeSpawn(cantidadTotal);

                for (int i = 0; i < posiciones.Count; i++)
                {
                    EnemyConfig selectedEnemy = posibleEnemigos[UnityEngine.Random.Range(0, posibleEnemigos.Count)];
                    string selectedName = selectedEnemy.enemyPrefab.GetComponent<ControladorEnemigos>().nombre;
                    GameObject objetoElegido = enemiesInGame.FirstOrDefault(x =>
                                                !x.activeInHierarchy &&
                                                x.GetComponent<ControladorEnemigos>().nombre == selectedName);
                    if (objetoElegido != null)
                    {
                        var controlador = objetoElegido.GetComponent<ControladorEnemigos>();
                        Vector3 posicion = posiciones[i];
                        int nuevoNivel = UnityEngine.Random.Range(minLevel, maxLevel);
                        controlador.ActivarEnemigo(posicion, nuevoNivel);
                    }
                    else
                    {
                        Debug.Log("No encontro enemigos para spawnear");
                    }
                }
            }
            else
            {
                EnemyConfig selectedEnemy = posibleEnemigos[UnityEngine.Random.Range(0, posibleEnemigos.Count)];
                int cantidad = UnityEngine.Random.Range(selectedEnemy.minSpawn, selectedEnemy.maxSpawn + 1);
                List<Vector3> posiciones = spawnArea.recibirPuntosDeSpawn(cantidad);
                string enemyName = selectedEnemy.enemyPrefab.GetComponent<ControladorEnemigos>().nombre;
                for (int i = 0; i < posiciones.Count; i++)
                {
                    GameObject objetoElegido = enemiesInGame.FirstOrDefault(x =>
                                                !x.activeInHierarchy &&
                                                x.GetComponent<ControladorEnemigos>().nombre == enemyName);
                    if (objetoElegido != null)
                    {
                        var controlador = objetoElegido.GetComponent<ControladorEnemigos>();
                        int nuevoNivel = UnityEngine.Random.Range(minLevel, maxLevel);
                        controlador.ActivarEnemigo(posiciones[i], nuevoNivel);
                    }
                    else
                    {
                        Debug.Log("No encontro enemigos para spawnear");
                    }
                }
            }
        }
        if (estado == spawnState.Normal || estado == spawnState.TestingBosses)
        {
            if (bossDistanceCounter > bossSpawnDistance)
            {
                bossDistanceCounter = 0;
                //función para spawnear al boss 
            }
        }
    }

    private void changeEnemies()
    {
        foreach (EnemyConfig enemydata in enemies)
        {
            enemydata.enableNow = false;
        }

        int cantidad = UnityEngine.Random.Range(1,indexEnemies + 1);
        for (int i = 0; i < cantidad; i++)
        {
            EnemyConfig nextEnemy;

            do
            {
                nextEnemy = enemies[UnityEngine.Random.Range(0, indexEnemies)];
            } while (nextEnemy.enableNow);

            nextEnemy.enableNow = true;
        }
    }
}
