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
        /// The second parameter refers to the SeedBehavior component of the Option Seed.
        /// The third parameter refers to the SeedTracker component of the item wielder.
        /// The last parameter is the computed damage multiplier based on configuration and item count of the owner.
        /// </summary>
        /// <param name="optionSeedOwner">The owner of the Option Seed.</param>
        /// <param name="actionToRun">An action to execute for each Option. The inputs are as follows:
        /// GameObject seed, SeedBehavior behavior, SeedTracker tracker, float multiplier.</param>
        public void FireForSeeds(CharacterBody optionSeedOwner, Action<GameObject, SeedBehavior, SeedTracker, float> actionToRun)
        {
            if (!optionSeedOwner) return;
            SeedTracker seedTracker = optionSeedOwner.GetComponent<SeedTracker>();
            if (!seedTracker) return;
            InputBankTest inputBankTest = seedTracker.inputBankTest;
            if (!inputBankTest) return;

            float multiplier = ComputeMultiplier(optionSeedOwner);
            actionToRun(seedTracker.leftSeed, seedTracker.leftBehavior, seedTracker, multiplier);
            actionToRun(seedTracker.rightSeed, seedTracker.rightBehavior, seedTracker, multiplier);
        }

        /// <summary>
        /// Sets the vertical offset multiplier for certain characters since some Option Seeds are positioned awkwardly. Vertical offset default is 0.4f.
        /// </summary>
        /// <param name="bodyName">The CharacterBody name of the character.</param>
        /// <param name="newValue">The multiplier value.</param>
        public void SetVerticalOffsetMultiplier(string bodyName, float newValue) => VerticalOffsetMultipliers[bodyName] = newValue;
    }
}