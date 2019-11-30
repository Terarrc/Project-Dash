using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SavedData
{
    public int worldLevel;
    public int roomNumber;
    public bool DoubleJumpAcquired;
    public bool DashAcquired;
    public bool EnergySlideAcquired;

    public SavedData(/* scene as parameter ? */)
    {
        // Get all Data
    }
}
