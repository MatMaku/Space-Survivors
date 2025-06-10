using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal : MonoBehaviour
{
    [Header("Valor")]
    public float Valor;

    [Header("Vida util")]
    public float lifeTime = 10f;
    public float blinkStartTime = 7f;
    private float timer = 0f;

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= blinkStartTime)
        {
            float blink = Mathf.PingPong(Time.time * 5f, 1f);
            meshRenderer.enabled = blink > 0.5f;
        }

        if (timer >= lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats.Instance.Curar(Valor);
            Destroy(gameObject);
        }
    }
}
