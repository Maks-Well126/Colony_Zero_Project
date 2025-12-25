using System;
using System.Collections;
using UnityEditor.Experimental;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(NavMeshAgent))]
public class RobotController : MonoBehaviour
{
    [Header("NavMesh")]
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Transform destinationPoint;

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
    [SerializeField] private string stopTag = "Stop";
    [SerializeField] private float minSpeedToRotate = 0.2f;

    [Header("Player interaction")]
    [SerializeField] private GameObject interactCanvas;
    [SerializeField] private float interactRange = 3f;
    [SerializeField] private Transform player;

    private float currentSteer;
    private bool isBlocked;
    private bool isMovingToDestination;   
    private Vector3 startPosition;
    private Quaternion startRotation;

    public event Action<GameObject> OnArtifactPick;

    private void Start()
    {
        agent.updateRotation = false;
        startPosition = transform.position;
        startRotation = transform.rotation;

        if (interactCanvas) interactCanvas.SetActive(false);
    }

    private void Update()
    {
        
        HandlePlayerInteraction();
        HandleMovement();
        AnimateWheels();
    }

    // -------------------- PLAYER INPUT --------------------
    private void HandlePlayerInteraction()
    {
        if (!player || !interactCanvas) return;

        float dist = Vector3.Distance(player.position, transform.position);
        interactCanvas.SetActive(dist <= interactRange);
        if (dist >= interactRange)
        {
            interactCanvas.SetActive(false);
        }

        if (dist <= interactRange && Keyboard.current.eKey.wasPressedThisFrame && !isMovingToDestination)
        {
            agent.isStopped = false;
            StartCoroutine(MoveToDestination());
        }
    }

    // -------------------- MOVEMENT --------------------
    private IEnumerator MoveToDestination()
    {
        if (destinationPoint == null) yield break;

        isMovingToDestination = true;

        // Едем к цели
        agent.SetDestination(destinationPoint.position);
        agent.isStopped = false;

        // Ждём, пока робот доедет
        while (Vector3.Distance(transform.position, destinationPoint.position) > agent.stoppingDistance)
            yield return null;

        agent.isStopped = true;

        
        // Возврат на старт
        agent.SetDestination(startPosition);
        agent.isStopped = false;

        while (Vector3.Distance(transform.position, startPosition) > agent.stoppingDistance)
            yield return null;

        agent.isStopped = true;
        transform.rotation = startRotation;
        isMovingToDestination = false;
    }
    
   

    // -------------------- BODY ROTATION --------------------
    private void HandleMovement()
    {
        if (isBlocked || agent.velocity.magnitude < minSpeedToRotate) return;

        RotateBodySmoothly();
    }

    private void RotateBodySmoothly()
    {
        Vector3 moveDir = agent.velocity.normalized;
        if (moveDir.sqrMagnitude < 0.001f) return;

        Quaternion targetRotation = Quaternion.LookRotation(moveDir, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, bodyTurnSpeed * Time.deltaTime);
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
        if (other.CompareTag("Artifact"))
        {            
            OnArtifactPick?.Invoke(other.gameObject);
            Debug.Log("Atref");                       

        }
        Debug.Log("Atref");
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
