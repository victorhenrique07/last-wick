# LESSONS - auto-maintained by scripts/lessons.py

> Machine-owned. Do NOT hand-edit. Changes are overwritten on the next `lessons.py` write.
> Canonical state lives in `.specs/lessons.json`. Edit lessons only via the script.
> promote_threshold=2 distinct features · window_days=45 · quarantine_threshold=2

## Confirmed (load these at Plan/Checks)

Corroborated across multiple features. Safe to apply as guidance.

### L-002 - A walk for a rule that lists several cases must exercise every listed case, the last one included, not only the first.
- signal: `weak_test` · recurrence: 5 feature(s) · scope: `walk` · harmful: 0
- features: run-rewards, hub-flow, priest-stats, tarot-table, blacksmith-weapon
- evidence: C3, C4 (finding 2) (walk) (+4 more)
- last seen: 2026-11-07T00:01:47Z

## Candidates (under observation - do NOT load as guidance yet)

Seen once or not yet corroborated. Tracked, not trusted.

### L-001 - When two listeners of the same GameEvents event depend on each other, never rely on subscription order; defer the dependent step to LateUpdate.
- signal: `ac_gap` · recurrence: 1 feature(s) · scope: `events` · harmful: 0
- features: run-rewards
- evidence: RunFinisher.cs OnBossDefeated (finding 1) (events)
- last seen: 2026-11-04T02:55:23Z

### L-003 - Implement every row of the design's Error Handling Strategy and give each one a check.
- signal: `ac_gap` · recurrence: 1 feature(s) · scope: `error-handling` · harmful: 0
- features: run-rewards
- evidence: RunRewards.cs:52, RunFinisher.cs:71 (findings 3 and 4) (error-handling)
- last seen: 2026-11-04T02:55:25Z

### L-004 - Record what was observed for each walked check; a blanket confirmation is not evidence.
- signal: `weak_test` · recurrence: 1 feature(s) · scope: `walk` · harmful: 0
- features: run-rewards
- evidence: C7, C9, C12, C17, C19 (finding 5) (walk)
- last seen: 2026-11-04T02:55:26Z

### L-005 - Every loop over a designer-edited asset list skips null entries and entries without an ID before reading them.
- signal: `ac_gap` · recurrence: 1 feature(s) · scope: `data` · harmful: 0
- features: priest-stats
- evidence: PermanentStats.cs:48 (finding 1) (data)
- last seen: 2026-11-05T01:30:27Z

### L-006 - Exponential costs are computed in double and clamped before converting to int.
- signal: `ac_gap` · recurrence: 1 feature(s) · scope: `economy` · harmful: 0
- features: priest-stats
- evidence: PriestShop.NextCost (finding 5) (economy)
- last seen: 2026-11-05T01:30:28Z

### L-007 - Every level read from the save is clamped to its valid range when the save loads and again where it is used.
- signal: `ac_gap` · recurrence: 1 feature(s) · scope: `save` · harmful: 0
- features: campfire-light
- evidence: SaveSystem.Sanitize / CampfireShop.Level (findings 1 and 2) (save)
- last seen: 2026-11-05T04:35:40Z

### L-008 - Sanitize every saved list of Ids for blanks and duplicates on load, not only for unknown Ids.
- signal: `ac_gap` · recurrence: 1 feature(s) · scope: `save` · harmful: 0
- features: tarot-table
- evidence: Assets/_Project/Code/Runtime/Save/SaveSystem.cs:103 (save)
- last seen: 2026-11-06T22:17:44Z

### L-009 - A walk for a numeric limit must read the value at the limit itself, on both sides.
- signal: `weak_test` · recurrence: 1 feature(s) · scope: `walk` · harmful: 0
- features: tarot-table
- evidence: C19 (walk)
- last seen: 2026-11-06T22:17:44Z

### L-010 - Every row of the design's Error Handling table needs a check, and its fallback must handle the case it still cannot fill.
- signal: `ac_gap` · recurrence: 1 feature(s) · scope: `design` · harmful: 0
- features: tarot-table
- evidence: Assets/_Project/Code/Runtime/UI/LevelUpPanel.cs:69 (design)
- last seen: 2026-11-06T22:17:44Z

### L-011 - Filters over data assets must skip entries with a blank Id, not only null entries.
- signal: `ac_gap` · recurrence: 1 feature(s) · scope: `data` · harmful: 0
- features: tarot-table
- evidence: Assets/_Project/Code/Runtime/Cards/CardLibrary.cs:24 (data)
- last seen: 2026-11-06T22:17:44Z

### L-012 - When a shipped asset leaves a field at zero, the walk must exercise that field with test data, or its code path is never proven.
- signal: `weak_test` · recurrence: 1 feature(s) · scope: `walk` · harmful: 0
- features: blacksmith-weapon
- evidence: Assets/_Project/Code/Runtime/Combat/WeaponStats.cs:35 (walk)
- last seen: 2026-11-07T00:01:47Z

### L-013 - A catalog's default entry must resolve even when it is missing from the catalog's list.
- signal: `ac_gap` · recurrence: 1 feature(s) · scope: `data` · harmful: 0
- features: blacksmith-weapon
- evidence: Assets/_Project/Code/Runtime/Combat/WeaponCatalog.cs:19 (data)
- last seen: 2026-11-07T00:01:47Z

### L-014 - Validate data fields the UI never shows, so a value the screen hides cannot silently block an action.
- signal: `ac_gap` · recurrence: 1 feature(s) · scope: `data` · harmful: 0
- features: blacksmith-weapon
- evidence: Assets/_Project/Code/Runtime/Combat/WeaponData.cs:22 (data)
- last seen: 2026-11-07T00:01:47Z

### L-015 - A walk must record every part its proof names, not only the first part.
- signal: `weak_test` · recurrence: 1 feature(s) · scope: `walk` · harmful: 0
- features: blacksmith-weapon
- evidence: C19 (walk)
- last seen: 2026-11-07T00:01:47Z

## Quarantined (failed when applied - ignore)

A confirmed lesson that recurred alongside failure. Kept for the maintainer to review.

_none_
