using UnityEngine;
using System.Collections;

public class TestPlayer : ATrackingEntity
{
    void Update() {
        transform.position = new Vector3(transform.position.x, transform.position.y, 1000);
    }
}
