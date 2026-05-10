// ============================================================
//  UDSM Line Scoring SDK — UDSMSpecialLineMarker
//  Attach to any mesh GameObject to mark it as a run control
//  or indicator trigger. OnValidate() auto-names the GO to
//  match the convention the mod runtime expects.
// ============================================================
using UnityEngine;

[DisallowMultipleComponent]
[AddComponentMenu("UDSM/UDSM Special Line Marker")]
public class UDSMSpecialLineMarker : MonoBehaviour
{
    public enum MarkerType
    {
        RunStart,        // run_start_line       – car stops here → 5 s countdown
        RunFinish,       // run_finish_line       – crossing ends the run
        InitiationLine,  // initiation_line       – crossing without drift → DQ
        BrakeZoneStart,  // brake_zone_start_N    – "BRAKE ZONE" HUD flash
        BrakeZoneEnd,    // brake_zone_end_N      – clears brake zone flash
    }

    public MarkerType Type  = MarkerType.RunStart;

    [Tooltip("Index used only for BrakeZoneStart/End to support multiple brake zones.")]
    public int Index = 1;

    private void OnValidate()
    {
        gameObject.name = BuildName(Type, Index);

        var mc = GetComponent<MeshCollider>();
        if (mc != null)
        {
            mc.convex    = true;
            mc.isTrigger = true;
        }
    }

    public static string BuildName(MarkerType type, int index = 1)
    {
        switch (type)
        {
            case MarkerType.RunStart:       return "run_start_line";
            case MarkerType.RunFinish:      return "run_finish_line";
            case MarkerType.InitiationLine: return "initiation_line";
            case MarkerType.BrakeZoneStart: return $"brake_zone_start_{index}";
            case MarkerType.BrakeZoneEnd:   return $"brake_zone_end_{index}";
            default:                         return "unknown_line";
        }
    }

    public static Color GetGizmoColor(MarkerType type)
    {
        switch (type)
        {
            case MarkerType.RunStart:       return Color.green;
            case MarkerType.RunFinish:      return Color.red;
            case MarkerType.InitiationLine: return Color.yellow;
            case MarkerType.BrakeZoneStart: return new Color(0f, 0.8f, 1f);
            case MarkerType.BrakeZoneEnd:   return new Color(0f, 0.4f, 0.8f);
            default:                         return Color.white;
        }
    }
}
