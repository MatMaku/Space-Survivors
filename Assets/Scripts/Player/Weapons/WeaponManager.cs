using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    [SerializeField]
    private List<Weapon> allWeapons = new List<Weapon>();

    public static WeaponManager Instance { get; private set; }

    void Start()
    {
        allWeapons.AddRange(GetComponentsInChildren<Weapon>(true));

        foreach (var weapon in allWeapons)
        {
            weapon.Initialize(gameObject);
        }
    }

    void Update()
    {
        foreach (var weapon in allWeapons)
        {
            weapon.UpdateWeapon();
        }
    }

    public void ApplyWeaponUpgrade(WeaponType weaponType)
    {
        // Buscar el arma por tipo en la lista allWeapons
        Weapon weapon = allWeapons.FirstOrDefault(w => w.weaponType == weaponType);

        if (weapon == null)
        {
            Debug.LogWarning($"No se encontró el arma del tipo {weaponType} en la lista de armas.");
            return;
        }

        // Si no está activa, activarla (nivel 1)
        if (!weapon.GetActive())
        {
            weapon.level = 1;
            ActivateWeapon(weapon.weaponType);
        }
        else
        {
            // Si ya está activa, subir el nivel hasta el máximo
            if (weapon.level < weapon.maxLevel)
            {
                weapon.LevelUp();
            }
            else
            {
                Debug.Log($"El arma {weaponType} ya alcanzó el nivel máximo.");
            }
        }
    }


    public void ActivateWeapon(WeaponType type)
    {
        foreach (var weapon in allWeapons)
        {
            if (weapon.weaponType == type)
            {
                weapon.SetActive(true);
            }
        }
    }

    public void DeactivateWeapon(WeaponType type)
    {
        foreach (var weapon in allWeapons)
        {
            if (weapon.weaponType == type)
            {
                weapon.SetActive(false);
            }
        }
    }
}
