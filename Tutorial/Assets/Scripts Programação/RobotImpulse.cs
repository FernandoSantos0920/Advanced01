using UnityEngine;

public class RobotImpulse : MonoBehaviour
{
    [Header("Knockback")]
    [SerializeField] private float knockbackDuration = 0.5f;

    [Header("Atribuição da derrubada")]
    [SerializeField] private float attributionWindow = 3f;

    private CharacterController controller;

    private Vector3 knockbackVelocity;
    private float knockbackTimer;

    private PlayerIdentifier.Player lastAttacker;
    private float attributionTimer;
    private bool hasValidAttacker;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Update()
    {
        if (knockbackTimer > 0f)
        {
            controller.Move(knockbackVelocity * Time.deltaTime);

            knockbackTimer -= Time.deltaTime;

            if (knockbackDuration > 0f)
            {
                knockbackVelocity = Vector3.Lerp(
                    knockbackVelocity,
                    Vector3.zero,
                    Time.deltaTime / knockbackDuration
                );
            }
        }

        if (hasValidAttacker)
        {
            attributionTimer -= Time.deltaTime;

            if (attributionTimer <= 0f)
            {
                hasValidAttacker = false;
            }
        }
    }

    public void ReceberEmpurrao(
        Vector3 forca,
        PlayerIdentifier.Player atacante)
    {
        knockbackVelocity = forca;
        knockbackTimer = knockbackDuration;

        lastAttacker = atacante;
        attributionTimer = attributionWindow;
        hasValidAttacker = true;
    }

    public bool TryGetLastAttacker(
        out PlayerIdentifier.Player atacante)
    {
        if (hasValidAttacker && attributionTimer > 0f)
        {
            atacante = lastAttacker;

            // Uma atribuição só pode gerar uma derrubada.
            hasValidAttacker = false;
            attributionTimer = 0f;

            return true;
        }

        atacante = default;
        return false;
    }
}