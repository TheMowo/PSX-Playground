using TMPro;
using UnityEngine;

public class M_CurrencyManager : MonoBehaviour
{
    public static M_CurrencyManager Instance;

    [Header("Currency")]
    [SerializeField] private int currentMoney = 0;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI moneyText;

    public int CurrentMoney => currentMoney;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        UpdateUI();
    }

    /// <summary>
    /// Adds currency to the player.
    /// </summary>
    public void AddMoney(int amount)
    {
        if (amount <= 0)
            return;

        currentMoney += amount;

        Debug.Log($"Received ${amount}. Balance: ${currentMoney}");

        UpdateUI();
    }

    /// <summary>
    /// Attempts to spend currency.
    /// Returns true if successful.
    /// </summary>
    public bool SpendMoney(int amount)
    {
        if (amount <= 0)
            return false;

        if (currentMoney < amount)
        {
            Debug.Log("Not enough money.");
            return false;
        }

        currentMoney -= amount;

        Debug.Log($"Spent ${amount}. Balance: ${currentMoney}");

        UpdateUI();

        return true;
    }

    /// <summary>
    /// Checks if player has enough money.
    /// </summary>
    public bool HasMoney(int amount)
    {
        return currentMoney >= amount;
    }

    private void UpdateUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"${currentMoney:N0}";
        }
    }
}