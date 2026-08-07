# CHAPPiE

Competition drift judging for **CarX Drift Racing Online**, built on KSL (Kino Script Loader).

CHAPPiE scores a run the way a judging panel does — line, angle, consistency and style — against zones the track author defines in their own map. It runs a full qualifying session: host-controlled runs, a live lobby scoreboard, judge review, retroactive DQ, and a CSV results sheet.

---

## I want to…

| | |
|---|---|
| **Drive on a CHAPPiE track** | [Install the mod](getting-started.md) |
| **Build a track that CHAPPiE can score** | [Track authoring guide](tutorials/) |
| **Look up a GameObject name** | [Naming contract](reference/naming-contract.md) |
| **Understand how a score is calculated** | [Scoring model](reference/scoring-model.md) |
| **Know what gets you DQ'd** | [DQ and deduction rules](reference/dq-rules.md) |
| **Read the SDK component reference** | [Components](reference/components.md) |

---

## How the two halves fit together

CHAPPiE is split into a **mod** and an **SDK**, and they never talk to each other directly.

The SDK is a handful of editor-only Unity scripts. They don't ship inside your map and they don't run in game — all they do is rename GameObjects to a fixed convention while you're authoring. Custom MonoBehaviours don't survive the AssetBundle round-trip into CarX, so a component can't be the carrier.

The mod scans the loaded scene for those **names**. That's the entire interface.

```
Maya / Blender          Unity + SDK              CarX + CHAPPiE
──────────────          ───────────              ──────────────
split zone meshes  →    components rename   →    mod finds
by depth band           the GameObjects           "zone_1_3_2"
                        to the contract           and scores it
```

The practical consequence, and the thing worth internalising before you start: **the names are the contract.** You can author a fully working CHAPPiE track with no SDK at all, by naming meshes by hand. The SDK exists so you don't have to, and so a typo becomes an inspector field instead of a silent scoring hole.

---

## Status

CHAPPiE is in active development and used for real events. The naming contract is additive-only — a map authored against it keeps working across mod updates.

- **SDK + these docs** — [udsm_drift_scoring_sdk](https://github.com/StuartLTL/udsm_drift_scoring_sdk)
- **Releases** — [udsm_drift_scoring_public](https://github.com/StuartLTL/udsm_drift_scoring_public)

Both are mirrored automatically from the private development repo, so what you see here always matches a real build.
