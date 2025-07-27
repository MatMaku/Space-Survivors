using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ElectricDischargeWeapon : Weapon
{
    [Header("Electric Discharge Settings")]
    public float range = 10f;
    public int maxBounces = 3;
    public LineRenderer lineRenderer;
    public float lineDuration = 0.2f;

    private float lineTimer = 0f;

    public override void Initialize(GameObject owner)
    {
        base.Initialize(owner);

        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    public override void UpdateWeapon()
    {
        if (!isActive || Time.time < nextFireTime)
            return;

        Fire();
        nextFireTime = Time.time + fireRate;
    }

    private void Fire()
    {
        List<Vector3> positions = new List<Vector3>();
        HashSet<Transform> hitEnemies = new HashSet<Transform>();

        Vector3 currentPos = owner.transform.position;
        positions.Add(currentPos);

        for (int bounce = 0; bounce < maxBounces; bounce++)
        {
            Transform closestEnemy = FindClosestEnemy(currentPos, hitEnemies);

            if (closestEnemy == null)
                break;

            hitEnemies.Add(closestEnemy);

            // Aplicar daño usando el script ControladorEnemigos
            ControladorEnemigos controlador = closestEnemy.GetComponent<ControladorEnemigos>();
            if (controlador != null)
            {
                controlador.recibirDaño(damage);
            }

            currentPos = closestEnemy.position;
            positions.Add(currentPos);
        }

        DrawElectricLine(positions);
    }

    private Transform FindClosestEnemy(Vector3 origin, HashSet<Transform> excluded)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        float minDistance = Mathf.Infinity;
        Transform closest = null;

        foreach (var enemy in enemies)
        {
            Transform enemyTransform = enemy.transform;

            if (excluded.Contains(enemyTransform))
                continue;

            float dist = Vector3.Distance(origin, enemyTransform.position);
            if (dist < minDistance && dist <= range)
            {
                minDistance = dist;
                closest = enemyTransform;
            }
        }

        return closest;
    }

    private void DrawElectricLine(List<Vector3> positions)
    {
        if (positions.Count < 2) return;

        lineRenderer.positionCount = positions.Count;
        lineRenderer.SetPositions(positions.ToArray());
        lineRenderer.enabled = true;
        lineTimer = lineDuration;
    }

    private void Update()
    {
        if (lineRenderer.enabled)
        {
            lineTimer -= Time.deltaTime;
            if (lineTimer <= 0)
            {
                lineRenderer.enabled = false;
            }
        }
    }
}
