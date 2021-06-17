using Chen.Helpers.CollectionHelpers;
using TILER2;
using UnityEngine;

namespace Chen.GradiusMod.Artifacts.Machines
{
    public partial class Machines : Artifact<Machines>
    {
        /// <summary>
        /// Adds a drone to the spawn pool for the enemies.
        /// </summary>
        /// <param name="masterObject">CharacterMaster GameObject of the drone.</param>
        /// <param name="spawnWeight">Spawn Weight of the drone.</param>
        /// <returns>True if the drone is added to the pool. False if it is already in the pool, or it was not added.</returns>
        public bool AddEnemyDroneType(GameObject masterObject, int spawnWeight)
        {
            if (spawnWeight <= 0 || !masterObject || EnemyDrones.Contains(masterObject)) return false;
            for (int i = 0; i < spawnWeight; i++)
            {
                EnemyDrones.Add(masterObject);
            }
            return true;
        }

        /// <summary>
        /// Removes a drone from the spawn pool for the enemies.
        /// </summary>
        /// <param name="masterObject">CharacterMaster GameObject of the drone.</param>
        /// <returns>True if the drone is removed from the pool. False if it is not in the pool.</returns>
        public bool RemoveEnemyDroneType(GameObject masterObject)
        {
            return EnemyDrones.RemoveAll(item => item == masterObject) > 0;
        }

        /// <summary>
        /// Assigns a drone as grounded. This will be used to determine if the drone should be repositioned to the ground to avoid floating stationary drones.
        /// </summary>
        /// <param name="bodyName">CharacterBody name of the drone.</param>
        /// <returns>True if the drone name is assigned. False if the name already exists.</returns>
        public bool AssignEnemyDroneAsGrounded(string bodyName)
        {
            return GroundedDrones.ConditionalAdd(bodyName, item => item == bodyName);
        }

        /// <summary>
        /// Unassigns a drone from being grounded.
        /// </summary>
        /// <param name="bodyName">CharacterBody name of the drone.</param>
        /// <returns>True if the drone name is removed. False if the name did not exist.</returns>
        public bool UnassignEnemyDroneAsGrounded(string bodyName)
        {
            return GroundedDrones.ConditionalRemove(bodyName);
        }
    }
}