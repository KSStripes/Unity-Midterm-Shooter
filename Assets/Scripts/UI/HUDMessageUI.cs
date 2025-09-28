using System.Collections;
using UnityEngine;
using TMPro;

[DisallowMultipleComponent]
public class HUDMessageUI : MonoBehaviour
{
    [Header("UI Refs")]
    [SerializeField] private CanvasGroup group;      // CanvasGroup on MessagePanel (bottom bar)
    [SerializeField] private TMP_Text messageText;   // TMP text inside MessagePanel

    [Header("Behaviour")]
    [SerializeField] private float fadeDuration = 0.25f;           // fade in/out (unscaled time)
    [SerializeField] private float typewriterCharsPerSecond = 0f;  // 0 = instant; else chars/sec
    [SerializeField] private bool blockRaycastsWhileVisible = false; // should bar catch clicks?
    [SerializeField] private GameObject[] hideWhileShowing;        // HUD bits to hide (e.g., crosshair)

    [Header("FX")]
    [SerializeField] private bool flickerOnShow = true; // tiny startup flicker for sci-fi feel
    [SerializeField] private float flickerSeconds = 0.2f;

    private Coroutine active;   // current running coroutine (single owner)
    private bool busy;

    void Reset()
    {
        group = GetComponent<CanvasGroup>();
        messageText = GetComponentInChildren<TMP_Text>(true);
    }

    void Awake()
    {
        // Start hidden; panel stays active so we can animate alpha
        if (group)
        {
            group.alpha = 0f;
            group.interactable = false;
            group.blocksRaycasts = false;
        }
        gameObject.SetActive(true);
    }

    /// <summary>Show a single auto-dismissing message.</summary>
    public Coroutine ShowAuto(MonoBehaviour host, string text, float displaySeconds = 2f)
    {
        if (host == null) host = this;
        if (active != null) { StopCoroutine(active); active = null; } // latest message wins
        active = host.StartCoroutine(CoShowAuto(text, displaySeconds));
        return active;
    }

    /// <summary>Show a sequence of auto-dismissing messages.</summary>
    public Coroutine ShowSequenceAuto(MonoBehaviour host, MessageItem[] items)
    {
        if (host == null) host = this;
        if (active != null) { StopCoroutine(active); active = null; }
        active = host.StartCoroutine(CoShowSequence(items));
        return active;
    }

    /// <summary>Cancel current message/sequence immediately.</summary>
    public void CancelCurrent()
    {
        if (active != null) { StopCoroutine(active); active = null; }
        StartCoroutine(FadeTo(0f));
        SetInteractable(false);
        ToggleHiddenHud(true);
        busy = false;
    }

    IEnumerator CoShowSequence(MessageItem[] items)
    {
        if (items == null || items.Length == 0) yield break;
        foreach (var m in items)
            yield return CoShowAuto(m.text, Mathf.Max(0.01f, m.displaySeconds));
        active = null;
    }

    IEnumerator CoShowAuto(string text, float seconds)
    {
        busy = true;
        ToggleHiddenHud(false);   // hide crosshair etc. while showing (optional)
        SetInteractable(true);    // enable input gating / raycast policy

        // Fade in
        yield return FadeTo(1f);

        // Optional micro-flicker for “boot” vibe
        if (flickerOnShow)
            yield return Flicker(group, flickerSeconds);

        // Type or set instantly
        if (typewriterCharsPerSecond > 0f)
            yield return Typewriter(text);
        else
            messageText.text = text;

        // Hold on screen for 'seconds'
        float tEnd = Time.unscaledTime + seconds;
        while (Time.unscaledTime < tEnd)
            yield return null;

        // Fade out and restore HUD
        yield return FadeTo(0f);
        SetInteractable(false);
        ToggleHiddenHud(true);
        busy = false;
    }

    void SetInteractable(bool on)
    {
        if (!group) return;
        group.interactable = on;
        group.blocksRaycasts = on && blockRaycastsWhileVisible; // only block if asked
    }

    void ToggleHiddenHud(bool show)
    {
        if (hideWhileShowing == null) return;
        foreach (var go in hideWhileShowing)
            if (go) go.SetActive(show);
    }

    IEnumerator FadeTo(float target)
    {
        if (!group) yield break;
        float start = group.alpha, t = 0f;
        while (t < fadeDuration)
        {
            t += Time.unscaledDeltaTime;
            group.alpha = Mathf.Lerp(start, target, t / fadeDuration);
            yield return null;
        }
        group.alpha = target;
    }

    IEnumerator Typewriter(string full)
    {
        messageText.text = "";
        float cps = Mathf.Max(1f, typewriterCharsPerSecond);
        int i = 0;
        while (i < full.Length)
        {
            i = Mathf.Min(full.Length, i + Mathf.CeilToInt(cps * Time.unscaledDeltaTime));
            messageText.text = full.Substring(0, i);
            yield return null;
        }
    }

    IEnumerator Flicker(CanvasGroup g, float seconds)
    {
        if (!g || seconds <= 0f) yield break;
        float end = Time.unscaledTime + seconds;
        while (Time.unscaledTime < end)
        {
            // subtle alpha noise → CRT/scanline vibe
            g.alpha = 0.85f + Mathf.PerlinNoise(Time.unscaledTime * 60f, 0f) * 0.15f;
            yield return null;
        }
        g.alpha = 1f;
    }
}

/// <summary>One auto message in a sequence.</summary>
[System.Serializable]
public struct MessageItem
{
    [TextArea] public string text;      // The line to show
    public float displaySeconds;        // How long to keep it visible before fading
}
