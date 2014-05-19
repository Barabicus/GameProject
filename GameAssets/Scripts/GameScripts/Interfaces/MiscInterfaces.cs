using UnityEngine;
using System.Collections;

public interface IRequestManager
{

}

public interface ISelectable
{
    bool IsSelected { get; set; }
}

public interface IResource
{
    Resource Resource { get; set; }
}

public interface IUnitName
{
    string UnitName { get; set; }
}

public interface ICitymanager
{
    CityManager CityManager { get; set; }
}

public interface IDamageable
{
    bool Damage(int damage);
    int CurrentHp { get; set; }
    int MaxHP { get; set; }
}

public interface IFactionFlag
{
    FactionFlags FactionFlags { get; set; }
}

public interface ICurrencyContainer
{
    int Currency { get; set; }
}

[System.Flags]
public enum FactionFlags
{
    None = 0x0,
    one,
    two,
    three,
    four
}
