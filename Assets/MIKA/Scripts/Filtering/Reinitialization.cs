using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public delegate void ReinitializeFilter(float q, float r);

public class Reinitialization : MonoBehaviour {
    public static event ReinitializeFilter Reinitialize;

    public InputField Q, R;

    public void TriggerReinitialization() {
        float q = float.Parse(Q.text);
        float r = float.Parse(R.text);

        if (Reinitialize != null)
            Reinitialize(q, r);
    }
}
