using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Stats
{
    public float VelocidadAtaque = 1;
    public float Recuperaci�nVida = 0;
    public int Nivel = 1;
    public float Exp = 0;
    public float MultiplicadorExp = 1;
    public float AlcanceExp = 0;
    public float ExpSiguienteNivel = 3;

    public event System.Action OnStatsUpdated;
    public event System.Action OnHealing;
    public event System.Action OnLevelUp;
    private Coroutine vidaCoroutine;

    private Dictionary<ShipUpgrade.ShipStatType, int> appliedShipUpgrades = new();

    public static PlayerStats Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        Vida = VidaMax;
        OnStatsUpdated?.Invoke();
    }
    private void Update()
    {
        if (Vida < VidaMax && vidaCoroutine == null && Recuperaci�nVida > 0)
        {
            vidaCoroutine = StartCoroutine(RecuperarVida());
        }
        else if ( (Vida >= VidaMax || Recuperaci�nVida <= 0) && vidaCoroutine != null)
        {
            StopCoroutine(vidaCoroutine);
            vidaCoroutine = null;
        }
    }

    private IEnumerator RecuperarVida()
    {
        while (Vida < VidaMax)
        {
            yield return new WaitForSeconds(1f);
            Curar(Recuperaci�nVida);
        }
        vidaCoroutine = null; 
    }


    public void Curar(float Cantidad)
    {
        Vida += Cantidad;
        if (Vida > VidaMax) 
        {
            Vida = VidaMax;
        }
        //OnStatsUpdated?.Invoke();
        OnHealing?.Invoke();
    }

    public void GanarExp(float Cantidad)
    {
        Exp += Cantidad * MultiplicadorExp;
        if (Exp >= ExpSiguienteNivel)
        {
            SubirNivel();
            Exp -= ExpSiguienteNivel;
            ExpSiguienteNivel = Nivel * 3;
        }
        OnStatsUpdated?.Invoke();
    }

    private void SubirNivel()
    {
        Nivel++;
        OnLevelUp?.Invoke();
    }    

    public override void RecibirDa�o(float Da�o)
    {
        Vida -= Da�o;
        if (Vida <= 0)
        {
            Muerte();
        }
        OnStatsUpdated?.Invoke();
    }

    protected override void Muerte()
    {
        //Muerte del jugador
    }

    public void ApplyShipUpgrade(ShipUpgrade.ShipStatType statType, float value)
    {
        switch (statType)
        {
            case ShipUpgrade.ShipStatType.VelocidadAtaque:
                VelocidadAtaque += value;
                break;
            case ShipUpgrade.ShipStatType.Recuperaci�nVida:
                Recuperaci�nVida += value;
                break;
            case ShipUpgrade.ShipStatType.MultiplicadorExp:
                MultiplicadorExp += value;
                break;
            case ShipUpgrade.ShipStatType.AlcanceExp:
                AlcanceExp += value;
                break;
            case ShipUpgrade.ShipStatType.VidaMax:
                VidaMax += value;
                Vida = Mathf.Min(Vida, VidaMax);
                break;
            case ShipUpgrade.ShipStatType.Velocidad:
                Velocidad += value;
                break;
            case ShipUpgrade.ShipStatType.MultiplicadorDa�o:
                MultiplicadorDa�o += value;
                break;
        }

        // L�gica de mejora...
        if (!appliedShipUpgrades.ContainsKey(statType))
            appliedShipUpgrades[statType] = 0;

        appliedShipUpgrades[statType]++;

        OnStatsUpdated?.Invoke();
        // Pod�s actualizar HUD, recalcular stats, etc. si hace falta
    }


    public int GetAppliedShipUpgradeCount(ShipUpgrade.ShipStatType statType)
    {
        return appliedShipUpgrades.TryGetValue(statType, out var count) ? count : 0;
    }

}
