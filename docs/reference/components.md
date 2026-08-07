# SDK component reference

Editor-only Unity components. Each one renames its GameObject in `OnValidate` to match the [naming contract](naming-contract.md), and does nothing at runtime.

Install by copying `UDSM_SDK/` into your track project at `Assets/UDSM_SDK/`. Components appear under **Add Component → UDSM/…**.

---

## `UDSMDriftZoneSegment`

One cell of a zone's depth grid. The component you'll use most.

| Field | Type | Meaning |
|---|---|---|
| `ZoneID` | int ≥ 0 | Groups cells into one scored zone |
| `LengthIndex` | int ≥ 1 | Position along the zone, 1 = entry |
| `DepthIndex` | int ≥ 1 | Depth band, 1 = shallow. Ignored for `Overshoot` |
| `Detection` | enum | How a hit is sampled |

**Detection types**

| Value | Resulting name | Sampled against |
|---|---|---|
| `Bumper` | `zone_{Z}_{L}_{D}` | Full car body footprint |
| `Wheel` | `wheel_{Z}_{L}_{D}` | The 4 wheel positions |
| `InnerClip` | `clip_{Z}_{L}_{D}` | Front-left, front-centre, front-right |
| `Overshoot` | `overshoot_{Z}_{L}` | Body footprint, past the deepest band |

Draws a wire mesh gizmo coloured by depth when selected: depth 1 blue, 2 cyan, 3 yellow, 4+ orange-red. Use the colour ramp to eyeball a finished zone.

No collider is required — detection uses renderer bounds. A convex `MeshCollider` with `isTrigger = true` can be added to `Bumper` cells for tighter body detection.

---

## `UDSMDriftZoneAngleRange`

Per-zone angle window. Names the GameObject `zone_angle_{ZoneID}_{Min}_{Max}`.

| Field | Meaning |
|---|---|
| `ZoneID` | Which zone this applies to |
| `Min` | Below this for `UnderAngleSustainSec` → under-angle deduction |
| `Max` | Upper end of the expected window |

**Place on its own empty GameObject.** Adding it to a zone cell makes two components fight over the GameObject name and one of them loses.

---

## `UDSMSpecialLineMarker`

Run-control triggers. Names the GameObject from the selected type.

| `Type` | Name | Behaviour |
|---|---|---|
| `RunStart` | `run_start_line` | Stationary, 4 wheels inside → countdown → run starts |
| `RunFinish` | `run_finish_line` | Crossing during a run ends it |
| `InitiationLine` | `initiation_line` | Crossing without a drift initiated → DQ |
| `BrakeZoneStart` | `brake_zone_start_{Index}` | HUD flash |
| `BrakeZoneEnd` | `brake_zone_end_{Index}` | Clears the flash |

`Index` only applies to the brake zones, for supporting several on one track.

Sets any `MeshCollider` on the object to `convex = true, isTrigger = true`.

> `run_start_line` should use a **`BoxCollider`**, not a mesh collider — the mod runs an oriented-bounding-box test so a rotated box follows the track correctly.

---

## `UDSMStartLightsController`

Lamp post. Renames the root to `start_lights` and each assigned panel renderer to `panel_1` … `panel_4`.

| Field | Meaning |
|---|---|
| `LightPanels` | 4 `Renderer`s, assigned top to bottom |
| `PointLights` | Optional `Light`s, same order |
| `WhiteOn` | Emission colour during the countdown |
| `RedOn` | Emission colour on GO |
| `Off` | Emission colour when dark |
| `GoFlashCount` | Flashes on GO. 0 = stay solid red |
| `GoFlashInterval` | Seconds per flash half-period |

Each panel needs its **own material instance** with **Emission enabled** and the `_EMISSION` keyword ticked. The mod writes `_EmissionColor` at runtime; shared materials make all four panels light together.

---

## `UDSMWallMarker` · `UDSMOffTrackMarker`

Name their GameObjects `wall_{Index}` and `offtrack_{Index}`.

**Neither currently drives any mod behaviour.** Wall contact comes from CarX's collision events on the car, and off-course reads CarX's per-wheel surface type. Both markers are harmless to place and useful for organising your hierarchy, but placing them does not change scoring.

See [naming contract](naming-contract.md#names-the-mod-no-longer-acts-on) for the detail, including the practical consequence: off-course follows your **surface materials**, so run-off areas authored as asphalt will not trigger it.

---

## Editor extras

`UDSMDriftZoneSegmentEditor` and `UDSMSpecialLineMarkerEditor` add scene-view labels and inspector conveniences. They live in `UDSM_SDK/Editor/` and must stay in an `Editor` folder — Unity will refuse to build otherwise.

---

## Versioning

The contract is additive. New names may be introduced; existing ones keep their meaning. A map authored against this reference keeps working across mod updates without a re-export.
