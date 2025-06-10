using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Exp : MonoBehaviour
{
    private Transform target;
    private float speed = 0.5f;
    private float acceleration = 3f;
    private bool isBeingCollected = false;

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
        if (isBeingCollected && target != null)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            speed += acceleration * Time.deltaTime;
            transform.position += direction * speed * Time.deltaTime;
        }

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

    public void StartCollecting(Transform player)
    {
        if (!isBeingCollected)
        {
            target = player;
            isBeingCollected = true;
            speed = 0.5f;
            meshRenderer.enabled = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerStats.Instance.GanarExp(Valor);
            Destroy(gameObject);
        }
    }
}
