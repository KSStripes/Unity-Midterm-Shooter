using UnityEngine;

public class ExplosionDamage : MonoBehaviour
{
    [SerializeField] private float radius = 15f;      // explosion radius
    [SerializeField] private float maxDamage = 40f;  // damage at the center
    [SerializeField] private string playerTag = "Player"; // get player by tag

    private HealthSystem playerHealth;
    private Transform player;

    private void Awake()
    {
        // Find the player and its HealthSystem once
        var playerGO = GameObject.FindGameObjectWithTag(playerTag);
        if (playerGO)
        {
            player = playerGO.transform;
            playerHealth = playerGO.GetComponent<HealthSystem>();
        }
    }

    private void OnEnable()
    {
        if (!player || !playerHealth)
        {
            Debug.LogWarning("ExplosionDamage: Player or HealthSystem not found.");
            Destroy(this);
            return;
        }

        // Distance from explosion to player
        float d = Vector3.Distance(transform.position, player.position);
        if (d > radius)
        {
            // Out of range; no damage
            Destroy(this);
            return;
        }

        // Linear falloff (center = maxDamage, radius edge = 0)
        float factor = Mathf.Clamp01(1f - d / radius);
        float dmg = maxDamage * factor;

        playerHealth.DecreaseHealth(dmg); // apply damage to the single HealthSystem
        Debug.Log($"Damage: {dmg}  Health: {playerHealth.GetCurrentHealth()}/{playerHealth.GetMaxHealth()}");

        Destroy(this); // one-shot; let FX continue playing
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.3f, 0.1f, 0.35f);
        Gizmos.DrawSphere(transform.position, radius);
    }
#endif
}
