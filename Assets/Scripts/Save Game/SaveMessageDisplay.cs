using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SaveMessageDisplay : MonoBehaviour
{
    private TextMeshProUGUI saveMessageText;
    private Color originalColor;

    private void Awake()
    {
        saveMessageText = GetComponent<TextMeshProUGUI>();
        originalColor = saveMessageText.color;
        originalColor.a = 0; // Make the text invisible initially
        saveMessageText.color = originalColor;
    }

    public void DisplaySaveMessage()
    {
        StartCoroutine(FadeInAndOut());
    }

    private IEnumerator FadeInAndOut()
    {
        // Fade in
        float elapsedTime = 0f;
        float fadeInDuration = 0.5f; // Duration of fade-in

        while (elapsedTime < fadeInDuration)
        {
            elapsedTime += Time.deltaTime;
            Color newColor = saveMessageText.color;
            newColor.a = Mathf.Lerp(0, 1, elapsedTime / fadeInDuration);
            saveMessageText.color = newColor;
            yield return null;
        }

        // Wait for 2 seconds
        yield return new WaitForSeconds(2f);

        // Fade out
        elapsedTime = 0f;
        float fadeOutDuration = 0.5f; // Duration of fade-out

        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            Color newColor = saveMessageText.color;
            newColor.a = Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration);
            saveMessageText.color = newColor;
            yield return null;
        }
    }
}