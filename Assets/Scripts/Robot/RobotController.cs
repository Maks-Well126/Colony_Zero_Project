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

    [Header("Distance Limit")]
    [SerializeField] private float maxDistanceFromPlayer = 10f; // Максимальная дистанция от игрока
    [SerializeField] private float resumeDistanceFromPlayer = 8f; // Дистанция, при которой робот возобновляет движение

    private RobotState currentState = RobotState.Idle;
    private int nextTripIndex = 0; // 0 = first, 1 = second

    // Для запоминания цели, когда робот остановился из-за дистанции
    private Transform pendingDestination;
    private RobotState pendingState;

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

        // Поиск игрока по тегу, если не назначен в инспекторе
        if (player == null)
            FindPlayerByTag();
    }

    private void FindPlayerByTag()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogWarning("Player not found with tag 'Player'! Please assign player manually in inspector.");
    }

    private void Update()
    {
        HandleInput();
        HandleStateLogic();
        HandleRotation();
        AnimateWheels();

        // Проверка дистанции от игрока
        CheckDistanceFromPlayer();
    }

    // ================= DISTANCE CHECK =================

    private void CheckDistanceFromPlayer()
    {
        if (player == null)
            return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        // Если робот слишком далеко от игрока - останавливаем его
        if (distanceToPlayer > maxDistanceFromPlayer && currentState != RobotState.Idle)
        {
            StopRobot();
        }
        // Если робот остановлен из-за дистанции и игрок подошел ближе resumeDistance - возобновляем движение
        else if (distanceToPlayer <= resumeDistanceFromPlayer && pendingDestination != null)
        {
            ResumeMovement();
        }
    }

    private void StopRobot()
    {
        if (currentState == RobotState.Idle)
            return;

        // Запоминаем, куда должен был ехать робот
        if (agent.hasPath)
        {
            pendingDestination = new GameObject("PendingDestination").transform;
            pendingDestination.position = agent.destination;

            // Запоминаем состояние
            if (currentState == RobotState.MovingToFirst)
                pendingState = RobotState.MovingToFirst;
            else if (currentState == RobotState.MovingToSecond)
                pendingState = RobotState.MovingToSecond;
            else if (currentState == RobotState.Returning)
                pendingState = RobotState.Returning;
        }

        // Останавливаем навмеш агента
        agent.isStopped = true;

        // Возвращаем робота в состояние Idle
        currentState = RobotState.Idle;

        Debug.Log("Robot stopped - too far from player!");
    }

    private void ResumeMovement()
    {
        if (pendingDestination == null)
            return;

        // Проверяем, не слишком ли далеко целевая точка от игрока сейчас
        float targetDistanceFromPlayer = Vector3.Distance(pendingDestination.position, player.position);
        if (targetDistanceFromPlayer <= maxDistanceFromPlayer)
        {
            // Возобновляем движение
            agent.isStopped = false;
            agent.SetDestination(pendingDestination.position);
            currentState = pendingState;

            // Очищаем временные данные
            Destroy(pendingDestination.gameObject);
            pendingDestination = null;

            Debug.Log("Robot resumed movement - player is close enough!");
        }
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

        // Проверяем, не слишком ли далеко целевая точка от игрока
        float targetDistanceFromPlayer = Vector3.Distance(target.position, player.position);
        if (targetDistanceFromPlayer > maxDistanceFromPlayer)
        {
            Debug.Log("Destination is too far from player!");
            return;
        }

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

        // Очищаем pendingDestination если был
        if (pendingDestination != null)
        {
            Destroy(pendingDestination.gameObject);
            pendingDestination = null;
        }
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

    private void OnDestroy()
    {
        // Очищаем временный объект при уничтожении робота
        if (pendingDestination != null)
            Destroy(pendingDestination.gameObject);
    }

    // ================= UI BUTTON CONTROL =================

    public void GoToFirstArtifact()
    {
        TryStartTrip(0);
        AudioManager.Instance.PlayButtonClick(0);
    }

    public void GoToSecondArtifact()
    {
        TryStartTrip(1);
        AudioManager.Instance.PlayButtonClick(0);
    }

    private void TryStartTrip(int index)
    {
        if (currentState != RobotState.Idle)
            return;

        if (IsObstacleAhead())
            return;

        Transform target = (index == 0)
            ? firstDestinationPoint
            : secondDestinationPoint;

        if (!target)
            return;

        if (!Artifact.isArtefact1Delivered && index == 1) { AudioManager.Instance.PlayButtonClick(3); return;  }
       
        if (player != null)
        {
            float targetDistanceFromPlayer =
                Vector3.Distance(target.position, player.position);

            if (targetDistanceFromPlayer > maxDistanceFromPlayer)
            {
                Debug.Log("Destination is too far from player!");
                return;
            }
        }
        AudioManager.Instance.PlayButtonClick(2);
        nextTripIndex = index;

        agent.isStopped = false;
        agent.SetDestination(target.position);

        currentState = (index == 0)
            ? RobotState.MovingToFirst
            : RobotState.MovingToSecond;
    }

    private enum RobotState
    {
        Idle,
        MovingToFirst,
        MovingToSecond,
        Returning
    }
}