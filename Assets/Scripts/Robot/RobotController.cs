using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class RobotController : MonoBehaviour
{
    [Header("NavMesh")]
    [SerializeField] private NavMeshAgent agent;

    [Header("Wheels (Transforms only)")]
    [SerializeField] private Transform frontLeft;
    [SerializeField] private Transform frontRight;
    [SerializeField] private Transform rearLeft;
    [SerializeField] private Transform rearRight;

    [Header("Visual settings")]
    [SerializeField] private float maxWheelTurnAngle = 30f;
    [SerializeField] private float wheelRotateSpeed = 360f;
    [SerializeField] private float bodyTurnSpeed = 4f;
    [SerializeField] private float steerSmooth = 4f;

    [Header("Stop settings")]
    private string stopTag = "Stop";
    [SerializeField] private float minSpeedToRotate = 0.2f;

    private float currentSteer;
    private bool isBlocked;       

    private void Start()
    {
        agent.updateRotation = false;
    }

    private void Update()
    {
        HandleInput();
        HandleMovement();
        AnimateWheels();
    }

    // -------------------- INPUT --------------------

    private void HandleInput()
    {
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            Ray ray = Camera.main.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                agent.SetDestination(hit.point);
            }
        }
    }

    // -------------------- MOVEMENT --------------------

    private void HandleMovement()
    {
        if (isBlocked || agent.velocity.magnitude < minSpeedToRotate)
            return;

        RotateBodySmoothly();
    }

    private void RotateBodySmoothly()
    {
        Vector3 moveDir = agent.velocity.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            bodyTurnSpeed * Time.deltaTime
        );
    }

    // -------------------- WHEELS --------------------

    private void AnimateWheels()
    {
        float speed = agent.velocity.magnitude;

        RotateWheel(frontLeft, speed);
        RotateWheel(frontRight, speed);
        RotateWheel(rearLeft, speed);
        RotateWheel(rearRight, speed);

        SteerFrontWheels();
    }

    private void RotateWheel(Transform wheel, float speed)
    {
        if (!wheel) return;
        wheel.Rotate(Vector3.right, speed * wheelRotateSpeed * Time.deltaTime, Space.Self);
    }

    private void SteerFrontWheels()
    {
        if (!agent.hasPath) return;

        Vector3 localTarget = transform.InverseTransformPoint(agent.steeringTarget);
        float targetSteer = Mathf.Clamp(localTarget.x / Mathf.Max(localTarget.magnitude, 0.1f), -1f, 1f);

        currentSteer = Mathf.Lerp(currentSteer, targetSteer, steerSmooth * Time.deltaTime);
        float steerAngle = currentSteer * maxWheelTurnAngle;

        SetWheelSteer(frontLeft, steerAngle);
        SetWheelSteer(frontRight, steerAngle);
    }

    private void SetWheelSteer(Transform wheel, float angle)
    {
        if (!wheel) return;

        Vector3 euler = wheel.localEulerAngles;
        wheel.localRotation = Quaternion.Euler(euler.x, angle, euler.z);
    }

    // -------------------- STOP LOGIC --------------------

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(stopTag))
        {
            isBlocked = true;
            agent.isStopped = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(stopTag))
        {
            isBlocked = false;
            agent.isStopped = false;
        }
    }

    
}
