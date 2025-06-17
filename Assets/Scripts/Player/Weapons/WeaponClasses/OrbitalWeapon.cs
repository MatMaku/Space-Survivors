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

    private List<GameObject> orbitals = new List<GameObject>();

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
            orbital.SetDamage(damage * PlayerStats.Instance.MultiplicadorDaño); // ahora el orbital sólo se encarga de detectar colisión y aplicar daño
        }

        UpdateVisualState();
    }

    public override void UpdateWeapon()
    {
        //if(!isActive)
        //{
        //    UpdateVisualState();
        //    return;
        //}

        // Asegurarse de que haya suficientes orbitals
        if (orbitals.Count != numberOfOrbitals)
        {
            RebuildOrbitals();
        }

        for (int i = 0; i < orbitals.Count; i++)
        {
            float angle = orbitSpeed * Time.time + (360f / numberOfOrbitals) * i;
            Vector3 offset = Quaternion.Euler(0, angle, 0) * Vector3.forward * orbitRadius;

            if (orbitals[i] != null)
                orbitals[i].transform.localPosition = offset;
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
            }
        }
    }
}
