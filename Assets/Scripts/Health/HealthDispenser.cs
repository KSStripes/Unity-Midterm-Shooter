// HealthDispenser.cs (no triggers, no rigidbody)
using UnityEngine;

public class HealthDispenser : MonoBehaviour
{
    [Header("Heal")]
    [SerializeField] private float healAmount = 30f;
    [SerializeField] private float useCooldownSeconds = 1.5f;

    [Header("Interaction")]
    [SerializeField] private string playerTag = "Player";
    [SerializeField] private float interactRadius = 2.0f;
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Header("Prompt")]
    [SerializeField] private string promptText = "Press F to charge Health";
    [SerializeField] private float promptSeconds = 1.6f;
    [SerializeField] private float promptCooldown = 2.0f;

    [Header("Refs")]
    [SerializeField] private UIController ui; // optional; auto-find

    Transform player;
    HealthSystem playerHealth;
    float nextUseTime = 30f; // allow recharge after 30s
    float nextPromptTime = 0f;
    bool wasInRange = false;

    void Awake()
    {
        if (!ui) ui = FindFirstObjectByType<UIController>(FindObjectsInactive.Include);

        // get player by tag once
        var go = GameObject.FindGameObjectWithTag(playerTag);
        if (go)
        {
            player = go.transform;
            playerHealth = go.GetComponentInParent<HealthSystem>();
        }
    }

    void Update()
    {
        if (!player || !playerHealth) return;

        float dist = Vector3.Distance(player.position, transform.position);
        bool inRange = dist <= interactRadius;

        // entered range
        if (inRange && !wasInRange)
        {
            MaybeShowHint(force: true);
        }
        // left range
        else if (!inRange && wasInRange)
        {
            // no persistent UI to hide; hints expire by themselves
        }
        wasInRange = inRange;

        if (!inRange) return;

        // keep hint visible periodically while in range & not full
        MaybeShowHint();

        if (Input.GetKeyDown(interactKey) && Time.time >= nextUseTime)
        {
            if (!IsFullHealth())
            {
                nextUseTime = Time.time + useCooldownSeconds;
                playerHealth.IncreaseHealth(healAmount);
                ui?.ShowHint("+Health", 0.8f);
            }
            else
            {
                ui?.ShowHint("Health is full", 1.0f);
            }
        }
    }

    bool IsFullHealth()
    {
        return playerHealth && playerHealth.GetCurrentHealth() >= playerHealth.GetMaxHealth();
    }

    void MaybeShowHint(bool force = false)
    {
        if (ui == null || IsFullHealth()) return;

        if (force || Time.time >= nextPromptTime)
        {
            nextPromptTime = Time.time + promptCooldown;
            ui.ShowHint(promptText, promptSeconds);
        }
    }

    // visualize radius in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, interactRadius);
    }
}
