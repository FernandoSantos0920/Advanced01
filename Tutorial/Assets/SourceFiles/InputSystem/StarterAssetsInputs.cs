using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		[Header("Input Values IFRN")]
		
		private PlayerInput playerInput;
		

#if ENABLE_INPUT_SYSTEM
		

		
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if(cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}
#endif

			private void Awake()
	{
		SetCursorState(cursorLocked);
		Cursor.visible = false;

		
	}

	private void Start()
	{
		playerInput = GetComponent<PlayerInput>();
	
		//Move
		playerInput.actions.FindAction("Move").performed += OnMovePerformed;
		playerInput.actions.FindAction("Move").canceled += context => move = Vector2.zero;
		
		//Look
		playerInput.actions.FindAction("Look").performed += OnLookPerformed;
		playerInput.actions.FindAction("Move").canceled += context => look = Vector2.zero;
		
		//Jump
		playerInput.actions.FindAction("Jump").started += context => jump = true;
		playerInput.actions.FindAction("Jump").canceled += context => jump = false;
		
		
		//Sprint
		playerInput.actions.FindAction("Sprint").started += context => sprint = true;
		playerInput.actions.FindAction("Sprint").canceled += context => sprint = false;
		
		//playerInput.actions.FindAction("Interact").started += context => InteractOM = true;
	}


	private void OnMovePerformed(InputAction.CallbackContext context)
	{
		move = context.action.ReadValue<Vector2>();
	}

	private void OnLookPerformed(InputAction.CallbackContext context)
	{
		look = context.action.ReadValue<Vector2>();
	}
	
	#region Old Input System

	public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !newState;  
			

		}
		
		#endregion
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
		
	}
	
}