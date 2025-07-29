using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class OrbitalBossController : EnemigoOrbital
{
    [Header("Rayo laser")]
    private LineRenderer _lineRenderer;
    public float largoLaser;
    
    private bool _laserActivado;
    public bool LaserActivado
    {
        get { return _laserActivado; }
    }

    public float nextLaserTime, nextLaserTimer;

    public float tiempoDeLaser;
    private float laserTimer;
    private float tiempoUltimoDa�o;
    public float intervaloDa�o;

    protected override void Awake()
    {
        base.Awake();
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.enabled = false;   
    }

    private void Start()
    {
        laserTimer = 0;
        nextLaserTimer = 0;
        _laserActivado = false;
    }

    protected override void Update()
    {
        base.Update();

        if (!_laserActivado)
        {
            nextLaserTimer += Time.deltaTime;
            if (nextLaserTimer >= nextLaserTime)
            {
                _laserActivado = true;
                nextLaserTimer = 0;
            }
        }
    }

    protected override void ControlDisparo()
    {
        if (!_laserActivado) 
        {
            temporizadorDisparo += Time.deltaTime;
            if (temporizadorDisparo >= tiempoEntreDisparos)
            {
                Disparar();
                temporizadorDisparo = 0f;
            }
        }
        else
        {
            dispararLaser();
        }
        
    }

    private void dispararLaser()
    {
        laserTimer += Time.deltaTime;
        if (laserTimer <= tiempoDeLaser)
        {
            if (_lineRenderer == null) return;

            Vector3 finLaser = spawnBalas.position + spawnBalas.forward * largoLaser;
            _lineRenderer.enabled = true;
            _lineRenderer.positionCount = 2;
            _lineRenderer.SetPosition(0, spawnBalas.position);
            _lineRenderer.SetPosition(1, finLaser);

            RaycastHit[] hits = Physics.RaycastAll(spawnBalas.position, spawnBalas.forward, largoLaser);
            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Player"))
                {
                    if (Time.time - tiempoUltimoDa�o >= intervaloDa�o)
                    {
                        hit.collider.GetComponent<PlayerStats>()?.RecibirDa�o(da�oActual);
                        tiempoUltimoDa�o = Time.time;
                    }
                    break;
                }
            }
        }
        else
        {
            _laserActivado = false;
            _lineRenderer.enabled = false;
            laserTimer = 0;
        }
    }

    public override void ActivarEnemigo(Vector3 nuevaPosicion, int nivel)
    {
        transform.position = nuevaPosicion;
        this.nivel = nivel;
        vidaActual = vidaBase + nivel * 1.5f;
        da�oActual = da�oBase + nivel / 5;
    }

    protected override void Morir()
    {
        GameOverController.Instance.TriggerWin();
        base.Morir();
    }
}
