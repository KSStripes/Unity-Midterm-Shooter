using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    // Player references
    private HealthSystem playerHealth;
    private ShootAbility shootAbility;
    private float nextLowHealthTime; // rate-limit for low health message

    [Header("Health UI")]
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthPercentText;

    [Header("Weapon UI")]
    [SerializeField] private GameObject bulletEnabled; //shown when bullet is selected
    [SerializeField] private GameObject rocketEnabled; //shown when rocket is selected

    [Header("Message Bar")]
    [SerializeField] private HUDMessageUI hud; // Ref to UI MessagePanel

    // Start tutorial text - fill in Inspector
    [Header("Start Tutorial (inline)")]
    [SerializeField] private bool playOnStart = true;     // click to auto-play when scene loads
    [SerializeField] private MessageItem[] startMessages; // fill text and display in inspector

    // Auto-messages
    [Header("Auto-messages")]
    [Tooltip("Show a hint when health % falls under this value (0.25 = 25%).")]
    [Range(0.05f, 0.9f)]
    [SerializeField] private float lowHealthThreshold = 0.25f;

    [Tooltip("Seconds to wait before showing the low-health message again.")]
    [SerializeField] private float lowHealthCooldown = 12f;

    [Tooltip("Low-health hint text.")]
    [SerializeField] private string lowHealthText = "Critical health! Find a charge point.";

    [SerializeField] private float lowHealthDisplaySeconds = 2.5f;

    [Tooltip("Message when the player dies.")]
    [SerializeField] private string deathMsgText = "YOU DIED";
    [SerializeField] private float deathMsgSeconds = 3.0f;



    private void Start()
    {
        //Find player once
        playerHealth = PlayerInput.Instance.GetComponent<HealthSystem>();
        shootAbility = PlayerInput.Instance.GetComponent<ShootAbility>();

        // Subscribe to events
        shootAbility.OnChangeStrategy += SelectShootingStrategy;
        playerHealth.OnHealthChanged += UpdateHealthSlider;
        playerHealth.OnDeath += DisplayDeathScreen;

        // Init health bar and shooting UI immediately
        healthBar.maxValue = playerHealth.GetMaxHealth();
        healthBar.value = playerHealth.GetCurrentHealth();
        healthPercentText.text = "100%";
        bulletEnabled.SetActive(false); // not grey out bullet
        rocketEnabled.SetActive(true); // grey out rocket at start

        // At scene-start, play start sequence
        if (hud && playOnStart && startMessages != null && startMessages.Length > 0)
            hud.ShowSequenceAuto(this, startMessages);
    }

    private void DisplayDeathScreen()
    {
        Debug.Log("Player has Died");

        // Show a death message on the bottom bar
        if (hud)
        {
            hud.ShowAuto(this, deathMsgText, deathMsgSeconds);
        }
    }

    // Vignette screen based on damage taken
    private void ChangeDamageEffect()
    {
        Volume volume = FindFirstObjectByType<Volume>();

        // link health with vignette
        if (volume.profile.TryGet(out Vignette vignette))
        {
            float normalized = healthBar.value / healthBar.maxValue; // returns value between 0 and 1
            float inverted = 1f - normalized;
            vignette.intensity.value = inverted * 0.45f;
        }
    }

    private void UpdateHealthSlider(float value)
    {
        healthBar.value = value;
        healthPercentText.text = (int)(healthBar.value / healthBar.maxValue * 100) + "%";
        ChangeDamageEffect(); // spawn blood effect
    }

    private void SelectShootingStrategy(int index)
    {
        if (index == 0)
        {
            bulletEnabled.SetActive(false);
            rocketEnabled.SetActive(true);
        }
        else if (index == 1)
        {
            bulletEnabled.SetActive(true);
            rocketEnabled.SetActive(false);
        }
    }
}
