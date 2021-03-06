using Framework.UI;
using Smbunity;
using UnityEngine;

namespace FreeCam
{
    internal class FreeCamController : MonoBehaviour
    {
        private MouseCursorManager _cursorManager;
        private bool _looking;
        private float _monkeyDistance = 3f;

        private bool _originalHUDVisibility;

        private Player _player;
        private UIManager _uiManager;

        private void Awake()
        {
            // Set the player object for future use
            _player = FindObjectOfType<Player>();

            // Hide the HUD
            if (Main.HideHUD)
            {
                _uiManager = FindObjectOfType<UIManager>();
                if (_uiManager != null)
                {
                    // In case the HUD was not visible before don't show it again by accident later on
                    _originalHUDVisibility = _uiManager.DisplayHUD;
                    _uiManager.DisplayHUD = false;
                }
            }

            // Hide the cursor
            _cursorManager = FindObjectOfType<MouseCursorManager>();
            if (_cursorManager != null)
                _cursorManager.CursorUI.Sleep();

            // Play the sound
            Sound.PlayOneShot(sound_id.cue.se_com_select);
        }

        private void Update()
        {
            // Set the speed values
            var fastMode = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            var movementSpeed = fastMode ? Main.FastMovementSpeed : Main.MovementSpeed;
            var tiltSpeed = fastMode ? Main.FastTiltSpeed : Main.TiltSpeed;
            var zoomSpeed = fastMode ? Main.FastZoomSpeed : Main.ZoomSpeed;

            // Movement
            if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
                transform.position -= transform.right * movementSpeed * Time.unscaledDeltaTime;
            if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
                transform.position += transform.right * movementSpeed * Time.unscaledDeltaTime;

            if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                transform.position += transform.forward * movementSpeed * Time.unscaledDeltaTime;
            if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
                transform.position -= transform.forward * movementSpeed * Time.unscaledDeltaTime;

            if (Input.GetKey(KeyCode.Q))
                transform.position += transform.up * movementSpeed * Time.unscaledDeltaTime;
            if (Input.GetKey(KeyCode.E))
                transform.position -= transform.up * movementSpeed * Time.unscaledDeltaTime;

            if (Input.GetKey(KeyCode.R))
                transform.position += Vector3.up * movementSpeed * Time.unscaledDeltaTime;
            if (Input.GetKey(KeyCode.F))
                transform.position -= Vector3.up * movementSpeed * Time.unscaledDeltaTime;

            // Tilting
            if (Input.GetKey(KeyCode.Z))
                transform.localEulerAngles += new Vector3(0f, 0f, tiltSpeed);
            if (Input.GetKey(KeyCode.X))
                transform.localEulerAngles -= new Vector3(0f, 0f, tiltSpeed);

            // Zooming with the scroll wheel
            var mouseAxis = Input.GetAxis("Mouse ScrollWheel");

            if (Input.GetKey(KeyCode.Space) && _player != null)
            {
                if (mouseAxis != 0)
                    _monkeyDistance += mouseAxis * zoomSpeed * Time.unscaledDeltaTime;

                _player.transform.position = transform.position + transform.rotation * new Vector3(0f, 0f, _monkeyDistance);
            }
            else if (mouseAxis != 0)
            {
                transform.position += transform.forward * mouseAxis * zoomSpeed * Time.unscaledDeltaTime;
            }

            // Looking with the mouse
            if (_looking)
            {
                var newRotationX = transform.localEulerAngles.y +
                                   Input.GetAxis("Mouse X") * Main.FreeLookSensitivity * Time.unscaledDeltaTime;
                var newRotationY = transform.localEulerAngles.x -
                                   Input.GetAxis("Mouse Y") * Main.FreeLookSensitivity * Time.unscaledDeltaTime;
                transform.localEulerAngles = new Vector3(newRotationY, newRotationX, transform.localEulerAngles.z);
            }

            if (Input.GetKeyDown(KeyCode.Mouse1))
            {
                _looking = true;

                // Lock the cursor in the center of the screen
                Cursor.lockState = CursorLockMode.Locked;
            }
            else if (Input.GetKeyUp(KeyCode.Mouse1))
            {
                _looking = false;

                // Unlock the cursor
                Cursor.lockState = CursorLockMode.None;
            }
        }

        // When the controller is destroyed.
        private void OnDisable()
        {
            // Unlock the cursor in case it's locked
            Cursor.lockState = CursorLockMode.None;

            // Show the UI elements
            if (_uiManager != null)
                _uiManager.DisplayHUD = _originalHUDVisibility;
            if (_cursorManager != null)
                _cursorManager.CursorUI.Wake();

            // Play the sound
            Sound.PlayOneShot(sound_id.cue.se_com_cancel);
        }
    }
}