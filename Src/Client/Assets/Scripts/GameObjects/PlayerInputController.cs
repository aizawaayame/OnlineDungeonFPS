using UnityEngine;
using Utilities;

namespace GameObjects
{
    public class PlayerInputController : MonoSingleton<PlayerInputController>
    {

        #region Public Properties  

        [Tooltip("Sensitivity multiplier for moving the camera around")]
        public float LookSensitivity = 1f;
        
        [Tooltip("Used to flip the vertical input axis")]
        public bool InvertYAxis = false;

        [Tooltip("Used to flip the horizontal input axis")]
        public bool InvertXAxis = false;
        
        #endregion

        #region Private Methods

        

        #endregion
        void Start()
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        float GetMouseLookAxis(string mouseInputName)
        {
            if (CanProcessInput())
            {
                float i = Input.GetAxisRaw(mouseInputName);
                if (InvertYAxis)
                {
                    i *= -1f;
                }
                i *= LookSensitivity;
                Debug.Log($"鼠标输入{i}");
                return i * 0.01f;
            }
            return 0f;
        }
        #region Public Methods

        public bool CanProcessInput()
        {
            return Cursor.lockState == CursorLockMode.Locked;
        }

        public Vector3 GetMoveInput()
        {
            if (CanProcessInput())
            {
                Vector3 move = new Vector3(
                    Input.GetAxisRaw("Horizontal"),
                    0f,
                    Input.GetAxisRaw("Vertical"));
                move = Vector3.ClampMagnitude(move, 1);
                Debug.Log($"移动输入{move.x},{move.y},{move.z}");
                return move;
            }
            return Vector3.zero;
        }

        public float GetLookInputsHorizontal()
        {
            return GetMouseLookAxis("Mouse X");
        }

        public float GetLookInputsVertical()
        {
            return GetMouseLookAxis("Mouse Y");
        }

        public bool GetJumpInputDown()
        {
            if (CanProcessInput())
            {
                return Input.GetButtonDown("Jump");
            }
            return false;
        }

        public bool GetJumpInputHeld()
        {
            if (CanProcessInput())
            {
                return Input.GetButton("Jump");
            }
            return false;
        }

        public bool GetSprintInputHeld()
        {
            if (CanProcessInput())
            {
                return Input.GetButton("Sprint");
            }
            return false;
        }
        #endregion
    }
}
