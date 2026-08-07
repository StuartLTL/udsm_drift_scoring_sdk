# 4. Export and test

Getting the map into CarX, and — more importantly — confirming CHAPPiE actually found your zones.

---

## Export the map

Export through the **Kino SDK / KnMap** pipeline exactly as you would for any CarX map. CHAPPiE adds no export steps of its own: your zone meshes are ordinary GameObjects with ordinary names, and that's all the mod needs on the other side.

The SDK components do not need to survive export. They're editor-only and will be stripped — that's expected and is why the contract is names.

> The KnMap export UI and its options are outside this documentation. If your map already exports and loads in CarX, nothing here changes that process.

Two things to confirm made it through:

- **GameObject names.** If the export renames or deduplicates anything, the contract breaks silently.
- **MeshRenderers.** Detection uses renderer bounds. A zone mesh exported without its renderer is invisible to the mod.

---

## Verify the scan

This is the step people skip, and it's the one that tells you whether any of the previous work landed.

Load the map with CHAPPiE installed and open:

```
{CarX install}/kino/mods/CHAPPiE/debug.log
```

The exact path is shown in the KSL menu under **Storage → Data**, and in the log's own header line.

### What you're looking for

On every scene load the mod logs its scan result:

```
[UDSM Line] Zone scan for: <your_scene>  (3 zones)
```

**Zone count is the headline number.** If it says 0, nothing you named was matched — check for `.001` suffixes, a capitalisation mismatch, or missing renderers. If it's lower than you authored, one zone's meshes are misnamed and got grouped elsewhere or skipped.

Angle markers log individually:

```
[UDSM Line] Zone 2 angle range: 40.0°–55.0°
```

### Broken meshes

```
[UDSM Line] zone_1_2_3 has zero-size bounds — mesh is broken, will be ignored for scoring
```

This means the mesh exported with no usable bounds — usually an unapplied transform, a mesh with no geometry, or a renderer that lost its mesh reference in the bundle.

Segments flagged this way are **excluded from the max-depth calculation**, so the zone still scores out of its remaining valid bands rather than penalising the driver for an unreachable one. But you've lost a band you intended to exist — fix it.

---

## Drive it

Checks worth doing deliberately, in this order. Each one isolates a different part of the setup:

**1. Start box.** Stop inside `run_start_line` with all four wheels in. The countdown should begin. If it doesn't, the box is too small or it's a `MeshCollider` rather than a `BoxCollider`.

**2. Start lights.** Watch the panel sequence during the countdown. All four lighting at once means shared materials. None lighting means emission isn't enabled on the material or the `_EMISSION` keyword is off.

**3. Depth bands.** Drive one zone deliberately shallow, then deliberately deep. The line score should move substantially. If it barely changes, your bands are too narrow relative to the car, or several cells share a depth index.

**4. Overshoot.** Aim past the deepest band. You should see an `Overshoot` deduction in the deductions panel. If not, the strip has a gap the car passed through.

**5. Off-course.** Put two wheels into the run-off and hold. You should be DQ'd after 0.15 s. If nothing happens, the run-off is authored as asphalt and has no `offtrack_` mesh over it — test each run-off area separately, since one may be grass and another tarmac.

**6. Finish line.** Cross it during a run. The run should end without you having to do anything else.

---

## Reading a scored run

The deductions panel lists every point-costing event with its timestamp, which is the fastest way to diagnose a zone that's behaving oddly. A run through a zone you thought was fine, showing three `MissedZoneSegment` deductions, tells you three length positions were never entered — usually a gap in the grid rather than a driver error.

The host's scoreboard has a per-driver expander showing the same log for remote drivers, and **Export CSV** writes the full breakdown: summary, per-zone, deduction log, and raw run history.

---

## Iterating

Zone geometry changes mean a re-export. To keep that loop short:

- Get one zone completely right before building the rest. The mistakes you make in zone 1 you'll make in all of them.
- Keep the `.fbx` source organised by zone so you can re-export a single zone rather than the whole track.
- The scan log tells you whether names landed without you needing to drive — load the map, read the log, alt-tab out.

---

## Packaging the mod itself

Only relevant if you're building CHAPPiE from source rather than using a release.

The project builds to `CHAPPiE.dll`, then the KSL SDK's `maykr` tool signs and packages it into `CHAPPiE.ksm`, which goes in `kino/mods/`. That's automated by the `PackageKsm` target in the `.csproj` and needs `CarXPath` pointing at your install.

The `.kmc` signing config is a **credential**. It is excluded from every published repo and must not be committed or shared.

---

## Related

- [Naming contract](../reference/naming-contract.md)
- [Scoring model](../reference/scoring-model.md)
- [DQ and deduction rules](../reference/dq-rules.md)
