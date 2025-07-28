using System.Collections.Generic;
using UnityEngine;
using static Cinemachine.DocumentationSortingAttribute;

[RequireComponent(typeof(LineRenderer))]
public class ElectricDischargeWeapon : Weapon
{
    [Header("Electric Discharge Settings")]
    public float baseRange = 10f;
    public float baseDamage = 5f;
    public int baseMaxBounces = 3;
    public float baseFireRate = 1f;

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

        Fire(); // Normal

        if (level >= 5)
        {
            Fire(secondRay: true); // Segundo rayo, distinto objetivo inicial
        }

        nextFireTime = Time.time + GetFireRate();
    }

    private void Fire(bool secondRay = false)
    {
        List<Vector3> positions = new List<Vector3>();
        HashSet<Transform> hitEnemies = new HashSet<Transform>();

        Vector3 currentPos = owner.transform.position;
        positions.Add(currentPos);

        Transform startEnemy = FindClosestEnemy(currentPos, hitEnemies, secondRay ? 1 : 0);
        if (startEnemy == null) return;

        hitEnemies.Add(startEnemy);

        // Daño al primer enemigo
        var controlador = startEnemy.GetComponent<ControladorEnemigos>();
        if (controlador != null)
            controlador.recibirDaño(GetDamage());

        currentPos = startEnemy.position;
        positions.Add(currentPos);

        int totalBounces = GetMaxBounces() - 1; // Ya usamos uno con el primero

        for (int bounce = 0; bounce < totalBounces; bounce++)
        {
            Transform closest = FindClosestEnemy(currentPos, hitEnemies);

            if (closest == null) break;

            hitEnemies.Add(closest);

            controlador = closest.GetComponent<ControladorEnemigos>();
            if (controlador != null)
                controlador.recibirDaño(GetDamage());

            currentPos = closest.position;
            positions.Add(currentPos);
        }

        DrawElectricLine(positions);
    }

    private Transform FindClosestEnemy(Vector3 origin, HashSet<Transform> excluded, int skip = 0)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        List<Transform> candidates = new List<Transform>();

        foreach (var enemy in enemies)
        {
            Transform t = enemy.transform;
            if (excluded.Contains(t)) continue;

            float dist = Vector3.Distance(origin, t.position);
            if (dist <= baseRange)
                candidates.Add(t);
        }

        candidates.Sort((a, b) =>
            Vector3.Distance(origin, a.position).CompareTo(Vector3.Distance(origin, b.position)));

        if (skip < candidates.Count)
            return candidates[skip];

        return null;
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

    // Mejora: rebotes extra por nivel
    private int GetMaxBounces()
    {
        int extraBounces = Mathf.Max(0, level - 1); // desde nivel 2 suma
        return baseMaxBounces + extraBounces;
    }

    // Mejora: daño aumentado a nivel 3+
    private float GetDamage()
    {
        return level >= 3 ? baseDamage * 1.5f : baseDamage;
    }

    // Mejora: velocidad aumentada a nivel 4+
    private float GetFireRate()
    {
        return level >= 4 ? baseFireRate * 0.75f : baseFireRate;
    }
}
