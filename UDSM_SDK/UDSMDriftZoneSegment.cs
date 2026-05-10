// ============================================================
//  UDSM Line Scoring SDK — UDSMDriftZoneSegment
//  Place this file in your Unity project (not the mod project).
//  The mod reads the GameObject name only; no DLL dependency.
// ============================================================
using UnityEngine;

/// <summary>
/// Attach to each zone mesh in your Unity scene.
///
/// OnValidate() renames the GameObject to the convention the UDSM
/// mod parses at runtime:
///
///   Bumper:     zone_{ZoneID}_{LengthIndex}_{DepthIndex}
///   Wheel:      wheel_{ZoneID}_{LengthIndex}_{DepthIndex}
///   InnerClip:  clip_{ZoneID}_{LengthIndex}_{DepthIndex}
///   Overshoot:  overshoot_{ZoneID}_{LengthIndex}     ← no depth (past max-depth)
///
/// The component also ensures a MeshCollider with isTrigger = true.
/// KnMap exports the name and collider state — the component itself
/// does not need to be present at runtime.
/// </summary>
[DisallowMultipleComponent]
[AddComponentMenu("UDSM/UDSM Drift Zone Segment")]
public class UDSMDriftZoneSegment : MonoBehaviour
{
    public enum TriggerType
    {
        Bumper,    // Full-body footprint detection (any body sample inside bounds)
        Wheel,     // Wheel-position detection (one of 4 wheel corners inside bounds)
        InnerClip, // Front-bumper detection — front-left, front-center, front-right
        Overshoot, // PENALTY mesh — placed past the deepest depth band; touching it
                   // means the driver overshot at this LengthIndex. Halves the depth
                   // ratio for that position even if max-depth was reached.
    }

    [Header("Zone Identity")]
    [Tooltip("Zone number. All segments with the same ZoneID form one scored zone.")]
    [Min(0)] public int ZoneID = 0;

    [Tooltip("Position along the zone (1 = entry, higher = further through).")]
    [Min(1)] public int LengthIndex = 1;

    [Tooltip("Depth from the track edge. 1 = shallow (near edge), higher = deep (near wall).")]
    [Min(1)] public int DepthIndex = 1;

    [Header("Detection")]
    [Tooltip("Bumper: any point of the car body footprint inside the mesh = hit.\n" +
             "Wheel: any of the 4 wheel positions inside the mesh = hit.\n" +
             "InnerClip: front-bumper points (front-left, front-center, front-right) inside the mesh = hit.\n" +
             "Overshoot: PENALTY mesh — placed past the deepest band. Hitting it means the driver " +
             "went too deep at this LengthIndex. DepthIndex is ignored for overshoot meshes.\n" +
             "Use InnerClip for inside-of-corner clipping points where the front bumper should brush by.")]
    public TriggerType Detection = TriggerType.Bumper;

    // ── Auto-naming (editor-only) ────────────────────────────────────────────
    // No collider is required — the mod detects zones via MeshRenderer bounds.
    // For Bumper segments on convex meshes you can optionally add a convex
    // MeshCollider (isTrigger = true) for more precise body-collision detection.
    private void OnValidate()
    {
        gameObject.name = BuildName(ZoneID, LengthIndex, DepthIndex, Detection);
    }

    // ── Gizmo (selected only — editor draws all-visible labels via DrawGizmo) ──
    private void OnDrawGizmosSelected()
    {
        var mf = GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null) return;
        Gizmos.color = DepthColor(DepthIndex);
        Gizmos.DrawWireMesh(mf.sharedMesh,
            transform.position, transform.rotation, transform.localScale);
    }

    // ── Helpers shared with the editor script ─────────────────────────────────
    public static string BuildName(int zoneId, int length, int depth,
                                   TriggerType type = TriggerType.Bumper)
    {
        // Overshoot meshes don't carry a depth — they ARE "past the deepest band".
        if (type == TriggerType.Overshoot)
            return $"overshoot_{zoneId}_{length}";

        string prefix;
        switch (type)
        {
            case TriggerType.Wheel:     prefix = "wheel"; break;
            case TriggerType.InnerClip: prefix = "clip";  break;
            default:                    prefix = "zone";  break;
        }
        return $"{prefix}_{zoneId}_{length}_{depth}";
    }

    public static Color DepthColor(int depth)
    {
        // 1 = blue (shallow) … 4+ = red (deep/wall)
        if (depth <= 1) return new Color(0.25f, 0.35f, 1.00f);
        if (depth == 2) return Color.cyan;
        if (depth == 3) return Color.yellow;
        return new Color(1.00f, 0.25f, 0.10f);
    }
}
