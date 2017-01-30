using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class DebugText : MonoBehaviour {
    private static DebugText instance;
    public Text text;

    void Awake()
    {
        instance = this;
    }
    void Start () {
        text = GetComponent<Text>();
	}

    public static void SetText(string s)
    {
        instance.text.text = s;
    }
}
