using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoBase : ControladorEnemigos
{
    private bool puedeAtacar = true;
    private float distanciaDeContacto = 0.3f;

    [Header("Separación entre enemigos")]
    public float radioSeparacion = 1.2f;
    public float fuerzaSeparacion = 1.5f;

    private void Update()
    {
        if (_player != null)
        {
            if (Vector3.Distance(_player.position, transform.position) < distanciaDeContacto && puedeAtacar)
            {
                _player.GetComponent<PlayerStats>().RecibirDaño(dañoActual);
                StartCoroutine(atacando());
            }
            else
            {
                SepararDeOtrosEnemigos();
                Vector3 destino = _player.position;
                destino.y = 0f;

                Vector3 origen = transform.position;
                origen.y = 0f;

                Vector3 direccion = (destino - origen).normalized;
                origen += direccion * velocidad * Time.deltaTime;
                origen.y = 0;
                transform.position = origen;
                rotarHaciaJugador();
            }

            if (Input.GetKeyDown(KeyCode.F))
            {
                Morir();
            }
        }
    }

    private void SepararDeOtrosEnemigos()
    {
        Collider[] cercanos = Physics.OverlapSphere(transform.position, radioSeparacion);
        Vector3 direccionRepulsiva = Vector3.zero;
        int cantidad = 0;

        foreach (Collider col in cercanos)
        {
            if (col.gameObject != this.gameObject && col.CompareTag("Enemy"))
            {
                Vector3 posA = transform.position;
                Vector3 posB = col.transform.position;
                posA.y = 0f;
                posB.y = 0f;
                Vector3 dir = posA - posB;
                float distancia = dir.magnitude;
                if (distancia > 0)
                {
                    direccionRepulsiva += dir.normalized / distancia; // más empuje si están más cerca
                    cantidad++;
                }
            }
        }

        if (cantidad > 0)
        {
            direccionRepulsiva /= cantidad;
            transform.position += direccionRepulsiva * fuerzaSeparacion * Time.deltaTime;
            Vector3 pos = transform.position;
            pos.y = 0f;
            transform.position = pos;
        }
    }

    IEnumerator atacando()
    {
        puedeAtacar = false;
        yield return new WaitForSeconds(0.5f);
        puedeAtacar = true;
    }
}
