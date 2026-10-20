using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class AttackScheduler : Singleton<AttackScheduler>
{
    [Header("IDK")]
    [SerializeField] private int listCapacity = 1;
    [SerializeField] private float defSlotStayDuration = 0.4f;

    struct SchedulerEntry
    {
        public float timestamp;
        public float slotStayDuration;

        public SchedulerEntry(float timestamp, float slotStayDuration)
        {
            this.timestamp = timestamp;
            this.slotStayDuration = slotStayDuration;
        }
    }

    private List<SchedulerEntry> attackSlots = new List<SchedulerEntry>();

    // Check if can attack --> if can, add to schedule + execute attack
    public bool VerifyAttackSlot()
    {
        UpdateSlots();
        // list count < capacity = slot available
        // >= capacity --> return false
        // if true, add entry to list, and return true

        if (attackSlots.Count >= listCapacity)
            return false;

        attackSlots.Add(new SchedulerEntry(Time.time, defSlotStayDuration));
        return true;
    }

    private void UpdateSlots()
    {
        // timestamp + stay durationn < Time.time --> remove from list
        attackSlots.RemoveAll(x => x.timestamp + x.slotStayDuration < Time.time);
    }
}
