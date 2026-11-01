using UnityEngine;

[CreateAssetMenu(fileName = "Card", menuName = "Last Wick/Cards Data")]
public class CardData : ScriptableObject
{
    public string Name;
    public string Description;

    [Header("Sobrevivência")]
    [SerializeField, Range(0f, 1f)] private float maxHealthBonus;

    [Header("Movimento")]
    [SerializeField, Range(0f, 1f)] private float moveSpeedBonus;

    [Header("Esquiva")]
    [SerializeField, Range(0f, 1f)] private float dodgeDistanceBonus;
    [SerializeField, Range(0f, 1f)] private float dodgeInvulnerabilityBonus;
    [Tooltip("Porcentagem a menos no cooldown da esquiva. 0,2 = 20% menor.")]
    [SerializeField, Range(0f, 1f)] private float dodgeCooldownReduction;

    [Header("Ataque")]
    [Tooltip("Dano fixo somado ao dano base, antes das porcentagens. Não é uma porcentagem.")]
    [SerializeField, Min(0f)] private float baseDamageBonus;
    [SerializeField, Range(0f, 1f)] private float damageBonus;
    [SerializeField, Range(0f, 1f)] private float fireRateBonus;

    [Header("Projétil")]
    [SerializeField, Range(0f, 1f)] private float projectileSpeedBonus;
    [Tooltip("Porcentagem a mais de vida do projétil, o que aumenta o alcance. 0,2 = 20% maior.")]
    [SerializeField, Range(0f, 1f)] private float projectileLifetimeBonus;
    [Tooltip("Inimigos a mais atravessados. É uma quantidade, não uma porcentagem.")]
    [SerializeField, Min(0)] private int pierceBonus;

    [Header("Luz")]
    [SerializeField, Range(0f, 1f)] private float lightRadiusBonus;

    [Header("Coleta")]
    [SerializeField, Range(0f, 1f)] private float xpPickupRadiusBonus;

    public float MaxHealthBonus => maxHealthBonus;
    public float MoveSpeedBonus => moveSpeedBonus;
    public float DodgeDistanceBonus => dodgeDistanceBonus;
    public float DodgeInvulnerabilityBonus => dodgeInvulnerabilityBonus;
    public float DodgeCooldownReduction => dodgeCooldownReduction;
    public float BaseDamageBonus => baseDamageBonus;
    public float DamageBonus => damageBonus;
    public float FireRateBonus => fireRateBonus;
    public float ProjectileSpeedBonus => projectileSpeedBonus;
    public float ProjectileLifetimeBonus => projectileLifetimeBonus;
    public int PierceBonus => pierceBonus;
    public float LightRadiusBonus => lightRadiusBonus;
    public float XpPickupRadiusBonus => xpPickupRadiusBonus;
}
