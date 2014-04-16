using UnityEngine;
using System.Collections;

public class BuilderMob : Mob {

    public override void Start()
    {
        skills.speed = 3;
        FactionFlags = global::FactionFlags.one;
        EnemyFlags = global::FactionFlags.two;
        MobAbiltiyFlags = MobAbiltiyFlags | MobFlags.CanBuild;
        base.Start();
    }

    protected override void LivingUpdate()
    {
        base.LivingUpdate();
    }

    public override void PerformAction(PerformActionVariables actionEvent)
    {
        base.PerformAction(actionEvent);
        if (actionEvent.entity.tag == "Mob")
        {
            AIPath.target = actionEvent.entity.transform;
            ActionTransform = actionEvent.entity.transform;
        }
        else if (actionEvent.entity.tag == "Building")
        {
            AIPath.target = actionEvent.entity.transform;
            ActionTransform = actionEvent.entity.transform;
        }
    }

}
