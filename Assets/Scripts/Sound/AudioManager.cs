using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour
{
    #region Audio Event Fields

    [Header("USER INTERFACE")]
    [SerializeField] private EventReference uiClick_ConfirmEvent;

    [SerializeField] private EventReference uiClick_CancelEvent;

    [SerializeField] private EventReference uiClick_SelectEvent;

    [SerializeField] private EventReference uiMvmt_SliderValEvent;

    [SerializeField] private EventReference uiMvmt_OpenEvent;

    [Header("GUM")]
    [SerializeField] private EventReference gumMove_MoveEvent;

    [SerializeField] private EventReference gumMove_InvalidEvent;

    [SerializeField] private EventReference gumStick_StickEvent;

    [SerializeField] private EventReference gumStick_UnstickEvent;

    [Header("TILES")]
    [SerializeField] private EventReference tlWin_EnterEvent;

    [SerializeField] private EventReference tlWin_ExitEvent;

    [SerializeField] private EventReference tlMud_EnterEvent;

    [Header("AMBIENCE")]
    [SerializeField] private EventReference ambPlchdr_Event;

    #endregion







}
