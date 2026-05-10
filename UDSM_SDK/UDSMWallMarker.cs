// ============================================================
//  UDSM Line Scoring SDK — UDSMWallMarker
//  Place this file in your Unity project (not the mod project).
//  The mod reads the GameObject name only; no DLL dependency.
// ============================================================
using UnityEngine;

/// <summary>
/// Marks a mesh as a wall / barrier. Hitting any GameObject whose
/// name starts with "wall" produces:
///
///   • Light tap (no significant angle / speed loss) → minor line deduction.
///   • Heavy hit (>= ImpactAngleDrop angle loss, OR >= ImpactSpeedDropKmh
///     km/h speed loss, OR a spin within 1 s of contact) → run zeroed (DQ).
///
/// IMPORTANT: place on its OWN GameObject — do NOT combine with
/// UDSMDriftZoneSegment / UDSMDriftZoneAngleRange / UDSMOffTrackMarker. They all
/// rename the GameObject and would clobber each other.
///
/// OnValidate() renames the GameObject to:
///   wall_{Index}
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("UDSM/UDSM Wall Marker")]
public class UDSMWallMarker : MonoBehaviour
{
    [Tooltip("Optional unique ID for this wall mesh. Only the prefix " +
             "'wall' is parsed by the mod — the index is just to keep " +
             "GameObject names unique in your scene.")]
    [Min(0)] public int Index = 0;

    private void OnValidate()
    {
        if (GetComponent<UDSMDriftZoneSegment>()    != null ||
            GetComponent<UDSMDriftZoneAngleRange>() != null ||
            GetComponent<UDSMOffTrackMarker>()      != null)
        {
            Debug.LogWarning(
                "[UDSM] UDSMWallMarker must be on its own GameObject, " +
                "not on a zone segment, angle-range, or off-track marker.", this);
            return;
        }

        gameObject.name = BuildName(Index);
    }

    public static string BuildName(int index) => $"wall_{index}";
}
