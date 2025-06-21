using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Ship Upgrade")]
public class ShipUpgrade : Upgrade
{
    public enum ShipStatType
    {
        VelocidadAtaque, RecuperaciónVida, MultiplicadorExp, AlcanceExp, VidaMax, Velocidad, MultiplicadorDaño
    }

    public ShipStatType statType;
    public float value;

    public override void ApplyUpgrade()
    {
        PlayerStats.Instance.ApplyShipUpgrade(statType, value);
    }
}
