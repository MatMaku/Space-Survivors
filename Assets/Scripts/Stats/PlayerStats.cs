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
    private Coroutine vidaCoroutine;

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
        if (Vida < VidaMax && vidaCoroutine == null)
        {
            vidaCoroutine = StartCoroutine(RecuperarVida());
        }
        else if (Vida >= VidaMax && vidaCoroutine != null)
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
    }

    public override void SumarVelocidad(float Cantidad)
    {
        Velocidad += Cantidad;
        OnStatsUpdated?.Invoke();
    }

    public override void SumarMultiplicadorDa�o(float Cantidad)
    {
        MultiplicadorDa�o += Cantidad;
        OnStatsUpdated?.Invoke();
    }

    public void SumarVelocidadAtaque(float Cantidad)
    {
        VelocidadAtaque += Cantidad;
        OnStatsUpdated?.Invoke();
    }

    public void SumarRecuperaci�nVida(float Cantidad)
    {
        Recuperaci�nVida += Cantidad;
        OnStatsUpdated?.Invoke();
    }

    public void SumarMultiplicadorExp(float Cantidad)
    {
        MultiplicadorExp += Cantidad;
        OnStatsUpdated?.Invoke();
    }

    public void SumarAlcanceExp(float Cantidad)
    {
        AlcanceExp += Cantidad;
        OnStatsUpdated?.Invoke();
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
}
