using UnityEngine;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        private PlayerIdentifier playerID;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again.")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state.")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        public GameObject CinemachineCameraTarget;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float CameraAngleOverride = 0.0f;
        public bool LockCameraPosition = false;

        [Tooltip("Trava a rotação horizontal da câmera atrás do Player")]
        public bool LockCameraRotation = false;

        [Header("Robo")]
        [SerializeField] private RoboData roboData;
        [SerializeField] private SkinnedMeshRenderer roboRenderer;

        private bool usarCorSecundaria;

        public Vector2 LookSensitivity = new Vector2(7.5f, 5.0f);

        [Header("Ataque")]
        [SerializeField] private float alcanceAtaque = 2.5f;
        [SerializeField] private float forcaVertical = 5f;
        [SerializeField] private float distanciaMaximaForca = 2.5f;
        [SerializeField] private float forcaAtual;

        [Header("Recarga do Ataque")]
        [Tooltip("Tempo, em segundos, até o próximo ataque ficar disponível.")]
        [SerializeField] private float tempoRecargaAtaque = 2f;

        private float tempoRestanteRecarga;
        private bool ataqueEmRecarga;

        // Cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        private Vector3 _cameraStartingPosition;
        private Quaternion _cameraStartingRotation;

        public bool IsRespawning { get; set; } = false;

        // Player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // Timeout
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // Animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif

        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;

        [SerializeField] private Camera _mainCameraPlayer;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput != null &&
                       _playerInput.currentControlScheme == "Keyboard";
#else
                return false;
#endif
            }
        }

        private void Start()
        {
            playerID = GetComponent<PlayerIdentifier>();

            _cinemachineTargetYaw =
                CinemachineCameraTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();

#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError(
                "Starter Assets package is missing dependencies. " +
                "Please use Tools/Starter Assets/Reinstall Dependencies to fix it"
            );
#endif

            AssignAnimationIDs();

            _cameraStartingPosition =
                CinemachineCameraTarget.transform.position;

            _cameraStartingRotation =
                CinemachineCameraTarget.transform.rotation;

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;

            AplicarDadosDoRobo();

            if (playerID != null)
            {
                PlayerOM.AttackCooldownChanged(playerID.ID, 1f);
            }
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            AtualizarRecargaAtaque();

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void AtualizarRecargaAtaque()
        {
            if (!ataqueEmRecarga || playerID == null)
                return;

            tempoRestanteRecarga -= Time.deltaTime;

            float progresso = tempoRecargaAtaque <= 0f
                ? 1f
                : 1f - (tempoRestanteRecarga / tempoRecargaAtaque);

            PlayerOM.AttackCooldownChanged(
                playerID.ID,
                Mathf.Clamp01(progresso)
            );

            if (tempoRestanteRecarga <= 0f)
            {
                tempoRestanteRecarga = 0f;
                ataqueEmRecarga = false;

                PlayerOM.AttackCooldownChanged(playerID.ID, 1f);
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(
                transform.position.x,
                transform.position.y - GroundedOffset,
                transform.position.z
            );

            Grounded = Physics.CheckSphere(
                spherePosition,
                GroundedRadius,
                GroundLayers,
                QueryTriggerInteraction.Ignore
            );

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            if (IsRespawning)
            {
                _cinemachineTargetYaw = 0f;
                _cinemachineTargetPitch = 0f;

                CinemachineCameraTarget.transform.position =
                    _cameraStartingPosition;

                CinemachineCameraTarget.transform.rotation =
                    _cameraStartingRotation;

                IsRespawning = false;
                return;
            }

            if (LockCameraRotation)
            {
                _cinemachineTargetYaw = transform.eulerAngles.y;

                if (_input.look.sqrMagnitude >= _threshold &&
                    !LockCameraPosition)
                {
                    float deltaTimeMultiplier =
                        IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                    _cinemachineTargetPitch +=
                        _input.look.y *
                        deltaTimeMultiplier *
                        LookSensitivity.y;
                }
            }
            else
            {
                if (_input.look.sqrMagnitude >= _threshold &&
                    !LockCameraPosition)
                {
                    float deltaTimeMultiplier =
                        IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                    _cinemachineTargetYaw +=
                        _input.look.x *
                        deltaTimeMultiplier *
                        LookSensitivity.x;

                    _cinemachineTargetPitch +=
                        _input.look.y *
                        deltaTimeMultiplier *
                        LookSensitivity.y;
                }
            }

            _cinemachineTargetYaw = ClampAngle(
                _cinemachineTargetYaw,
                float.MinValue,
                float.MaxValue
            );

            _cinemachineTargetPitch = ClampAngle(
                _cinemachineTargetPitch,
                BottomClamp,
                TopClamp
            );

            CinemachineCameraTarget.transform.rotation =
                Quaternion.Euler(
                    _cinemachineTargetPitch + CameraAngleOverride,
                    _cinemachineTargetYaw,
                    0.0f
                );
        }

        private void Move()
        {
            float targetSpeed =
                _input.sprint ? SprintSpeed : MoveSpeed;

            if (_input.move == Vector2.zero)
                targetSpeed = 0.0f;

            float currentHorizontalSpeed =
                new Vector3(
                    _controller.velocity.x,
                    0.0f,
                    _controller.velocity.z
                ).magnitude;

            float speedOffset = 0.1f;

            float inputMagnitude =
                _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(
                    currentHorizontalSpeed,
                    targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate
                );

                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(
                _animationBlend,
                targetSpeed,
                Time.deltaTime * SpeedChangeRate
            );

            if (_animationBlend < 0.01f)
                _animationBlend = 0f;

            Vector3 inputDirection =
                new Vector3(
                    _input.move.x,
                    0.0f,
                    _input.move.y
                ).normalized;

            if (_input.move != Vector2.zero)
            {
                _targetRotation =
                    Mathf.Atan2(inputDirection.x, inputDirection.z) *
                    Mathf.Rad2Deg +
                    _mainCameraPlayer.transform.eulerAngles.y;

                float rotation = Mathf.SmoothDampAngle(
                    transform.eulerAngles.y,
                    _targetRotation,
                    ref _rotationVelocity,
                    RotationSmoothTime
                );

                transform.rotation =
                    Quaternion.Euler(0.0f, rotation, 0.0f);
            }

            Vector3 targetDirection =
                Quaternion.Euler(0.0f, _targetRotation, 0.0f) *
                Vector3.forward;

            _controller.Move(
                targetDirection.normalized *
                (_speed * Time.deltaTime) +
                new Vector3(0.0f, _verticalVelocity, 0.0f) *
                Time.deltaTime
            );

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity =
                        Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(
            float lfAngle,
            float lfMin,
            float lfMax)
        {
            if (lfAngle < -360f)
                lfAngle += 360f;

            if (lfAngle > 360f)
                lfAngle -= 360f;

            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen =
                new Color(0.0f, 1.0f, 0.0f, 0.35f);

            Color transparentRed =
                new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = Grounded
                ? transparentGreen
                : transparentRed;

            Gizmos.DrawSphere(
                new Vector3(
                    transform.position.x,
                    transform.position.y - GroundedOffset,
                    transform.position.z
                ),
                GroundedRadius
            );
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(
                        0,
                        FootstepAudioClips.Length
                    );

                    AudioSource.PlayClipAtPoint(
                        FootstepAudioClips[index],
                        transform.TransformPoint(_controller.center),
                        FootstepAudioVolume
                    );
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(
                    LandingAudioClip,
                    transform.TransformPoint(_controller.center),
                    FootstepAudioVolume
                );
            }
        }

        public void ResetCameraRotation(float targetYaw)
        {
            _cinemachineTargetYaw = targetYaw;
            _cinemachineTargetPitch = 0f;

            CinemachineCameraTarget.transform.rotation =
                Quaternion.Euler(
                    _cinemachineTargetPitch,
                    _cinemachineTargetYaw,
                    0f
                );

            Debug.Log($"Camera Yaw reset to {targetYaw} degrees.");
        }

        #region Observer do Player em acao

        private void OnEnable()
        {
            PlayerOM.ChangeCoins += OnCoinsChanged;
        }

        private void OnDisable()
        {
            PlayerOM.ChangeCoins -= OnCoinsChanged;
        }

        private void OnCoinsChanged(
            PlayerIdentifier.Player player,
            int quantidade)
        {
            if (playerID == null || player != playerID.ID)
                return;

            MoveSpeed += 1f;
            SprintSpeed += 1f;
        }

        #endregion

        public void OnAttack(InputValue value)
        {
            if (!value.isPressed)
                return;

            if (ataqueEmRecarga)
                return;

            Atacar();
        }

        private void Atacar()
        {
            ThirdPersonController[] players =
                FindObjectsByType<ThirdPersonController>(
                    FindObjectsSortMode.None
                );

            ThirdPersonController oponente = null;

            foreach (ThirdPersonController player in players)
            {
                if (player != this)
                {
                    oponente = player;
                    break;
                }
            }

            if (oponente == null)
            {
                Debug.LogWarning("Nenhum oponente foi encontrado!");
                return;
            }

            float distancia = Vector3.Distance(
                transform.position,
                oponente.transform.position
            );

            Debug.Log(
                $"{playerID.ID} atacou. " +
                $"Distância até o oponente: {distancia:F2}"
            );

            if (distancia > alcanceAtaque)
            {
                Debug.Log(
                    $"{playerID.ID}: oponente está fora do alcance."
                );

                return;
            }

            float fatorForca = 1f - Mathf.Clamp01(
                distancia / alcanceAtaque
            );

            float forcaHorizontal = forcaAtual * Mathf.Lerp(
                0.5f,
                1.5f,
                fatorForca
            );

            Vector3 direcaoEmpurrao =
                transform.forward * forcaHorizontal;

            direcaoEmpurrao.y = forcaVertical;

            RobotImpulse knockback =
                oponente.GetComponent<RobotImpulse>();

            if (knockback != null)
            {
                knockback.ReceberEmpurrao(
                    direcaoEmpurrao,
                    playerID.ID
                );

                if (tempoRecargaAtaque > 0f)
                {
                    tempoRestanteRecarga = tempoRecargaAtaque;
                    ataqueEmRecarga = true;

                    PlayerOM.AttackCooldownChanged(
                        playerID.ID,
                        0f
                    );
                }
                else
                {
                    tempoRestanteRecarga = 0f;
                    ataqueEmRecarga = false;

                    PlayerOM.AttackCooldownChanged(
                        playerID.ID,
                        1f
                    );
                }

                Debug.Log(
                    $"{playerID.ID} empurrou {oponente.playerID.ID} " +
                    $"com força horizontal de {forcaHorizontal:F2}."
                );
            }
            else
            {
                Debug.LogWarning(
                    "O oponente não possui o componente RobotImpulse!"
                );
            }
        }

        #region Configuração do Robô

        public void ConfigurarRobo(
            RoboData dados,
            bool aplicarCorSecundaria = false)
        {
            if (dados == null)
            {
                Debug.LogError(
                    $"Nenhum RoboData foi fornecido para {gameObject.name}."
                );

                return;
            }

            roboData = dados;
            usarCorSecundaria = aplicarCorSecundaria;

            AplicarDadosDoRobo();
        }

        private void AplicarDadosDoRobo()
        {
            if (roboData == null)
            {
                Debug.LogWarning(
                    $"Nenhum RoboData foi definido para {gameObject.name}."
                );

                return;
            }

            MoveSpeed = roboData.velocidadeInicial;
            SprintSpeed = roboData.velocidadeSprint;
            forcaAtual = roboData.forcaEmpurrao;

            transform.localScale =
                Vector3.one * roboData.tamanho;

            if (roboRenderer != null)
            {
                roboRenderer.material.color = usarCorSecundaria ? roboData.corSecundaria
                    : roboData.corPrincipal;
            }
            else
            {
                Debug.LogWarning(
                    $"Nenhum SkinnedMeshRenderer foi definido em {gameObject.name}."
                );
            }
        }

        #endregion
    }
}