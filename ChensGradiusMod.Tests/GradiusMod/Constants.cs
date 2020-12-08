using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chen.GradiusMod.Tests.GradiusMod
{
    [TestClass]
    public class Constants
    {
        [TestMethod]
        public void ModVer_Length_ReturnsCorrectFormat()
        {
            string result = GradiusModPlugin.ModVer;
            const int ModVersionCount = 3;

            int count = result.Split('.').Length;

            Assert.AreEqual(ModVersionCount, count);
        }

        [TestMethod]
        public void ModName_Value_ReturnsCorrectName()
        {
            string result = GradiusModPlugin.ModName;
            const string ModName = "ChensGradiusMod";

            Assert.AreEqual(ModName, result);
        }

        [TestMethod]
        public void ModGuid_Value_ReturnsCorrectGuid()
        {
            string result = GradiusModPlugin.ModGuid;
            const string ModGuid = "com.Chen.ChensGradiusMod";

            Assert.AreEqual(ModGuid, result);
        }
    }
}