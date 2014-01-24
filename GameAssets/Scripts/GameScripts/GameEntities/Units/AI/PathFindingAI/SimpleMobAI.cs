using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Seeker))]
public class SimpleMobAI : AIPath {


    public override Transform target
    {
        get
        {
            return base.target;
        }
        set
        {
            Debug.Log("set target");
            if (value == null)
            {
                base.target = null;
                return;
            }
            Vector3 heading = value.transform.position - transform.position;
            heading.Normalize();
            targetCoord = value.transform.position - heading * (endReachedDistance);
        }
    }

    /** Minimum velocity for moving */
    public float sleepVelocity = 0.4F;

    public bool debug = false;

    private Animator anim;

    public new void Start()
    {
        anim = GetComponent<Animator>();
        base.Start();
    }

    public override void OnTargetReached()
    {
        base.OnTargetReached();
        target = null;
        targetCoord = null;

    }

    public override Vector3 GetFeetPosition()
    {
        return tr.position;
    }

    protected new void FixedUpdate()
    {

        //Get velocity in world-space
        Vector3 velocity;
        if (canMove)
        {

            //Calculate desired velocity
            Vector3 dir = CalculateVelocity(GetFeetPosition());

            if (debug)
            {
                Vector3 heading = Vector3.zero;
                if (target != null || targetCoord != null)
                {
                    heading = (target != null ? target.position : targetCoord.Value) - transform.position;
                }
                Debug.Log(heading.magnitude + " : " + heading.sqrMagnitude);
            }

            //Rotate towards targetDirection (filled in by CalculateVelocity)
            RotateTowards(targetDirection);

            dir.y = 0;
            if (dir.sqrMagnitude > sleepVelocity * sleepVelocity)
            {
                //If the velocity is large enough, move
            }
            else
            {
                //Otherwise, just stand still (this ensures gravity is applied)
                dir = Vector3.zero;
            }

            if (navController != null)
            {
#if FALSE
				navController.SimpleMove (GetFeetPosition(), dir);
#endif
            }
            else if (controller != null)
                controller.SimpleMove(dir);
            else
                Debug.LogWarning("No NavmeshController or CharacterController attached to GameObject");

            velocity = controller.velocity;
        }
        else
        {
            velocity = Vector3.zero;
        }

        //Calculate the velocity relative to this transform's orientation
        Vector3 relVelocity = tr.InverseTransformDirection(velocity);
        relVelocity.y = 0;

        if (velocity.sqrMagnitude <= sleepVelocity * sleepVelocity)
        {
            anim.SetFloat("Speed", 0);
        }
        else
        {
            anim.SetFloat("Speed", speed / 3);
        }
    }

}
