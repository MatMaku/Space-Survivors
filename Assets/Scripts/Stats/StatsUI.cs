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
        PlayerStats.Instance.OnHealing += UpdateHP;
        UpdateUI();
        UpdateHP();
    }

    private void UpdateUI()
    {
        vidaSlider.maxValue = PlayerStats.Instance.VidaMax;
        expSlider.maxValue = PlayerStats.Instance.ExpSiguienteNivel;
        expSlider.value = PlayerStats.Instance.Exp;
        nivelText.text = "Nv: " + PlayerStats.Instance.Nivel.ToString();
    }

    private void UpdateHP()
    {        
        vidaSlider.value = PlayerStats.Instance.Vida;
    }
}
