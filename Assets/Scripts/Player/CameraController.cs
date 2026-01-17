using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;

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
        Debug.Log("Mouse X : " + mouseX);
        Debug.Log("Mouse Y : " + mouseY);
        mouseX = Input.GetAxisRaw("Mouse X") * sensX;
        mouseY = Input.GetAxisRaw("Mouse Y") * sensY;
        
        holder.position = playerTransform.position;

        switch (CurrentState.state)
        {
            case CurrentState.States.Grounded:
                xRotHolder -= mouseY;
                yRotHolder += mouseX;

                holder.rotation = Quaternion.Euler(xRotHolder, yRotHolder, 0);
                anchor.localRotation = Quaternion.Euler(0, 0, 0);

                sensX = xSensitivity;
                sensY = ySensitivity;

                lastX = xRotHolder;
                lastY = yRotHolder;
                Debug.Log("States.Grounded");
                break;
            case CurrentState.States.Flying:
                xRotAnchor -= mouseY;
                yRotAnchor += mouseX;

                anchor.localRotation = Quaternion.Euler(xRotAnchor, yRotAnchor, 0);
                // Clamp rotation
                xRotAnchor = Mathf.Clamp(xRotAnchor, lastX - maxFlightRotation, lastX + maxFlightRotation);
                yRotAnchor = Mathf.Clamp(yRotAnchor, lastY - maxFlightRotation, lastY + maxFlightRotation);

                // --- Compute proximity to clamp edges ---
                float xT = Mathf.InverseLerp(lastX - maxFlightRotation, lastX + maxFlightRotation, xRotHolder);
                float yT = Mathf.InverseLerp(lastY - maxFlightRotation, lastY + maxFlightRotation, yRotHolder);

                float edgeProximity = Mathf.Max(Mathf.Abs(xT - 0.5f), Mathf.Abs(yT - 0.5f)) * 2f; // 0=center, 1=edge
                float edgeProximityX = (xT - 0.5f) * 2f; // 0=center, 1=edge
                float edgeProximityY = (yT - 0.5f) * 2f; // 0=center, 1=edge

                sensX = Mathf.Lerp(0f, xSensitivity, 1f - edgeProximity);
                sensY = Mathf.Lerp(0f, ySensitivity, 1f - edgeProximity);

                //TurnCameraOnFlight(edgeProximityX, edgeProximityY);

                Debug.Log($"States.Flying | SensX: {sensX:F2}");
                break;
        }
    }

    private void TurnCameraOnFlight(float x, float y)
    {
        double holderX = (double)x;
        double holderY = (double)y;

        holder.localEulerAngles += new Vector3((float)holderX * 0.75f, (float)holderY * 0.75f, 0);
    }
    public void CenterMouse()
    {
        Vector2 center = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Mouse.current.WarpCursorPosition(center);
        InputState.Change(Mouse.current.position, center);
    }
    public void SetToGroundMode()
    {

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
    static CameraController cameraController 
    {
        get { return cameraController; }
        set { if (isInitialized == false) cameraController = value; } 
    }
    static bool isInitialized = false;

    public static void Initialize(CameraController cc) { cameraController = cc; isInitialized = true; }
    public static void SetGround() => cameraController.SetToGroundMode();
    public static void SetFlight() => cameraController.SetToFlightMode();
}
