using System;
using Assets._Project.Code.Runtime.Run;
using LastWick.Combat;

public static class GameEvents
{
    // Combate e progresso
    public static event Action<int> EnemyKilled;          // pontos ganhos na barra
    public static event Action ArenaCleared;              // não sobrou inimigo após o marco
    public static event Action<int> MilestoneReached;     // índice do marco atingido
    public static event Action<Health> BossSpawned;       // vida do boss que acabou de nascer
    public static event Action<int> BossDefeated;         // índice do boss derrotado

    // Tempo e fim de run
    public static event Action RunTimeExpired;            // o relógio de 10:00 zerou
    public static event Action PlayerDied;                // a vida do player chegou a 0
    public static event Action<RunResult> RunFinished;    // estatísticas para a tela de resultado

    // Progressão da run
    public static event Action<int> XpGained;             // XP coletado do chão
    public static event Action<int> LevelUp;              // novo nível da run

    public static void RaiseEnemyKilled(int points) => EnemyKilled?.Invoke(points);
    public static void RaiseMilestoneReached(int index) => MilestoneReached?.Invoke(index);
    public static void RaiseArenaCleared() => ArenaCleared?.Invoke();
    public static void RaiseBossSpawned(Health boss) => BossSpawned?.Invoke(boss);
    public static void RaiseBossDefeated(int index) => BossDefeated?.Invoke(index);
    public static void RaiseRunTimeExpired() => RunTimeExpired?.Invoke();
    public static void RaisePlayerDied() => PlayerDied?.Invoke();
    public static void RaiseRunFinished(RunResult result) => RunFinished?.Invoke(result);
}
