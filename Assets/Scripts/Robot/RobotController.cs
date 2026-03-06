using System;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] private Transform m_checkDistObject;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject interactCanvas;
    [SerializeField] private float interactRange = 3f;

    [Header("Audio")]
    [SerializeField] private AudioSource AudioSource;
    [SerializeField] private AudioSource AudioSourceDamage;
    [SerializeField] private AudioClip mooveClip;
    [SerializeField] private AudioClip damageClip;
    [SerializeField] private float moveSoundSmooth = 3f;

    [Header("Obstacle Detection")]
    [SerializeField] private float obstacleCheckDistance = 1.5f;
    [SerializeField] private LayerMask obstacleLayer;

    [Header("HP")]
    [SerializeField] private HealthComponent m_health;
    [SerializeField] private float m_startHealth = 200f;
    [SerializeField] private float m_upgradeAmount = 100f;
    [SerializeField] private float m_repairAmount = 100f;
    [SerializeField] private Image m_LineHPBar;
    [SerializeField] private Image m_poraChinit;

    [SerializeField] private RobotPickupTrigger pickupTrigger;

    private RobotState currentState = RobotState.Idle;
    private int nextTripIndex = 0;

    private bool isInObstacleTrigger;

    [SerializeField] private float stopDistance = 12f;
    [SerializeField] private float resumeDistance = 10f;

    private bool stoppedByDistance;

    private Vector3 startPosition;
    private Quaternion startRotation;

    private float currentSteer;

    private bool m_isUpgraded;
    private bool m_isBroken;
    private bool m_playerNearby;

    public event Action<GameObject> OnArtifactPick;

    private void Awake()
    {        
        pickupTrigger.OnArtifactPick += OnArtifactPicked;
    }

    private void Start()
    {
        if (!agent)
            agent = GetComponent<NavMeshAgent>();

        agent.updateRotation = false;

        startPosition = transform.position;
        startRotation = transform.rotation;

        if (interactCanvas)
            interactCanvas.SetActive(false);

        if (player == null)
            FindPlayerByTag();

        Artifact.isArtefact1Delivered = Save.LoadLevel1State();

        m_health.Initialize(m_startHealth);
        m_health.Died += OnRobotBroken;
        m_health.Damaged += OnRobotDamaged;
    }

    private void FindPlayerByTag()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");

        if (playerObject != null)
            player = playerObject.transform;
        else
            Debug.LogWarning("Player not found with tag Player");
    }

    private void Update()
    {
        HandleInput();
        HandleStateLogic();
        HandleRotation();
        AnimateWheels();
        UpdateMoveSound();
        CheckDistance();
        HealthBar();
    }

    private void HealthBar()
    {        
        m_LineHPBar.fillAmount =
            m_health.CurrentHealth / m_health.MaxHealth;
        if(m_health.CurrentHealth <= 0) m_poraChinit.gameObject.SetActive(true);
        else m_poraChinit.gameObject.SetActive(false);
    }

    private void OnArtifactPicked(GameObject artifact)
    {
        Debug.Log("Artifact picked: " + artifact.name);
    }

    // ================= SOUND =================

    private void UpdateMoveSound()
    {
        if (AudioSource == null || mooveClip == null)
            return;

        bool isMoving = agent.velocity.magnitude > 0.1f && !agent.isStopped;

        float targetVolume = agent.velocity.magnitude / agent.speed;

        AudioSource.volume = Mathf.Lerp(
            AudioSource.volume,
            isMoving ? targetVolume : 0f,
            Time.deltaTime * moveSoundSmooth
        );

        if (isMoving && !AudioSource.isPlaying)
        {
            AudioSource.clip = mooveClip;
            AudioSource.Play();
        }
    }
    private void OnRobotDamaged()
    {
        if (AudioSourceDamage != null && damageClip != null)
        {
            AudioSourceDamage.PlayOneShot(damageClip);
        }
    }

    // ================= ROBOT DAMAGE =================

    private void OnRobotBroken()
    {
        m_isBroken = true;
        agent.isStopped = true;
        currentState = RobotState.Idle;

        Debug.Log("Robot is broken!");
    }

    public void UpgradeHealth()
    {
        if (m_health == null || m_isUpgraded)
            return;

        m_health.IncreaseMaxHealth(m_upgradeAmount);
        m_isUpgraded = true;

        m_health.HealToMax();
    }

    public void RepairRobotInField()
    {
        if (!m_isBroken || m_health == null)
            return;

        if (m_health.CurrentHealth > 0)
            return;

        m_health.Heal(m_repairAmount, revive: true);

        m_isBroken = false;
        agent.isStopped = false;
    }

    public void RepairRobotAtBase()
    {
        if (m_health == null)
            return;

        m_health.HealToMax(revive: true);
        m_isBroken = false;
        agent.isStopped = false;
    }

    // ================= INPUT =================

    private void HandleInput()
    {
        if (!player || !interactCanvas)
            return;

        float dist = Vector3.Distance(player.position, transform.position);
        interactCanvas.SetActive(dist <= interactRange);

        if (dist <= interactRange &&
            Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (m_isBroken)
            {
                RepairRobotInField();
                return;
            }
        }
    }

    // ================= TRIP =================

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

        Transform target =
            index == 0 ? firstDestinationPoint : secondDestinationPoint;

        if (!target)
            return;

        if (!Artifact.isArtefact1Delivered && index == 1)
        {
            AudioManager.Instance.PlayButtonClick(3);
            return;
        }

        AudioManager.Instance.PlayButtonClick(2);
        AudioManager.Instance.PlayMusic(1);

        nextTripIndex = index;

        agent.isStopped = false;
        agent.SetDestination(target.position);

        currentState =
            index == 0 ? RobotState.MovingToFirst : RobotState.MovingToSecond;
    }

    // ================= STATE MACHINE =================

    private void HandleStateLogic()
    {
        if (isInObstacleTrigger)
            return;

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

        nextTripIndex = nextTripIndex == 0 ? 1 : 0;
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

        Quaternion targetRotation = Quaternion.LookRotation(moveDir);

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

        wheel.Rotate(Vector3.right,
            speed * wheelRotateSpeed * Time.deltaTime,
            Space.Self);
    }

    private void SteerFrontWheels()
    {
        if (!agent.hasPath)
            return;

        Vector3 localTarget =
            transform.InverseTransformPoint(agent.steeringTarget);

        float targetSteer = Mathf.Clamp(
            localTarget.x / Mathf.Max(localTarget.magnitude, 0.1f),
            -1f,
            1f
        );

        currentSteer = Mathf.Lerp(
            currentSteer,
            targetSteer,
            steerSmooth * Time.deltaTime
        );

        float steerAngle = currentSteer * maxWheelTurnAngle;

        SetWheelSteer(frontLeft, steerAngle);
        SetWheelSteer(frontRight, steerAngle);
    }

    private void SetWheelSteer(Transform wheel, float angle)
    {
        if (!wheel)
            return;

        Vector3 euler = wheel.localEulerAngles;

        wheel.localRotation = Quaternion.Euler(
            euler.x,
            angle,
            euler.z
        );
    }

    // ================= TRIGGERS =================

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("www");
            isInObstacleTrigger = true;
            agent.isStopped = true;
            return;
        }

        if (other.CompareTag("Artifact"))
        {
            OnArtifactPick?.Invoke(other.gameObject);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            Debug.Log("Zxc");
            isInObstacleTrigger = false;

            if (currentState != RobotState.Idle)
                agent.isStopped = false;
        }
    }
    private void CheckDistance()
    {
        if (player == null)
            return;

        float dist = Vector3.Distance(m_checkDistObject.position, player.position);

        if (dist > stopDistance && !stoppedByDistance)
        {
            stoppedByDistance = true;
            agent.isStopped = true;
        }

        if (dist < resumeDistance && stoppedByDistance)
        {
            stoppedByDistance = false;

            if (currentState != RobotState.Idle)
                agent.isStopped = false;
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