# Download

Two pieces, and most people only need the first.

The **mod** is what scores runs — everyone in the lobby installs it. The **SDK** is a set of Unity editor scripts you only need if you're authoring a track.

<div class="dl-grid">
  <div class="dl-card">
    <h3>CHAPPiE mod</h3>
    <p class="dl-kind">KSL mod &middot; .ksm</p>
    <p class="dl-desc">The scoring mod itself. Drop it in your KSL mods folder. Everyone in the lobby needs the same build, host and drivers alike.</p>
    <a class="dl-btn dl-btn-primary" href="{{ '/downloads/CHAPPiE.ksm' | relative_url }}" download>Download CHAPPiE.ksm</a>
    <p class="dl-meta">
      <span>{% if site.data.release.mod_version and site.data.release.mod_version != "" %}v{{ site.data.release.mod_version }} &middot; {% endif %}Built {{ site.data.build.mod_date | default: "—" }}</span>
      <span>{{ site.data.build.mod_size | default: "—" }}</span>
    </p>
    <p class="dl-meta"><code>SHA-256 {{ site.data.build.mod_sha256 | default: "—" }}</code></p>
  </div>
  <div class="dl-card">
    <h3>Track SDK</h3>
    <p class="dl-kind">Unity editor scripts &middot; .zip</p>
    <p class="dl-desc">The components that name your zone meshes to the contract while you author. Editor-only — nothing from here ships inside your map or runs in game.</p>
    <a class="dl-btn" href="{{ '/downloads/CHAPPiE-SDK.zip' | relative_url }}" download>Download CHAPPiE-SDK.zip</a>
    <p class="dl-meta">
      <span>Built {{ site.data.build.sdk_date | default: "—" }}</span>
      <span>{{ site.data.build.sdk_size | default: "—" }}</span>
    </p>
    <p class="dl-meta"><code>SHA-256 {{ site.data.build.sdk_sha256 | default: "—" }}</code></p>
  </div>
</div>

---

## Installing the mod

CHAPPiE runs on the KSL stack, so that goes in first.

1. Switch CarX to the **`[moddable]`** Steam beta branch.
2. Install **BepInEx x64**, then **Kino**, then **KSL**, then the **KSL.CarX** extension.
3. Put `CHAPPiE.ksm` in `{CarX install}/kino/mods/`.
4. Launch. CHAPPiE appears in the KSL mod list.

The default Steam path is:

```
C:\Program Files (x86)\Steam\steamapps\common\CarX Drift Racing Online\kino\mods\
```

Everyone in a lobby should be on the **same build**. Mixed versions can leave a driver invisible to the host's roster, which means their runs aren't tracked and the one-car-at-a-time lock won't cover them. The Roles panel flags mismatches when it sees them.

Full walkthrough: **[Getting started](getting-started.md)**.

---

## Installing the SDK

Unzip it into your Unity track project under `Assets/`. You'll get a `UDSM_SDK/` folder, and the components appear under **Add Component → UDSM/…**.

It needs no packages and touches nothing at runtime — the scripts live in an `Editor` folder and rename GameObjects while you work. Custom MonoBehaviours don't survive the AssetBundle round-trip into CarX, so the names are the only thing that crosses over. That's the whole interface, and it's why you can author a working track with no SDK at all if you're willing to type the names by hand.

Start at **[Track authoring](tutorials/)**, or go straight to the **[naming contract](reference/naming-contract.md)**.

---

## Verifying a download

Both files publish with a SHA-256 above, computed by the release workflow from the exact bytes it uploaded. To check a file matches:

```powershell
Get-FileHash CHAPPiE.ksm -Algorithm SHA256
```

```bash
sha256sum CHAPPiE.ksm
```

If the hash doesn't match what's on this page, don't install it — re-download and check again.

---

## Source and history

These files are mirrored from the private development repository on every release, which is why the page above can state a build date and hash rather than a promise.

- **SDK source + this site** — [udsm_drift_scoring_sdk](https://github.com/StuartLTL/udsm_drift_scoring_sdk)
- **Release binaries** — [udsm_drift_scoring_public](https://github.com/StuartLTL/udsm_drift_scoring_public)

Found a bug? Open an issue on the releases repo with your KSL version, your CarX build, and the `[CHAPPiE ...]` lines from `kino/output.log`.
