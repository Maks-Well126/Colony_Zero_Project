using UnityEngine;
using System.Collections;

public class RobotGripper : MonoBehaviour
{
    [SerializeField] private Transform craneArm;
    [SerializeField] private Transform pickPosition;
    [SerializeField] private float rotateSpeed = 90f;

    [Header("Pick rotation")]
    [SerializeField] private float pickYawAngle = 45f; // Û„ÓÎ ÔÓ‚ÓÓÚ‡ ÔÓ Y

    [SerializeField] private RobotController controller;

    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = craneArm.localRotation;

        if (controller != null)
        {
            controller.OnArtifactPick += HandlePick;
        }
    }

    private void HandlePick(GameObject artifact)
    {
        StartCoroutine(PickRoutine(artifact));
    }

    private IEnumerator PickRoutine(GameObject artifact)
    {
        // --- ÷≈À≈¬Œ… œŒ¬Œ–Œ“ “ŒÀ‹ Œ œŒ Y ---
        Quaternion targetRotation =
            initialRotation * Quaternion.Euler(0f, pickYawAngle, 0f);

        // œÓ‚ÓÓÚ Í Û„ÎÛ
        while (Quaternion.Angle(craneArm.localRotation, targetRotation) > 0.5f)
        {
            craneArm.localRotation = Quaternion.RotateTowards(
                craneArm.localRotation,
                targetRotation,
                rotateSpeed * Time.deltaTime
            );
            yield return null;
        }

        // --- œŒƒ¡Œ– ¿–“≈‘¿ “¿ ---
        artifact.transform.SetParent(pickPosition);
        artifact.transform.localPosition = Vector3.zero;
        artifact.transform.localRotation = Quaternion.identity;

        Rigidbody rb = artifact.GetComponent<Rigidbody>();
        if (rb != null)
            rb.isKinematic = true;

        yield return new WaitForSeconds(0.5f);

        // --- ¬Œ«¬–¿“ ¬ »—’ŒƒÕŒ≈ œŒÀŒ∆≈Õ»≈ ---
        while (Quaternion.Angle(craneArm.localRotation, initialRotation) > 0.5f)
        {
            craneArm.localRotation = Quaternion.RotateTowards(
                craneArm.localRotation,
                initialRotation,
                rotateSpeed * Time.deltaTime
            );
            yield return null;
        }
    }
}
