using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class connectedEnemies : ControladorEnemigos
{
    public float velocidadRotación = 90f;
    public float distanciaUmbral = 0.5f;

    private Vector3 destinoActual;

    public List<chainEnemy> navesHijas = new List<chainEnemy>();

    private void Start()
    {
        conectarNaves();
        for (int i = 0; i < navesHijas.Count; i++)
        {
            navesHijas[i].whenShipIsDestroy += ReorganizarNaves;
        }

        destinoActual = _player.transform.position;
    }

    private void Update()
    {
        if(_player == null) return;

        Vector3 direccion = destinoActual - transform.position;
        direccion.y = 0;

        if (direccion.magnitude < distanciaUmbral)
        {
            destinoActual = _player.position;
        }
        else
        {
            transform.position += direccion.normalized * velocidad * Time.deltaTime;
        }

        //transform.Rotate(Vector3.up * velocidadRotación * Time.deltaTime);
    }

    public override void ActivarEnemigo(Vector3 nuevaPosicion, int nivel)
    {
        base.ActivarEnemigo(nuevaPosicion, nivel);
        for (int i = 0; i < navesHijas.Count; i++)
        {
            float angle = (360 / navesHijas.Count) * i;
            Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * 0.5f;
            navesHijas[i].ActivarEnemigo(offset, nivel);
        }

        conectarNaves();
    }

    public void conectarNaves()
    {
        for (int i = 0; i < navesHijas.Count; i++)
        {
            if (!navesHijas[i].IsDeath)
            {
                for (int j = i + 1; j < navesHijas.Count + i; j++)
                {
                    if (!navesHijas[j % navesHijas.Count].IsDeath)
                    {
                        navesHijas[i].nextShip = navesHijas[j % navesHijas.Count].gameObject;
                        break;
                    }
                }
            }
        }
    }


    private void ReorganizarNaves(chainEnemy enemy)
    {
        if (hayNaves()) 
        {
            conectarNaves();
        }
        else
        {
            Morir();
        }
    }

   private bool hayNaves()
    {
        for (int i = 0; i < navesHijas.Count; i++)
        {
            if (!navesHijas[i].IsDeath)
            {
                return true;
            }
        }
        return false;
    }
}
// Es el que contiene a todas las naves que forman la cadena de laseres
// Se mueve hacia el jugador a velocidad constante
// No recibe daño directamente, si no que cada nave interna tiene su propia vida
// Controla la conexiones entre las naves y de donde sale y entra los rayos