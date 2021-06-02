using Chen.GradiusMod.Items.GradiusOption.Components;
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
        /// </summary>
        /// <param name="optionSeedOwner">The owner of the Option Seed.</param>
        /// <param name="actionToRun">An action to execute for each Option. The inputs are as follows:
        /// GameObject seed, SeedBehavior behavior.</param>
        public void FireForSeeds(CharacterBody optionSeedOwner, Action<GameObject, SeedBehavior> actionToRun)
        {
            FireForSeeds(optionSeedOwner, actionToRun, (chance, _behavior) => Util.CheckRoll(chance, optionSeedOwner.master));
        }

        /// <summary>
        /// Loops through all the Option Seeds of the item wielder. The action has 2 useful parameters to use.
        /// The first parameter refers to the Option Seed itself. It is a GameObject.
        /// The second parameter refers to the SeedBehavior component of the Option Seed.
        /// The check can be customized in this overload. It has 2 parameters for use.
        /// The first parameter is the computed chance.
        /// The second parameter is the SeedBehavior of the Option Seed.
        /// It returns true or false if the proc is activated or not.
        /// </summary>
        /// <param name="optionSeedOwner">The owner of the Option Seed.</param>
        /// <param name="actionToRun">An action to execute for each Option. The inputs are as follows:
        /// GameObject seed, SeedBehavior behavior.</param>
        /// <param name="customCheck">Custom check for proc. Input are as follows:
        /// float chance, SeedBehavior behavior. Returns true or false.</param>
        public void FireForSeeds(CharacterBody optionSeedOwner, Action<GameObject, SeedBehavior> actionToRun, Func<float, SeedBehavior, bool> customCheck)
        {
            if (!optionSeedOwner) return;
            SeedTracker seedTracker = optionSeedOwner.GetComponent<SeedTracker>();
            if (!seedTracker) return;
            InputBankTest inputBankTest = seedTracker.inputBankTest;
            if (!inputBankTest) return;

            float chance = ProcComputation(procValue, GetCount(optionSeedOwner));

            if (customCheck(chance, seedTracker.leftBehavior)) actionToRun(seedTracker.leftSeed, seedTracker.leftBehavior);
            if (customCheck(chance, seedTracker.rightBehavior)) actionToRun(seedTracker.rightSeed, seedTracker.rightBehavior);
        }

        /// <summary>
        /// Sets the vertical offset multiplier for certain characters since some Option Seeds are positioned awkwardly. Vertical offset default is 0.4f.
        /// </summary>
        /// <param name="masterName">The CharacterMaster name of the character.</param>
        /// <param name="newValue">The multiplier value.</param>
        public void SetVerticalOffsetMultiplier(string masterName, float newValue) => VerticalOffsetMultipliers[masterName] = newValue;

        /// <summary>
        /// Used for storing a proc check value through the SeedBehavior's objectData dictionary so that it can be used for other states of the character.
        /// Whether the proc is successful or not, it will always be stored in the dictionary.
        /// </summary>
        /// <param name="chance">Computed chance used for rolling.</param>
        /// <param name="behavior">The Option Seed SeedBehavior component.</param>
        /// <param name="key">Key that will be used to store in the dictionary.</param>
        /// <returns>True or false if the proc is successful or not.</returns>
        public bool StoreProcCheck(float chance, SeedBehavior behavior, string key)
        {
            bool activated = Util.CheckRoll(chance, behavior.ownerMaster);
            behavior.O[key] = activated;
            return activated;
        }

        /// <summary>
        /// Checks the dictionary from the specified SeedBehavior for the key given as parameter, then returns it as a boolean value.
        /// </summary>
        /// <param name="behavior">The Option Seed SeedBehavior component which contains the dictionary.</param>
        /// <param name="key">Key to check the value with.</param>
        /// <returns>The stored value from the dictionary.</returns>
        public bool CheckStoredProc(SeedBehavior behavior, string key) => behavior.O.ContainsKey(key) && (bool)behavior.O[key];
    }
}