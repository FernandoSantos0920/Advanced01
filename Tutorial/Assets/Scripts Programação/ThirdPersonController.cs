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

        [Tooltip("Rotation speed of the Player when pressing A/D")]
        public float RotationSpeed = 180f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Camera")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degrees to override the camera")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;

        [Header("Camera Rotation")]
        [Tooltip("Camera follows the Player rotation automatically")]
        public bool LockCameraRotation = true;

        [Tooltip("Sensitivity used when controlling the camera with Look")]
        public Vector2 LookSensitivity = new Vector2(7.5f, 5.0f);

        // Cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // Camera starting position and rotation
        private Vector3 _cameraStartingPosition;
        private Quaternion _cameraStartingRotation;

        // Respawn
        public bool IsRespawning { get; set; } = false;

        // Player
        private float _speed;
        private float _animationBlend;

        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // Timeout deltatime
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

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

        private void Start()
        {
            playerID = GetComponent<PlayerIdentifier>();

            _hasAnimator = TryGetComponent(out _animator);

            _controller = GetComponent<CharacterController>();

            _input = GetComponent<StarterAssetsInputs>();

#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError(
                "Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix this"
            );
#endif

            AssignAnimationIDs();

            // A câmera começa alinhada com o Player
            _cinemachineTargetYaw = transform.eulerAngles.y;
            _cinemachineTargetPitch = 0f;

            // Salva posição e rotação inicial da câmera
            _cameraStartingPosition =
                CinemachineCameraTarget.transform.position;

            _cameraStartingRotation =
                CinemachineCameraTarget.transform.rotation;

            // Reset dos timeouts
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed =
                Animator.StringToHash("Speed");

            _animIDGrounded =
                Animator.StringToHash("Grounded");

            _animIDJump =
                Animator.StringToHash("Jump");

            _animIDFreeFall =
                Animator.StringToHash("FreeFall");

            _animIDMotionSpeed =
                Animator.StringToHash("MotionSpeed");
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
                _animator.SetBool(
                    _animIDGrounded,
                    Grounded
                );
            }
        }

        private void CameraRotation()
        {
            // ==========================================
            // RESPAWN
            // ==========================================

            if (IsRespawning)
            {
                _cinemachineTargetYaw =
                    transform.eulerAngles.y;

                _cinemachineTargetPitch = 0f;

                CinemachineCameraTarget.transform.position =
                    _cameraStartingPosition;

                CinemachineCameraTarget.transform.rotation =
                    Quaternion.Euler(
                        0f,
                        _cinemachineTargetYaw,
                        0f
                    );

                IsRespawning = false;

                return;
            }

            // ==========================================
            // CÂMERA ATRÁS DO PLAYER
            // ==========================================

            if (LockCameraRotation)
            {
                // A câmera acompanha a rotação horizontal
                // do Player.
                _cinemachineTargetYaw =
                    transform.eulerAngles.y;

                // Atualmente a câmera não usa Look.
                _cinemachineTargetPitch = 0f;
            }

            _cinemachineTargetPitch = ClampAngle(
                _cinemachineTargetPitch,
                BottomClamp,
                TopClamp
            );

            CinemachineCameraTarget.transform.rotation =
                Quaternion.Euler(
                    _cinemachineTargetPitch +
                    CameraAngleOverride,

                    _cinemachineTargetYaw,

                    0f
                );
        }

        private void Move()
        {
            float targetSpeed =
                _input.sprint
                    ? SprintSpeed
                    : MoveSpeed;

            // ==========================================
            // VELOCIDADE
            // ==========================================

            if (_input.move == Vector2.zero)
            {
                targetSpeed = 0.0f;
            }

            float currentHorizontalSpeed =
                new Vector3(
                    _controller.velocity.x,
                    0.0f,
                    _controller.velocity.z
                ).magnitude;

            float speedOffset = 0.1f;

            float inputMagnitude =
                _input.analogMovement
                    ? _input.move.magnitude
                    : 1f;

            if (
                currentHorizontalSpeed <
                targetSpeed - speedOffset ||

                currentHorizontalSpeed >
                targetSpeed + speedOffset
            )
            {
                _speed = Mathf.Lerp(
                    currentHorizontalSpeed,
                    targetSpeed * inputMagnitude,
                    Time.deltaTime *
                    SpeedChangeRate
                );

                _speed =
                    Mathf.Round(
                        _speed * 1000f
                    ) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend =
                Mathf.Lerp(
                    _animationBlend,
                    targetSpeed,
                    Time.deltaTime *
                    SpeedChangeRate
                );

            if (_animationBlend < 0.01f)
            {
                _animationBlend = 0f;
            }

            // ==========================================
            // A/D = ROTACIONAR PLAYER
            // ==========================================

            if (Mathf.Abs(_input.move.x) > _threshold)
            {
                float rotation =
                    _input.move.x *
                    RotationSpeed *
                    Time.deltaTime;

                transform.Rotate(
                    0f,
                    rotation,
                    0f
                );
            }

            // ==========================================
            // W/S = ANDAR PARA FRENTE / TRÁS
            // ==========================================

            Vector3 targetDirection =
                transform.forward *
                _input.move.y;

            _controller.Move(
                targetDirection.normalized *
                (_speed * Time.deltaTime)

                +

                new Vector3(
                    0.0f,
                    _verticalVelocity,
                    0.0f
                ) *
                Time.deltaTime
            );

            // ==========================================
            // ANIMAÇÕES
            // ==========================================

            if (_hasAnimator)
            {
                _animator.SetFloat(
                    _animIDSpeed,
                    _animationBlend
                );

                _animator.SetFloat(
                    _animIDMotionSpeed,
                    inputMagnitude
                );
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta =
                    FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(
                        _animIDJump,
                        false
                    );

                    _animator.SetBool(
                        _animIDFreeFall,
                        false
                    );
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (
                    _input.jump &&
                    _jumpTimeoutDelta <= 0.0f
                )
                {
                    _verticalVelocity =
                        Mathf.Sqrt(
                            JumpHeight *
                            -2f *
                            Gravity
                        );

                    if (_hasAnimator)
                    {
                        _animator.SetBool(
                            _animIDJump,
                            true
                        );
                    }
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -=
                        Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta =
                    JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -=
                        Time.deltaTime;
                }
                else
                {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(
                            _animIDFreeFall,
                            true
                        );
                    }
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity +=
                    Gravity *
                    Time.deltaTime;
            }
        }

        private static float ClampAngle(
            float lfAngle,
            float lfMin,
            float lfMax)
        {
            if (lfAngle < -360f)
            {
                lfAngle += 360f;
            }

            if (lfAngle > 360f)
            {
                lfAngle -= 360f;
            }

            return Mathf.Clamp(
                lfAngle,
                lfMin,
                lfMax
            );
        }

        public void ResetCameraRotation(
            float targetYaw)
        {
            _cinemachineTargetYaw =
                targetYaw;

            _cinemachineTargetPitch =
                0f;

            CinemachineCameraTarget.transform.rotation =
                Quaternion.Euler(
                    0f,
                    _cinemachineTargetYaw,
                    0f
                );

            Debug.Log(
                $"Camera Yaw reset to {targetYaw} degrees."
            );
        }

        #region Observer do Player em acao

        private void OnEnable()
        {
            PlayerOM.ChangeCoins +=
                OnCoinsChanged;
        }

        private void OnDisable()
        {
            PlayerOM.ChangeCoins -=
                OnCoinsChanged;
        }

        private void OnCoinsChanged(
            PlayerIdentifier.Player player,
            int quantidade)
        {
            if (player != playerID.ID)
            {
                return;
            }

            MoveSpeed += 1f;
            SprintSpeed += 1f;
        }

        #endregion

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen =
                new Color(
                    0.0f,
                    1.0f,
                    0.0f,
                    0.35f
                );

            Color transparentRed =
                new Color(
                    1.0f,
                    0.0f,
                    0.0f,
                    0.35f
                );

            Gizmos.color =
                Grounded
                    ? transparentGreen
                    : transparentRed;

            Gizmos.DrawSphere(
                new Vector3(
                    transform.position.x,
                    transform.position.y -
                    GroundedOffset,
                    transform.position.z
                ),
                GroundedRadius
            );
        }

        private void OnFootstep(
            AnimationEvent animationEvent)
        {
            if (
                animationEvent.animatorClipInfo.weight >
                0.5f
            )
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(
                        0,
                        FootstepAudioClips.Length
                    );

                    AudioSource.PlayClipAtPoint(
                        FootstepAudioClips[index],
                        transform.TransformPoint(
                            _controller.center
                        ),
                        FootstepAudioVolume
                    );
                }
            }
        }

        private void OnLand(
            AnimationEvent animationEvent)
        {
            if (
                animationEvent.animatorClipInfo.weight >
                0.5f
            )
            {
                AudioSource.PlayClipAtPoint(
                    LandingAudioClip,
                    transform.TransformPoint(
                        _controller.center
                    ),
                    FootstepAudioVolume
                );
            }
        }
    }
}