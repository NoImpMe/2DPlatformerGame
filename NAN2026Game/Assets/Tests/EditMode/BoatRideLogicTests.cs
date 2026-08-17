using NUnit.Framework;
using NAN2026.Core;

namespace NAN2026.Tests
{
    public class BoatRideLogicTests
    {
        [Test] public void ResetPositionOnDeath_HasHome_MovesToHomeXY()
        {
            BoatRideLogic.ResetPositionOnDeath(42f, 3f, true, 10f, 5f, out float rx, out float ry);
            Assert.AreEqual(10f, rx, 0.0001f);
            Assert.AreEqual(5f, ry, 0.0001f);
        }

        [Test] public void ResetPositionOnDeath_NoHome_KeepsCurrentPosition()
        {
            BoatRideLogic.ResetPositionOnDeath(42f, 3f, false, 10f, 5f, out float rx, out float ry);
            Assert.AreEqual(42f, rx, 0.0001f);
            Assert.AreEqual(3f, ry, 0.0001f);
        }
    }
}
