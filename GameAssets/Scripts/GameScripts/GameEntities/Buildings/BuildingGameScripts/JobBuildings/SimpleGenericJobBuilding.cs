using UnityEngine;
using System.Collections;

public class SimpleGenericJobBuilding : JobBuilding
{
    public Transform point;

    enum JobType
    {
        Woodcutting,
        Building
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Awake()
    {
        base.Awake();
    }

    public override void PerformAction(PerformActionEvent actionEvent)
    {
        base.PerformAction(actionEvent);
        switch (actionEvent.tag)
        {
            case "Mob":
                AddWorker(actionEvent.entity as Mob);
                (actionEvent.entity as Mob).JobTask = JobTask;
                break;
        }
    }

    protected override void Tick()
    {
        base.Tick();
        foreach (RaycastHit hit in Physics.SphereCastAll(new Ray(point.position, Vector3.forward), 100f, 1f))
        {
            if (hit.collider.GetComponent<ActiveEntity>() == null)
            {
                Debug.Log("Null: " + hit.collider.tag);
            }
            else
            {
                hit.collider.GetComponent<ActiveEntity>().ObjectHighlight.ConstantOn(Color.green);
            }
        }
    }

    void JobTask(Mob mob)
    {
        Debug.Log(mob.ActionEntity);
        if (mob.ActionEntity == null)
        {
            foreach (RaycastHit hit in Physics.SphereCastAll(new Ray(point.position, Vector3.up), 20f, 50f, 1 << 11))
            {
            }
        }
        mob.ActionEntity = mob;
    }

}

