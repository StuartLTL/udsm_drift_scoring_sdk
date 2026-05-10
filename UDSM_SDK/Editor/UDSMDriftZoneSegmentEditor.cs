// ============================================================
//  UDSM Line Scoring SDK — UDSMDriftZoneSegmentEditor
//  Place inside an Editor/ folder in your Unity project.
// ============================================================
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UDSMDriftZoneSegment)), CanEditMultipleObjects]
public class UDSMDriftZoneSegmentEditor : UnityEditor.Editor
{
    private SerializedProperty _zoneId, _lengthIndex, _depthIndex, _detection;

    private void OnEnable()
    {
        _zoneId      = serializedObject.FindProperty("ZoneID");
        _lengthIndex = serializedObject.FindProperty("LengthIndex");
        _depthIndex  = serializedObject.FindProperty("DepthIndex");
        _detection   = serializedObject.FindProperty("Detection");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("Drift Zone Segment", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_zoneId);
        EditorGUILayout.PropertyField(_lengthIndex);

        // Overshoot meshes don't have a depth — hide the field to avoid confusion.
        bool isOvershoot = _detection.enumValueIndex ==
            (int)UDSMDriftZoneSegment.TriggerType.Overshoot;
        if (!isOvershoot)
            EditorGUILayout.PropertyField(_depthIndex);
        EditorGUILayout.PropertyField(_detection);

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            foreach (var t in targets)
                ((UDSMDriftZoneSegment)t).SendMessage("OnValidate", null,
                    SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            serializedObject.ApplyModifiedProperties();
        }

        // ── Preview of generated name ─────────────────────────────────────────
        EditorGUILayout.Space(4);
        if (!_zoneId.hasMultipleDifferentValues &&
            !_lengthIndex.hasMultipleDifferentValues &&
            !_depthIndex.hasMultipleDifferentValues)
        {
            var type = (UDSMDriftZoneSegment.TriggerType)_detection.enumValueIndex;
            string preview = UDSMDriftZoneSegment.BuildName(
                _zoneId.intValue, _lengthIndex.intValue, _depthIndex.intValue, type);
            EditorGUILayout.HelpBox($"Name → {preview}", MessageType.None);
        }
        else
        {
            EditorGUILayout.HelpBox("Multiple values selected.", MessageType.None);
        }

        // ── Multi-edit quick tools ─────────────────────────────────────────────
        if (targets.Length > 1)
        {
            EditorGUILayout.Space(6);
            EditorGUILayout.LabelField(
                $"Multi-Edit  ({targets.Length} segments selected)", EditorStyles.miniBoldLabel);

            // Depth row buttons
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Set Depth:", GUILayout.Width(68));
            for (int d = 1; d <= 4; d++)
            {
                var style = new GUIStyle(GUI.skin.button);
                style.normal.textColor = UDSMDriftZoneSegment.DepthColor(d);
                style.fontStyle = FontStyle.Bold;
                if (GUILayout.Button($"D{d}", style, GUILayout.Width(34)))
                    SetAllDepth(d);
            }
            EditorGUILayout.EndHorizontal();

            // Length offset
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Length offset:", GUILayout.Width(88));
            if (GUILayout.Button("-1", GUILayout.Width(32))) OffsetAll(0, -1);
            if (GUILayout.Button("+1", GUILayout.Width(32))) OffsetAll(0, +1);
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Zone:", GUILayout.Width(38));
            if (GUILayout.Button("-",  GUILayout.Width(24))) OffsetAll(-1, 0);
            if (GUILayout.Button("+",  GUILayout.Width(24))) OffsetAll(+1, 0);
            EditorGUILayout.EndHorizontal();
        }
    }

    // ── Scene-view label for the selected object ──────────────────────────────
    private void OnSceneGUI()
    {
        var seg = (UDSMDriftZoneSegment)target;
        Handles.color = UDSMDriftZoneSegment.DepthColor(seg.DepthIndex);
        Handles.Label(seg.transform.position + Vector3.up * 0.35f,
            $"Z{seg.ZoneID}:L{seg.LengthIndex}:D{seg.DepthIndex}",
            EditorStyles.whiteLabel);
    }

    // ── Always-visible gizmos for all UDSMDriftZoneSegments in the scene ─────────
    [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy)]
    private static void DrawGizmoAlways(UDSMDriftZoneSegment seg, GizmoType type)
    {
        bool selected = (type & GizmoType.Active) != 0;

        // Wire mesh
        var mf = seg.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            Color col = UDSMDriftZoneSegment.DepthColor(seg.DepthIndex);
            col.a = selected ? 0.9f : 0.45f;
            Gizmos.color = col;
            Gizmos.DrawWireMesh(mf.sharedMesh,
                seg.transform.position, seg.transform.rotation, seg.transform.localScale);
        }

        // Coordinate label
        Handles.color = UDSMDriftZoneSegment.DepthColor(seg.DepthIndex);
        Handles.Label(
            seg.transform.position + Vector3.up * 0.5f,
            $"Z{seg.ZoneID}:L{seg.LengthIndex}:D{seg.DepthIndex}",
            EditorStyles.miniLabel);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────
    private void SetAllDepth(int depth)
    {
        Undo.RecordObjects(targets, "UDSM Set Depth");
        foreach (var t in targets)
        {
            var seg = (UDSMDriftZoneSegment)t;
            seg.DepthIndex = depth;
            seg.SendMessage("OnValidate", null, SendMessageOptions.DontRequireReceiver);
            EditorUtility.SetDirty(seg);
        }
    }

    private void OffsetAll(int zoneOffset, int lengthOffset)
    {
        Undo.RecordObjects(targets, "UDSM Offset Indices");
        foreach (var t in targets)
        {
            var seg = (UDSMDriftZoneSegment)t;
            seg.ZoneID      = Mathf.Max(0, seg.ZoneID      + zoneOffset);
            seg.LengthIndex = Mathf.Max(1, seg.LengthIndex + lengthOffset);
            seg.SendMessage("OnValidate", null, SendMessageOptions.DontRequireReceiver);
            EditorUtility.SetDirty(seg);
        }
    }
}
