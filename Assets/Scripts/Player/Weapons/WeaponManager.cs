using System.Collections.Generic;
using UnityEngine;

public class WeaponManager : MonoBehaviour
{
    private List<Weapon> allWeapons = new List<Weapon>();

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
