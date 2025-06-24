using UnityEngine;

public class LaserBeamWeapon : Weapon
{
    [Header("Configuración base")]
    public float baseMaxDistance = 3f;
    public float baseLaserWidth = 0.05f;
    public float baseDamage = 0.1f;
    public float baseTickRate = 1f;

    [Header("Incrementos por mejora")]
    public float extraDistancePerLevel = 2f;
    public float extraWidthPerLevel = 0.05f;
    public float extraDamagePerLevel = 0.2f;
    public float tickRateMultiplierLevel4 = 0.5f; // reduce tickRate a 50% del original

    [Header("Estado actual")]
    private float maxDistance;
    private float laserWidth;
    private float damage;
    private float tickRate;
    private float tickTimer = 0f;

    private LineRenderer lineRenderer;
    private Transform firePoint;

    public override void Initialize(GameObject owner)
    {
        base.Initialize(owner);

        firePoint = owner.transform;
        if (firePoint == null)
        {
            Debug.LogWarning("No se encontró el FirePoint en el objeto.");
        }

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

        ApplyUpgrades();
    }

    public override void UpdateWeapon()
    {
        if (!isActive || firePoint == null) return;

        ShootLaser();
        ApplyUpgrades();
    }

    private void ApplyUpgrades()
    {
        maxDistance = baseMaxDistance;
        laserWidth = baseLaserWidth;
        damage = baseDamage;
        tickRate = baseTickRate;

        if (level >= 2)
        {
            maxDistance = baseMaxDistance + extraDistancePerLevel * (level - 1);
            laserWidth = baseLaserWidth + extraWidthPerLevel * (level - 1);
        }

        if (level >= 3)
        {
            damage = baseDamage + extraDamagePerLevel * (level);
        }

        if (level >= 4)
        {
            tickRate = baseTickRate * tickRateMultiplierLevel4 / (level - 3); // reduce el tiempo entre ticks
        }
    }

    private void ShootLaser()
    {
        Vector3 origin = firePoint.position;
        Vector3 direction = firePoint.forward;
        Vector3 endPoint = origin + direction * maxDistance;

        RaycastHit[] hits = Physics.SphereCastAll(origin, laserWidth / 2f, direction, maxDistance);

        tickTimer += Time.deltaTime;
        if (tickTimer >= tickRate)
        {
            tickTimer = 0f;

            foreach (var hit in hits)
            {
                if (hit.collider.CompareTag("Enemy"))
                {
                    var enemy = hit.collider.GetComponent<ControladorEnemigos>();
                    if (enemy != null)
                    {
                        enemy.recibirDaño(damage * PlayerStats.Instance.MultiplicadorDaño);
                    }
                    endPoint = hit.point;
                }
            }
        }

        

        UpdateLaserVisual(origin, endPoint);
    }

    private void UpdateLaserVisual(Vector3 start, Vector3 end)
    {
        if (lineRenderer == null) return;

        lineRenderer.enabled = true;
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);
    }

}
