using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossController : MonoBehaviour
{
    [Header("Datos Boss")]
    public float velocidad;
    private float velocidadRotacion = 5f;
    public int vidaMax, vidaActual;
    public int cantExp;
    public Transform spawnBalas;

    private Transform _player;
    public GameObject expPrefab;
    public GameObject balaPrefab;
    public float balaSpeed;
    public float expSpreadForce = 2f;

    [Header("Movimiento orbital")]
    public float velocidadAngular = 90f;        // grados por segundo
    public float radioOrbita = 4f;               // distancia fija al jugador

    private float anguloActual = 0f;

    [Header("Disparo")]
    public float tiempoEntreDisparos = 1.5f;
    public float velocidadBala = 10f;

    private float temporizadorDisparo = 0f;

    private void Awake()
    {
        vidaActual = vidaMax;
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");

        if (jugador != null)
        {
            _player = jugador.transform;
        }

        anguloActual = UnityEngine.Random.Range(0f, 360f);

        //StartCoroutine(disparar());
    }

    private void Update()
    {
        if (_player == null) return;

        // Movimiento orbital fijo
        anguloActual += velocidadAngular * Time.deltaTime;
        float radianes = anguloActual * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Cos(radianes), 0f, Mathf.Sin(radianes)) * radioOrbita;
        Vector3 destino = _player.position + offset;

        transform.position = destino;

        // Rotar hacia el jugador
        rotarHaciaJugador();

        // Disparo
        temporizadorDisparo += Time.deltaTime;
        if (temporizadorDisparo >= tiempoEntreDisparos)
        {
            Disparar();
            temporizadorDisparo = 0f;
        }
    }

    private void Disparar()
    {
        if (balaPrefab == null || spawnBalas == null) return;

        GameObject bala = Instantiate(balaPrefab, spawnBalas.position, spawnBalas.rotation);
        Rigidbody rb = bala.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = spawnBalas.forward * velocidadBala;
        }
    }

    IEnumerator disparar()
    {
        while (true)
        {
            Vector3 direccion = Vector3.zero;
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
}
