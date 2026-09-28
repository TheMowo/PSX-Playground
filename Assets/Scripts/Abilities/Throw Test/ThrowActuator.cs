using UnityEngine;

public class ThrowActuator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Boomerang boomerangPrefab;
    [SerializeField] private Transform throwPoint;
    [SerializeField] private GameObject heldWeaponModel;

    private bool weaponOut;

    private void Update()
    {
        if (Input.GetMouseButtonDown(1) && !weaponOut)
        {
            ThrowWeapon();
        }
    }

    private void ThrowWeapon()
    {
        weaponOut = true;

        heldWeaponModel.SetActive(false);

        Boomerang spawnedWeapon = Instantiate(
            boomerangPrefab,
            throwPoint.position,
            throwPoint.rotation);

        spawnedWeapon.Initialize(
            this,
            Camera.main.transform.forward);
    }

    public void WeaponReturned()
    {
        weaponOut = false;

        if (heldWeaponModel != null)
            heldWeaponModel.SetActive(true);
    }
}