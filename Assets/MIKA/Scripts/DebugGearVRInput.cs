using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugGearVRInput : MonoBehaviour {
    private Text m_Text;
    // Use this for initialization
    void Start () {
        m_Text = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKey(KeyCode.Escape)) {
            m_Text.text = "calibrate";
        } else {
            m_Text.text = "";
        }
    }
}
