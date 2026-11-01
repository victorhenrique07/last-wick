using UnityEngine;

namespace LastWick.Player
{
    // Status base do player, válidos no jogo todo. As cartas nunca escrevem aqui.
    [CreateAssetMenu(fileName = "PlayerStats", menuName = "Last Wick/Player Stats")]
    public class PlayerStats : ScriptableObject
    {
        [Header("Sobrevivência")]
        [Min(1f)] public float MaxHealth = 100f;

        [Header("Movimento")]
        [Min(0f)] public float MoveSpeed = 1.5f;

        [Header("Esquiva")]
        [Tooltip("Distância percorrida, em unidades.")]
        [Min(0f)] public float DodgeDistance = 2.5f;
        [Tooltip("Duração da invencibilidade, contada a partir do início da esquiva.")]
        [Min(0f)] public float DodgeInvulnerability = 0.5f;
        [Min(0f)] public float DodgeCooldown = 2f;

        [Header("Ataque")]
        [Tooltip("Dano do próprio player, somado ao dano da arma.")]
        [Min(0f)] public float Damage = 0f;

        [Header("Luz")]
        [Min(0f)] public float LightRadius = 5f;

        [Header("Coleta")]
        [Min(0f)] public float XpPickupRadius = 2f;
    }
}
