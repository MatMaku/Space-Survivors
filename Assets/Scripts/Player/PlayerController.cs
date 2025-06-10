using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{
    [Header("Componentes")]
    [SerializeField] private ParticleSystem[] PropulsoresEffect;
    [SerializeField] private FixedJoystick joystick;
    [SerializeField] private float Velocidad;

    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        PlayerStats.Instance.OnStatsUpdated += ActualizarVelocidad;
    }
    void Update()
    {
        bool isMoving = rb.velocity.magnitude > 0f;

        foreach (ParticleSystem ps in PropulsoresEffect)
        {
            var emission = ps.emission;
            emission.enabled = isMoving;
        }
    }

    private void ActualizarVelocidad()
    {
        Velocidad = PlayerStats.Instance.Velocidad;
    }

    private void FixedUpdate()
    {
        Vector3 velocity = new Vector3(joystick.Horizontal * Velocidad, rb.velocity.y, joystick.Vertical * Velocidad);
        rb.velocity = velocity;

        if (joystick.Horizontal != 0 || joystick.Vertical != 0)
        {
            Quaternion targetRotation = Quaternion.LookRotation(velocity);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.fixedDeltaTime * (Velocidad * 3));
        }
    }
}
