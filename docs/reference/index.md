# Reference

The precise version of everything. If a tutorial and a reference page disagree, the reference page is right — these are written against the code.

| | |
|---|---|
| [Naming contract](naming-contract.md) | Every GameObject name the mod reads, and what it does. The actual interface between a map and CHAPPiE. |
| [Scoring model](scoring-model.md) | How line, angle, consistency, X-Factor and momentum combine into a final score. |
| [DQ and deduction rules](dq-rules.md) | Every rule that zeroes or reduces a run, with thresholds and the `config.json` key for each. |
| [SDK components](components.md) | Field-by-field reference for the Unity authoring components. |

---

## Two things worth knowing before you read any of this

**The contract is names, not components.** The SDK components are editor-only helpers that write GameObject names. They're stripped when CarX loads your map. Anything correctly named works, whatever produced the name.

**Every threshold is tunable.** Values quoted throughout are the shipped defaults from `config.json`. The host's config is what applies at an event, and the exported CSV records the weights that were actually used.
