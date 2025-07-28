using UnityEngine;

public class SniperWeapon : Weapon
{
    [Header("Piercing Sniper Settings")]
    public GameObject projectilePrefab;
    public Transform firePoint;  
    public float projectileSpeed = 10f;    

    public override void UpdateWeapon()
    {
        if (!isActive || Time.time < nextFireTime)
            return;

        Fire();
        nextFireTime = Time.time + GetFireRate();
    }

    private void Fire()
    {
        GameObject target = FindTarget();
        if (target == null) return;
        
        Vector3 direction = (target.transform.position - firePoint.position).normalized;
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));        

        var snipe = projectile.GetComponent<SniperProyectile>();
        if (snipe != null)
        {
            snipe.SetOwner(this);
            snipe.SetLevel(level);
            snipe.SetDamage(damage);
        }
    }

    private GameObject FindTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        GameObject closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (var enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDistance)
            {
                closest = enemy;
                closestDistance = dist;
            }
        }

        return closest;
    }

    private float GetFireRate()
    {
        return fireRate;
    }

    // Llamado desde el proyectil si mata al primer enemigo (solo nivel 5)
    public void FirstKill()
    {
        if (level >= 5)
        {
            float remainingCooldown = nextFireTime - Time.time;
            nextFireTime = Time.time + remainingCooldown * 0.1f;
        }
    }
}
