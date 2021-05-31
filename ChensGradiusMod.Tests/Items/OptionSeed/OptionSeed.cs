using Microsoft.VisualStudio.TestTools.UnitTesting;
using ClassBeingTested = Chen.GradiusMod.Items.OptionSeed.OptionSeed;

namespace Chen.GradiusMod.Tests.Items.OptionSeed
{
    [TestClass]
    public class OptionSeed
    {
        [TestMethod]
        public void DebugCheck_Toggled_ReturnsFalse()
        {
            bool result = ClassBeingTested.DebugHookCheck();

            Assert.IsFalse(result);
        }
    }
}