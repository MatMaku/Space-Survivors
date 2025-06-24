using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UpgradeManager : MonoBehaviour
{
    [SerializeField] private List<Weapon> allWeapons = new List<Weapon>();
    [SerializeField] private List<Button> upgradeButtons = new List<Button>(); // Botones para mejoras
    [SerializeField] private GameObject upgradeCanvas; // Canvas de mejoras

    [Header("Lista de todas las mejoras disponibles")]
    public List<Upgrade> allUpgrades;

    [Header("Opciones ofrecidas al jugador")]
    public List<Upgrade> currentOptions = new List<Upgrade>();

    private const int MAX_SHIP_UPGRADES_PER_TYPE = 5;
    private const int MAX_UPGRADE_OPTIONS = 3;

    private void Start()
    {
        PlayerStats.Instance.OnLevelUp += GenerateUpgradeOptions;
    }

    public void GenerateUpgradeOptions()
    {
        currentOptions.Clear();

        List<Upgrade> availableShipUpgrades = GetAvailableShipUpgrades();
        List<Upgrade> availableWeaponUpgrades = GetAvailableWeaponUpgrades();

        Upgrade shipOption = availableShipUpgrades.Count > 0 ? GetRandomUpgrade(availableShipUpgrades) : null;
        Upgrade weaponOption = availableWeaponUpgrades.Count > 0 ? GetRandomUpgrade(availableWeaponUpgrades) : null;

        if (shipOption != null) currentOptions.Add(shipOption);
        if (weaponOption != null) currentOptions.Add(weaponOption);

        List<Upgrade> combinedPool = new List<Upgrade>();
        combinedPool.AddRange(availableShipUpgrades);
        combinedPool.AddRange(availableWeaponUpgrades);

        while (currentOptions.Count < MAX_UPGRADE_OPTIONS && combinedPool.Count > 0)
        {
            Upgrade next = GetRandomUpgrade(combinedPool);
            if (!currentOptions.Contains(next))
                currentOptions.Add(next);
            combinedPool.Remove(next);
        }

        SetupUpgradeUI(); // Muestra en UI, pausa el juego

        Debug.Log("Opciones generadas:");
        foreach (var upgrade in currentOptions)
        {
            Debug.Log($"- {upgrade.upgradeName} ({upgrade.type})");
        }
    }

    private List<Upgrade> GetAvailableShipUpgrades()
    {
        var result = new List<Upgrade>();

        foreach (var upgrade in allUpgrades)
        {
            if (upgrade is ShipUpgrade shipUpgrade)
            {
                int appliedCount = PlayerStats.Instance.GetAppliedShipUpgradeCount(shipUpgrade.statType);
                if (appliedCount < MAX_SHIP_UPGRADES_PER_TYPE)
                {
                    result.Add(upgrade);
                }
            }
        }

        return result;
    }

    private List<Upgrade> GetAvailableWeaponUpgrades()
    {
        var result = new List<Upgrade>();

        foreach (var upgrade in allUpgrades)
        {
            if (upgrade is WeaponUpgrade weaponUpgrade)
            {
                var weapon = allWeapons.FirstOrDefault(w => w.weaponType == weaponUpgrade.weaponType);
                if (weapon == null) continue;

                int currentLevel = weapon.level;
                bool isActive = weapon.GetActive();

                if ((!isActive && weaponUpgrade.level == 1) ||
                    (isActive && weaponUpgrade.level == currentLevel + 1 && currentLevel < 5))
                {
                    result.Add(upgrade);
                }
            }
        }

        return result;
    }

    private Upgrade GetRandomUpgrade(List<Upgrade> pool)
    {
        int index = Random.Range(0, pool.Count);
        return pool[index];
    }

    private void SetupUpgradeUI()
    {
        Time.timeScale = 0f; // Pausa el juego
        upgradeCanvas.SetActive(true);

        for (int i = 0; i < upgradeButtons.Count; i++)
        {
            if (i < currentOptions.Count)
            {
                var upgrade = currentOptions[i];

                Button btn = upgradeButtons[i];
                btn.gameObject.SetActive(true);

                // Asignar textos
                TextMeshProUGUI[] texts = btn.GetComponentsInChildren<TextMeshProUGUI>();
                if (texts.Length >= 2)
                {
                    texts[0].text = upgrade.upgradeName;
                    texts[1].text = upgrade.description;
                }

                // Remover listeners viejos y agregar el nuevo
                btn.onClick.RemoveAllListeners();
                btn.onClick.AddListener(() =>
                {
                    upgrade.ApplyUpgrade();
                    CloseUpgradeUI();
                });
            }
            else
            {
                upgradeButtons[i].gameObject.SetActive(false);
            }
        }
    }

    private void CloseUpgradeUI()
    {
        upgradeCanvas.SetActive(false);
        Time.timeScale = 1f; // Reanuda el juego
    }
}
