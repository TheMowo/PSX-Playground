using System;
using TMPro;
using UnityEngine;

public class M_DeliveryManager : MonoBehaviour
{
    public static M_DeliveryManager Instance;

    // Events
    public event Action<ActiveDelivery> OnDeliveryStarted;
    public event Action<int> OnDeliveryCompleted;
    public event Action OnDeliveryCancelled;

    // UI
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI distanceText;
    [SerializeField] private TextMeshProUGUI statusText;

    // Data
    private ActiveDelivery activeDelivery;

    public bool HasActiveDelivery => activeDelivery != null;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Update()
    {
        if (activeDelivery == null)
        {
            statusText.text = string.Empty;
            return;
        }

        statusText.text = "Holding Package";

        UpdateDistanceUI();

        if (!activeDelivery.IsOvertime)
        {
            activeDelivery.TimeRemaining -= Time.deltaTime;

            if (activeDelivery.TimeRemaining <= 0f)
            {
                activeDelivery.TimeRemaining = 0f;
                activeDelivery.IsOvertime = true;

                Debug.Log("Delivery entered overtime.");
            }
        }
        else
        {
            activeDelivery.Overtime += Time.deltaTime;
        }

        UpdateUI();
    }

    public void StartDelivery(
        DeliveryData data,
        Transform destination)
    {
        if (HasActiveDelivery)
        {
            Debug.LogWarning("Already delivering.");
            return;
        }

        if (data == null || destination == null)
        {
            Debug.LogError("Invalid delivery setup.");
            return;
        }

        activeDelivery = new ActiveDelivery(data, destination);

        OnDeliveryStarted?.Invoke(activeDelivery);

        M_InventoryManager.Instance.AddItem(
            data.packageItem,
            1);

        Debug.Log(
            $"Started delivery: {data.deliveryName}" +
            $" -> {destination.name}");
    }

    public void CompleteDelivery()
    {
        if (activeDelivery == null)
            return;

        if (!M_InventoryManager.Instance.HasItem(
                activeDelivery.Data.packageItem))
        {
            Debug.LogWarning(
                "Cannot complete delivery. Package missing.");
            return;
        }

        int reward = CalculateReward();

        Debug.Log(
            $"Delivery complete. Reward: {reward}");

        M_CurrencyManager.Instance.AddMoney(reward);

        M_InventoryManager.Instance.RemoveItem(
            activeDelivery.Data.packageItem,
            1);

        OnDeliveryCompleted?.Invoke(reward);

        EndDelivery();

        statusText.text = "Holding Package";
    }

    public void CancelDelivery()
    {
        if (activeDelivery == null)
            return;

        Debug.Log("Delivery cancelled.");

        if (M_InventoryManager.Instance.HasItem(
                activeDelivery.Data.packageItem))
        {
            M_InventoryManager.Instance.RemoveItem(
                activeDelivery.Data.packageItem,
                1);
        }

        OnDeliveryCancelled?.Invoke();

        EndDelivery();
    }

    private void EndDelivery()
    {
        activeDelivery = null;

        if (timerText != null)
            timerText.text = string.Empty;
    }

    private int CalculateReward()
    {
        int penalty =
            Mathf.RoundToInt(activeDelivery.Overtime) *
            activeDelivery.Data.overtimePenaltyPerSecond;

        int reward =
            activeDelivery.Data.baseReward - penalty;

        return Mathf.Max(0, reward);
    }

    public Transform GetCurrentDestination()
    {
        return activeDelivery?.Destination;
    }

    public ActiveDelivery GetActiveDelivery()
    {
        return activeDelivery;
    }

    private void UpdateUI()
    {
        if (activeDelivery == null)
        {
            return;
        }

        if (!activeDelivery.IsOvertime)
        {
            int minutes =
                Mathf.FloorToInt(
                    activeDelivery.TimeRemaining / 60f);

            int seconds =
                Mathf.FloorToInt(
                    activeDelivery.TimeRemaining % 60f);

            timerText.text =
                $"{minutes:00}:{seconds:00}";
        }
        else
        {
            int overtime =
                Mathf.FloorToInt(
                    activeDelivery.Overtime);

            int reward =
                CalculateReward();

            timerText.text =
                $"OVERTIME +{overtime}s | ${reward}";
        }
    }

    private void UpdateDistanceUI()
    {
        if (distanceText == null)
            return;

        GameObject player = PlayerPublicRef.Instance.gameObject;

        float distance = Vector3.Distance(
            player.transform.position,
            activeDelivery.Destination.position);

        distanceText.text = $"{distance:0}m";
    }
}