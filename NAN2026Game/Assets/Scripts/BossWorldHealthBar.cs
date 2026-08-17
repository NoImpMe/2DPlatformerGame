using UnityEngine;
using NAN2026;
using NAN2026.Core;

/// <summary>
/// EnemyAI/MonsterHealth 기반이 아닌 보스(MinoBoss, MidBoss_FireKnight 등 IBossHealthSource 구현체)
/// 머리 위에 뜨는 체력바. 기존 WorldHealthBar(EnemyAIConfig+NHNDemo.MonsterHealth 전용)와 완전히
/// 별개의 컴포넌트라 팀 몬스터 체계(EnemyAI/MonsterHealth)는 전혀 건드리지 않는다.
/// UI Canvas 없이 SpriteRenderer 두 장(배경/채움)으로 직접 그린다(WorldHealthBar와 동일 방식).
/// </summary>
[DisallowMultipleComponent]
public sealed class BossWorldHealthBar : MonoBehaviour
{
    [SerializeField] private BossHealthBarConfig config;
    [Tooltip("비워두면 같은 오브젝트에서 IBossHealthSource를 구현한 컴포넌트를 자동으로 찾는다")]
    [SerializeField] private MonoBehaviour source;

    private IBossHealthSource health;
    private Transform fillTransform;
    private SpriteRenderer fillRenderer;
    private static Sprite whitePixel;
    private static Sprite centeredWhitePixel;

    private void Awake()
    {
        health = source as IBossHealthSource;
        if (health == null)
        {
            foreach (MonoBehaviour mb in GetComponents<MonoBehaviour>())
            {
                IBossHealthSource candidate = mb as IBossHealthSource;
                if (candidate != null) { health = candidate; break; }
            }
        }
    }

    private void Start()
    {
        BuildBar();
    }

    private void OnEnable()
    {
        if (health != null) health.OnHealthChanged += HandleHealthChanged;
    }

    private void OnDisable()
    {
        if (health != null) health.OnHealthChanged -= HandleHealthChanged;
    }

    private static Sprite GetWhitePixel()
    {
        if (whitePixel != null) return whitePixel;
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        // pivot을 왼쪽(0, 0.5)에 둬서, 스케일을 줄여도 왼쪽 끝이 고정된 채로 오른쪽부터 줄어드는
        // 체력바를 만들 수 있다 (WorldHealthBar와 동일한 트릭).
        whitePixel = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0f, 0.5f), 1f);
        return whitePixel;
    }

    private static Sprite GetCenteredWhitePixel()
    {
        if (centeredWhitePixel != null) return centeredWhitePixel;
        Texture2D tex = new Texture2D(1, 1);
        tex.SetPixel(0, 0, Color.white);
        tex.Apply();
        centeredWhitePixel = Sprite.Create(tex, new Rect(0f, 0f, 1f, 1f), new Vector2(0.5f, 0.5f), 1f);
        return centeredWhitePixel;
    }

    private void BuildBar()
    {
        Vector3 offset = config != null ? config.healthBarOffset : new Vector3(0f, 1.6f, 0f);
        Vector2 size = config != null ? config.healthBarSize : new Vector2(1.2f, 0.16f);
        Color bg = config != null ? config.healthBarBackground : new Color(0f, 0f, 0f, 0.75f);
        Color fill = config != null ? config.healthBarFill : new Color(0.85f, 0.15f, 0.15f, 1f);

        GameObject barRoot = new GameObject("BossHealthBar");
        barRoot.transform.SetParent(transform, false);
        barRoot.transform.localPosition = offset;

        GameObject bgGO = new GameObject("Background");
        bgGO.transform.SetParent(barRoot.transform, false);
        SpriteRenderer bgSr = bgGO.AddComponent<SpriteRenderer>();
        bgSr.sprite = GetCenteredWhitePixel();
        bgSr.color = bg;
        bgSr.sortingOrder = 60;
        bgGO.transform.localScale = new Vector3(size.x, size.y, 1f);

        GameObject fillGO = new GameObject("Fill");
        fillGO.transform.SetParent(barRoot.transform, false);
        fillRenderer = fillGO.AddComponent<SpriteRenderer>();
        fillRenderer.sprite = GetWhitePixel();
        fillRenderer.color = fill;
        fillRenderer.sortingOrder = 61;
        fillTransform = fillGO.transform;
        fillTransform.localPosition = new Vector3(-size.x * 0.5f, 0f, 0f);
        fillTransform.localScale = new Vector3(size.x, size.y * 0.7f, 1f);

        if (health != null)
            HandleHealthChanged(health.CurrentHealth, health.MaxHealth);
    }

    private void HandleHealthChanged(int current, int max)
    {
        if (fillTransform == null) return;

        float ratio = EnemyAILogic.HealthRatio(current, max);
        Vector2 size = config != null ? config.healthBarSize : new Vector2(1.2f, 0.16f);
        Vector3 scale = fillTransform.localScale;
        scale.x = size.x * ratio;
        fillTransform.localScale = scale;
    }
}
