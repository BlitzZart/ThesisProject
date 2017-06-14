using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MIKA {
    public class UI_ShowPlayerID : MonoBehaviour {
        TextMesh textMesh;
        IMikaTrackedEntity trackedEntity;

        // Use this for initialization
        void Start() {
            textMesh = GetComponent<TextMesh>();
            trackedEntity = transform.parent.GetComponent<IMikaTrackedEntity>();
        }

        // Update is called once per frame
        void Update() {
            textMesh.text = trackedEntity.GetID().ToString();
        }
    }
}