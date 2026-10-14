using UnityEngine;

public class Boomerang : MonoBehaviour
{
    private enum State
    {
        FlyingOut,
        Returning
    }

    [Header("Movement")]
    [SerializeField] private float throwDistance = 15f;
    [SerializeField] private float travelSpeed = 25f;
    [SerializeField] private float returnSpeed = 35f;

    [Header("Rotation")]
    [SerializeField] private Transform model;
    [SerializeField] private float spinSpeed = 720f;

    [Header("Combat")]
    [SerializeField] private float damage = 25f;

    [Header("Collision")]
    [SerializeField] private LayerMask hitLayers;

    private State currentState;

    private Vector3 targetPosition;
    private ThrowActuator owner;
    private Transform returnTarget;

    public void Initialize(ThrowActuator ownerScript, Vector3 throwDirection)
    {
        owner = ownerScript;
        returnTarget = ownerScript.transform;

        targetPosition =
            transform.position +
            throwDirection.normalized * throwDistance;

        currentState = State.FlyingOut;
    }

    private void Update()
    {
        RotateWeapon();

        switch (currentState)
        {
            case State.FlyingOut:
                FlyOut();
                break;

            case State.Returning:
                ReturnToPlayer();
                break;
        }
    }

    private void RotateWeapon()
    {
        if (model != null)
        {
            model.Rotate(Vector3.up, spinSpeed * Time.deltaTime, Space.Self);
        }
    }

    private void FlyOut()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            travelSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentState = State.Returning;
        }
    }

    private void ReturnToPlayer()
    {
        transform.position = Vector3.MoveTowards(
            transform.position,
            returnTarget.position,
            returnSpeed * Time.deltaTime);

        if (Vector3.Distance(
            transform.position,
            returnTarget.position) < 1f)
        {
            owner.WeaponReturned();
            Destroy(gameObject);
        }
    }

    private void HandleHit(GameObject obj)
    {
        if(obj.TryGetComponent<IDamageable>(out IDamageable damageable))
        {
            damageable.TakeDamage(damage);
            Debug.Log($"{name} dealt {damage} damage to {damageable}");
        }

        if (currentState == State.Returning)
            return;

        if ((hitLayers.value & (1 << obj.layer)) == 0)
            return;

        currentState = State.Returning;
    }

    private void OnTriggerEnter(Collider other)
    {
        HandleHit(other.gameObject);
    }

    private void OnCollisionEnter(Collision other)
    {
        HandleHit(other.gameObject);
    }
}