using UnityEngine;
using System.Collections;

public class SetPositionTo : MonoBehaviour {
    public Transform target;
    private int lerpSpeed = 25;
    private float height = 1.60f;

    private void Update () {
        if (target == null)
            return;
        transform.position = Vector3.Lerp(transform.position, new Vector3(target.position.x, height, target.position.z), lerpSpeed * Time.deltaTime);
	}
}
