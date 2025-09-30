// HealthDispenser.cs (no triggers/rigidbody). Distance check + 30s lockout.
// Patterns: SRP (dispenser handles proximity/logic), Facade (UI via UIController).

using UnityEngine;

public class HealthDispenser : MonoBehaviour
{
    [Header("Heal")]
    [SerializeField] private float healAmount = 30f;                 // HP per use

    [Header("Cooldowns")]
    [SerializeField] private float rechargeCooldownSeconds = 30f;    // lockout after use
    [SerializeField] private float hintRepeatSeconds = 2.0f;         // re-hint cadence

    [Header("Interaction")]
    [SerializeField] private string playerTag = "Player";            // player only
    [SerializeField] private float interactRadius = 2.0f;            // use range
    [SerializeField] private KeyCode interactKey = KeyCode.F;        // use key

    [Header("Prompt")]
    [SerializeField] private string promptReady = "Press F to charge Health"; // ready text
    [SerializeField] private string promptFull = "Health is full";            // full text
    [SerializeField] private string promptCharged = "+Health";                // feedback text
    [SerializeField] private float promptSeconds = 1.6f;                      // hint duration

    [Header("Refs")]
    [SerializeField] private UIController ui;                         // auto-found if null

    // cached player refs
    Transform player;
    HealthSystem playerHealth;

    // timers
    float nextHintTime = 0f;      // anti-spam for hints
    float nextReadyTime = 0f;     // next time dispenser usable
    bool wasInRange = false;      // range edge detection

    void Awake()
    {
        if (!ui) ui = FindFirstObjectByType<UIController>(FindObjectsInactive.Include); // Facade target

        // cache player + health once
        var go = GameObject.FindGameObjectWithTag(playerTag);
        if (go)
        {
            player = go.transform;
            playerHealth = go.GetComponentInParent<HealthSystem>();
        }
    }

    void Update()
    {
        if (!player || !playerHealth) return; // missing deps → no-op

        // proximity check (no physics triggers)
        bool inRange = Vector3.Distance(player.position, transform.position) <= interactRadius;

        // on enter range → show immediate context hint
        if (inRange && !wasInRange) ShowContextHint(force: true);
        wasInRange = inRange;
        if (!inRange) return;

        // keep hint alive periodically while standing in range
        ShowContextHint();

        // handle interaction
        if (Input.GetKeyDown(interactKey))
        {
            // still cooling down → show remaining seconds
            if (Time.time < nextReadyTime)
            {
                ui?.ShowHint($"Recharging: {Mathf.Ceil(nextReadyTime - Time.time)}s", 1.0f);
                return;
            }

            // ready: heal if not full, then start lockout
            if (!IsFullHealth())
            {
                playerHealth.IncreaseHealth(healAmount);        // HealthSystem clamps + notifies UI
                ui?.ShowHint(promptCharged, 0.8f);              // feedback ping
                nextReadyTime = Time.time + rechargeCooldownSeconds;
            }
            else
            {
                ui?.ShowHint(promptFull, 1.0f);   // avoid wasted presses
            }
        }
    }

    bool IsFullHealth()
    {
        // guard + simple full check
        return playerHealth && playerHealth.GetCurrentHealth() >= playerHealth.GetMaxHealth();
    }

    void ShowContextHint(bool force = false)
    {
        if (ui == null) return;  // no HUD available
        if (!force && Time.time < nextHintTime) return;
        nextHintTime = Time.time + hintRepeatSeconds;

        // choose best hint for current state
        if (Time.time < nextReadyTime)
            ui.ShowHint($"Recharging: {Mathf.Ceil(nextReadyTime - Time.time)}s", promptSeconds);
        else if (IsFullHealth())
            ui.ShowHint(promptFull, promptSeconds);
        else
            ui.ShowHint(promptReady, promptSeconds);
    }

    // editor aid: visualize range
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
