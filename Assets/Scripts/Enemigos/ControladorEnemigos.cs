using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControladorEnemigos : MonoBehaviour
{
    protected Transform _player;

    #region Datos del enemigo
    [Header("Datos de los enemigos")]
    public string nombre;
    public int nivel;
    public int vidaBase;
    public int da�oBase;
    public float velocidad;
    public float resistencia; //al empuje
    public int cantMaxExp, cantMinExp;
    #endregion

    #region prefab de experiencia y vida
    [Header("Experiencia")]
    public GameObject expPrefab;
    public GameObject vidaPrefab;
    public float expSpreadForce = 2.0f;
    #endregion
    
    [SerializeField]
    public float vidaActual;
    protected float da�oActual;
    private float velocidadRotacion = 5f;

    protected virtual void Awake()
    {
        vidaActual = vidaBase;
        da�oActual = da�oBase;
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
            _player = jugador.transform;
        
    }

    protected virtual void rotarHaciaJugador()
    {
        Vector3 direccion = (_player.position - transform.position).normalized;
        direccion.y = 0f;

        if (direccion != Vector3.zero)
        {
            Quaternion rotacionObjetivo = Quaternion.LookRotation(direccion);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotacionObjetivo,
                                                    velocidadRotacion * Time.deltaTime);
        }
    }

    public virtual void recibirDa�o(float cantidad)
    {
        vidaActual -= cantidad;
        if (vidaActual <= 0)
        {
            Morir();
        }
    }

    protected virtual void Morir()
    {
        int amountToDrop = UnityEngine.Random.Range(cantMinExp, cantMaxExp + 1);

        for (int i = 0; i < amountToDrop; i++)
        {
            GameObject exp = Instantiate(expPrefab, transform.position,Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));
            Rigidbody rb = exp.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector2 planarRandom = UnityEngine.Random.insideUnitCircle.normalized;
                Vector3 randomDir = new Vector3(planarRandom.x, 0f, planarRandom.y);
                rb.AddForce(randomDir * expSpreadForce, ForceMode.Impulse);
            }
        }

        bool dejarCura = UnityEngine.Random.value < 1 - (PlayerStats.Instance.Vida / PlayerStats.Instance.VidaMax);

        if (dejarCura)
        {
            int objVida = UnityEngine.Random.Range(1,3);
            for (int j = 0; j < objVida; j++ )
            {
                GameObject life = Instantiate(vidaPrefab, transform.position, Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));
                Rigidbody lifeRb = life.GetComponent<Rigidbody>();
                if (lifeRb != null)
                {
                    Vector2 planarRandom = UnityEngine.Random.insideUnitCircle.normalized;
                    Vector3 randomDir = new Vector3(planarRandom.x, 0f, planarRandom.y);
                    lifeRb.AddForce(randomDir * expSpreadForce, ForceMode.Impulse);
                }
            }
        }
        gameObject.SetActive(false);
    }

    public virtual void ActivarEnemigo(Vector3 nuevaPosicion)
    {
        transform.position = nuevaPosicion;
        vidaActual = vidaBase;
        gameObject.SetActive(true);
    }

    public virtual void ActivarEnemigo(Vector3 nuevaPosicion, int nivel)
    {
        transform.position = nuevaPosicion;
        vidaActual = Mathf.RoundToInt(vidaBase + (nivel + nivel /4));
        da�oActual = Mathf.RoundToInt(da�oBase + (da�oBase * (nivel/6)));
        gameObject.SetActive(true);
    }

    public void ajustarEstadisticas(float escala)
    {
        vidaActual = Mathf.RoundToInt(vidaBase * escala);
        da�oActual = Mathf.RoundToInt(da�oBase * escala);
    }
}
