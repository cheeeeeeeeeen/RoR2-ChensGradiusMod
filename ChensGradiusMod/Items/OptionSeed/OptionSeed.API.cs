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
        /// GameObject optionSeed, SeedTracker seedTracker.</param>
        public void FireForSeeds(CharacterBody optionSeedOwner, Action<GameObject, SeedTracker> actionToRun)
        {
            FireForSeeds(optionSeedOwner, actionToRun, (chance, seed, tracker) => Util.CheckRoll(chance, optionSeedOwner.master));
        }

        /// <summary>
        /// Loops through all the Option Seeds of the item wielder. The action has 4 useful parameters to use.
        /// The first parameter refers to the Option Seed itself. It is a GameObject.
        /// The second parameter refers to the SeedTracker component of the owner.
        /// The last parameter refers to the direction of the Option Seed. It is a normalized Vector3.
        /// The check can be customized in this overload. It has 1 parameter which is the chance for proc.
        /// It returns true or false if the proc is activated or not.
        /// </summary>
        /// <param name="optionSeedOwner">The owner of the Option Seed.</param>
        /// <param name="actionToRun">An action to execute for each Option. The inputs are as follows:
        /// GameObject optionSeed, SeedTracker seedTracker.</param>
        /// <param name="customCheck">Custom check for proc. Input is the computed chance. It returns true or false.</param>
        public void FireForSeeds(CharacterBody optionSeedOwner, Action<GameObject, SeedTracker> actionToRun, Func<float, GameObject, SeedTracker, bool> customCheck)
        {
            if (!optionSeedOwner) return;
            SeedTracker seedTracker = optionSeedOwner.GetComponent<SeedTracker>();
            if (!seedTracker) return;
            InputBankTest inputBankTest = seedTracker.inputBankTest;
            if (!inputBankTest) return;

            float chance = ProcComputation(procValue, GetCount(optionSeedOwner));

            if (customCheck(chance, seedTracker.leftSeed, seedTracker)) actionToRun(seedTracker.leftSeed, seedTracker);
            if (customCheck(chance, seedTracker.rightSeed, seedTracker)) actionToRun(seedTracker.rightSeed, seedTracker);
        }
    }
}