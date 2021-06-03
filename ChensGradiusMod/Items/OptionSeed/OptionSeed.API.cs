using Chen.GradiusMod.Items.OptionSeed.Components;
using RoR2;
using System;
using UnityEngine;

namespace Chen.GradiusMod.Items.OptionSeed
{
    public partial class OptionSeed
    {
        /// <summary>
        /// Loops through all the Option Seeds of the item wielder. The action has 2 useful parameters to use.
        /// The first parameter refers to the Option Seed itself. It is a GameObject.
        /// The second parameter refers to the SeedBehavior component of the Option Seed.
        /// The last parameter refers to the SeedTracker component of the item wielder.
        /// </summary>
        /// <param name="optionSeedOwner">The owner of the Option Seed.</param>
        /// <param name="actionToRun">An action to execute for each Option. The inputs are as follows:
        /// GameObject seed, SeedBehavior behavior, SeedTracker tracker.</param>
        public void FireForSeeds(CharacterBody optionSeedOwner, Action<GameObject, SeedBehavior, SeedTracker> actionToRun)
        {
            if (!optionSeedOwner) return;
            SeedTracker seedTracker = optionSeedOwner.GetComponent<SeedTracker>();
            if (!seedTracker) return;
            InputBankTest inputBankTest = seedTracker.inputBankTest;
            if (!inputBankTest) return;

            actionToRun(seedTracker.leftSeed, seedTracker.leftBehavior, seedTracker);
            actionToRun(seedTracker.rightSeed, seedTracker.rightBehavior, seedTracker);
        }

        /// <summary>
        /// Sets the vertical offset multiplier for certain characters since some Option Seeds are positioned awkwardly. Vertical offset default is 0.4f.
        /// </summary>
        /// <param name="bodyName">The CharacterBody name of the character.</param>
        /// <param name="newValue">The multiplier value.</param>
        public void SetVerticalOffsetMultiplier(string bodyName, float newValue) => VerticalOffsetMultipliers[bodyName] = newValue;

        /// <summary>
        /// Automatically computes for the damage multiplier based on the configuration and number of Option Seed items the owner has stacked.
        /// </summary>
        /// <param name="itemCount">The number of Option Seed items the owner has.</param>
        /// <returns>The computed multiplier.</returns>
        public float ComputeMultiplier(int itemCount) => damageMultiplier + (stackDamageMultiplier * (itemCount - 1));

        /// <summary>
        /// Automatically computes for the damage multiplier based on the configuration and number of Option Seed items the owner has stacked.
        /// </summary>
        /// <param name="ownerBody">Character Body of the owner.</param>
        /// <returns>The computed multiplier.</returns>
        public float ComputeMultiplier(CharacterBody ownerBody) => ComputeMultiplier(GetCount(ownerBody));
    }
}