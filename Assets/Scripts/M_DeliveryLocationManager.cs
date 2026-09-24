using UnityEngine;

public class M_DeliveryLocationManager : MonoBehaviour
{
    public static M_DeliveryLocationManager Instance;

    [SerializeField] private DeliveryDestination[] locations;

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetRandomDestination()
    {
        int randomIndex =
            Random.Range(0, locations.Length);

        return locations[randomIndex].transform;
    }
}