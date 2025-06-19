using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    public int damage = 1;

    private void Start()
    {
        Destroy(gameObject,10f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Hacer daño al jugador
            Debug.Log("Toco al jugador");
            other.GetComponent<PlayerStats>().RecibirDaño(damage);
            Destroy(gameObject);
        }
    }
}
