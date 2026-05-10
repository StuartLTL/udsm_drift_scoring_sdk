// ============================================================
//  UDSM Line Scoring SDK — UDSMSpecialLineMarkerEditor
//  Place inside an Editor/ folder in your Unity project.
// ============================================================
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(UDSMSpecialLineMarker))]
public class UDSMSpecialLineMarkerEditor : UnityEditor.Editor
{
    private SerializedProperty _type;
    private SerializedProperty _index;

    private void OnEnable()
    {
        _type  = serializedObject.FindProperty("Type");
        _index = serializedObject.FindProperty("Index");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        EditorGUILayout.LabelField("UDSM Special Line Marker", EditorStyles.boldLabel);
        EditorGUILayout.Space(4);

        EditorGUI.BeginChangeCheck();
        EditorGUILayout.PropertyField(_type, new GUIContent("Line Type"));

        var mt = (UDSMSpecialLineMarker.MarkerType)_type.enumValueIndex;
        bool showIndex = mt == UDSMSpecialLineMarker.MarkerType.BrakeZoneStart
                      || mt == UDSMSpecialLineMarker.MarkerType.BrakeZoneEnd;
        if (showIndex)
            EditorGUILayout.PropertyField(_index, new GUIContent("Zone Index"));

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
            ((UDSMSpecialLineMarker)target).SendMessage("OnValidate", null,
                SendMessageOptions.DontRequireReceiver);
        }
        else
        {
            serializedObject.ApplyModifiedProperties();
        }

        EditorGUILayout.Space(4);
        string preview = UDSMSpecialLineMarker.BuildName(mt, showIndex ? _index.intValue : 1);
        EditorGUILayout.HelpBox($"Name → {preview}", MessageType.None);

        EditorGUILayout.Space(4);
        switch (mt)
        {
            case UDSMSpecialLineMarker.MarkerType.RunStart:
                EditorGUILayout.HelpBox(
                    "Car stops on this mesh → 5 s countdown → run auto-starts.",
                    MessageType.Info);
                break;
            case UDSMSpecialLineMarker.MarkerType.RunFinish:
                EditorGUILayout.HelpBox(
                    "Crossing this mesh during a run stops scoring immediately.",
                    MessageType.Info);
                break;
            case UDSMSpecialLineMarker.MarkerType.InitiationLine:
                EditorGUILayout.HelpBox(
                    "Crossing without drift → automatic DQ.",
                    MessageType.Warning);
                break;
            case UDSMSpecialLineMarker.MarkerType.BrakeZoneStart:
            case UDSMSpecialLineMarker.MarkerType.BrakeZoneEnd:
                EditorGUILayout.HelpBox(
                    "Visual indicator only — shows 'BRAKE ZONE' on the driver's HUD.",
                    MessageType.None);
                break;
        }
    }

    private void OnSceneGUI()
    {
        var m = (UDSMSpecialLineMarker)target;
        Handles.color = UDSMSpecialLineMarker.GetGizmoColor(m.Type);
        Handles.Label(
            m.transform.position + Vector3.up * 0.6f,
            m.Type.ToString().Replace("_", " "),
            EditorStyles.whiteLabel);
    }

    [DrawGizmo(GizmoType.Active | GizmoType.NotInSelectionHierarchy)]
    private static void DrawGizmoAlways(UDSMSpecialLineMarker m, GizmoType type)
    {
        var mf = m.GetComponent<MeshFilter>();
        if (mf != null && mf.sharedMesh != null)
        {
            Color col = UDSMSpecialLineMarker.GetGizmoColor(m.Type);
            col.a = (type & GizmoType.Active) != 0 ? 0.9f : 0.4f;
            Gizmos.color = col;
            Gizmos.DrawWireMesh(mf.sharedMesh,
                m.transform.position, m.transform.rotation, m.transform.localScale);
        }

        Handles.color = UDSMSpecialLineMarker.GetGizmoColor(m.Type);
        Handles.Label(
            m.transform.position + Vector3.up * 0.5f,
            m.gameObject.name,
            EditorStyles.miniLabel);
    }
}
