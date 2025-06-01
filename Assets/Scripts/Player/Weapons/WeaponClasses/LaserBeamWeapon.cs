using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class LaserBeamWeapon : Weapon
{
    [Header("Punto de disparo del láser")]
    public Transform firePoint;

    [Header("Propiedades del láser")]
    public float maxDistance = 20f;
    public float laserWidth = 0.5f;

    [Header("Visual")]
    public LineRenderer lineRenderer;

    private RaycastHit hitInfo;

    public override void UpdateWeapon()
    {
        if (!isActive || firePoint == null) return;

        Vector3 origin = firePoint.position;
        Vector3 direction = firePoint.forward;

        bool hit = Physics.SphereCast(origin, laserWidth / 2f, direction, out hitInfo, maxDistance);

        Vector3 endPoint = origin + direction * maxDistance;

        if (hit)
        {
            endPoint = hitInfo.point;

            if (hitInfo.collider.CompareTag("Enemy"))
            {
                //Enemy enemy = hitInfo.collider.GetComponent<Enemy>();
                //if (enemy != null)
                //{
                //    enemy.TakeDamage(damage * Time.deltaTime);
                //}
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
