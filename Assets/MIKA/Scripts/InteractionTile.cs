using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshRenderer))]
public class InteractionTile : MonoBehaviour {

    public Material on, off;
    private MeshRenderer meshRenderer;

	void Start () {
        meshRenderer = GetComponent<MeshRenderer>();
	}
	void Update () {
	
	}

    private void OnTriggerEnter(Collider other) {
        meshRenderer.material = on;
    }
    private void OnTriggerExit(Collider other) {
        meshRenderer.material = off;
    }
}