# CHAPPiE SDK

Unity authoring scripts for building tracks that **CHAPPiE** can judge, for *CarX Drift Racing Online*.

**Full documentation: [chappie.online](https://chappie.online)**

---

## What this is

CHAPPiE scores a run against zones the track author defines. This SDK is how you define them.

The mod reads GameObject **names** at runtime — custom MonoBehaviours don't survive the AssetBundle round-trip into CarX, so a component can't be the carrier. Every script here is an editor-only helper that renames its GameObject in `OnValidate`.

**The names are the contract; the components are convenience.** You can author a fully working CHAPPiE track by naming meshes by hand. The SDK exists so a typo becomes an inspector field instead of a silently unscored zone.

---

## Install

1. Copy `Assets/UDSM_SDK/` into your Unity track project.
2. Components appear under **Add Component → UDSM/…**.
3. Author, export through the Kino SDK as normal, test.

Nothing here ships inside your map or runs in game.

---

## Components

| Component | Names the GameObject | Purpose |
|---|---|---|
| `UDSMDriftZoneSegment` | `zone_` / `wheel_` / `clip_` / `overshoot_` | One cell of a zone's depth grid |
| `UDSMDriftZoneAngleRange` | `zone_angle_{ID}_{min}_{max}` | Per-zone angle window |
| `UDSMSpecialLineMarker` | `run_start_line`, `run_finish_line`, `initiation_line`, `brake_zone_*` | Run control triggers |
| `UDSMStartLightsController` | `start_lights`, `panel_1`…`panel_4` | Lamp post |
| `UDSMWallMarker` | `wall_{N}` | Organisational only — see below |
| `UDSMOffTrackMarker` | `offtrack_{N}` | Organisational only — see below |

Full field reference: [chappie.online/reference/components](https://chappie.online/reference/components).

---

## The depth grid in one paragraph

A zone is a grid of separate invisible meshes, split by **length** (steps along the track) and **depth** (bands toward the wall). At each length position the mod records the deepest band the car reached and divides by the deepest band you authored. An `overshoot_` strip beyond the deepest band halves that position and deducts, so "deeper is better" stops at the wall.

Every cell must be its **own mesh object** — one mesh with the grid as faces has single bounds and can't be scored.

See [designing the zone grid](https://chappie.online/tutorials/01-zone-design) before you model anything.

---

## Markers that currently do nothing

`UDSMWallMarker` and `UDSMOffTrackMarker` are still shipped, but **neither drives mod behaviour**:

- Wall contact comes from CarX's own collision events on the car, so any solid barrier already registers taps and impact DQs. You don't need to mark walls.
- Off-course reads CarX's per-wheel surface type and counts anything that isn't asphalt. The `offtrack_` prefix isn't parsed.

They're harmless and useful for organising a hierarchy. The practical consequence worth knowing: **off-course follows your surface materials**, so run-off areas authored as asphalt won't trigger the off-course DQ. Test it on your map.

---

## Versioning

The contract is additive. New names may be introduced; existing ones keep their meaning. A map authored against the current reference keeps working across mod updates without a re-export.

---

## License

MIT. See `LICENSE`.
