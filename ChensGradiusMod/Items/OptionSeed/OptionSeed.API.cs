using Chen.GradiusMod.Items.OptionSeed.Components;
using RoR2;
using System;
using UnityEngine;

namespace Chen.GradiusMod.Items.OptionSeed
{
    public partial class OptionSeed
    {
        /// <summary>
        /// Loops through all the Option Seeds of the item wielder. The action has 4 useful parameters to use.
        /// The first parameter refers to the Option Seed itself. It is a GameObject.
        /// The second parameter refers to the SeedTracker component of the owner.
        /// The last parameter refers to the direction of the Option Seed. It is a normalized Vector3.
        /// </summary>
        /// <param name="optionSeedOwner">The owner of the Option Seed.</param>
        /// <param name="actionToRun">An action to execute for each Option. The inputs are as follows:
        /// GameObject optionSeed, SeedTracker seedTracker, Vector3 direction.</param>
        public void FireForAllSeeds(CharacterBody optionSeedOwner, Action<GameObject, SeedTracker, Vector3> actionToRun)
        {
            if (!optionSeedOwner) return;
            SeedTracker seedTracker = optionSeedOwner.GetComponent<SeedTracker>();
            if (!seedTracker) return;

            InputBankTest inputBankTest = seedTracker.inputBankTest;
            if (!inputBankTest) return;

            Vector3 direction = inputBankTest.aimDirection.normalized;
            actionToRun(seedTracker.leftSeed, seedTracker, direction);
            actionToRun(seedTracker.rightSeed, seedTracker, direction);
        }
    }
}