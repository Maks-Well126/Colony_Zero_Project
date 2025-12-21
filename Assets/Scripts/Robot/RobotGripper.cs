using UnityEngine;

public class RobotGripper : MonoBehaviour
{
    private enum GripperState
    {
        Idle,
        LoweringArm,
        ClosingClaws,
        Holding
    }

    [Header("Arm")]
    [SerializeField] private Transform armJoint;   // сустав рукава
    [SerializeField] private float armDownAngle = -45f;
    [SerializeField] private float armUpAngle = 0f;
    [SerializeField] private float armSpeed = 40f;

    [Header("Claws")]
    [SerializeField] private Transform leftClaw;
    [SerializeField] private Transform rightClaw;
    [SerializeField] private float openAngle = 30f;
    [SerializeField] private float closeAngle = 5f;
    [SerializeField] private float clawSpeed = 60f;

    [Header("Grab")]
    [SerializeField] private string grabbableTag = "Grabbable";

    private GripperState state = GripperState.Idle;
    private bool objectDetected;
    private Rigidbody grabbedBody;
    private FixedJoint grabJoint;

    // -------------------- UPDATE --------------------

    private void Update()
    {
        // пример управления
        if (Input.GetKeyDown(KeyCode.E) && state == GripperState.Idle)
        {
            state = GripperState.LoweringArm;
        }

        ProcessState();
    }

    // -------------------- STATE MACHINE --------------------

    private void ProcessState()
    {
        switch (state)
        {
            case GripperState.LoweringArm:
                LowerArm();
                break;

            case GripperState.ClosingClaws:
                CloseClaws();
                break;

            case GripperState.Holding:
                HoldObject();
                break;
        }
    }

    // -------------------- ARM --------------------

    private void LowerArm()
    {
        RotateArm(armDownAngle);

        if (Mathf.Abs(GetArmAngle() - armDownAngle) < 1f || objectDetected)
        {
            state = GripperState.ClosingClaws;
        }
    }

    private void RotateArm(float targetAngle)
    {
        Quaternion target = Quaternion.Euler(targetAngle, 0, 0);
        armJoint.localRotation = Quaternion.RotateTowards(
            armJoint.localRotation,
            target,
            armSpeed * Time.deltaTime
        );
    }

    private float GetArmAngle()
    {
        return armJoint.localEulerAngles.x > 180
            ? armJoint.localEulerAngles.x - 360
            : armJoint.localEulerAngles.x;
    }

    // -------------------- CLAWS --------------------

    private void CloseClaws()
    {
        RotateClaw(leftClaw, closeAngle);
        RotateClaw(rightClaw, -closeAngle);
    }

    private void RotateClaw(Transform claw, float angle)
    {
        Quaternion target = Quaternion.Euler(angle, 0, 0);
        claw.localRotation = Quaternion.RotateTowards(
            claw.localRotation,
            target,
            clawSpeed * Time.deltaTime
        );
    }

    // -------------------- PHYSICS GRAB --------------------

    private void OnCollisionEnter(Collision collision)
    {
        if (state != GripperState.ClosingClaws) return;
        if (!collision.collider.CompareTag(grabbableTag)) return;

        Rigidbody rb = collision.rigidbody;
        if (!rb) return;

        Grab(rb);
    }

    private void Grab(Rigidbody rb)
    {
        grabbedBody = rb;

        grabJoint = gameObject.AddComponent<FixedJoint>();
        grabJoint.connectedBody = rb;
        grabJoint.breakForce = Mathf.Infinity;
        grabJoint.breakTorque = Mathf.Infinity;

        state = GripperState.Holding;
    }

    private void HoldObject()
    {
        // держим объект
        RotateArm(armUpAngle);
    }

    // -------------------- OPTIONAL RELEASE --------------------

    public void Release()
    {
        if (grabJoint)
            Destroy(grabJoint);

        grabbedBody = null;
        state = GripperState.Idle;
    }
}
