using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class RobotController : MonoBehaviour
{    

    [Header("NavMesh")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform firstDestinationPoint;
    [SerializeField] private Transform secondDestinationPoint;

    [Header("Wheels")]
    [SerializeField] private Transform frontLeft;
    [SerializeField] private Transform frontRight;
    [SerializeField] private Transform rearLeft;
    [SerializeField] private Transform rearRight;

    [Header("Visual Settings")]
    [SerializeField] private float maxWheelTurnAngle = 30f;
    [SerializeField] private float wheelRotateSpeed = 360f;
    [SerializeField] private float bodyTurnSpeed = 4f;
    [SerializeField] private float steerSmooth = 4f;

    [Header("Interaction")]
    [SerializeField] private Transform player;
    [SerializeField] private GameObject interactCanvas;
    [SerializeField] private float interactRange = 3f;

    [Header("Obstacle Detection")]
    [SerializeField] private float obstacleCheckDistance = 1.5f;
    [SerializeField] private LayerMask obstacleLayer;

    private RobotState currentState = RobotState.Idle;
    private int nextTripIndex = 0; // 0 = first, 1 = second

    private Vector3 startPosition;
    private Quaternion startRotation;

    private float currentSteer;

    public event Action<GameObject> OnArtifactPick;

    private void Start()
    {
        if (!agent)
            agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;

        startPosition = transform.position;
        startRotation = transform.rotation;

        if (interactCanvas)
            interactCanvas.SetActive(false);
    }

    private void Update()
    {
        HandleInput();
        HandleStateLogic();
        HandleRotation();
        AnimateWheels();
    }

    // ================= INPUT =================

    private void HandleInput()
    {
        if (!player || !interactCanvas)
            return;

        float dist = Vector3.Distance(player.position, transform.position);
        interactCanvas.SetActive(dist <= interactRange);

        if (dist <= interactRange &&
            Keyboard.current.eKey.wasPressedThisFrame &&
            currentState == RobotState.Idle)
        {
            if (IsObstacleAhead())
                return;

            StartNextTrip();
        }
    }

    private void StartNextTrip()
    {
        Transform target = (nextTripIndex == 0)
            ? firstDestinationPoint
            : secondDestinationPoint;

        if (!target)
            return;

        agent.isStopped = false;
        agent.SetDestination(target.position);

        currentState = (nextTripIndex == 0)
            ? RobotState.MovingToFirst
            : RobotState.MovingToSecond;
    }

    // ================= STATE MACHINE =================

    private void HandleStateLogic()
    {
        if (currentState == RobotState.Idle)
            return;

        if (agent.pathPending)
            return;

        if (agent.remainingDistance > agent.stoppingDistance)
            return;

        switch (currentState)
        {
            case RobotState.MovingToFirst:
            case RobotState.MovingToSecond:
                StartReturnToBase();
                break;

            case RobotState.Returning:
                FinishTrip();
                break;
        }
    }

    private void StartReturnToBase()
    {
        agent.SetDestination(startPosition);
        currentState = RobotState.Returning;
    }

    private void FinishTrip()
    {
        agent.isStopped = true;
        transform.rotation = startRotation;

        nextTripIndex = (nextTripIndex == 0) ? 1 : 0;
        currentState = RobotState.Idle;
    }

    // ================= OBSTACLE =================

    private bool IsObstacleAhead()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        Vector3 direction = transform.forward;

        return Physics.Raycast(origin, direction, obstacleCheckDistance, obstacleLayer);
    }

    // ================= ROTATION =================

    private void HandleRotation()
    {
        if (agent.velocity.sqrMagnitude < 0.01f)
            return;

        Vector3 moveDir = agent.velocity.normalized;
        Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            targetRotation,
            bodyTurnSpeed * Time.deltaTime
        );
    }

    // ================= WHEELS =================

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
        if (!wheel)
            return;

        wheel.Rotate(Vector3.right, speed * wheelRotateSpeed * Time.deltaTime, Space.Self);
    }

    private void SteerFrontWheels()
    {
        if (!agent.hasPath)
            return;

        Vector3 localTarget = transform.InverseTransformPoint(agent.steeringTarget);
        float targetSteer = Mathf.Clamp(
            localTarget.x / Mathf.Max(localTarget.magnitude, 0.1f),
            -1f,
            1f
        );

        currentSteer = Mathf.Lerp(currentSteer, targetSteer, steerSmooth * Time.deltaTime);
        float steerAngle = currentSteer * maxWheelTurnAngle;

        SetWheelSteer(frontLeft, steerAngle);
        SetWheelSteer(frontRight, steerAngle);
    }

    private void SetWheelSteer(Transform wheel, float angle)
    {
        if (!wheel)
            return;

        Vector3 euler = wheel.localEulerAngles;
        wheel.localRotation = Quaternion.Euler(euler.x, angle, euler.z);
    }

    // ================= TRIGGERS =================

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Artifact"))
        {
            OnArtifactPick?.Invoke(other.gameObject);
        }
    }
    private enum RobotState
    {
        Idle,
        MovingToFirst,
        MovingToSecond,
        Returning
    }
}
