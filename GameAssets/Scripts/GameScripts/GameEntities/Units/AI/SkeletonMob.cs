using UnityEngine;
using System.Collections;

public class SkeletonMob : Mob
{

    // Use this for initialization
    void Start()
    {
        FactionFlags = global::FactionFlags.two;
        EnemyFlags = global::FactionFlags.one & global::FactionFlags.three;
        MobName = "Skelly";
        skills.speed = 1;
        skills.attackPower = 5;
        skills.attackSpeed = 0.5f;
        skills.buildPower = 5;
        base.Start();
    }

    protected override void LivingUpdate()
    {
        base.LivingUpdate();
        switch (CurrentActivity)
        {
            case ActivityState.Attacking:
                if (distanceToTarget() < 5f)
                {
                    Attack(ActionEntity, skills.attackPower);
                }
                break;
        }
    }

    protected override void OnTriggerLOSEnter(TriggerData data)
    {
        base.OnTriggerLOSEnter(data);
        if (data.triggerType == TagTypes.mob)
        {
            if (IsEnemey(data.entity.FactionFlags))
            {
                enemies.Add(data.entity);
                CurrentActivity = ActivityState.Attacking;
                SetEntityAndFollow(data.entity);
            }
        }
    }



}
