using UnityEngine;
using System.Collections;

public struct BlueprintContract
{
    private BuildingConstructor _buildingConst;
    private Building _hasContract;

    public BlueprintContract(BuildingConstructor buildingConst, Building hasContract)
    {
        this._buildingConst = buildingConst;
        this._hasContract = hasContract;
    }
}
