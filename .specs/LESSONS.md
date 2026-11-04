# LESSONS - auto-maintained by scripts/lessons.py

> Machine-owned. Do NOT hand-edit. Changes are overwritten on the next `lessons.py` write.
> Canonical state lives in `.specs/lessons.json`. Edit lessons only via the script.
> promote_threshold=2 distinct features · window_days=45 · quarantine_threshold=2

## Confirmed (load these at Plan/Checks)

Corroborated across multiple features. Safe to apply as guidance.

_none_

## Candidates (under observation - do NOT load as guidance yet)

Seen once or not yet corroborated. Tracked, not trusted.

### L-001 - When two listeners of the same GameEvents event depend on each other, never rely on subscription order; defer the dependent step to LateUpdate.
- signal: `ac_gap` · recurrence: 1 feature(s) · scope: `events` · harmful: 0
- features: run-rewards
- evidence: RunFinisher.cs OnBossDefeated (finding 1) (events)
- last seen: 2026-11-04T02:55:23Z

### L-002 - A walk for a rule that lists several cases must exercise every listed case, the last one included, not only the first.
- signal: `weak_test` · recurrence: 1 feature(s) · scope: `walk` · harmful: 0
- features: run-rewards
- evidence: C3, C4 (finding 2) (walk)
- last seen: 2026-11-04T02:55:25Z

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

## Quarantined (failed when applied - ignore)

A confirmed lesson that recurred alongside failure. Kept for the maintainer to review.

_none_
