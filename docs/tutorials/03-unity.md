# 3. Unity setup

Import the meshes, attach the SDK, place the run-control geometry.

---

## Install the SDK

Copy the `UDSM_SDK/` folder into your track project at `Assets/UDSM_SDK/`. The components appear under **Add Component → UDSM/…**.

These are editor-only helpers. They rename their GameObject in `OnValidate` and do nothing at runtime — they're stripped when CarX loads the map, which is expected.

---

## Import the zone meshes

Drag the FBX in. Check three things before going further:

**Scale.** Put a known-size object next to a zone cell. If your cells are 10× or 0.01× what you modelled, fix the FBX import scale factor rather than scaling in the scene — a scaled transform can produce bounds that don't match the visible mesh.

**Orientation.** Zones should be standing up, covering the track surface. If they're lying on their side, your Blender export axes were wrong.

**Names survived.** Check the hierarchy for `.001` suffixes or Unity's own deduplication. Fix now, before you attach anything.

---

## Attach `UDSMDriftZoneSegment`

Select a zone cell → **Add Component → UDSM → UDSM Drift Zone Segment**.

Set:

| Field | Value |
|---|---|
| ZoneID | Which zone this belongs to |
| LengthIndex | Position along, 1 = entry |
| DepthIndex | Depth band, 1 = shallow |
| Detection | Usually `Bumper` |

The GameObject renames itself the moment you change a field. The scene view draws a wire mesh coloured by depth: **blue → cyan → yellow → red** as depth increases.

That gizmo is the main reason to use the SDK. Scrub the camera over a finished zone and the colour ramp should march cleanly from track side to wall. A cell that's the wrong colour has the wrong depth index, and you can see it without reading a single name.

### Detection types

| Type | Use for |
|---|---|
| `Bumper` | Almost everything. Full body footprint — forgiving, matches how a judge sees "the car was in there". |
| `Wheel` | When you want precise depth on the rims rather than the bodywork. Stricter. |
| `InnerClip` | Inside-of-corner clips where the front bumper should brush past. Samples front-left, front-centre, front-right only. |
| `Overshoot` | The penalty strip past the deepest band. Ignores DepthIndex. |

You can mix types within a zone. A common pattern is `Bumper` for the main bands and `InnerClip` for a tight apex.

### Doing it faster

Select a whole row of cells at the same depth and add the component to all of them at once, then set ZoneID and DepthIndex on the multi-selection. Only LengthIndex differs per cell, so you're editing one field each.

---

## Angle windows

For a zone that needs a specific angle range, create an **empty GameObject**, add **UDSM Drift Zone Angle Range**, set ZoneID, Min and Max.

Put it on its own object. If you add it to a zone mesh, both components rename the GameObject and the last one to run wins — you'll lose either the zone cell or the marker.

---

## Special lines

**Add Component → UDSM → UDSM Special Line Marker** on a mesh, then pick the type.

### `run_start_line`

The one with a geometry requirement: **use a `BoxCollider`, not a mesh collider.**

The mod does an oriented-bounding-box test, so the box can be rotated to follow the track. Size it to hold a stationary car comfortably — all four wheels must be inside it and the car must be stopped for the countdown to start. Too tight and drivers can't arm it; too loose and it arms in the run-off.

### `run_finish_line`, `initiation_line`

Convex `MeshCollider` with `isTrigger = true`. Make them wide enough to span the whole track — a driver taking an unusual line must still cross them.

`initiation_line` is a DQ trigger: crossing it without having initiated a drift zeroes the run. Place it where you genuinely expect drivers to already be sideways, not at the entry to the approach road.

### Brake zones

`brake_zone_start_{N}` / `brake_zone_end_{N}` in pairs. These only flash a HUD notice — no scoring effect on their own.

---

## Start lights

Attach **UDSM Start Lights Controller** to the root of your lamp post. The root renames to `start_lights`.

Drag the four panel `MeshRenderer`s into the array **top to bottom**. They rename to `panel_1` … `panel_4`.

Then, for each panel:

1. Give it its **own material instance**. Duplicate the material in the Project window — if all four share one material, all four light together and the countdown is meaningless.
2. Enable **Emission** on the material.
3. Confirm the `_EMISSION` keyword is ticked. Unity strips emission from a build if it was never enabled in the editor, and the runtime write then does nothing.

The mod drives `_EmissionColor` directly. If panels stay dark in game, it's nearly always one of those three.

HDRP scenes usually need the emission intensity raised — that's `StartLightsEmissionIntensity` in the player's `config.json`, default 15.

---

## Walls and off-track

Walls need no marking. See [naming contract](../reference/naming-contract.md#wall) for why.

Wall contact comes from CarX's own collision events, so any solid barrier already registers taps and impact DQs.

Off-course reads CarX's per-wheel surface type and counts anything that isn't asphalt, so grass, dirt and gravel need no marking either.

**Paved run-off is the exception.** CarX reports asphalt under every wheel there, so surface type can never flag it and a driver can run as wide as they like. Cover those areas with meshes carrying **`UDSMOffTrackMarker`** (`offtrack_{Index}`) — wheels inside one count as off-surface regardless of the material. Escape roads, paved infield and tarmac run-off all want this.

---

## Before exporting

- [ ] Every zone cell has the component and correct indices
- [ ] Depth colours ramp cleanly track-side → wall in the scene view
- [ ] Overshoot strips placed past the deepest band
- [ ] `run_start_line` has a `BoxCollider` sized for a stopped car
- [ ] `run_finish_line` and `initiation_line` span the full track width
- [ ] Start light panels each have their own emissive material
- [ ] Zone materials are transparent and renderers are enabled
- [ ] Run-off is either non-asphalt material, or covered by `offtrack_` meshes

---

Next: [4. Export and test](04-export-and-test.md)
