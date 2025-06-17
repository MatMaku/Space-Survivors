using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public float damage;

    protected virtual void  OnTriggerEnter(Collider other)
    {
        // Lógica de daño al enemigo
        if (other.CompareTag("Enemy"))
        {
            // Acceder al enemigo y aplicar daño
            var enemy = other.GetComponent<ControladorEnemigos>();
            if (enemy != null)
            {
                enemy.recibirDaño(damage);
            }

            //Destroy(gameObject);
        }
    }
}
