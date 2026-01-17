using UnityEngine;

public class FlightMovement : PlayerState
{
    Rigidbody playerRB;

    [Header("Standard Parameters")]
    [SerializeField] private float flightAcceleration;
    [SerializeField] private float maxFlightSpeed;
    [SerializeField] private float torqueAcceleration;
    [SerializeField] private float maxTorqueSpeed;
    [SerializeField] private Transform characterModel;

    [Header("Camera Parameters")]
    [SerializeField] private CameraController camController;

    private int xMove;
    private int zMove;

    private int xRotation;
    private int zRotation;

    public override void SwitchPlayerState()
    {
        Debug.Log("2");
        MeshRenderer renderer = characterModel.GetComponent<MeshRenderer>();
        renderer.material = controller.g_mat;

        CameraManager.SetGround();

        controller._pbm.enabled = true;
        controller._fm.enabled = false;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        playerRB = GetComponent<Rigidbody>();
        this.enabled = false;
    }

    private void OnEnable()
    {
        if (playerRB != null)
        {
            playerRB.useGravity = false;
            playerRB.linearDamping = 0.2f;

            playerRB.angularDamping = 1f;
            playerRB.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
        }

        CurrentState.state = CurrentState.States.Flying;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            SwitchPlayerState();
        }

        playerRB.AddForce(MovementInput() * flightAcceleration, ForceMode.Acceleration);

        Vector3 walkVel = Vector3.Scale(playerRB.linearVelocity, new Vector3(1f, 0f, 1f));
        if (walkVel.magnitude > 0.01f) RotateCharacterOnMovement();

        Debug.Log(playerRB.angularVelocity);
    }

    private Vector3 MovementInput()
    {
        xMove = (Input.GetKey(KeyCode.D) ? 1 : 0) - (Input.GetKey(KeyCode.A) ? 1 : 0);
        zMove = (Input.GetKey(KeyCode.W) ? 1 : 0) - (Input.GetKey(KeyCode.S) ? 1 : 0);

        // Get yaw (ignore camera pitch & roll)
        float yaw = camController.holder.eulerAngles.y;
        Quaternion yawRotation = camController.transform.rotation;

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

    private void FlightTurning()
    {
        if (playerRB.linearVelocity.sqrMagnitude > 10f)
        {

        }
        else
        {
            xRotation = 0;
            zRotation = 0;
        }
    }
}
