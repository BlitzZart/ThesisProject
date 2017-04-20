using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

    public GameObject prefab;

	// Use this for initialization
	void Start () {
        StartCoroutine(Fire());
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private IEnumerator Fire() {
        while(true) {
            yield return new WaitForSeconds(0.1f);
            GameObject go = Instantiate(prefab, transform.position, Quaternion.identity);
            go.GetComponent<Rigidbody>().AddForce(-transform.up * 25, ForceMode.Impulse);
        }
    }

}
