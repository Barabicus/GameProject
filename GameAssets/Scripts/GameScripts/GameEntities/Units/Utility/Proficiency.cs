using UnityEngine;
using System.Collections;

public class Proficiency : MonoBehaviour {

    [System.Flags]
    public enum WeaponFlags
    {
        none = 0x0,
        sword,
        axe,
        staff
    }

    public WeaponFlags wepFlags = WeaponFlags.none;

    public void AddFlag(WeaponFlags flag)
    {
        wepFlags = wepFlags | flag;
    }

    public void RemoveFlag(WeaponFlags flag)
    {
        wepFlags = wepFlags & (~flag);
    }

}
