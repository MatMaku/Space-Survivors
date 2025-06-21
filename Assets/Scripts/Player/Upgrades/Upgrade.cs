using UnityEngine;

public enum UpgradeType { Weapon, Ship }

public abstract class Upgrade : ScriptableObject
{
    public string upgradeName;
    public string description;
    public Sprite icon;
    public UpgradeType type;

    public abstract void ApplyUpgrade();
}
