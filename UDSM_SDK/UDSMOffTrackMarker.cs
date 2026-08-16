// ============================================================
//  UDSM Line Scoring SDK — UDSMOffTrackMarker
//  Place this file in your Unity project (not the mod project).
//  The mod reads the GameObject name only; no DLL dependency.
// ============================================================
using UnityEngine;

/// <summary>
/// Marks a mesh as off-track. Any wheel inside it counts as off-surface for
/// the off-course DQ and the one-wheel-off deduction.
///
/// WHEN YOU NEED THIS: only where the surface material can't say it itself.
/// The mod reads CarX's per-wheel surface type and treats anything that isn't
/// Asphalt as off, so grass, dirt, sand and gravel already work unmarked. A
/// PAVED run-off does not — CarX reports asphalt under every wheel there, so
/// without a marker a driver can run arbitrarily wide onto an escape road and
/// never trigger off-course. Cover those areas with this.
///
/// GIVE IT A COLLIDER. Detection asks the physics system what each wheel is
/// standing on and checks whether that surface belongs to an offtrack_ object,
/// so a marker with a collider works whatever shape it is — and it only counts
/// the surface actually under the wheel, not everything below it, so a ground
/// plane running under the circuit doesn't flag the whole track.
///
/// Without a collider the mod falls back to the mesh triangles, which needs
/// Read/Write Enabled on the model import. With neither, the marker is IGNORED
/// and says so in debug.log. It is deliberately never approximated by its
/// bounding box: run-off wraps around the racing line, so a box drawn round it
/// contains the line, and honouring that disqualifies every driver on every run.
///
/// IMPORTANT: place on its OWN GameObject (the mesh you want to mark
/// as off-track). Do NOT combine with UDSMDriftZoneSegment / UDSMDriftZoneAngleRange —
/// they all rename the GameObject in OnValidate() and would clobber each other.
///
/// OnValidate() renames the GameObject to:
///   offtrack_{Index}
///
/// Index is just a unique counter so multiple off-track meshes can co-exist
/// in one scene. The mod doesn't care about the value — only the prefix.
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("UDSM/UDSM Off-Track Marker")]
public class UDSMOffTrackMarker : MonoBehaviour
{
    [Tooltip("Optional unique ID for this off-track patch. Only the prefix " +
             "'offtrack' is parsed by the mod — the index is just to keep " +
             "GameObject names unique in your scene.")]
    [Min(0)] public int Index = 0;

    private void OnValidate()
    {
        // Refuse to rename if a UDSMDriftZoneSegment / UDSMDriftZoneAngleRange
        // is on the same GameObject — their names encode different data.
        if (GetComponent<UDSMDriftZoneSegment>()    != null ||
            GetComponent<UDSMDriftZoneAngleRange>() != null)
        {
            Debug.LogWarning(
                "[UDSM] UDSMOffTrackMarker must be on its own GameObject, " +
                "not on a zone segment or angle-range marker.", this);
            return;
        }

        gameObject.name = BuildName(Index);
    }

    public static string BuildName(int index) => $"offtrack_{index}";
}
