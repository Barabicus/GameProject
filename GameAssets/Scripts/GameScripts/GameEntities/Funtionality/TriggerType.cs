using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// The TriggerType class specifies what tags are valid for dealing with Line Of Sight collision detection.
/// Any tag that is added here is deemed valid and will cause either the ChaseLOS or TriggerLOS to fire
/// a collision event.
/// </summary>
public static  class TriggerType {

    private static List<int> valid = new List<int>();

    static TriggerType()
    {
        valid.Add(TagTypes.mob);
        valid.Add(TagTypes.building);
        valid.Add(TagTypes.tree);
    }

    public static bool isValid(int type)
    {
        return valid.Contains(type);
    }

    public static bool isValid(string type)
    {
        return valid.Contains(type.GetHashCode());
    }

}
