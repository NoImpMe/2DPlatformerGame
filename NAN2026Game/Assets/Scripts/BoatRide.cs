using UnityEngine;
using UnityEngine.Tilemaps;
using NAN2026.Core;

namespace NAN2026
{
    // 배: 갑판을 밟으면 물 끝(오른쪽)까지 항해. 탑승자는 갑판 이동량만큼 같이 운반.
    public class BoatRide : MonoBehaviour
    {
        public BoatRideConfig config;
        private Transform player;
        private PlayerHealth playerHealth;
        private Transform boatHome; // BoatPos: 사망 시 배가 돌아갈 원위치 마커
        private Tilemap water;
        private float targetX;

        private void Start()
        {
            var p = PlayerLocator.Find();
            if (p != null)
            {
                player = p.transform;
                playerHealth = p.GetComponent<PlayerHealth>();
                if (playerHealth != null) playerHealth.OnPlayerDied += HandlePlayerDied;
            }
            var w = GameObject.Find("Stage_Wall");
            if (w != null) water = w.GetComponent<Tilemap>();
            targetX = ComputeWaterEndX();

            var homeGO = GameObject.Find("BoatPos");
            if (homeGO != null) boatHome = homeGO.transform;
        }

        // 플레이어가 죽으면(체크포인트 부활 전) 배를 원위치(BoatPos)로 되돌린다.
        // 승선 중 죽어도 안전 — 배가 순간이동하면 RiderOnDeck() 거리 체크가 곧바로 false가 되어
        // 다음 FixedUpdate에서 항해가 저절로 멈추다.
        private void HandlePlayerDied()
        {
            bool hasHome = boatHome != null;
            float homeX = hasHome ? boatHome.position.x : 0f;
            float homeY = hasHome ? boatHome.position.y : 0f;
            BoatRideLogic.ResetPositionOnDeath(transform.position.x, transform.position.y, hasHome, homeX, homeY,
                out float rx, out float ry);
            transform.position = new Vector3(rx, ry, transform.position.z);
            SetJumpLock(false);
        }

        private float ComputeWaterEndX()
        {
            if (water == null || config == null) return transform.position.x;
            var c0 = water.WorldToCell(transform.position);
            int rowY = c0.y;
            bool found = false;
            for (int dy = 1; dy >= -2 && !found; dy--)
                if (water.GetTile(new Vector3Int(c0.x, c0.y + dy, 0)) != null) { rowY = c0.y + dy; found = true; }
            if (!found) return transform.position.x;
            int x = c0.x;
            while (water.GetTile(new Vector3Int(x + 1, rowY, 0)) != null) x++;
            return water.CellToWorld(new Vector3Int(x, rowY, 0)).x + 1f - config.deckHalfWidth - config.edgeMargin;
        }

        private bool RiderOnDeck()
        {
            if (player == null || config == null) return false;
            Vector3 d = player.position - transform.position;
            return Mathf.Abs(d.x) <= config.deckHalfWidth
                && d.y >= config.deckTopOffset - 0.4f
                && d.y <= config.deckTopOffset + config.riderGrace;
        }

        private bool jumpLockOn;

        // 내릴 때·씬을 떠날 때 잠금이 남지 않게 하는 안전핀 (FAIL#27: 전역 static 은 참조 카운트가 없다)
        private void OnDisable()
        {
            SetJumpLock(false);
            if (playerHealth != null) playerHealth.OnPlayerDied -= HandlePlayerDied;
        }

        private void SetJumpLock(bool on)
        {
            if (jumpLockOn == on) return;
            jumpLockOn = on;
            PlayerController2D.JumpLocked = on;
        }

        private void FixedUpdate()
        {
            if (config == null) return;
            bool aboard = RiderOnDeck();
            // 항해 중에만 잠근다. 종점에 닿으면 풀어야 내릴 수 있다(종점 갑판 y28.69 → 다음 발판까지 5.3u 건너뜀)
            SetJumpLock(config.lockJumpWhileSailing && aboard && transform.position.x < targetX);
            if (!aboard) return; // 밟고 있는 동안만 항해
            float nx = Mathf.MoveTowards(transform.position.x, targetX, config.sailSpeed * Time.fixedDeltaTime);
            float dx = nx - transform.position.x;
            if (dx == 0f) return;
            transform.position = new Vector3(nx, transform.position.y, transform.position.z);
            player.position += new Vector3(dx, 0f, 0f);
        }
    }
}