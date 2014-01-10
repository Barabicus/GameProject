using UnityEngine;
using System.Collections;

/// <summary>
/// Tag Types in a singleton for storing the cached values of the Entity tags
/// </summary>
public static class TagTypes {

    public readonly static int mob = "Mob".GetHashCode();
    public readonly static int building = "Building".GetHashCode();
    public readonly static int tree = "Tree".GetHashCode();
    public readonly static int blueprint = "Blueprint".GetHashCode();
    public readonly static int rock = "Rock".GetHashCode();
    public readonly static int ground = "Ground".GetHashCode();

}
