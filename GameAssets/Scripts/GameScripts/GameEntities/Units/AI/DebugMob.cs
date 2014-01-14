using UnityEngine;
using System.Collections;

public class DebugMob : Mob {

    protected override void Start()
    {
        MobName = "debugMob";
        skills.speed = 3;
        skills.attackPower = 1;
        skills.attackSpeed = 0.5f;
        skills.buildPower = 5;
        FactionFlags = global::FactionFlags.one;
        EnemyFlags = global::FactionFlags.two;
        MobAbiltiyFlags = MobAbiltiyFlags | MobFlags.CanBuild | MobFlags.CanWoodcut;
       // Weapon.ChangeWeapon(WeaponGroup.WeaponType.Sword, 0, WeaponHand.Right);
        base.Start();

    }

    protected override void LivingUpdate()
    {
        base.LivingUpdate();
        if (ActionTransform != null)
        {
            switch (CurrentActivity)
            {
                case ActivityState.Attacking:
                    if (distanceToTarget() < 5f)
                    {
                        Attack(ActionEntity, skills.attackPower);
                    }
                    break;
                case ActivityState.Woodcutting:
                    if (distanceToTarget() < 5f)
                    {
                        if(ActionTransform.GetComponent<WorldResource>().CanHarvest(MobAbiltiyFlags))
                            TryPerformAction(new PerformActionEvent(this, tag, new int[]{1}), ActionEntity);
                        Animator.SetTrigger("chopWood");
                        Debug.Log("R: " + Resource);
                    }
                    break;
                case ActivityState.Building:
                    if (distanceToTarget() < 5f)
                    {
                        TryPerformAction(new PerformActionEvent(this, tag), ActionEntity);
                    }
                    break;
            }
        }
    }

}
