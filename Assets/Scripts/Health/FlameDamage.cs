using UnityEngine;

public class FlameDamage : MonoBehaviour
{
    [SerializeField] private float radius = 2f;            // flame harm radius
    [SerializeField] private float damagePerSecond = 10f;  // damage at the center per second
    [SerializeField] private bool linearFalloff = true;    // reduce damage toward the edge
    [SerializeField] private string playerTag = "Player";  // player must be tagged "Player"

    private HealthSystem playerHealth;   // reference to the single HealthSystem on the player
    private Transform player;            // player's transform (for distance checks)

    private void Awake()
    {
        // Find the player and its HealthSystem once
        var go = GameObject.FindGameObjectWithTag(playerTag);
        if (go)
        {
            player = go.transform;
            playerHealth = go.GetComponent<HealthSystem>();
        }
        if (!playerHealth) Debug.LogWarning("FlameDamage: Player or HealthSystem not found.");
    }

    private void Update()
    {
        if (!player || !playerHealth) return;

        // Quick distance check (squared for speed)
        Vector3 toPlayer = player.position - transform.position;
        float r2 = radius * radius;
        float d2 = toPlayer.sqrMagnitude;
        if (d2 > r2) return; // outside radius → no damage

        // Damage factor: 1 at center, 0 at radius edge (if falloff enabled)
        float factor = 1f;
        if (linearFalloff)
        {
            float d = Mathf.Sqrt(d2);
            factor = Mathf.Clamp01(1f - d / radius);
        }

        // Apply DPS scaled by deltaTime
        float dmg = damagePerSecond * factor * Time.deltaTime;
        if (dmg <= 0f) return;

        playerHealth.DecreaseHealth(dmg);
        Debug.Log($"Damage: {dmg}  Health: {playerHealth.GetCurrentHealth()}/{playerHealth.GetMaxHealth()}");
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(1f, 0.4f, 0.1f, 0.25f);
        Gizmos.DrawSphere(transform.position, radius);
    }
#endif
}
