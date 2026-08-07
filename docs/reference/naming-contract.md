# Naming contract

This is the entire interface between a track and CHAPPiE. The mod scans the loaded scene at load time and after every scene change, matches GameObject names against the patterns below, and ignores everything else.

No component needs to be present at runtime. Custom MonoBehaviours are stripped from the map's AssetBundle when CarX loads it, which is *why* the contract is names rather than components. The [SDK components](components.md) exist to write these names for you and keep them consistent.

Names are matched **case-sensitively** for zone meshes and **case-insensitively** for special lines.

---

## Zone meshes

The core of line scoring. Every scored zone is a grid of separate meshes.

| Pattern | Detection | What counts as a hit |
|---|---|---|
| `zone_{ZoneID}_{Length}_{Depth}` | Bumper | Any point of the car's body footprint inside the mesh bounds |
| `wheel_{ZoneID}_{Length}_{Depth}` | Wheel | Any of the 4 wheel positions inside the bounds |
| `clip_{ZoneID}_{Length}_{Depth}` | Inner clip | Front-bumper points (front-left, front-centre, front-right) |
| `overshoot_{ZoneID}_{Length}` | Penalty | Body footprint — **no depth index**, see below |

All three indices are integers.

- **ZoneID** — groups meshes into one scored zone. Every mesh sharing an ID is one zone.
- **Length** — position *along* the zone. `1` = entry, higher = further through.
- **Depth** — how far *in* toward the wall. `1` = shallow (track side), higher = deeper.

Detection uses the mesh's **renderer bounds**, so a collider is not required. For `Bumper` segments on convex shapes you can add a convex `MeshCollider` with `isTrigger = true` for tighter body detection, but it's optional.

### Overshoot meshes

`overshoot_{ZoneID}_{Length}` carries no depth index — it *is* "past the deepest band". Place it immediately beyond your highest-depth mesh at that length position. Touching it halves the depth ratio for that position even if max depth was reached, and fires an `Overshoot` deduction.

This is what stops "deeper is always better" from rewarding a driver who ran out of road.

### Worked example

A three-length, three-depth zone with an overshoot strip:

```
zone_1_1_1   zone_1_1_2   zone_1_1_3   overshoot_1_1     ← entry
zone_1_2_1   zone_1_2_2   zone_1_2_3   overshoot_1_2     ← mid
zone_1_3_1   zone_1_3_2   zone_1_3_3   overshoot_1_3     ← exit
      ↑            ↑            ↑            ↑
  shallow                     deep      too deep
```

Max depth is detected automatically per zone — there is no config file listing it. If the deepest mesh you author for zone 1 is `_3`, then depth 3 is 100% for that zone.

Depth and length numbering must be contiguous from 1. A gap (say depth 1 and 3 with no 2) makes the missing band unreachable and skews the ratio.

---

## Per-zone angle range

```
zone_angle_{ZoneID}_{Min}_{Max}
```

Overrides the angle window for one zone. Values may be integers or floats — `zone_angle_2_35_55` and `zone_angle_2_35.5_55.0` both parse.

Drop below `Min` inside that zone for `UnderAngleSustainSec` and you take an under-angle deduction. Put this on its **own** GameObject — never on a zone mesh, because both components rename their GameObject and would fight.

Parsed before the `zone_` check, so the `zone_angle_` prefix is not mistaken for a bumper segment.

---

## Special lines

Matched case-insensitively. Exact names except the brake zones, which match on prefix.

| Name | Behaviour |
|---|---|
| `run_start_line` | Car stationary with all 4 wheels inside → 5 s countdown → run starts |
| `run_finish_line` | Crossing during an active run ends it |
| `initiation_line` | Crossing without having initiated a drift → DQ |
| `brake_zone_start_{N}` | Flashes `BRAKE ZONE` on the driver's HUD |
| `brake_zone_end_{N}` | Clears the flash |

**`run_start_line` needs a `BoxCollider`, not a mesh collider.** The mod does an oriented-bounding-box test so the box can be rotated to follow the track. A `MeshCollider` is used as a fallback but is less predictable on a rotated box.

The other lines want a convex `MeshCollider` with `isTrigger = true`.

---

## Start lights

| Name | Role |
|---|---|
| `start_lights` | Root of the lamp post |
| `panel_1` … `panel_4` | The four emissive panels, top to bottom |

The mod finds these by name and writes `_EmissionColor` directly. Each panel renderer needs its **own material instance** — if they share one material, all four light up together.

Enable **Emission** on the material and make sure the `_EMISSION` keyword is ticked, or the runtime write has no visible effect. HDRP scenes usually need `StartLightsEmissionIntensity` in the 5–25 range; it's in `config.json`.

Sequence: panel 1 white at 3 s remaining, panels 1–2 at 2 s, panels 1–2–3 at 1 s, all four red on GO.

---

## Names the mod no longer acts on

Both of these are accepted in a scene and cause no errors, but **neither currently drives any behaviour**. They're documented here so you don't spend time placing them expecting an effect.

### `wall_{Index}`

Wall contact is detected through CarX's own collision events on the car (`RaceCar.OnCollisionEnterEvent`), which fire for any solid collider. The mod explicitly skips `wall_`-prefixed objects during the scene scan — no trigger is attached, no bounds cached.

**You do not need to mark walls.** Any solid barrier already registers wall taps and wall-impact DQs. Keep the marker if you use it to group barriers in your hierarchy; it costs nothing.

### `offtrack_{Index}`

Off-course detection reads CarX's per-wheel `surfaceType` and counts any wheel not on `Asphalt`. Nothing in the mod parses the `offtrack_` prefix.

That means off-course currently follows **the surface material CarX assigns**, not your marker. If your run-off areas are authored as asphalt, they will not register as off-course regardless of the marker. Worth checking on your map before relying on the off-course DQ.

> Both markers remain in the SDK. If you want the mod to honour them, that's a mod-side change — raise it rather than assuming the marker is doing something.

---

## Authoring without the SDK

Everything above is a string. You can rename meshes by hand in Unity, or export them from Maya or Blender already named correctly, and CHAPPiE will score them.

The SDK's value is that `ZoneID`, `Length` and `Depth` become inspector fields with a live gizmo, so a mistyped index shows up as a wrong colour in the scene view instead of a zone that silently never scores.
