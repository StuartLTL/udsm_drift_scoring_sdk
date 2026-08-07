# Track authoring

How to build a map CHAPPiE can judge, from an empty scene to a signed `.ksm`.

This assumes you can already build and export a CarX map. CHAPPiE adds geometry and names on top of that — it doesn't change how your map is made.

---

## The pipeline

```
1. Design          Decide where zones go and how deep they run.
   ↓                Paper or a top-down screenshot. No tools yet.

2. Model           Build the zone grid as separate meshes,
   Maya/Blender     one per (length × depth) cell.

3. Unity           Import, attach SDK components, place lines,
   + CHAPPiE SDK    walls and start lights.

4. Export          KnMap → AssetBundle → test in CarX.
   Kino SDK

5. Verify          Drive it. Check debug.log for scan results.
```

Steps 2 and 3 are where CHAPPiE-specific work happens. Everything else is the normal CarX map process.

---

## Guides

| | |
|---|---|
| [1. Designing the zone grid](01-zone-design.md) | What a zone is, how deep to go, how many bands. Read this first — it's the part that decides whether your track judges well. |
| [2. Building the meshes](02-modelling.md) | Maya and Blender. Splitting, naming, pivots, export settings. |
| [3. Unity setup](03-unity.md) | SDK components, special lines, start lights, common mistakes. |
| [4. Export and test](04-export-and-test.md) | KnMap, packaging, and how to confirm the mod actually found your zones. |

---

## Before you start

**You don't need the SDK.** The [naming contract](../reference/naming-contract.md) is the real interface — meshes named correctly work whether they came from an SDK component, a Blender export, or you renaming them by hand. The SDK just makes the names hard to get wrong.

**Zone meshes are invisible in game.** They're detection volumes. Give them a transparent or disabled material — nothing about them should be visible to the driver.

**Detection uses renderer bounds, not colliders.** A zone mesh needs a `MeshRenderer` to be found. Colliders are optional and only sharpen body detection on convex bumper segments.

**The grid is axis-independent.** A zone mesh can be rotated any way you like. "Depth" is defined by which mesh you name `_1` vs `_3`, not by a world axis.
