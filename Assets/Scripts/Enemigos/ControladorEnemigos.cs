using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ControladorEnemigos : MonoBehaviour
{
    protected Transform _player;

    #region Datos del enemigo
    [Header("Datos de los enemigos")]
    public string nombre;
    public int vidaBase;
    public int dañoBase;
    public float velocidad;
    public float resistencia; //al empuje
    public int cantMaxExp, cantMinExp;
    #endregion

    #region prefab de experiencia
    [Header("Experiencia")]
    public GameObject expPrefab;
    public float expSpreadForce = 2.0f;
    #endregion

    private float vidaActual;
    private float dañoActual;
    private float velocidadRotacion = 5f;

    protected virtual void Awake()
    {
        vidaActual = vidaBase;
        dañoActual = dañoBase;
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

    public virtual void recibirDaño(float cantidad)
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

        gameObject.SetActive(false);
    }

    public virtual void ActivarEnemigo(Vector3 nuevaPosicion)
    {
        transform.position = nuevaPosicion;
        vidaActual = vidaBase;
        gameObject.SetActive(true);
    }

    public void ajustarEstadisticas(float escala)
    {
        vidaActual = Mathf.RoundToInt(vidaBase * escala);
        dañoActual = Mathf.RoundToInt(dañoBase * escala);
        Debug.Log($"La nueva vida es:{vidaActual} y el nuevo daño es {dañoActual}");
    }
}
