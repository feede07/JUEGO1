using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
public class ThirdPersonMovement : MonoBehaviour
{
    [Header("Movimiento")]
    public float moveSpeed = 4f;
    public float rotationSpeed = 10f;
    public float gravity = -9.81f;

    [Header("Referencias")]
    public Transform cameraTransform; // Si lo dejas vacío, usa la cámara principal automáticamente

    private CharacterController controller;
    private Animator animator;
    private Vector3 velocity;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();

        if (cameraTransform == null && Camera.main != null)
        {
            cameraTransform = Camera.main.transform;
        }
    }

    void Update()
    {
        // 1. Leer input del jugador
        float horizontal = Input.GetAxis("Horizontal"); // A/D o flechas
        float vertical = Input.GetAxis("Vertical");     // W/S o flechas

        Vector3 inputDirection = new Vector3(horizontal, 0f, vertical).normalized;

        // 2. Calcular dirección relativa a la cámara
        Vector3 moveDirection = Vector3.zero;
        if (inputDirection.magnitude >= 0.1f && cameraTransform != null)
        {
            float targetAngle = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg
                                 + cameraTransform.eulerAngles.y;

            // Rotar el personaje suavemente hacia la dirección de movimiento
            float angle = Mathf.SmoothDampAngle(
                transform.eulerAngles.y, targetAngle, ref rotationVelocity, 0.1f);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            moveDirection = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }

        // 3. Mover al personaje
        controller.Move(moveDirection.normalized * moveSpeed * Time.deltaTime);

        // 4. Aplicar gravedad
        if (controller.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f; // pequeña fuerza para mantenerlo pegado al suelo
        }
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);

        // 5. Actualizar el Animator
        float currentSpeed = inputDirection.magnitude * moveSpeed;
        animator.SetFloat("Speed", currentSpeed);
    }

    private float rotationVelocity;
}