using UnityEngine;
using UnityEngine.UI;

public class TestMouseDirections : MonoBehaviour
{
    public float threshold = 0.3f;
    public float deadZone = 50f;

    [SerializeField] private Image m_upImage;
    [SerializeField] private Image m_downImage;
    [SerializeField] private Image m_leftImage;
    [SerializeField] private Image m_rightImage;

    [SerializeField] private GameObject m_Radial;

    private Color m_highlightColor;
    private Color m_defaultColor;

    private MouseDirectionChecker.MouseDirection m_lastDirection = MouseDirectionChecker.MouseDirection.Center;

    void Start()
    {
        if (ColorUtility.TryParseHtmlString("#00FCFF", out m_highlightColor))
        {
        }
        else
        {
            m_highlightColor = Color.yellow;
        }

        m_defaultColor = Color.white;
        m_lastDirection = MouseDirectionChecker.MouseDirection.Center;
    }

    void Update()
    {
        MouseDirectionChecker.MouseDirection currentDirection =
            MouseDirectionChecker.GetMouseDirection(threshold, deadZone);

        ResetAllColors();

        if (currentDirection != MouseDirectionChecker.MouseDirection.Center)
        {
            switch (currentDirection)
            {
                case MouseDirectionChecker.MouseDirection.Right:
                    m_rightImage.color = m_highlightColor;
                    if (Input.GetMouseButtonDown(0)) m_Radial.gameObject.SetActive(false);
                    break;
                case MouseDirectionChecker.MouseDirection.Left:
                    m_leftImage.color = m_highlightColor;
                    break;
                case MouseDirectionChecker.MouseDirection.Up:
                    m_upImage.color = m_highlightColor;
                    break;
                case MouseDirectionChecker.MouseDirection.Down:
                    m_downImage.color = m_highlightColor;
                    break;
            }

            if (currentDirection != m_lastDirection)
            {
                PlayHoverSound();
            }
        }

        m_lastDirection = currentDirection;
        if (Input.GetKeyDown(KeyCode.Escape)) 
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            m_Radial.gameObject.SetActive(false);
        } 
    }

    private void PlayHoverSound()
    {
        AudioManager.Instance.PlayButtonHover();
    }

    private void ResetAllColors()
    {
        if (m_upImage != null) m_upImage.color = m_defaultColor;
        if (m_downImage != null) m_downImage.color = m_defaultColor;
        if (m_leftImage != null) m_leftImage.color = m_defaultColor;
        if (m_rightImage != null) m_rightImage.color = m_defaultColor;
    }
}