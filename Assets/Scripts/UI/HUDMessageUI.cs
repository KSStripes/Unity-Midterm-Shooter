using System.Collections;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
[RequireComponent(typeof(CanvasGroup))] // ensure a CanvasGroup exists on this object
public class HUDMessageUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private CanvasGroup group;    // Ref to CanvasGroup on MessagePanel
    [SerializeField] private TMP_Text messageText; // Ref to TMP text inside the MessagePanel

    [Header("Behaviour")]
    [SerializeField, Min(0f)] private float fadeDuration = 0.35f; // 0 = instant cut

    private Coroutine active; // current running coroutine

    void Reset()
    {
        // Auto-wire common refs when the component is first added
        group = GetComponent<CanvasGroup>();
        messageText = GetComponentInChildren<TMP_Text>(true);
    }

    void OnValidate()
    {
        // Keep refs wired when editing in Inspector
        if (!group) group = GetComponent<CanvasGroup>();
        if (!messageText) messageText = GetComponentInChildren<TMP_Text>(true);
    }

    void Awake()
    {
        // Start hidden
        if (!group) group = GetComponent<CanvasGroup>(); // ensure ref exists
        group.alpha = 0f;
        group.interactable = false;
        group.blocksRaycasts = false;
    }

    void OnDisable()
    {
        // stop any running coroutines and hide
        if (active != null) { StopCoroutine(active); active = null; }
        if (group) group.alpha = 0f;
        if (messageText) messageText.text = "";
    }

    /// Show a single message.
    public void ShowAuto(string text, float displaySeconds = 2f)
    {
        if (active != null) { StopCoroutine(active); active = null; } // latest message wins
        active = StartCoroutine(CoShowAuto(text, displaySeconds));
    }

    /// Show a sequence of messages.
    public void ShowSequenceAuto(MessageItem[] items)
    {
        if (items == null || items.Length == 0) return;
        if (active != null) { StopCoroutine(active); active = null; }
        active = StartCoroutine(CoShowSequence(items));
    }

    /// Cancel current message sequence immediately.
    public void CancelCurrent()
    {
        if (active != null) { StopCoroutine(active); active = null; }
        StartCoroutine(FadeTo(0f));  // hide depending on fadeDuration
        if (messageText) messageText.text = "";
    }

    private IEnumerator CoShowSequence(MessageItem[] items)
    {
        foreach (var m in items)
            yield return CoShowAuto(m.text, Mathf.Max(0.01f, m.displaySeconds));
        active = null;
    }

    private IEnumerator CoShowAuto(string text, float seconds)
    {
        if (!group || !messageText) yield break;

        messageText.text = "";  // clear previous line before fade-in
        yield return FadeTo(1f);  // fade in
        messageText.text = text; // set new line

        // hold on screen for a bit
        yield return new WaitForSecondsRealtime(Mathf.Max(0f, seconds));

        yield return FadeTo(0f); // fade out
        messageText.text = ""; // clear after fade-out
        active = null;
    }

    private IEnumerator FadeTo(float target)
    {
        if (!group) yield break;
        if (fadeDuration <= 0f) { group.alpha = target; yield break; } // instant cut

        float start = group.alpha, t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        group.alpha = target;
    }
}

[System.Serializable]
public struct MessageItem
{
    [TextArea] public string text;   // The line to show
    public float displaySeconds;  // How long to keep it visible before fading
}
