using UnityEngine;
using System.Collections;

public class StorageBuilding : JobBuilding {

	// Use this for initialization
	void Start () {
	
	}

    public override void PerformAction(PerformActionEvent actionEvent)
    {
        base.PerformAction(actionEvent);
        if (actionEvent.tag.Equals("Mob"))
        {
            Mob m = actionEvent.entity.GetComponent<Mob>();
            AddWorker(m);

            switch (m.CurrentActivity)
            {
                case ActivityState.Supplying:

                    break;
            }
        }
        
    }

}
