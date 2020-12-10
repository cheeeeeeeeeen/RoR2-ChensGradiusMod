using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassBeingTested = Chen.GradiusMod.Drones.LaserDrone.LaserDrone2;

namespace Chen.GradiusMod.Tests.Drones
{
    [TestClass]
    public class LaserDrone2
    {
        [TestMethod]
        public void DebugCheck_Toggled_ReturnsFalse()
        {
            bool result = ClassBeingTested.DebugCheck();

            Assert.IsFalse(result);
        }
    }
}