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
            //Hacer da�o al jugador
            Debug.Log("Toco al jugador");
            other.GetComponent<PlayerStats>().RecibirDa�o(damage);
            Destroy(gameObject);
        }
    }
}
