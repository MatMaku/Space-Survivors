using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Weapon Upgrade")]
public class WeaponUpgrade : Upgrade
{
    public WeaponType weaponType;
    public int level; // Nivel progresivo (1 a 5)

    public override void ApplyUpgrade()
    {
        WeaponManager.Instance.ApplyWeaponUpgrade(weaponType);
    }
}
