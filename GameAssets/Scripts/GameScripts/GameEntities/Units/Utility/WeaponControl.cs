using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Proficiency))]
public class WeaponControl : MonoBehaviour
{

    public Transform weaponHandLeft;
    public Transform weaponHandRight;

    public Transform currentWeaponLeft;
    public Transform currentWeaponRight;

    // Reference to the proficieny associated with this object
    Proficiency prof;

    public void Awake()
    {
        prof = GetComponent<Proficiency>();
        if (weaponHandLeft == null)
            Debug.LogError(gameObject.name + "'s left hand weapon transform has not been set!");
        if (weaponHandRight == null)
            Debug.LogError(gameObject.name + "'s right hand weapon transform has not been set!");

    }

    public void RemoveWeapon(WeaponHand weaponHand)
    {
        switch (weaponHand)
        {
            case WeaponHand.Left:
                // Destroy current weapon
                if (currentWeaponLeft != null)
                {
                    currentWeaponLeft.transform.parent = null;
                    Destroy(currentWeaponLeft.gameObject);
                }
                break;
            case WeaponHand.Right:
                if (currentWeaponRight != null)
                {
                    currentWeaponRight.transform.parent = null;
                    Destroy(currentWeaponRight.gameObject);
                }
                break;
            case WeaponHand.Both:
                if (currentWeaponLeft != null)
                {
                    currentWeaponLeft.transform.parent = null;
                    Destroy(currentWeaponLeft.gameObject);
                }
                if (currentWeaponRight != null)
                {
                    currentWeaponRight.transform.parent = null;
                    Destroy(currentWeaponRight.gameObject);
                }
                break;
        }
    }

    public bool ChangeWeapon(WeaponGroup.WeaponType weaponType, int index, WeaponHand weaponHand)
    {
        // Remove weapon first
        RemoveWeapon(weaponHand);

        Transform weaponPrefab;
        if (WeaponGroup.Instance.GetWeapon(index, prof, out weaponPrefab, weaponType))
        {
            Transform w;
            switch (weaponHand)
            {
                case WeaponHand.Left:
                    w = Instantiate(weaponPrefab, weaponHandLeft.position, weaponHandLeft.rotation) as Transform;
                    currentWeaponLeft = w;
                    w.parent = weaponHandLeft;
                    break;
                case WeaponHand.Right:
                    w = Instantiate(weaponPrefab, weaponHandRight.position, weaponHandRight.rotation) as Transform;
                    currentWeaponRight = w;
                    w.parent = weaponHandRight;
                    break;
                case WeaponHand.Both:
                    w = Instantiate(weaponPrefab, weaponHandLeft.position, weaponHandLeft.rotation) as Transform;
                    currentWeaponLeft = w;
                    w.parent = weaponHandLeft;

                    w = Instantiate(weaponPrefab, weaponHandRight.position, weaponHandRight.rotation) as Transform;
                    currentWeaponRight = w;
                    w.parent = weaponHandRight;
                    break;
            }
            // Weapon equipped
            return true;
        }
        else
        {
            return false;
        }
    }
}

public enum WeaponHand
{
    Left,
    Right,
    Both
}
