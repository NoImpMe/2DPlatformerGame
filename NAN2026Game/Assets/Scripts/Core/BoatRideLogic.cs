namespace NAN2026.Core
{
    /// 순수 로직: UnityEngine 비의존. EditMode 테스트 대상.
    /// 플레이어 사망 시 배가 돌아갈 위치를 계산한다.
    public static class BoatRideLogic
    {
        /// 원위치 마커(BoatPos)를 찾았으면(hasHome) 그 좌표로, 못 찾았으면 현재 좌표를 그대로 유지한다(안전한 no-op).
        public static void ResetPositionOnDeath(float currentX, float currentY, bool hasHome, float homeX, float homeY,
            out float resultX, out float resultY)
        {
            resultX = hasHome ? homeX : currentX;
            resultY = hasHome ? homeY : currentY;
        }
    }
}
