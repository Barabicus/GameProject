using UnityEngine;
using System.Collections;

public class OgreAI : Mob
{

    // Use this for initialization
    void Start()
    {
        MobName = "something";
        skills.speed = 3;
        skills.attackPower = 1;
        skills.attackSpeed = 0.5f;
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
                case ActivityState.Supplying:
                    TryPerformAction(new PerformActionEvent(this, tag), ActionEntity);
                    break;
                case ActivityState.Woodcutting:
                    TryPerformAction(new PerformActionEvent(this, tag), ActionEntity);
                    Animator.SetTrigger("chopWood");
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
                    CurrentActivity = ActivityState.Supplying;
                    SetEntityAndFollow(actionEvent.entity);
                }
                break;
            case "Tree":
                CurrentActivity = ActivityState.Woodcutting;
                SetEntityAndFollow(actionEvent.entity);
                break;
        }
    }


}
