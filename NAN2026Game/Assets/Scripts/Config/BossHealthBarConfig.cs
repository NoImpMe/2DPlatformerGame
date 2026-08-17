using UnityEngine;

/// <summary>
/// EnemyAI 기반이 아닌 보스(BossWorldHealthBar)용 체력바 시각 수치의 단일 기준.
/// EnemyAIConfig.healthBar* 와 같은 형태를 따르되, 쫄몹 AI 수치와 섞이지 않도록 분리했다.
/// MonoBehaviour(BossWorldHealthBar)에 숫자 리터럴을 두지 않기 위한 Config.
/// </summary>
[CreateAssetMenu(fileName = "BossHealthBarConfig", menuName = "NAN2026/Boss Health Bar Config")]
public sealed class BossHealthBarConfig : ScriptableObject
{
    [Tooltip("보스 트랜스폼 기준 로컬 좌표(스케일 적용 전). 보스 스프라이트 상단보다 살짝 위로 잡는다")]
    public Vector3 healthBarOffset = new Vector3(0f, 2.4f, 0f);
    public Vector2 healthBarSize = new Vector2(1.6f, 0.18f);
    public Color healthBarBackground = new Color(0f, 0f, 0f, 0.75f);
    public Color healthBarFill = new Color(0.85f, 0.15f, 0.15f, 1f);
}
