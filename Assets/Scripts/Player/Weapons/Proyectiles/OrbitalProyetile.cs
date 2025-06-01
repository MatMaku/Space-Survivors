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
            // Aplicar da�o al enemigo
            Debug.Log($"Da�o de {damage} al enemigo por colisi�n orbital.");
            // other.GetComponent<Enemy>()?.TakeDamage(damage);
        }
    }
}
