using System;
using Assets._Project.Code.Runtime.Run;
using UnityEngine;

// Relógio da run. Conta o tempo passado e avisa uma única vez quando o limite do RunConfig acaba.
public class RunTimer : MonoBehaviour
{
    [SerializeField] private RunConfig config;

    private float elapsed;
    private bool running = true;

    public float Elapsed => elapsed;
    public float Remaining => Mathf.Max(0f, config.TimeLimit - elapsed);
    public bool IsRunning => running;

    private void OnEnable()
    {
        GameEvents.PlayerDied += Stop;
        GameEvents.RunFinished += OnRunFinished;
    }

    private void OnDisable()
    {
        GameEvents.PlayerDied -= Stop;
        GameEvents.RunFinished -= OnRunFinished;
    }

    private void Update()
    {
        if (!running) return;

        elapsed += Time.deltaTime;

        if (elapsed >= config.TimeLimit)
        {
            elapsed = config.TimeLimit;
            // Para antes de avisar, para o evento não disparar de novo no próximo frame.
            running = false;
            GameEvents.RaiseRunTimeExpired();
        }
    }

    public void Stop() => running = false;

    private void OnRunFinished(RunResult result) => Stop();

    // Tempo restante como "mm:ss". O teto faz o relógio só mostrar 0:00 quando acabar de verdade.
    public string Format()
    {
        int total = Mathf.CeilToInt(Remaining);
        return $"{total / 60}:{total % 60:00}";
    }
}
