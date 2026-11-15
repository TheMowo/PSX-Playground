using UnityEngine;

public class ThrowActuator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Boomerang boomerangPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject heldWeaponModel;

    private bool weaponOut;

    private void Start()
    {
        if (M_InputManager.Instance != null)
        {
            M_InputManager.Instance.AltAttackPerformed += OnAltAttackPerformed;
        }
    }

    private void OnDestroy()
    {
        if (M_InputManager.Instance != null)
        {
            M_InputManager.Instance.AltAttackPerformed -= OnAltAttackPerformed;
        }
    }

    private void OnAltAttackPerformed()
    {
        if (weaponOut)
            return;

        ThrowWeapon();
    }

    private void ThrowWeapon()
    {
        weaponOut = true;

        if (heldWeaponModel != null)
        {
            heldWeaponModel.SetActive(false);
        }

        Boomerang spawnedWeapon = Instantiate(boomerangPrefab, throwPoint.position, throwPoint.rotation);

        spawnedWeapon.Initialize(this, Camera.main.transform.forward);
    }

    public void WeaponReturned()
    {
        weaponOut = false;

        if (heldWeaponModel != null)
        {
            heldWeaponModel.SetActive(true);
        }
    }
}