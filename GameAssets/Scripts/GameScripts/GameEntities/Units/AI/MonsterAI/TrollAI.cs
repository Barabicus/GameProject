using UnityEngine;
using System.Collections;

public class TrollAI : Mob {

    // Use this for initialization
    void Start()
    {
        MobName = "Mr.Troll";
        skills.speed = 3;
        skills.attackPower = 1;
        skills.attackSpeed = 0.5f;
        skills.buildPower = 5;
        FactionFlags = global::FactionFlags.one;
        EnemyFlags = global::FactionFlags.two;
        base.Start();
    }

    protected override void LivingUpdate()
    {
        base.LivingUpdate();
        if (ActionTransform != null)
        {
            switch (CurrentActivity)
            {
                case ActivityState.Building:
                    TryPerformAction(new PerformActionEvent(this, tag), ActionEntity);
                    break;
            }
        }
    }

    public override void PerformAction(PerformActionEvent actionEvent)
    {
        base.PerformAction(actionEvent);
        switch (actionEvent.tag)
        {
            case "BluePrint":
                if (!IsEnemey(actionEvent.entity.FactionFlags))
                {
                    CurrentActivity = ActivityState.Building;
                    SetEntityAndFollow(actionEvent.entity);
                }
                break;
        }
    }
	
}
