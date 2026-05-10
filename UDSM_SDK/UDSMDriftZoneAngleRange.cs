// ============================================================
//  UDSM Line Scoring SDK — UDSMDriftZoneAngleRange
//  Place this file in your Unity project (not the mod project).
//  The mod reads the GameObject name only; no DLL dependency.
// ============================================================
using UnityEngine;

/// <summary>
/// Optional per-zone angle range marker.
///
/// IMPORTANT: place this component on its OWN GameObject — NOT on a
/// UDSMDriftZoneSegment. Both components rename the GameObject in
/// OnValidate(), so sharing a GO would clobber the segment's name and
/// break depth-zone scoring. A simple empty child GO is enough.
///
/// OnValidate() renames the GameObject to:
///   zone_angle_{ZoneID}_{MinAngle}_{MaxAngle}
///
/// The mod scans for this prefix at scene load and applies the range
/// when the car is inside the matching zone:
///   • MaxAngle replaces the global high-angle deduction threshold
///     (no penalty until the car exceeds MaxAngle while in this zone).
///   • MinAngle is reserved for future use (target-angle scoring).
///
/// Use higher MaxAngle on tight inner-clipping corners that demand
/// extreme angle, and lower MaxAngle on shallow sweepers where the
/// driver should stay calmer.
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("UDSM/UDSM Drift Zone Angle Range")]
public class UDSMDriftZoneAngleRange : MonoBehaviour
{
    [Header("Target Zone")]
    [Tooltip("Zone ID this range applies to (must match the segments' ZoneID).")]
    [Min(0)] public int ZoneID = 0;

    [Header("Allowed Angle Range (degrees)")]
    [Tooltip("Minimum desired drift angle inside this zone. Reserved for future scoring use.")]
    [Range(0f, 90f)] public float MinAngle = 30f;

    [Tooltip("Maximum allowed drift angle inside this zone before the high-angle " +
             "deduction triggers. Set higher for tight clipping corners that demand " +
             "extreme angle (e.g. 80°), lower for shallow sweepers (e.g. 55°).")]
    [Range(0f, 90f)] public float MaxAngle = 77f;

    private void OnValidate()
    {
        if (MaxAngle < MinAngle) MaxAngle = MinAngle;

        // Refuse to rename if a UDSMDriftZoneSegment is on the same GameObject —
        // the segment's name encodes ZoneID/Length/Depth and must not be
        // overwritten. Move this component to an empty sentinel GO instead.
        if (GetComponent<UDSMDriftZoneSegment>() != null)
        {
            Debug.LogWarning(
                "[UDSM] UDSMDriftZoneAngleRange must be on its own GameObject, not " +
                "on a UDSMDriftZoneSegment. Move it to an empty child/sibling GO " +
                "so the segment's name (zone_X_Y_Z) is preserved.", this);
            return;
        }

        gameObject.name = BuildName(ZoneID, MinAngle, MaxAngle);
    }

    public static string BuildName(int zoneId, float minAngle, float maxAngle)
    {
        // Round to 1 decimal to keep names tidy; parser tolerates either.
        return $"zone_angle_{zoneId}_{minAngle:F1}_{maxAngle:F1}";
    }
}
