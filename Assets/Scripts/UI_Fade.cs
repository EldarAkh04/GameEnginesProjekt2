using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Fade : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 1.0f;
    public Coroutine fadeEffectCo { get; private set; }

    private void Start()
    {
        DoFadeIn();
    }

    public void DoFadeIn()
    {
        FadeEffect(0f);
    }

    public void DoFadeOut()
    {
        FadeEffect(1f);
    }

    private void FadeEffect(float targetAlpha)
    {
        // Stoppe vorherige Coroutine, wenn sie läuft
        if (fadeEffectCo != null)
            StopCoroutine(fadeEffectCo);

        fadeEffectCo = StartCoroutine(ChangeAlphaCo(targetAlpha));
    }

    private IEnumerator ChangeAlphaCo(float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = targetAlpha;
    }
}
