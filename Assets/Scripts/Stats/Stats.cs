using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Stats : MonoBehaviour
{
    [Header("Atributos")]
    public float VidaMax = 100;
    public float Vida;
    public float Velocidad;
    public float MultiplicadorDaņo;
    
    public abstract void RecibirDaņo(float Daņo);

    protected abstract void Muerte();
}
