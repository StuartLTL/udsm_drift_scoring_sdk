# Scoring model

How CHAPPiE turns a run into a number.

All values below are the shipped defaults from `config.json`. Every one is tunable per event — the host's config is what applies, and the exported CSV carries the weights actually used.

---

## The final score

```
Final = Line×0.40 + Angle×0.30 + Consistency×0.10 + X-Factor×0.20 + Momentum×0.00
        − Penalties − Deductions
```

Each component is 0–100 before weighting. A DQ or an incomplete run scores 0 regardless of components.

| Component | Weight | Source |
|---|---|---|
| Line | 40% | Depth reached through the zone grid |
| Angle | 30% | Drift angle against the zone's window |
| Consistency | 10% | Variance between and within zones |
| X-Factor | 20% | Judge slider, 0–100 |
| Momentum | 0% | Captured but unweighted by default |

**Momentum is recorded and exported but does not affect the score** until a host raises its weight. If you do, drop the others so the total stays at 1.0.

---

## Line — the depth grid

Line is the reason zones are split into a grid rather than one mesh.

For each **(zone, length) position**, the mod records the deepest depth index the car reached:

```
depthRatio = bestDepth / maxDepthForThatZone
```

Zone score is the average of `depthRatio` across every length position in that zone, ×100. The overall line score is the average across all zones. Every zone is worth the same regardless of how many segments it has.

Three consequences worth designing around:

**Missing a whole length position hurts more than being shallow.** A position never entered contributes 0, not "shallow". A driver who runs wide past your entry band loses that position entirely.

**Max depth is per zone, not global.** A zone authored 3 deep and a zone authored 5 deep are both scored out of their own maximum. You don't have to keep depth counts consistent between zones — though keeping them consistent makes the numbers easier to reason about.

**Broken meshes are excluded.** A segment with zero-size bounds (a bad export) is dropped from the max-depth calculation, so an unreachable band doesn't drag the ratio down. It's logged as a warning at scan time — check `debug.log` if a zone scores lower than it looks.

### Overshoot

Touching `overshoot_{zone}_{length}` halves the depth ratio for that position, even when max depth was reached, and fires an `Overshoot` deduction (−2 by default).

This is what makes "deeper is better" stop at the wall.

### Ideal line

If the host has recorded a reference lap, line scoring blends the gate result with deviation from that lap:

```
Line = gateScore×60% + idealLineScore×40%
```

The ideal-line score compares the driver's path to the reference at 10 Hz, weighting **lateral deviation 60% and angle deviation 40%**, with samples inside a scored zone counted **2.5× heavier** than samples between zones. Full penalty at 6 m lateral or 40° of angle difference.

No reference lap recorded means no blend — line is the gate result alone.

---

## Angle

Measured against the zone's window. The default window comes from `MaxExpectedAngle` (55°); a `zone_angle_{ID}_{min}_{max}` marker overrides it for that zone.

Below the zone's `Min` for `UnderAngleSustainSec` (0.3 s) costs `UnderAngleDeductionPts` (−2), with a 2 s cooldown so the driver gets a chance to recover before it fires again.

Angle for scoring comes from CarX's own `DriftController.driftAngle` — the same value the game shows the driver, so your score and their HUD agree.

> That value is `acos(dot(forward, velocity.normalized))` against the instantaneous 3D velocity. It's the right number for scoring, but it moves when an impact deflects the car's velocity even though the body hasn't turned. The straightening DQ therefore uses a different, chassis-referenced angle — see [DQ rules](dq-rules.md).

---

## Consistency

Three inputs, all about *stability* rather than magnitude:

- **Inter-zone variance** — how much the average angle differs between zones
- **Intra-zone stability** — angle standard deviation within each zone
- **Trend penalty** — a run that degrades from start to finish is penalised

Corrections (mid-drift countersteer saves) reduce consistency directly. They appear in the deduction log marked as **not** counted in the total, because their cost is already inside the consistency number — counting them again would be double-jeopardy.

---

## X-Factor

A 0–100 judge slider, 20% of the final score. The host can drag it after a run ends and the leaderboard entry updates in place rather than keeping the original number.

This is the deliberate human channel. Everything else is mechanical.

---

## What the driver sees

The score panel breaks out each weighted component, the ideal-line result if one was recorded, and the full deduction log with timestamps. The host sees the same for every driver in the lobby via the scoreboard expander, and the CSV export carries all of it plus a per-zone breakdown.

---

## Related

- [Naming contract](naming-contract.md) — how zones are defined
- [DQ and deduction rules](dq-rules.md) — what zeroes or reduces a run
