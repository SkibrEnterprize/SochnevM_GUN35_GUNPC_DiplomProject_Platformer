using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(ResizeZoneTrigger))]
public class ResizeZoneTriggerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        var script = (ResizeZoneTrigger)target;

        script.ResetToDefaultScale = EditorGUILayout.Toggle("Reset To Default", script.ResetToDefaultScale);

        if (!script.ResetToDefaultScale)
        {
            script.TargetObjectScale = EditorGUILayout.Slider("Target Object Scale", script.TargetObjectScale, 0.1f, 2f);
        }

        if (GUI.changed) EditorUtility.SetDirty(target);
    }
}