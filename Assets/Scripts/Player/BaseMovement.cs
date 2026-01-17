using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBaseMovement : PlayerState
{
    Rigidbody playerRB;

    [Header("Walk Parameters")]
    [SerializeField] float walkSpeed;
    [SerializeField] float maxWalkSpeed;
    [SerializeField] float stepBackDistance;

    [Header("Jump Parameters")]
    [SerializeField] float jumpStrength;
    [SerializeField] CollisionCheck groundedCheck;

    [Header("Camera Parameters")]
    [SerializeField] private CameraController camController;

    [Header("Model Parameters")]
    [SerializeField] private Transform characterModel;

    private int xMove;
    private int zMove;

    private GameObject camAlignObj;
    private Transform camAlign;

    public override void SwitchPlayerState()
    {
        MeshRenderer renderer = characterModel.GetComponent<MeshRenderer>();
        renderer.material = controller.f_mat;

        CameraManager.SetFlight();

        controller._fm.enabled = true;
        controller._pbm.enabled = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();

        camAlignObj = new GameObject("camAlignObj");
        camAlign = camAlignObj.GetComponent<Transform>();
    }

    private void OnEnable()
    {
        if (playerRB != null)
        {
            playerRB.useGravity = true;
            playerRB.linearDamping = 1f;

            playerRB.angularDamping = 1f;
            playerRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        }

        CurrentState.state = CurrentState.States.Grounded;
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        // Ground Movement
        playerRB.AddRelativeForce(MovementInput() * walkSpeed, ForceMode.VelocityChange);
        Vector3 horizontalVel = new Vector3(playerRB.linearVelocity.x, 0, playerRB.linearVelocity.z);
        Vector3 clampedXZ = Vector3.ClampMagnitude(horizontalVel, maxWalkSpeed);

        playerRB.linearVelocity = new Vector3(clampedXZ.x, playerRB.linearVelocity.y, clampedXZ.z);

        // Jump
        if (Input.GetKeyDown(KeyCode.Space) && groundedCheck.isColliding) playerRB.AddRelativeForce(Vector3.up * jumpStrength, ForceMode.Impulse);

        // Rotate On Movement
        Vector3 walkVel = Vector3.Scale(playerRB.linearVelocity, new Vector3(1f, 0f, 1f));
        if (walkVel.magnitude > 0.01f) RotateCharacterOnMovement();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            SwitchPlayerState();
        }
    }

    private Vector3 MovementInput()
    {
        xMove = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        zMove = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);

        // Get yaw (ignore camera pitch & roll)
        float yaw = camController.holder.eulerAngles.y;
        Quaternion yawRotation = Quaternion.Euler(0, yaw, 0);

        // Input direction relative to camera
        Vector3 inputDir = new Vector3(xMove, 0, zMove).normalized;
        return yawRotation * inputDir;
    }

    private void RotateCharacterOnMovement()
    {
        if (Mathf.Approximately(characterModel.rotation.eulerAngles.y, camController.holder.eulerAngles.y)) return;
        
        float newYRotation = Mathf.MoveTowardsAngle(characterModel.rotation.eulerAngles.y, camController.holder.eulerAngles.y, 3f);
        characterModel.rotation = Quaternion.Euler(characterModel.rotation.eulerAngles.x, newYRotation, characterModel.rotation.eulerAngles.z);
    }
}
