using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassBeingTested = Chen.GradiusMod.Drones.PsyDrone.PsyDrone;

namespace Chen.GradiusMod.Tests.Drones
{
    [TestClass]
    public class PsyDrone
    {
        [TestMethod]
        public void DebugCheck_Toggled_ReturnsFalse()
        {
            bool result = ClassBeingTested.DebugCheck();

            Assert.IsFalse(result);
        }
    }
}