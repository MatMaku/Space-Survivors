using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatsUI : MonoBehaviour
{
    [SerializeField] private Slider vidaSlider;
    [SerializeField] private Slider expSlider;
    [SerializeField] private TextMeshProUGUI nivelText;

    private void Start()
    {
        PlayerStats.Instance.OnStatsUpdated += UpdateUI;
        UpdateUI();
    }

    private void UpdateUI()
    {
        vidaSlider.maxValue = PlayerStats.Instance.VidaMax;
        vidaSlider.value = PlayerStats.Instance.Vida;
        expSlider.maxValue = PlayerStats.Instance.ExpSiguienteNivel;
        expSlider.value = PlayerStats.Instance.Exp;
        nivelText.text = "Nv: " + PlayerStats.Instance.Nivel.ToString();
    }
}
