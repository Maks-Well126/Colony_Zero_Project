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

    private Color m_highlightColor;
    private Color m_defaultColor;

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
    }

    void Update()
    {
        MouseDirectionChecker.MouseDirection direction =
            MouseDirectionChecker.GetMouseDirection(threshold, deadZone);

        ResetAllColors();

        switch (direction)
        {
            case MouseDirectionChecker.MouseDirection.Right:
                m_rightImage.color = m_highlightColor;
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

            case MouseDirectionChecker.MouseDirection.Center:
                break;
        }
    }

    private void ResetAllColors()
    {
        if (m_upImage != null) m_upImage.color = m_defaultColor;
        if (m_downImage != null) m_downImage.color = m_defaultColor;
        if (m_leftImage != null) m_leftImage.color = m_defaultColor;
        if (m_rightImage != null) m_rightImage.color = m_defaultColor;
    }
}