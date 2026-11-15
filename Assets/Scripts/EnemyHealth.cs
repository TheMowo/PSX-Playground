using UnityEngine;

public class EnemyHealth : MonoBehaviour, IDamageable
{
    private float baseHealth = 50; // Stored, unchanged
    private float currentHealth; // Active health pool
    private float maxHealth; // In the event that an enemy can gain overhealth, use this. (for capping currentHealth & health gains)

    [SerializeField] private bool isKillable = true;

    [Header("")]
    [SerializeField] private DamagePopup popupPrefab;
    [SerializeField] private Transform headPosition;

    public void Setup()
    {
        currentHealth = baseHealth;
    }

    public void TakeDamage(DamageInfo damageInfo)
    {
        currentHealth -= damageInfo.Damage;

        if (damageInfo.DamageType == DamageType.BoomerangOutgoing)
        {
            Debug.Log("Hit by outgoing boomerang"); //Scoring goes here
            M_ScoreManager.Instance.AddScore(ScoreId.BoxThrow);
        }
        else if (damageInfo.DamageType == DamageType.BoomerangReturning)
        {
            Debug.Log("Hit by returning boomerang"); //Scoring goes here
            M_ScoreManager.Instance.AddScore(ScoreId.BoxReturn);
        }
        else if (damageInfo.DamageType == DamageType.Dash)
        {
            Debug.Log("Hit by dash"); //Scoring goes here
            M_ScoreManager.Instance.AddScore(ScoreId.Dash);
        }
        else if (damageInfo.DamageType == DamageType.Lava)
        {
            Debug.Log("Hit by lava"); //Scoring goes here
            M_ScoreManager.Instance.AddScore(ScoreId.Lava);
        }
        else if (damageInfo.DamageType == DamageType.Void)
        {
            Debug.Log("Hit by void"); //Scoring goes here
            M_ScoreManager.Instance.AddScore(ScoreId.Void);
        }

        if (!isKillable)
            return;

        if (currentHealth <= 0)
            Death();
    }

    private void Death()
    {
        Destroy(this.gameObject);
    }

    private void SpawnHitEffect(float damage)
    {
        Vector3 spawnPos = headPosition.position;

        spawnPos += Camera.main.transform.forward * 4f;

        DamagePopup popup = Instantiate(popupPrefab, headPosition.position, Quaternion.identity);

        popup.Initialize(damage);
    }
}
