using UnityEngine;

public class DoubleShotWeapon : Weapon
{
    [Header("Configuración de disparo")]
    public GameObject projectilePrefab;
    public Transform firePointLeft;
    public Transform firePointRight;

    public override void UpdateWeapon()
    {
        if (!isActive || Time.time < nextFireTime) return;

        FireProjectiles();
        nextFireTime = Time.time + (1f / fireRate);
    }

    private void FireProjectiles()
    {
        if (projectilePrefab == null || firePointLeft == null || firePointRight == null)
        {
            Debug.LogWarning("DoubleShotWeapon: faltan referencias.");
            return;
        }

        CreateProjectile(firePointLeft);
        CreateProjectile(firePointRight);
    }

    private void CreateProjectile(Transform firePoint)
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);

        // Si el proyectil tiene un script con daño, pasamos el valor
        var damageComponent = proj.GetComponent<Projectile>();
        if (damageComponent != null)
        {
            damageComponent.SetDamage(damage);
        }
    }
}
