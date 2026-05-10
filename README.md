# UDSM Drift Scoring — SDK

Unity-side scripts for authoring tracks compatible with the **UDSM Drift Scoring** mod for *CarX Drift Racing Online*.

The mod itself reads GameObject **names** at runtime (custom MonoBehaviours don't survive the AssetBundle round-trip into CarX), so every component in this SDK is a thin editor-only helper that auto-renames its GameObject in `OnValidate`. The names are the contract; the components are convenience.

---

## Installation

1. Copy the `UDSM_SDK/` folder into your Unity track-authoring project under `Assets/UDSM_SDK/`.
2. The components show up under **Add Component → UDSM/...** in the inspector.
3. Author your track, export to `.ksm`, drop in the player's `kino/mods/` folder.

---

## Component reference

### `UDSMDriftZoneSegment`
Marks a single grid cell of a depth-scoring zone. Renames its GameObject to one of:

| TriggerType | Naming pattern              | What gets sampled         |
|-------------|-----------------------------|---------------------------|
| `Bumper`    | `zone_{ZoneID}_{L}_{D}`      | Full 11-point body footprint (forgiving body contact) |
| `Wheel`     | `wheel_{ZoneID}_{L}_{D}`     | 4 wheel corners only (precise rim depth) |
| `InnerClip` | `clip_{ZoneID}_{L}_{D}`      | 3 front-bumper points (apex clipping) |
| `Overshoot` | `overshoot_{ZoneID}_{L}`     | 4 wheel corners — past the deepest band |

`ZoneID` groups segments into one zone. `L` (length) is the position along the zone, `D` (depth) is how deep in. Hitting a higher `D` at a given `L` scores more line points; hitting the overshoot strip past the deepest `D` deducts.

### `UDSMDriftZoneAngleRange`
Per-zone min/max drift angle override. Names the GO `zone_angle_{ZoneID}_{Min}_{Max}`. Place on its own GameObject (not on a zone segment).

### `UDSMOffTrackMarker`
Mark a mesh as off-track (any wheel resting on it counts as off-surface for DQ / one-wheel-off deductions). Names the GO `offtrack_{Index}`.

### `UDSMWallMarker`
Mark a mesh as a wall/barrier (light contact = `WallTap` deduction; heavy contact = DQ). Names the GO `wall_{Index}`.

### `UDSMSpecialLineMarker`
Marks special trigger lines: `run_start_line`, `run_finish_line`, `initiation_line`, `brake_zone_start`, `brake_zone_end`. Renames the GO to the canonical name based on the selected line type.

### `UDSMStartLightsController`
Lamp-post controller. Renames the root GO to `start_lights` and each assigned panel renderer's GO to `panel_1` through `panel_4`. Drag your 4 panel `MeshRenderer`s into the inspector array (top → bottom). Each panel must have its own emissive material instance.

---

## Mod-side runtime

Detection logic, scoring, networking, lobby sync, and the in-game HUD live in the **mod** repo, not here. The mod finds these named GameObjects in the loaded scene and drives them — emission on the start lights, hit detection on zones, off-track checks on wheels, etc.

You don't need this SDK at runtime; it only matters at track-authoring time.

---

## Versioning

This SDK tracks the contract version of the mod. As long as your scenes use the names listed above, future mod releases will keep working — additive changes only.

---

## License

MIT.
