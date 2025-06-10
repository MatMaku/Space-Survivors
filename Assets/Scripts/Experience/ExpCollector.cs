using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpCollector : MonoBehaviour
{
    private SphereCollider Collider;

    private void Start()
    {
        Collider = GetComponent<SphereCollider>();
        PlayerStats.Instance.OnStatsUpdated += ActualizarAlcance;
        ActualizarAlcance();
    }

    private void ActualizarAlcance()
    {
        Collider.radius = PlayerStats.Instance.AlcanceExp;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Exp"))
        {
            Exp exp = other.GetComponent<Exp>();
            if (exp != null)
            {
                exp.StartCollecting(transform);
            }
        }
    }
}
