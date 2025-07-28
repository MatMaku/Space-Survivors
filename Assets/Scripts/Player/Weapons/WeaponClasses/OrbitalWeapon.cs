using System;
using System.Collections.Generic;
using UnityEngine;

public class OrbitalWeapon : Weapon
{
    [Header("Orbital Settings")]
    public GameObject orbitalPrefab;
    public float orbitRadius = 2f;
    public float orbitSpeed = 90f; // grados por segundo
    public int numberOfOrbitals = 1;
    public float bonusDmg = 0.1f;

    private List<GameObject> orbitals = new List<GameObject>();
    private List<GameObject> secondaryOrbitals = new List<GameObject>();


    public override void Initialize(GameObject owner)
    {
        base.Initialize(owner);
        RebuildOrbitals();
    }

    private void Start()
    {
        PlayerStats.Instance.OnStatsUpdated += UpdateStats;
    }

    private void UpdateStats()
    {
        foreach (var orb in orbitals)
        {
            orb.GetComponent<OrbitalProjectile>().SetDamage(damage * PlayerStats.Instance.MultiplicadorDaño);
        }

    }

    private void RebuildOrbitals()
    {
        // Destruir existentes si hay menos de los necesarios
        foreach (var orb in orbitals)
        {
            if (orb != null) Destroy(orb);
        }
        orbitals.Clear();

        for (int i = 0; i < numberOfOrbitals; i++)
        {
            GameObject instance = Instantiate(orbitalPrefab, transform);
            orbitals.Add(instance);

            OrbitalProjectile orbital = instance.GetComponent<OrbitalProjectile>();
            //Si es lvl 3 o mas gana 10% de daño por nivel
            orbital.SetDamage(damage * PlayerStats.Instance.MultiplicadorDaño * (1f + level >= 3? bonusDmg * level : 0) ); 
            // ahora el orbital sólo se encarga de detectar colisión y aplicar daño
        }

        UpdateVisualState();
    }

    public override void UpdateWeapon()
    {
        // Nivel 1 y 2: cantidad de orbitals
        numberOfOrbitals = level + 2;

        // Nivel 3: aumenta daño y distancia
        if (level >= 3)
        {
            orbitRadius = 2f + 0.5f * (level - 2); // por ejemplo: nivel 3 = 2.5f
        }

        // Nivel 4: aumenta velocidad de rotación
        if (level >= 4)
        {
            orbitSpeed = 180f; // aumento del default 90
        }

        // Asegurar rebuild si cambia la cantidad
        if (orbitals.Count != numberOfOrbitals)
        {
            RebuildOrbitals();
        }

        // Movimiento del primer anillo
        for (int i = 0; i < orbitals.Count; i++)
        {
            float angle = orbitSpeed * Time.time + (360f / numberOfOrbitals) * i;
            Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * orbitRadius;

            if (orbitals[i] != null)
                orbitals[i].transform.localPosition = offset;
        }

        // Nivel 5: segundo anillo opuesto
        if (level >= 5)
        {
            EnsureSecondaryRing();

            for (int i = 0; i < secondaryOrbitals.Count; i++)
            {
                float angle = -orbitSpeed * Time.time + (360f / secondaryOrbitals.Count) * i;
                Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * (orbitRadius + 1f);

                if (secondaryOrbitals[i] != null)
                    secondaryOrbitals[i].transform.localPosition = offset;
            }
        }
    }

    private void EnsureSecondaryRing()
    {
        // Si ya están creados, no hacer nada
        if (secondaryOrbitals.Count == numberOfOrbitals)
            return;

        foreach (var orb in secondaryOrbitals)
        {
            if (orb != null) Destroy(orb);
        }
        secondaryOrbitals.Clear();

        for (int i = 0; i < numberOfOrbitals; i++)
        {
            GameObject instance = Instantiate(orbitalPrefab, transform);
            secondaryOrbitals.Add(instance);

            OrbitalProjectile orbital = instance.GetComponent<OrbitalProjectile>();
            orbital.SetDamage(damage * PlayerStats.Instance.MultiplicadorDaño);
        }
    }    

    protected override void UpdateVisualState()
    {
        foreach (var orb in orbitals)
        {
            if (orb != null)
            {
                var renderer = orb.GetComponentInChildren<MeshRenderer>();
                if (renderer != null) renderer.enabled = isActive;
                var collider = orb.GetComponentInChildren<SphereCollider>();
                if (collider != null) collider.enabled = isActive;
            }
        }
    }
}

