using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TutorialToggle : MonoBehaviour
{
    public RectTransform panel;
    public Button showButton;
    public Button hideButton;
    public Button playButton;
    public Button quitButton;
    public float slideDuration = 0.3f;
    public float slideDistance = 200f; // How far above the visible area the panel starts

    private Vector2 hiddenPos;
    private Vector2 shownPos;

    void Start()
    {
        shownPos = panel.anchoredPosition;
        hiddenPos = shownPos + new Vector2(0, slideDistance);
        panel.anchoredPosition = hiddenPos;
        panel.gameObject.SetActive(false);

        showButton.onClick.AddListener(() => StartCoroutine(SlideIn()));
        hideButton.onClick.AddListener(() => StartCoroutine(SlideOut()));
    }

    IEnumerator SlideIn()
    {
        playButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        panel.gameObject.SetActive(true);
        float time = 0f;
        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = time / slideDuration;
            panel.anchoredPosition = Vector2.Lerp(hiddenPos, shownPos, t);
            yield return null;
        }
        panel.anchoredPosition = shownPos;
    }

    IEnumerator SlideOut()
    {
        float time = 0f;
        while (time < slideDuration)
        {
            time += Time.deltaTime;
            float t = time / slideDuration;
            panel.anchoredPosition = Vector2.Lerp(shownPos, hiddenPos, t);
            yield return null;
        }
        panel.anchoredPosition = hiddenPos;
        panel.gameObject.SetActive(false);
        playButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
    }
}
