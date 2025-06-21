using System;
using UnityEngine;

public class DoubleShotWeapon : Weapon
{
    [Header("Configuración de disparo")]
    public GameObject projectilePrefab;
    public Transform firePointLeft;
    public Transform firePointRight;
    bool dobleFire = false;
    float bonusDmg = 0.1f;

    private void Start()
    {
        PlayerStats.Instance.OnStatsUpdated += SetFireRate;
    }

    private void SetFireRate()
    {
        fireRate = PlayerStats.Instance.VelocidadAtaque;
    }

    public override void UpdateWeapon()
    {
        if (!isActive || Time.time < nextFireTime) return;

        FireProjectiles();        
    }

    private void FireProjectiles()
    {
        if (projectilePrefab == null || firePointLeft == null || firePointRight == null)
        {
            Debug.LogWarning("DoubleShotWeapon: faltan referencias.");
            return;
        }               
        
        // Nivel 1 y 2: disparo doble clásico
        CreateProjectile(firePointLeft, firePointLeft.forward);
        CreateProjectile(firePointRight, firePointRight.forward);

        // Nivel 3+: agregar disparos diagonales
        if (level >= 3)
        {
            // Calcular rotaciones hacia adelante en diagonal (frontal-izquierda y frontal-derecha)
            Vector3 diagonalLeft = Quaternion.Euler(0, -15f, 0) * transform.forward;
            Vector3 diagonalRight = Quaternion.Euler(0, 15f, 0) * transform.forward;

            CreateProjectile(firePointLeft, diagonalLeft);
            CreateProjectile(firePointRight, diagonalRight);
        }        

        // Delay corto entre ráfagas si es nivel 4 (ráfaga doble)          
        if(level >= 4 && dobleFire)
        {
            nextFireTime = Time.time + (0.2f);
            dobleFire = false;
        }
        else
        {
            nextFireTime = Time.time + (1f / fireRate); // velocida de disparo normal
            dobleFire = true;
        }            

    }

    private void CreateProjectile(Transform firePoint, Vector3 direction)
    {
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(direction));

        // Si el proyectil tiene un script con daño, pasamos el valor
        var damageComponent = proj.GetComponent<DoubleShotProyectile>();
        if (damageComponent != null)
        {
            damageComponent.SetDamage(damage * PlayerStats.Instance.MultiplicadorDaño * level >= 2? bonusDmg * level : 1); //si es nivel 2 o mas se suma 10% de daño por nivel

            if(level >= 5)
                damageComponent.pierce = true;
        }
    }
}
