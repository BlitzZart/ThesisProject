using UnityEngine;
using System.Collections;

public class MikaPharusPlayerManager : APharusPlayerManager
{
    public override void RemovePlayer(int trackID) {
        foreach (ATrackingEntity player in _playerList.ToArray()) {

        }
        base.RemovePlayer(trackID);
    }
}
