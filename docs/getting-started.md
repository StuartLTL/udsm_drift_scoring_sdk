# Getting started

Installing CHAPPiE and running a session. For building tracks, see the [track authoring guide](tutorials/).

---

## Install

CHAPPiE is a KSL mod, so you need the KSL stack first.

1. Switch CarX to the **`[moddable]`** Steam beta branch.
2. Install **BepInEx x64**, then **Kino**, then **KSL**, then the **KSL.CarX** extension.
3. [Download `CHAPPiE.ksm`](download.md) and drop it into `{CarX install}/kino/mods/`.
4. Launch. CHAPPiE appears in the KSL mod list.

Everyone in a lobby should run the **same build**. Mixed versions can leave a driver invisible to the host's roster, which means their runs won't be tracked and the one-car-at-a-time lock won't cover them. The Roles panel flags version mismatches.

---

## Where your data lives

Everything is under one folder:

```
{CarX install}/kino/mods/CHAPPiE/
  config.json          all tuning
  tracks/              *.chptrk track presets (includes reference laps)
  sessions/            qualification sessions
    exports/           results CSVs
  snapshots/           judge review map images
  debug.log            this session
  debug.prev.log       the one before
```

The KSL menu shows the resolved path under **Storage → Data**. If the install folder isn't writable, the mod falls back to LocalLow and that line tells you.

---

## Roles

**The room creator is the host.** CHAPPiE reads that from CarX directly — whoever made the lobby owns the session, and it doesn't change while you're in it.

The host assigns staff from the **Roles** tab: everyone running CHAPPiE in the lobby appears there. Staff can judge, DQ, control runs and record the reference line, and their own laps are excluded from qualifying.

There's a manual **Force HOST / Force CLIENT** override in the KSL menu for edge cases, like running an event from a room somebody else created. Leave it on **Auto** otherwise.

---

## Running a session

1. **Load the track preset** (Setup tab) or let the SDK markers define it.
2. **Record a reference lap** if you want ideal-line scoring. Host or any staff member can — a staff recording is sent to the host, who adopts it and pushes it to the whole lobby, including people who join later.
3. **Start Session** on the Scoreboard tab.
4. **Set the lobby to Competition** if you want the one-car-on-course lock.
5. Drivers run. Use **Next Driver** on the Run tab to call whose run starts — that triggers their start-light countdown.
6. **End Session**, then **Export CSV**.

The scoreboard shows `● ON RUN` against the driver currently on course. Scores land when their run ends.

---

## Judging tools

**DQ** from any scoreboard row. If that driver is on course the button reads **DQ & end run** and reaches their client; otherwise it marks their stored score. Either way the reason is recorded and shown in the STATUS column, and **↺ OK** reverses a judge-issued DQ without losing anything.

**The ▼ expander** on a scoreboard row shows that driver's full deduction log with timestamps — the wall tap at 12.4s, the missed clip — which is what you want when a score is disputed.

**X-Factor** is a judge slider worth 20%. Drag it after a run and the leaderboard entry updates in place.

**Judge Review** replays the run with a map, ghost and scrubber.

---

## Tuning

All thresholds live in `config.json` and are documented in [DQ and deduction rules](reference/dq-rules.md). Weights, angle windows, deduction values, wall-tap sensitivity — all of it.

Your tuning survives mod updates. The config carries a version, and an upgrade only resets fields whose meaning actually changed.

---

## When something looks wrong

`debug.log` first. It records the zone scan result on every scene load, role changes with the reason, packet registration, every score received, and every DQ with the values that triggered it.

Common ones:

| Symptom | Look at |
|---|---|
| No zones scoring | `[UDSM Line] Zone scan for: …  (0 zones)` — names didn't match |
| A driver's runs never appear | Are you actually host? Check the role line in the panel header |
| Scoreboard empty on a client | Host role, and whether the client shows in the host's Roles tab |
| Off-course never fires | Run-off is paved, so surface type reads asphalt. Cover it with [`offtrack_` meshes](reference/naming-contract.md#offtrack) |
| Start lights dark | Shared material, or `_EMISSION` not enabled |
