using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Singleton class to store the list of ActiveEntities that can be selected
/// Note this class does not actually store the list of selected entities, rather
/// just a list of entites that can be selected. For example the players units in a living state
/// when a unit dies they are removed from this list and are therefor rendered un-selectable.
/// Similiary enemy units and environment entities are always unselectable
/// </summary>
public static class SelectableList{

    private static List<ActiveEntity> _list = new List<ActiveEntity>();

    public static List<ActiveEntity> SelectedList
    {
        get
        {
            return _list;
        }
    }

    public static void AddSelectableEntity(ActiveEntity t)
    {
        _list.Add(t);
    }

    public static void RemoveSelectableEntity(ActiveEntity t)
    {
        _list.Remove(t);
    }

}
