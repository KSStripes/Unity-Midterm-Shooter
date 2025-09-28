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
    [SerializeField] private bool playOnStart = true; // play Tutorial when scene loads
    [SerializeField] private MessageItem[] startMessages; // tutorial sequence in Inspector

    [Header("Low Health")]
    [Range(0.05f, 0.9f)]
    [SerializeField] float lowHealthThreshold = 0.25f;
    [SerializeField] float lowHealthCooldown = 12f;
    [SerializeField] string lowHealthText = "Critical health! Find a charge point.";
    [SerializeField] float lowHealthDisplaySeconds = 2.5f;

    [Header("Death")]
    [SerializeField] string deathMsgText = "YOU DIED";
    [SerializeField] float deathMsgSeconds = 3f;



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
            hud.ShowSequenceAuto(startMessages);
    }

    private void DisplayDeathScreen()
    {
        if(hud) hud.ShowAuto(deathMsgText, deathMsgSeconds);
    }

    // Show game state change message in UI, activated by StateChangeTrigger
    public void ShowStateChangeUI(GameState state, float seconds = 1.8f)
    {
        if(hud) hud.ShowAuto($"Entering {state}", seconds);
    }


    // Vignette screen based on damage taken
    void ChangeDamageEffect()
    {
        var volume = FindFirstObjectByType<Volume>(FindObjectsInactive.Include);
        if (volume && volume.profile && volume.profile.TryGet(out Vignette v))
        {
            float frac = healthBar.value / healthBar.maxValue;
            v.intensity.value = (1f - frac) * 0.45f;
        }
    }

    private void UpdateHealthSlider(float value)
    {
        // Update bar + % label
        healthBar.value = value;
        healthPercentText.text = (int)(healthBar.value / healthBar.maxValue * 100) + "%";

        // Blood/vignette effect
        ChangeDamageEffect();

        // Low-health warning (cooldown-gated)
        float frac = healthBar.value / healthBar.maxValue; // 0..1
        if (hud && frac <= lowHealthThreshold && Time.unscaledTime >= nextLowHealthTime)
        {
            nextLowHealthTime = Time.unscaledTime + lowHealthCooldown;
            hud.ShowAuto(lowHealthText, lowHealthDisplaySeconds);
        }
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
