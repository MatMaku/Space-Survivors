using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserBeamWeapon : Weapon
{
    [Header("Punto de disparo del l�ser")]
    public Transform firePoint;

    [Header("Propiedades del l�ser")]
    public float maxDistance = 10f;
    public float laserWidth = 0.5f;

    [Header("Visual")]
    public LineRenderer lineRenderer;

    private RaycastHit hitInfo;

    public override void UpdateWeapon()
    {
        if (!isActive || firePoint == null) return;

        Vector3 origin = firePoint.position;
        Vector3 direction = firePoint.forward;

        RaycastHit[] hits = Physics.SphereCastAll(origin, laserWidth / 2f, direction, maxDistance);

        Vector3 endPoint = origin + direction * maxDistance;

        foreach (var hit in hits)
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                var enemy = hit.collider.GetComponent<ControladorEnemigos>();
                if (enemy != null)
                {
                    enemy.recibirDa�o(damage * PlayerStats.Instance.MultiplicadorDa�o);
                }

                // Ajustamos el punto final para mostrar visualmente el impacto si quer�s
                endPoint = hit.point;

                // Si solo quer�s da�ar al primer enemigo que toca, pod�s cortar ac�:
                // break;
            }
        }

        UpdateLaserVisual(origin, endPoint);
    }

    private void UpdateLaserVisual(Vector3 start, Vector3 end)
    {
        if (lineRenderer == null) return;

        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, start);
        lineRenderer.SetPosition(1, end);

        // Aplicar ancho visual
        lineRenderer.startWidth = laserWidth;
        lineRenderer.endWidth = laserWidth;
    }

    public override void Initialize(GameObject owner)
    {
        base.Initialize(owner);
        damage = 0.1f;
        lineRenderer = GetComponent<LineRenderer>();
        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }
    }

    protected override void UpdateVisualState()
    {
        base.UpdateVisualState();        
        if (lineRenderer != null)
        {
            lineRenderer.enabled = isActive;
        }
    }
}
