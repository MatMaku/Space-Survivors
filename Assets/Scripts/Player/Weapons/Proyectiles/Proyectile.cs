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
        // L�gica de da�o al enemigo
        if (other.CompareTag("Enemy"))
        {
            // Acceder al enemigo y aplicar da�o
            //var enemy = other.GetComponent<Enemy>();
            //if (enemy != null)
            //{
            //    enemy.TakeDamage(damage);
            //}

            Destroy(gameObject);
        }
    }
}
