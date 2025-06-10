using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    [Header("Áreas de movimiento")]
    public LayerMask groundLayer;
    public Vector3 areaSize;
    public float distanciaDelCentro;
    private Vector3 centro;

    [Header("Datos Boss")]
    public float velocidad;
    private float velocidadRotacion = 5f;
    public int vidaMax, vidaActual;
    public int cantExp;
    public Transform spawnBalas;

    private Transform _player;
    private NavMeshAgent _agent;
    private bool estaArriba;
    public GameObject expPrefab;
    public GameObject balaPrefab;
    public float balaSpeed;
    public float expSpreadForce = 2f;
    private Vector3 _destino;
    private float timer, intervaloTeletransporte = 5;

    private void Start()
    {
        vidaActual = vidaMax;
        _agent = GetComponent<NavMeshAgent>();
        _agent.speed = velocidad;
        _agent.updateRotation = false;
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");

        if (jugador != null)
        {
            _player = jugador.transform;
        }

        estaArriba = false;
        transform.position = generarDestino();
        _destino = generarDestino();
        StartCoroutine(disparar());
    }

    private void Update()
    {
        centro = getCenterPoint();
        movimiento();
        teletransporte();
        //Debug.Log($"Punto en el centro de la pantalla (x,z): {centro}");
    }

    private void movimiento()
    {
        if (Vector3.Distance(transform.position, _destino) > 0.8f) 
        {
            _agent.SetDestination(_destino);
            //Debug.Log($"Me estoy moviendo, falta: {Vector3.Distance(transform.position, _destino)}");
        }
        else
        {
            _destino = generarDestino();
            _agent.SetDestination(_destino);
            //Debug.Log($"Cambie el destino: {_destino}");
        }
        rotarHaciaJugador();
    }

    private Vector3 generarDestino() 
    {
        float z = (estaArriba) ? centro.z + distanciaDelCentro :
                                     centro.z - distanciaDelCentro;
        float x = recalcularPosicionHorizontal();
        Vector3 destino = new Vector3(x, transform.position.y, z);
        return destino;
    }

    private void teletransporte()
    {
        timer += Time.deltaTime;

        if (timer > intervaloTeletransporte)
        {
            estaArriba = !estaArriba;
            float z = (estaArriba) ? centro.z + distanciaDelCentro :
                                     centro.z - distanciaDelCentro;
            transform.position = new Vector3(transform.position.x,1,z);
            _destino.z = z;
            timer = 0;
        }
    }

    private float recalcularPosicionHorizontal()
    {
        float largo = UnityEngine.Random.Range(0, areaSize.x / 2f);
        float x;
        if (centro.x - _destino.x < 0)
        {
            x = centro.x - largo;
        }
        else
        {
            x = centro.x + largo;
        }

        return x;
    }

    IEnumerator disparar()
    {
        while (true)
        {
            Vector3 direccion = (estaArriba)? Vector3.back : Vector3.forward;
            GameObject bala = Instantiate(balaPrefab,spawnBalas.position,Quaternion.identity);
            Rigidbody rbBala = bala.GetComponent<Rigidbody>();
            if (rbBala != null)
            {
                rbBala.AddForce(direccion * balaSpeed, ForceMode.Impulse);
            }
            yield return new WaitForSeconds(0.5f);
        }
    }

    private void rotarHaciaJugador()
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

    public virtual void takeDamage(int amount)
    {
        vidaActual -= amount;
        if (vidaActual <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        for (int i = 0; i < cantExp; i++)
        {
            GameObject exp = Instantiate(expPrefab, transform.position,
                                        Quaternion.Euler(0f, 0f, UnityEngine.Random.Range(0f, 360f)));
            Rigidbody rb = exp.GetComponent<Rigidbody>();
            if (rb != null)
            {
                Vector3 randomDir = UnityEngine.Random.insideUnitSphere.normalized;
                rb.AddForce(randomDir * expSpreadForce, ForceMode.Impulse);
            }
        }

        Destroy(gameObject);
    }

    public Vector3 getCenterPoint()
    {
        Vector3 screenCenter = new Vector3(Screen.width/2f, Screen.height /2f,0f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenter);

        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, groundLayer))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(centro + Vector3.forward * distanciaDelCentro, areaSize);
        Gizmos.DrawWireCube(centro + Vector3.back * distanciaDelCentro, areaSize);
    }
}
