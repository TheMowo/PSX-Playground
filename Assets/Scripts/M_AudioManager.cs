using FMOD.Studio;
using FMODUnity;
using UnityEngine;

public class M_AudioManager : Singleton<M_AudioManager>
{
    #region Player Refs

    [Header("Player")]
    [SerializeField] private EventReference playerFootstepEvent;
    [SerializeField] private EventReference playerJumpEvent;
    [SerializeField] private EventReference playerLandEvent;
    [SerializeField] private EventReference playerDashEvent;
    [SerializeField] private EventReference playerDamageEvent;
    [SerializeField] private EventReference playerDeathEvent;

    [SerializeField] private EventReference uiTypewriterSound;

    #endregion

    #region Player Play Methods

    public void PlayPlayerFootstep(GameObject player, SurfaceType surface)
    {
        if (playerFootstepEvent.IsNull)
            return;

        EventInstance instance = RuntimeManager.CreateInstance(playerFootstepEvent);

        instance.setParameterByName("Surface", (int)surface);

        RuntimeManager.AttachInstanceToGameObject(instance, player);

        instance.start();
        instance.release();
    }

    public void PlayPlayerJump(GameObject player)
    {
        PlayAttachedOneShot(playerJumpEvent, player);
    }

    public void PlayPlayerLand(GameObject player)
    {
        PlayAttachedOneShot(playerLandEvent, player);
    }

    public void PlayPlayerDash(GameObject player)
    {
        PlayAttachedOneShot(playerDashEvent, player);
    }

    public void PlayPlayerDamage(GameObject player)
    {
        PlayAttachedOneShot(playerDamageEvent, player);
    }

    public void PlayPlayerDeath(GameObject player)
    {
        PlayAttachedOneShot(playerDeathEvent, player);
    }

    public void PlayTypewriterClack()
    {
        PlayGenericOneShot(uiTypewriterSound);
    }

    #endregion

    #region Generic

    [Tooltip("Equivalent of AudioSource.Play(sound)")]
    public void PlayGenericOneShot(EventReference eventReference)
    {
        if (eventReference.IsNull)
            return;

        EventInstance instance = RuntimeManager.CreateInstance(eventReference);

        instance.start();
        instance.release();
    }

    [Tooltip("Plays a sound attached to an object")]
    public void PlayAttachedOneShot(EventReference eventReference, GameObject attachedObject)
    {
        if (eventReference.IsNull)
            return;

        RuntimeManager.PlayOneShotAttached(eventReference, attachedObject);
    }

    public void StopAllSfx()
    {
        RuntimeManager.GetBus("Bus:/SFX").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    public void StopAllAudio()
    {
        RuntimeManager.GetBus("Bus:/").stopAllEvents(FMOD.Studio.STOP_MODE.ALLOWFADEOUT);
    }

    #endregion
}