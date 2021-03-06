﻿using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Chen.GradiusMod.Tests
{
    [TestClass]
    public class GradiusMod
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

        [TestMethod]
        public void DebugCheck_Toggled_ReturnsFalse()
        {
            bool result = GradiusModPlugin.DebugCheck();

            Assert.IsFalse(result);
        }
    }
}