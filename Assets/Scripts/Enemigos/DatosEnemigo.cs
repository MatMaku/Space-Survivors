using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Enemigos")]
public class DatosEnemigo : ScriptableObject
{
    public string nombre;
    public int id;
    public int vida;
    public int daño;
    public float velocidad;
    public float resistencia; //al empuje
    public int cantMaxExp, cantMinExp;
    public int cantMaxSpawn, cantMinSpawn;
}
