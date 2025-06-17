using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public float damage;

    protected virtual void  OnTriggerEnter(Collider other)
    {
        // L�gica de da�o al enemigo
        if (other.CompareTag("Enemy"))
        {
            // Acceder al enemigo y aplicar da�o
            var enemy = other.GetComponent<ControladorEnemigos>();
            if (enemy != null)
            {
                enemy.recibirDa�o(damage);
            }

            //Destroy(gameObject);
        }
    }
}
