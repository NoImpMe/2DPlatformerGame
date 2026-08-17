using System;

namespace NAN2026
{
    /// <summary>
    /// EnemyAI/MonsterHealth 체계를 쓰지 않는 보스(MinoBoss, MidBoss_FireKnight 등 자체 hp 필드를 쓰는
    /// 보스)가 구현하는 최소 계약. BossWorldHealthBar가 이 인터페이스만 보고 체력바를 그린다.
    /// 새 보스를 추가할 때 이 인터페이스만 구현하면 별도 코드 없이 BossWorldHealthBar를 그대로 붙일 수 있다.
    /// </summary>
    public interface IBossHealthSource
    {
        int CurrentHealth { get; }
        int MaxHealth { get; }

        /// <summary>체력이 바뀔 때마다 (현재, 최대)를 통지한다. 초기값 동기화를 위해 구독 시점에 최소 한 번은
        /// 별도로 값을 읽어야 할 수 있다(생성자/Awake 타이밍에 따라 첫 통지를 놓칠 수 있으므로).</summary>
        event Action<int, int> OnHealthChanged;
    }
}
