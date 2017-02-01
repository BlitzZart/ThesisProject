using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using ModularIK;

public class VisibilityManager : MonoBehaviour {
    public RawImage topDownView;
    public SkinnedMeshRenderer model;
    public PlayerFoot[] feet;
    private IKControl ikControl;
    private UnityPharusFootTracking footTracking;
    private MeshRenderer center;

    void Start() {

    }

    private bool GetFeet()
    {
        if (feet.Length < 2)
            feet = FindObjectsOfType<PlayerFoot>();

        return feet.Length > 1;
    }
    private bool GetIK()
    {
        if (ikControl == null)
            ikControl = FindObjectOfType<IKControl>();

        return ikControl != null;
    }
    private bool GetTrackingEntity()
    {
        if (footTracking == null)
            footTracking = FindObjectOfType<UnityPharusFootTracking>();

        return footTracking != null;
    }
    private bool GetCenter()
    {
        if (!GetTrackingEntity())
            return false;

        if (center == null)
            center = footTracking.GetComponentInChildren<MeshRenderer>();

        return center != null;
    }

    public void EnableIK()
    {
        if (!GetIK())
            return;

        ikControl.ikActive = !ikControl.ikActive;
    }
    public void ShowTopDown() {
        topDownView.enabled = !topDownView.enabled;
    }
    public void ShowLineRenderers()
    {
        if (!GetFeet())
            return;

        foreach (PlayerFoot item in feet)
        {
            LineRenderer lr = item.GetComponent<LineRenderer>();
            lr.enabled = !lr.enabled;
        }
    }
    public void ShowFeet() {
        if (!GetFeet())
            return;

        foreach (PlayerFoot item in feet)
        {
            MeshRenderer mr = item.GetComponent<MeshRenderer>();
            mr.enabled = !mr.enabled;
        }
    }
    public void ShowCenter()
    {
        if (!GetCenter())
            return;

        center.enabled = !center.enabled;
    }
    public void ShowModel() {
        model.enabled = !model.enabled;
    }

    public void EnableFilter()
    {
        if (!GetTrackingEntity())
            return;

        footTracking.applyFilterOnFeet = !footTracking.applyFilterOnFeet;
    }
    public void EnableCorrection()
    {
        if (!GetTrackingEntity())
            return;

        footTracking.correctCrossing = !footTracking.correctCrossing;
    }
    public void EnableFootHeight()
    {
        if (!GetFeet())
            return;

        foreach (PlayerFoot item in feet)
        {
            item.enableFootHeight = !item.enableFootHeight;
        }
    }

}
