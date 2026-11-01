using System;
using Assets._Project.Code.Runtime.Run;
using UnityEngine;

public class RunProgress : MonoBehaviour
{
    [SerializeField] private RunConfig config; // lista de marcos, em pontos

    private int points;
    private int nextMilestone;

    public int Points => points;
    public int Kills { get; private set; }
    public int MilestonesReached => nextMilestone;
    public event Action<int> PointsChanged;   // pontos acumulados, para a barra da HUD

    private void OnEnable() => GameEvents.EnemyKilled += AddPoints;
    private void OnDisable() => GameEvents.EnemyKilled -= AddPoints;

    private void AddPoints(int amount)
    {
        Kills++;
        points += amount;
        PointsChanged?.Invoke(points);

        // O while cobre o caso de um único abate atravessar dois marcos.
        while (nextMilestone < config.Milestones.Length && points >= config.Milestones[nextMilestone])
        {
            GameEvents.RaiseMilestoneReached(nextMilestone);
            nextMilestone++;
        }
    }
}
