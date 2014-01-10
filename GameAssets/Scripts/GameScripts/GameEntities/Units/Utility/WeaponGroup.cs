using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponGroup : MonoBehaviour
{


    #region Properties

    public static WeaponGroup Instance { get; private set; }

    #endregion

    #region Fields

    /// <summary>
    /// Variable should not be accessed be accessed directly for actual gameplay scripts, use GetSword instead
    /// </summary>
    public Transform[] swords;
    /// <summary>
    /// Variable should not be accessed be accessed directly for actual gameplay scripts, use GetStaff instead
    /// </summary>
    public Transform[] staffs;
    /// <summary>
    /// Variable should not be accessed be accessed directly for actual gameplay scripts, use GetAxe instead
    /// </summary>
    public Transform[] axes;

    #endregion

    #region Enums

    public enum WeaponType
    {
        Sword,
        Staff,
        Axe
    }

    #endregion

    #region Initializers

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Debug.LogWarning("Instance group is not null");
            Destroy(gameObject);
        }
    }

    #endregion

    /// <summary>
    /// Gets the weapon at the specified index if the proficieny allows it
    /// </summary>
    /// <param name="index">The index of the weapon</param>
    /// <param name="prof">The characters proficiency</param>
    /// <param name="weapon">The actual weapon that is returned. Null if the character does not meet the required proficiency</param>
    /// <returns>Returns true if the character can use the weapon with the supplied proficieny</returns>
    public bool GetWeapon(int index, Proficiency prof, out Transform weapon, WeaponType weaponType)
    {
        // Check to see if the proficieny of the character can use this item
        if ((prof.wepFlags & WeaponFlagType(weaponType)) != Proficiency.WeaponFlags.none)
        {
            weapon = swords[index];
            return true;
        }
        else
        {
            weapon = null;
            return false;
        }
    }

    /// <summary>
    /// Helper class to get the weaponType
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private Proficiency.WeaponFlags WeaponFlagType(WeaponType type)
    {
        switch (type)
        {
            case WeaponType.Axe:
                return Proficiency.WeaponFlags.axe;
                break;
            case WeaponType.Staff:
                return Proficiency.WeaponFlags.staff;
                break;
            case WeaponType.Sword:
                return Proficiency.WeaponFlags.sword;
                break;
            default:
                return Proficiency.WeaponFlags.none;
        }
    }



}
