# DQ and deduction rules

A **DQ** zeroes the run and ends it immediately — the driver doesn't have to reach the finish line for the result to register.

A **deduction** subtracts points; the run still counts.

All thresholds below are `config.json` defaults and are tunable per event.

---

## Disqualifications

| Rule | Condition | Config keys |
|---|---|---|
| **Spin** | Angle ≥ 90° sustained 0.25 s | `SpinDQAngle`, `SpinDQSustain` |
| **Off-course** | ≥ 2 wheels off surface for 0.15 s | `OffCourseWheelsDQ`, `OffCourseTimeToDQ` |
| **Straightened** | Chassis slip < 8° **and** yaw rate < 12°/s for 1.0 s | `StraighteningAngleThreshold`, `StraighteningTimeToDQ`, `StraighteningYawRateMax` |
| **Wall impact** | Wall contact plus ≥ 25 km/h speed loss or ≥ 20° angle loss within 0.7 s | `WallImpactSpeedDropKmh`, `WallImpactAngleDrop`, `WallImpactWindowSec` |
| **No initiation** | Crossing `initiation_line` without having initiated a drift | — |
| **Car reset** | Driver resets or respawns mid-run | — |
| **Manual** | Staff-issued, live or retroactive | — |

### Straightening — which angle it uses

This one is worth understanding because it behaves differently from the score.

Scoring uses CarX's drift angle, which is the angle between where the car points and its **instantaneous velocity vector**. After a wall hit that vector gets deflected while the body stays aimed down the track, so the number stays high even though the car has visibly straightened. It also includes vertical velocity, so a bounce inflates it, and near-zero speed makes it noise.

The straightening check therefore uses a separate **chassis slip angle**: ground-projected, ignored below 3 m/s, and measured against a heading smoothed over ~0.25 s. That's the angle between where the car points and where it has actually been going — what a judge sees.

Both are degrees, so a threshold tuned against the old behaviour carries over.

A drift-side flip only counts as a left-right transition (and suppresses the DQ) if the car is **actually rotating**. A wall impact can flip the reported side for a few frames while the car tracks dead straight; that no longer buys an exemption.

After a wall tap, the time-to-DQ halves — a tap alone isn't a DQ, but tap-then-straighten is.

### Car reset

Resetting the car during a run ends it. Detected from the physical signature of the teleport rather than a hook on any one CarX method, so every reset route is covered: the waypoint reset, reset-to-player, UGC spawn reset.

Two signatures, either one triggers:

- Position jumps more than `max(8 m, speed × dt × 4)` in a single frame
- Position jumps ≥ 3 m **and** speed collapses from > 5 m/s to ~0 in one frame

The second exists because CarX's reset path zeroes the rigidbody velocity, and nothing can cover 3 m in a frame and be stationary at the end of it.

### Wall impact

Contact only counts as a wall if the contact normal is steeper than ~45° from horizontal (`WallContactNormalMaxAbsY = 0.7`) and the closing speed along that normal is at least 0.1 m/s. Sliding along a wall or resting against it is not a tap.

---

## Deductions

| Rule | Cost | Config key |
|---|---|---|
| High angle sustained | −5 at 80° for 0.5 s | `MajorAngleDeductionPts` |
| Under zone minimum angle | −2 per trigger, 2 s cooldown | `UnderAngleDeductionPts` |
| One wheel off surface | −3 per second | `OneWheelOffDeductionPerSec` |
| Missed clipping point | −10 each | `MissedClipDeductionPts` |
| Missed zone segment | −1 per (zone, length) below max depth | `MissedZoneSegmentPts` |
| Overshoot | −2 per position | `OvershootDeductionPts` |
| Wall tap | −1 | `WallTapDeductionPts` |
| Major wall tap | Upgraded tap after ≥ 15° angle loss within 1.0 s | `WallMajorAngleDropDeg` |
| Excessive braking | Flat, outside marked brake zones | `ExcessiveBraking*` |
| Correction | Informational — cost is already in Consistency | — |

A wall tap must be held for `WallTapMinContactSec` (0.25 s) before it registers, which filters brief glances on tight lines. Repeat taps have a 0.3 s cooldown so a continuous scrape logs as a few events rather than dozens.

**Corrections are logged but not subtracted.** They already reduced the consistency component; the deduction log shows them marked "not counted in total" so the columns reconcile.

---

## Staff DQ

Staff — the host plus anyone assigned the staff role — can DQ from the scoreboard.

- **Driver on course** → the DQ reaches their client and ends the run.
- **Driver already scored** → the host marks the stored score.

The scoreboard picks the right one based on who is currently running; the button changes to **DQ & end run** when it will be live.

A retroactive DQ records who issued it and the reason in dedicated fields. It does **not** touch the run's zone breakdown, so you can still show the driver what happened, and **↺ OK** restores the run fully.

Reinstating only clears judge-issued DQs. A spin, off-course, straighten or car reset stays — that verdict isn't the judge's to undo from this button.

---

## Related

- [Scoring model](scoring-model.md)
- [Naming contract](naming-contract.md) — `zone_angle_`, `initiation_line`, brake zones
