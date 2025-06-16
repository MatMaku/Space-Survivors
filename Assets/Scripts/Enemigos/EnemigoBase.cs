using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemigoBase : ControladorEnemigos
{
    private bool puedeAtacar = true;
    private float distanciaDeContacto = 1f;

    [Header("Separación entre enemigos")]
    public float radioSeparacion = 1.2f;
    public float fuerzaSeparacion = 1.5f;

    private void Update()
    {
        if (_player != null)
        {
            //Debug.Log(Vector3.Distance(_player.position,transform.position));
            if (Vector3.Distance(_player.position, transform.position) < distanciaDeContacto && puedeAtacar)
            {
                Debug.Log("La nave enemiga ataco al jugador");
                _player.GetComponent<PlayerStats>().RecibirDaño(daño);
                StartCoroutine(atacando());
            }
            else
            {
                SepararDeOtrosEnemigos();
                Vector3 direccion = (_player.position - transform.position).normalized;
                transform.position += direccion * velocidad * Time.deltaTime;
                rotarHaciaJugador();
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
                Vector3 dir = transform.position - col.transform.position;
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
        }
    }

    IEnumerator atacando()
    {
        puedeAtacar = false;
        yield return new WaitForSeconds(5f);
        puedeAtacar = true;
    }
}
