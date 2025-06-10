using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private Transform _player;
    
    [SerializeField] private DatosEnemigo _data;
    public DatosEnemigo data {  get { return _data; } }

    private int currentHealth;
    private float velocidadRotacion = 5f;

    public GameObject expPrefab;
    public float expSpreadForce = 2f;

    private bool puedeAtacar = true;

    private float stoppingDistance = 0.5f;

    private void Awake()
    {
        currentHealth = _data.vida;
        GameObject jugador = GameObject.FindGameObjectWithTag("Player");
        if (jugador != null)
        {
            _player = jugador.transform;
        }
    }

    private void Update()
    {
        #region movimiento sin NavMesh
        if (_player != null)
        {
            Vector3 direccion = (_player.position - transform.position).normalized;
            transform.position += direccion * _data.velocidad * Time.deltaTime;
            rotarHaciaJugador();

            if (Vector3.Distance(_player.position, transform.position) < stoppingDistance && puedeAtacar)
            {
                Debug.Log("La nave enemiga ataco al jugador");
                StartCoroutine(atacando());
            }
        }
        #endregion

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
        currentHealth -= amount;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        int amountToDrop = UnityEngine.Random.Range(_data.cantMinExp, _data.cantMaxExp + 1);

        for (int i = 0; i < amountToDrop; i++)
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

        gameObject.SetActive(false);
    }

    public void volverActivar()
    {
        currentHealth = data.vida;
        gameObject.SetActive(true);
    }

    IEnumerator atacando()
    {
        puedeAtacar = false;
        yield return new WaitForSeconds(5f);
        puedeAtacar = true;
    }
}
