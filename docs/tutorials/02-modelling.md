# 2. Building the meshes

Maya and Blender both work. The task is identical in either: take the area of a zone, cut it into a grid, and end up with **one separate mesh object per cell**.

The critical word is *separate*. One mesh with the grid drawn on it as faces is a single object with single bounds — the mod cannot tell depth 1 from depth 3 inside it. Each cell must be its own object.

---

## What each cell needs to be

- Its own object, not a face group or a submesh
- Convex-ish and roughly box-shaped — bounds are what's tested
- Tall enough that a car body passes through it, not just skims the top
- Overlapping its neighbours slightly, or at least sharing an edge with no gap

**Height matters more than people expect.** Detection tests points against the mesh's bounds. If your cells are flat planes, a car pitched under braking can pass over one without registering. Extrude them into slabs — roughly car height is a safe default, taller does no harm.

**Gaps are worse than overlaps.** A car crossing a seam between two cells should always be inside one of them. Overlap by a few centimetres.

---

## Maya

### Building the grid

1. Model the zone footprint as a single flat polygon following the track edge.
2. Extrude it upward to about car height.
3. Cut it into the grid with **Mesh Tools → Multi-Cut**, or model one cell and duplicate it along the two axes.
4. **Mesh → Separate** to break the result into individual objects.

Multi-Cut on a curved zone is usually easier than duplicating, because the cells need to follow the track's curve rather than a straight grid.

### Naming

Maya's naming rules are friendlier than the contract needs, so you can name objects to their final form directly:

```
zone_1_1_1, zone_1_1_2, zone_1_1_3
zone_1_2_1, zone_1_2_2, zone_1_2_3
```

If you'd rather rename in Unity with the SDK inspector, name them anything predictable here — `z1_L1_D1` — and fix them up after import. Both work; naming in Maya means the mesh is correct the moment it lands.

Beware Maya's auto-renaming on duplicate: `zone_1_1_1` duplicated becomes `zone_1_1_2` on its own, which happens to be the right pattern going one way and completely wrong going the other. Check the outliner before exporting.

### Pivots

**Freeze transforms** (`Modify → Freeze Transformations`) and **delete history** before export. A cell with a baked-in scale or rotation offset can produce bounds that don't match what you see.

Centre each pivot on its cell (`Modify → Center Pivot`). Not strictly required — bounds are world-space — but it makes the Unity scene far easier to work with.

### Export

FBX, with:

- **Smoothing groups** off (irrelevant, these are invisible)
- **Triangulate** on
- **Units: centimetres** — Maya default is centimetres and Unity expects a 1 unit = 1 metre result. If your track imports at the wrong scale, this is where it went wrong.

---

## Blender

### Building the grid

1. Model the zone footprint as a plane following the track edge.
2. Extrude up to about car height.
3. Cut the grid with loop cuts (`Ctrl+R`) — one set along the length, one set across the depth.
4. `P → By Loose Parts` after separating faces, or select each cell's faces and `P → Selection`.

For a curved zone, the cleanest approach is often to model one cell, then use an **Array modifier along a Curve** following the track edge for the length steps, and a second array for depth. Apply the modifiers, then separate by loose parts.

### Naming

Blender's outliner names become the object names on export. Name them to the contract:

```
zone_1_1_1
zone_1_1_2
...
```

Blender appends `.001`, `.002` to duplicate names, and **that suffix survives FBX export** — `zone_1_1_1.001` does not match the contract and will not be scored. This is the single most common way a Blender-authored zone silently fails.

After separating by loose parts, rename every object explicitly. Batch Rename (`F2` with multiple selected) handles it, or use a short script:

```python
import bpy
# Name the selected cells in selection order: zone_{ZID}_{L}_{D}
ZID, DEPTHS = 1, 3
for i, ob in enumerate(bpy.context.selected_objects):
    length = i // DEPTHS + 1
    depth  = i %  DEPTHS + 1
    ob.name = f"zone_{ZID}_{length}_{depth}"
```

Selection order is not guaranteed to be the order you clicked, so sort or verify in the outliner before trusting it.

### Transforms

`Ctrl+A → All Transforms` on every cell before export. An unapplied scale is the Blender equivalent of Maya's unfrozen transform and causes the same bounds mismatch.

### Export

FBX, with:

- **Apply Transform** ticked
- **Forward: -Z**, **Up: Y** — Blender is Z-up, Unity is Y-up. Getting this wrong lays your zones on their side, which is hard to spot when they're invisible.
- **Limit to: Selected Objects** if your scene has the whole track in it

---

## Materials

Zone meshes must not be visible in game.

Give them all one shared transparent material, or a material you'll disable the renderer on later. Do **not** delete the `MeshRenderer` — the mod finds zones by renderer bounds, so a mesh with no renderer is invisible to CHAPPiE too.

A fully transparent material is the safest option: renderer present, nothing drawn.

---

## Before you export

- [ ] Every cell is a separate object
- [ ] Names match the contract exactly, no `.001` suffixes
- [ ] Transforms frozen / applied
- [ ] Cells are slabs, not planes
- [ ] No gaps between neighbouring cells
- [ ] Overshoot strips modelled beyond the deepest band
- [ ] Material is transparent, renderer intact

---

Next: [3. Unity setup](03-unity.md)
