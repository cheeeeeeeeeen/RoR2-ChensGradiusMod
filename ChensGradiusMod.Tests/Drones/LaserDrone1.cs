using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassBeingTested = Chen.GradiusMod.LaserDrone1;

namespace Chen.GradiusMod.Tests.Drones
{
    [TestClass]
    public class LaserDrone1
    {
        [TestMethod]
        public void DebugCheck_Toggled_ReturnsFalse()
        {
            bool result = ClassBeingTested.DebugCheck();

            Assert.IsFalse(result);
        }
    }
}