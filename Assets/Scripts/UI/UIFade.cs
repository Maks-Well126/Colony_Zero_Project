using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class UIFade : MonoBehaviour
{
    [SerializeField] private Image image;
    [SerializeField] private float fadeSpeed = 1f;

    public void Start()
    {
        StartCoroutine(FadeOut());
    }

    private IEnumerator FadeOut()
    {
        Color color = image.color;

        while (color.a > 0)
        {
            color.a -= Time.deltaTime * fadeSpeed;
            image.color = color;

            yield return null;
        }

        color.a = 0;
        image.color = color;

        gameObject.SetActive(false);
    }
}