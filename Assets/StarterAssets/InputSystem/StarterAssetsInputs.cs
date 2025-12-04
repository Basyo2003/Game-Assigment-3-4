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

		// Allows external systems to temporarily disable all player input (e.g. during dialogue)
		private bool _inputEnabled = true;
		public bool InputEnabled => _inputEnabled;

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

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


		public void MoveInput(Vector2 newMoveDirection)
		{
			move = _inputEnabled ? newMoveDirection : Vector2.zero;
		} 

		public void LookInput(Vector2 newLookDirection)
		{
			look = _inputEnabled ? newLookDirection : Vector2.zero;
		}

		public void JumpInput(bool newJumpState)
		{
			jump = _inputEnabled && newJumpState;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = _inputEnabled && newSprintState;
		}

		public void SetInputEnabled(bool enabled)
		{
			_inputEnabled = enabled;

			if (!enabled)
			{
				move = Vector2.zero;
				look = Vector2.zero;
				jump = false;
				sprint = false;
			}
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		private void SetCursorState(bool newState)
		{
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
		}

		// Legacy keyboard fallback: if the new Input System callbacks are not wired up,
		// allow WASD / arrow keys + Space + LeftShift to control movement, jump and sprint.
		// This is intentionally permissive so the player can still move while debugging input issues.
		private void Update()
		{
			if (!_inputEnabled) return;

			// Read WASD / arrow keys
			Vector2 kb = Vector2.zero;
			if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) kb.y += 1f;
			if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) kb.y -= 1f;
			if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) kb.x -= 1f;
			if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) kb.x += 1f;

			if (kb != Vector2.zero)
			{
				// normalize so diagonal movement isn't faster
				move = kb.normalized;
			}

			// Jump and sprint fallbacks
			jump = Input.GetKey(KeyCode.Space);
			sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		}
	}
	
}