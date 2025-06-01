using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Configuración de movimiento")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 5f;
    public float fixedY = 1f;

    private Rigidbody rb;
    private Vector3 targetPosition;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetPosition = transform.position;
        targetPosition.y = fixedY;
    }

    void Update()
    {
        // Detectar entrada táctil o con mouse
        Vector3 newTarget = targetPosition;

#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
        {
            newTarget = GetWorldPositionFromScreen(Input.mousePosition);
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            newTarget = GetWorldPositionFromScreen(touch.position);
        }
#endif

        // Mantener altura fija
        newTarget.y = fixedY;

        targetPosition = newTarget;
    }

    void FixedUpdate()
    {
        // Movimiento hacia el objetivo
        Vector3 direction = targetPosition - rb.position;
        direction.y = 0f;

        if (direction.sqrMagnitude > 0.01f)
        {
            // Mover
            Vector3 moveStep = direction.normalized * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + moveStep);

            // Rotar suavemente hacia la dirección
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            Quaternion smoothedRotation = Quaternion.Slerp(rb.rotation, targetRotation, rotationSpeed * Time.fixedDeltaTime);
            rb.MoveRotation(smoothedRotation);
        }
    }

    // Convierte una posición en pantalla (táctil o mouse) a una posición en el plano XZ a la altura Y=fixedY
    Vector3 GetWorldPositionFromScreen(Vector3 screenPosition)
    {
        Ray ray = Camera.main.ScreenPointToRay(screenPosition);
        Plane plane = new Plane(Vector3.up, Vector3.up * fixedY); // plano horizontal en Y = fixedY

        if (plane.Raycast(ray, out float enter))
        {
            return ray.GetPoint(enter);
        }

        return transform.position;
    }
}
