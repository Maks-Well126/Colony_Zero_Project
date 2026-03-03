using UnityEngine;

public class RotateSkybox : MonoBehaviour
{
    [SerializeField] private float m_rotationSpeed = 1f;
    [SerializeField] private float m_currentRotation = 0f;

    private void Update()
    {
        if (RenderSettings.skybox == null) return;

        m_currentRotation += m_rotationSpeed * Time.deltaTime;
        m_currentRotation %= 360f;

        RenderSettings.skybox.SetFloat("_Rotation", m_currentRotation);
    }
}