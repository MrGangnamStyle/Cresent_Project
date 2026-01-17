using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

namespace CresentProject.Player
{
    public class CameraController : MonoBehaviour
    {
        public float xSensitivity;
        public float ySensitivity;
        private float sensX;
        private float sensY;

        public Transform holder;
        public Transform anchor;
        [SerializeField] private Transform playerTransform;
        [SerializeField] private Transform playerModel;

        [HideInInspector] public float xRotHolder;
        [HideInInspector] public float yRotHolder;
        [HideInInspector] public float xRotAnchor;
        [HideInInspector] public float yRotAnchor;


        [SerializeField] private float maxFlightRotation;
        private float lastX;
        private float lastY;

        float mouseX;
        float mouseY;

        void Start()
        {
            sensX = xSensitivity;
            sensY = ySensitivity;

            Cursor.lockState = CursorLockMode.Locked;
            CurrentState.state = CurrentState.States.Grounded;

            CameraManager.Initialize(this);
        }

        private void Update()
        {
            //Debug.Log("Mouse X : " + mouseX);
            //Debug.Log("Mouse Y : " + mouseY);
            mouseX = Input.GetAxisRaw("Mouse X") * sensX;
            mouseY = Input.GetAxisRaw("Mouse Y") * sensY;

            holder.position = playerTransform.position;

            xRotHolder -= mouseY;
            yRotHolder += mouseX;

            //holder.rotation = Quaternion.Euler(xRotHolder, yRotHolder, 0);
            switch (CurrentState.state)
            {
                case CurrentState.States.Grounded:
                    xRotHolder = Mathf.Clamp(xRotHolder, -maxFlightRotation, maxFlightRotation);
                    holder.rotation = Quaternion.Euler(xRotHolder, yRotHolder, 0);

                    Debug.Log("States.Grounded");
                    break;
                case CurrentState.States.Flying:
                    xRotAnchor -= mouseY;
                    yRotAnchor += mouseX;

                    anchor.localRotation = Quaternion.Euler(xRotAnchor, yRotAnchor, 0);
                    // Clamp rotation
                    xRotAnchor = Mathf.Clamp(xRotAnchor, -maxFlightRotation, maxFlightRotation);
                    yRotAnchor = Mathf.Clamp(yRotAnchor, -maxFlightRotation, maxFlightRotation);

                    // --- Compute proximity to clamp edges ---
                    float xT = Mathf.InverseLerp(-maxFlightRotation, maxFlightRotation, xRotAnchor);
                    float yT = Mathf.InverseLerp(-maxFlightRotation, maxFlightRotation, yRotAnchor);

                    float edgeProximity = Mathf.Max(Mathf.Abs(xT - 0.5f), Mathf.Abs(yT - 0.5f)) * 2f; // 0=center, 1=edge
                    float edgeProximityX = (xT - 0.5f) * 2f; // 0=center, 1=edge
                    float edgeProximityY = (yT - 0.5f) * 2f; // 0=center, 1=edge

                    sensX = SetCameraSlowness(edgeProximity, xSensitivity, sensX);
                    sensY = SetCameraSlowness(edgeProximity, xSensitivity, sensY);

                    //Debug.Log($"States.Flying | SensX: {sensX:F2}");
                    break;
            }   
        }

        private float SetCameraSlowness(float edgeProx, float sens, float curSens)
        {
            float result = Mathf.Lerp(sens / 10f, sens, 1f - edgeProx);
            if (result < curSens) { Debug.Log("Lesser"); return result; }
            else { Debug.Log("Greater"); return sens; }
        }

        public void CenterMouse()
        {
            Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
            Mouse.current.WarpCursorPosition(center);
            InputState.Change(Mouse.current.position, center);
        }
        public void SetToGroundMode()
        {
            anchor.localRotation = Quaternion.Euler(0, 0, 0);

            sensX = xSensitivity;
            sensY = ySensitivity;
        }
        public void SetToFlightMode()
        {
            CenterMouse();

            xRotAnchor = 0;
            yRotAnchor = 0;
        }
    }

    public static class CameraManager
    {
        static CameraController cameraController { get; set; }

        public static void Initialize(CameraController cc) { cameraController = cc; }
        public static void SetGround() => cameraController.SetToGroundMode();
        public static void SetFlight() => cameraController.SetToFlightMode();
    }
}
