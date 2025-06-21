using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Ship Upgrade")]
public class ShipUpgrade : Upgrade
{
    public enum ShipStatType
    {
        VelocidadAtaque, Recuperaci�nVida, MultiplicadorExp, AlcanceExp, VidaMax, Velocidad, MultiplicadorDa�o
    }

    public ShipStatType statType;
    public float value;

    public override void ApplyUpgrade()
    {
        PlayerStats.Instance.ApplyShipUpgrade(statType, value);
    }
}
