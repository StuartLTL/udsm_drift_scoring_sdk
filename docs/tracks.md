# Tracks

Maps that carry CHAPPiE scoring geometry — zone meshes, run lines, the things that turn a lap into a number.

A track only needs marking **once**. The names are the whole interface, so anyone can author them and anyone can run an event on the result.

---

{% if site.data.tracks.tracks and site.data.tracks.tracks.size > 0 %}
| Track | Scene name | Author | Where to get it | Zones | Lines | Checked |
|---|---|---|---|---|---|---|
{% for t in site.data.tracks.tracks -%}
| {% if t.url %}[{{ t.name | default: t.scene }}]({{ t.url }}){% else %}{{ t.name | default: t.scene }}{% endif %} | `{{ t.scene }}` | {{ t.author | default: "—" }} | {{ t.where | default: "—" }} | {{ t.zones | default: "—" }} | {{ t.lines | default: "—" }} | {{ t.verified | default: "—" }} |
{% endfor %}

{% for t in site.data.tracks.tracks -%}
{%- if t.notes %}
**{{ t.name }}** — {{ t.notes }}
{% endif -%}
{% endfor %}
{% else %}
> **No tracks listed yet.** If you've marked one up, the section below is how it gets here — it takes about a minute.
{% endif %}

---

## Is a track CHAPPiE-ready?

Load it and open the KSL menu → **CHAPPiE → Home**. It reports what it found:

```
Scoring zones on this map: 8
Run lines: start, finish, initiation
```

If it says **no scoring zones found**, the map has no CHAPPiE geometry and runs won't score. That isn't a fault in the track — most maps have never been marked up.

The check is honest about partial setups too. A track with zones but no `run_start_line` still scores; you just start runs from the panel instead of the start box.

---

## Getting a track listed

1. Load the track and open **KSL → CHAPPiE → Home**.
2. Press **Copy track report**. That puts a filled-in summary on your clipboard — zone count, run lines, CHAPPiE version.
3. Open an issue on [udsm_drift_scoring_sdk](https://github.com/StuartLTL/udsm_drift_scoring_sdk/issues), paste it in, and say who made the track and roughly where to find it.

Or skip the issue and send a pull request adding your entry to `docs/_data/tracks.yml` directly.

The report is generated from what the mod actually detected on the loaded map, so an entry can't claim geometry the track doesn't have.

**A download link is optional.** Most CarX tracks move around on Discord or between friends and have no stable URL, and demanding one would keep working tracks off this list for no good reason. What matters is the **scene name** — the mod fills that in exactly, and it's what lets a driver tell whether a track they already have is the one listed here. `Where to get it` can just say "Kino Discord" or "ask the author".

---

## Marking up your own track

You don't need permission or a submission to use CHAPPiE on a map you've built — mark it up and run an event the same day.

| | |
|---|---|
| [Track authoring tutorial](tutorials/) | Maya/Blender → Unity → Kino, end to end |
| [Naming contract](reference/naming-contract.md) | Every GameObject name the mod reads |
| [SDK components](reference/components.md) | The Unity components that write those names |

The SDK components are editor-only helpers that set GameObject names, and they're stripped when CarX loads the map. Anything named correctly works, whatever produced the name — so a track marked up by hand in Blender is as valid as one built with the SDK.
