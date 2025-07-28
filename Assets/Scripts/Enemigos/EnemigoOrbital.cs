using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoOrbital : ControladorEnemigos
{
    [Header("Balas")]
    public Transform spawnBalas;
    public GameObject balasPrefab;
    public float velocidadBalas;

    [Header("Movimiento orbital")]
    private bool acercandose = true;
    public float velocidadAngular = 90f;
    public float velocidadAproximación = 0.5f;
    public float radioOrbita = 4f;
    private float anguloActual = 0f;

    [Header("Disparo")]
    public float tiempoEntreDisparos = 1.5f;
    protected float temporizadorDisparo = 0f;

    protected virtual void Update()
    {
        if (_player == null) return;

        float distanciaAlJugador = Vector3.Distance(transform.position, _player.position);

        if (acercandose)
        {
            if (distanciaAlJugador <= radioOrbita)
            {
                acercandose = false;

                Vector3 direccion = (transform.position - _player.position).normalized;
                anguloActual = Mathf.Atan2(direccion.z, direccion.x) * Mathf.Rad2Deg;

            }
            else
            {
                // Movimiento de aproximación
                Vector3 direccion = (_player.position - transform.position).normalized;
                Vector3 movimiento = direccion * velocidadAproximación * Time.deltaTime;
                transform.position += movimiento;

                // Forzamos Y = 0
                Vector3 pos = transform.position;
                pos.y = 0f;
                transform.position = pos;

                rotarHaciaJugador();
                return;

            }
        }

        // Movimiento orbital fijo
        anguloActual += velocidadAngular * Time.deltaTime;
        float radianes = anguloActual * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(Mathf.Cos(radianes), 0f, Mathf.Sin(radianes)) * radioOrbita;
        Vector3 destino = _player.position + offset;
        destino.y = 0f;

        transform.position = destino;

        // Rotar hacia el jugador
        rotarHaciaJugador();

        // Disparo
        ControlDisparo();
    }

    protected virtual void ControlDisparo()
    {
        temporizadorDisparo += Time.deltaTime;
        if (temporizadorDisparo >= tiempoEntreDisparos)
        {
            Disparar();
            temporizadorDisparo = 0f;
        }
    }

    protected void Disparar()
    {
        if (balasPrefab == null || spawnBalas == null) return;

        // Dirección horizontal hacia el jugador
        Vector3 objetivo = _player.position;
        objetivo.y = 0f; //  Forzar misma altura

        Vector3 origen = spawnBalas.position;
        origen.y = 0f; //  Asegurar que la bala también dispare desde altura 0

        Vector3 direccionAlJugador = (objetivo - origen).normalized;

        // Agregar desviación aleatoria (ajustable)
        float desviacionGrados = 10f;
        Quaternion desviacion = Quaternion.Euler(0f, Random.Range(-desviacionGrados, desviacionGrados), 0f);
        Vector3 direccionFinal = desviacion * direccionAlJugador;

        GameObject bala = Instantiate(balasPrefab, spawnBalas.position, Quaternion.LookRotation(direccionFinal));
        Rigidbody rb = bala.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direccionFinal * velocidadBalas;
        }
    }

    public override void ActivarEnemigo(Vector3 nuevaPosicion)
    {
        base.ActivarEnemigo(nuevaPosicion);
        acercandose = true;
    }
}
