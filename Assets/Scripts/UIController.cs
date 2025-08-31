using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    private HealthSystem playerHealth;
    private ShootAbility shootAbility;
    [SerializeField] private Slider healthBar;

    [SerializeField] private TextMeshProUGUI healthPercentText;
    [SerializeField] private GameObject bulletEnabled;
    [SerializeField] private GameObject rocketEnabled;

    private void Start()
    {
        playerHealth = PlayerInput.Instance.GetComponent<HealthSystem>();
        shootAbility = PlayerInput.Instance.GetComponent<ShootAbility>();

        shootAbility.OnChangeStrategy += SelectShootingStrategy;

        playerHealth.OnHealthChanged += UpdateHealthSlider;
        playerHealth.OnDeath += DisplayDeathScreen;

        healthBar.maxValue = playerHealth.GetMaxHealth();
        healthBar.value = playerHealth.GetCurrentHealth();
        healthPercentText.text = "100%";
        bulletEnabled.SetActive(false); // not grey out bullet
        rocketEnabled.SetActive(true); // grey out rocket at start
    }

    private void DisplayDeathScreen()
    {
        Debug.Log("Player has Died");
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
