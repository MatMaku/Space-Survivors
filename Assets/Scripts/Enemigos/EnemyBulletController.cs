using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBulletController : MonoBehaviour
{
    public int damage = 1;


    void Update()
    {
        if (Mathf.Abs(transform.position.z) > 10)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Hacer daño al jugador
            Debug.Log("Toco al jugador");
            Destroy(gameObject);
        }
    }
}
