using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

public class RobotController : MonoBehaviour
{
    public NavMeshAgent agent;                 // ← NavMeshAgent
    public Rigidbody rb;                       // ← Rigidbody

    public WheelCollider frontLeft;            // ← переднее левое колесо
    public WheelCollider frontRight;           // ← переднее правое колесо
    public WheelCollider rearLeft;             // ← заднее левое колесо
    public WheelCollider rearRight;             // ← заднее правое колесо

    public Transform frontLeftMesh;             // ← визуальная модель колеса
    public Transform frontRightMesh;
    public Transform rearLeftMesh;
    public Transform rearRightMesh;

    public float motorForce = 1500f;           // ← сила двигателя
    public float maxSteerAngle = 30f;           // ← максимальный угол поворота

    [Header("Steering")]
    public float steerSpeed = 5f;        // ← скорость поворота руля
    private float currentSteer = 0f;     // ← текущий угол

    [Header("Speed")]
    public float maxSpeed = 10f;          // ← макс скорость
    public float brakeForce = 3000f;      // ← торможение


    void Update()
    {
        // Проверяем нажатие ПРАВОЙ кнопки мыши (новый Input System)
        if (Mouse.current.rightButton.wasPressedThisFrame)
        {
            // Получаем позицию мыши
            Vector2 mousePos = Mouse.current.position.ReadValue();

            // Пускаем луч из камеры в точку мыши
            Ray ray = Camera.main.ScreenPointToRay(mousePos);

            // Проверяем, куда кликнули
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                // Передаём точку в NavMeshAgent
                agent.SetDestination(hit.point);
            }
        }
    }



    void FixedUpdate()
    {
        // Если пути нет — тормозим
        if (!agent.hasPath)
        {
            ApplyBrake();
            return;
        }

        // Направление следующей точки пути
        Vector3 target = agent.steeringTarget;
        Vector3 localTarget = transform.InverseTransformPoint(target);

        // Защита от деления на 0
        float distance = Mathf.Max(localTarget.magnitude, 0.1f);

        // Желаемый угол поворота
        float targetSteer = (localTarget.x / distance) * maxSteerAngle;

        // СГЛАЖИВАНИЕ ПОВОРОТА
        currentSteer = Mathf.Lerp(currentSteer, targetSteer, steerSpeed * Time.fixedDeltaTime);

        // Подаём угол
        frontLeft.steerAngle = currentSteer;
        frontRight.steerAngle = currentSteer;

        // Ограничение скорости
        float speed = rb.linearVelocity.magnitude;

        if (speed < maxSpeed && agent.remainingDistance > 1.5f)
        {
            rearLeft.motorTorque = motorForce;
            rearRight.motorTorque = motorForce;
            ReleaseBrake();
        }
        else
        {
            ApplyBrake();
        }

        // Обновление визуальных колёс
        UpdateWheel(frontLeft, frontLeftMesh);
        UpdateWheel(frontRight, frontRightMesh);
        UpdateWheel(rearLeft, rearLeftMesh);
        UpdateWheel(rearRight, rearRightMesh);
    }

    void ApplyBrake()
    {
        rearLeft.motorTorque = 0;
        rearRight.motorTorque = 0;

        rearLeft.brakeTorque = brakeForce;
        rearRight.brakeTorque = brakeForce;
    }

    void ReleaseBrake()
    {
        rearLeft.brakeTorque = 0;
        rearRight.brakeTorque = 0;
    }


    void UpdateWheel(WheelCollider col, Transform mesh)
    {
        Vector3 pos;
        Quaternion rot;
        col.GetWorldPose(out pos, out rot);
        mesh.position = pos;
        mesh.rotation = rot;
    }
}
