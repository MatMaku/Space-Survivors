using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class chainEnemy : ControladorEnemigos
{
    private bool _isDeath;

    public bool IsDeath
    {
        get { return _isDeath; }
    }


    public int indexCode;
	public float maxRadius, minRadius;
	[SerializeField] private float movementSpeed;
	[SerializeField] private Transform _padre;
    [SerializeField] public GameObject nextShip;
    private float anguloActual;
    private float radioActual;
    private float tiempo;
    private LineRenderer lineRenderer;
    public Action<chainEnemy> whenShipIsDestroy;

    private float tiempoUltimoDaño; 
    public float intervaloDaño;

    protected override void Awake()
    {
        base.Awake();
        lineRenderer = GetComponent<LineRenderer>();
        anguloActual = UnityEngine.Random.Range(0f, 360f);
        radioActual = UnityEngine.Random.Range(minRadius, maxRadius);
        _padre = transform.parent.GetComponent<Transform>();
    }

    private void Start()
    {
        _isDeath = false;
    }
    private void Update()
    {
        DibujarLaser();
    }
    
    private void DibujarLaser()
    {
        if (nextShip == null || lineRenderer == null) 
        {
            lineRenderer.enabled = false;
            return;
        }

        if (nextShip.GetComponent<chainEnemy>().IsDeath)
        {
            lineRenderer.enabled = false;
            return;
        }
        Vector3 inicio = transform.position;
        Vector3 fin = nextShip.transform.position;

        // Visual
        lineRenderer.enabled = true;
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, inicio);
        lineRenderer.SetPosition(1, fin);

        // Física: rayo entre ambas naves
        Vector3 direccion = (fin - inicio).normalized;
        float distancia = Vector3.Distance(inicio, fin);
        RaycastHit[] hits = Physics.RaycastAll(inicio, direccion, distancia);
        foreach (var hit in hits)
        {
            Debug.Log(hit.collider.name);
            if (hit.collider.CompareTag("Player"))
            {
                if (Time.time - tiempoUltimoDaño >= intervaloDaño)
                {
                    Debug.Log("Toco al jugador");
                    hit.collider.GetComponent<PlayerStats>()?.RecibirDaño(dañoActual);
                    tiempoUltimoDaño = Time.time;
                }
                break;
            }
        }
    }

    public override void ActivarEnemigo(Vector3 nuevaPosicion, int nivel)
    {
        base.ActivarEnemigo(nuevaPosicion, nivel);
        _isDeath = false;
    }

    protected override void Morir()
    {
        base.Morir();
        _isDeath = true;
        whenShipIsDestroy?.Invoke(this);
    }
}

// Este es cada una de las naves que estan conectadas por laseres
// Solo giran alrededor del objeto padre y aumentan o disminuyen la distancia con el centro
// Tiene su propia vida, si se muere activa un evento en el objeto padre
// Tiene que haber almenos dos naves conectadas para que este activo el laser
// Su vida y daño son dadas por el objeto padre
// Si la cantidad de naves es por lo menos 3 se cierra el circuito