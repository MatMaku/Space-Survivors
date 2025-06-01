using UnityEngine;

public class OrbitalProjectile : MonoBehaviour
{
    private float damage = 1f;

    public void SetDamage(float dmg)
    {
        damage = dmg;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            // Aplicar daño al enemigo
            Debug.Log($"Daño de {damage} al enemigo por colisión orbital.");
            // other.GetComponent<Enemy>()?.TakeDamage(damage);
        }
    }
}
