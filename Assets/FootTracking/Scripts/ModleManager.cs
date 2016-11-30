using UnityEngine;
using System.Collections;

public class ModleManager : MonoBehaviour { 
    private PlayerWithFeet trackedPlayer;

    public void SetUsedCenter() {
        if (trackedPlayer == null)
            trackedPlayer = FindObjectOfType<PlayerWithFeet>();
        if (trackedPlayer == null)
            return;
        trackedPlayer.SetUsedCenter(!trackedPlayer.useCorrectedCenter);
    }
}