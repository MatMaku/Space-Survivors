using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

[Serializable]
public class spawnIntervals
{
    public float inicio;
    public float fin;
}

[Serializable]
public class EnemyConfig
{
    public GameObject enemyPrefab;
    public int minSpawn, maxSpawn;
    public int maximunAmount;
    public bool enableNow = true;
    public List<spawnIntervals> intervalos;
}

[Serializable]
public class gameEvents
{
    public float cuandoOcurre;
    public bool ocurrio;
    public bool hayOleada;
    public int waveAmount;
    public bool ApareceElBoss;
}

public class SpawnerEnemies : MonoBehaviour
{
    public List<EnemyConfig> enemies = new List<EnemyConfig>();
    private List<GameObject> enemiesInGame = new List<GameObject>();
    public List<gameEvents> eventos = new List<gameEvents>();

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
    public float finDeFase = 1000;
    [SerializeField] private float distanciaEntreFases; 
    [Range(0,10)]
    public int minDuracionFase, maxDuracionFase;
    public int level = 1;
    public float spawnTime;
    private Coroutine spawnRoutine;
    #endregion

    #region Control Jefes
    [Header("Control spawn Jefe")]
    public GameObject bossPrefab;
    public int bossLevel;  
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
        changeEnemies();
        spawnRoutine = StartCoroutine(spawnEnemiesRoutine());
    }

    private void Update()
    {
        float avanceEfectuado = velocidad * Time.deltaTime;
        distanciaRecorrida += avanceEfectuado;
        DistanciaUI.text = Mathf.RoundToInt(distanciaRecorrida).ToString() + "KM";

        distanciaEntreFases += avanceEfectuado;
        if (distanciaEntreFases >= finDeFase)
        {
            distanciaEntreFases = 0;
            level += 1; 
        }
        changeEnemies();
        checkEvents();
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
        SpawnAreaController spawnArea = spawnZones[UnityEngine.Random.Range(0, spawnZones.Count)];
        
        var posibleEnemigos = enemies.Where(e => e.enableNow).ToList();
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
                int nuevoNivel = level;
                controlador.ActivarEnemigo(posiciones[i], nuevoNivel);
            }
            else
            {
                Debug.Log("No encontro enemigos para spawnear");
            }

        }
    }

    private void changeEnemies()
    {
        foreach (EnemyConfig enemy in enemies) 
        {
            for (int i = 0; i < enemy.intervalos.Count; i++)
            {
                if (distanciaRecorrida >= enemy.intervalos[i].inicio && distanciaRecorrida < enemy.intervalos[i].fin)
                {
                    enemy.enableNow = true;
                    break;
                }
                enemy.enableNow = false;
            }
        }  
    }

    private void checkEvents()
    {
        foreach (gameEvents e in eventos)
        {
            if (distanciaRecorrida >= e.cuandoOcurre && !e.ocurrio)
            {
                producirEvento(e);
                break;
            }
        }
    }

    private void producirEvento(gameEvents ge)
    {
        ge.ocurrio = true;
        if (ge.hayOleada)
        {
            generarOleada(ge.waveAmount);
        }else if (ge.ApareceElBoss)
        {
            SpawnAreaController spawnArea = spawnZones[UnityEngine.Random.Range(0, spawnZones.Count)];
            List<Vector3> posicion = spawnArea.recibirPuntosDeSpawn(1);
            GameObject boss = Instantiate(bossPrefab, posicion[0], Quaternion.identity);
            ControladorEnemigos bossControl = boss.GetComponent<ControladorEnemigos>();
            bossControl.ActivarEnemigo(posicion[0], bossLevel);
        }
    }

    private void generarOleada(int amount)
    {
        SpawnAreaController spawnArea = spawnZones[UnityEngine.Random.Range(0, spawnZones.Count)];
        List<Vector3> posiciones = spawnArea.recibirPuntosDeSpawn(amount);

        var posibleEnemigos = enemies.Where(e => e.enableNow).ToList();

        for (int i = 0; i < posiciones.Count; i++)
        {
            EnemyConfig selectedEnemy = posibleEnemigos[UnityEngine.Random.Range(0, posibleEnemigos.Count)];
            string enemyName = selectedEnemy.enemyPrefab.GetComponent<ControladorEnemigos>().nombre;
            GameObject objetoElegido = enemiesInGame.FirstOrDefault(x =>
                                        !x.activeInHierarchy &&
                                        x.GetComponent<ControladorEnemigos>().nombre == enemyName);
            if (objetoElegido != null)
            {
                var controlador = objetoElegido.GetComponent<ControladorEnemigos>();
                int nuevoNivel = level;
                controlador.ActivarEnemigo(posiciones[i], nuevoNivel);
            }
            else
            {
                Debug.Log("No encontro enemigos para spawnear");
            }
        }
    }
}
