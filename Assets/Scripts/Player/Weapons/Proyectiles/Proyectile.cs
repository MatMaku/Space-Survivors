using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 10f;
    private float damage;

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    void Update()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        // Lógica de daño al enemigo
        if (other.CompareTag("Enemy"))
        {
            // Acceder al enemigo y aplicar daño
            //var enemy = other.GetComponent<Enemy>();
            //if (enemy != null)
            //{
            //    enemy.TakeDamage(damage);
            //}

            Destroy(gameObject);
        }
    }
}
