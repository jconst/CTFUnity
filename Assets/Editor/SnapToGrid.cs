using UnityEngine;
using UnityEditor;
using System.Collections;
 
public class SnapToGrid : ScriptableObject {
 
    const float snapGranularity = 0.25f;

    [MenuItem ("Window/Snap to Grid %g")]
    static void MenuSnapToGrid() {
        foreach (Transform t in Selection.GetTransforms(SelectionMode.TopLevel | SelectionMode.OnlyUserModifiable)) {
            t.position = new Vector3 (
                Mathf.Round(t.position.x / snapGranularity) * snapGranularity,
                Mathf.Round(t.position.y / snapGranularity) * snapGranularity,
                Mathf.Round(t.position.z / snapGranularity) * snapGranularity
            );
        }
    }
}