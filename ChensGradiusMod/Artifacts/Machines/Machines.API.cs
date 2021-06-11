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
        /// <returns>True if the drone is added in the pool. False if it is already in the pool.</returns>
        public bool AddEnemyDroneType(GameObject masterObject)
        {
            return EnemyDrones.ConditionalAdd(masterObject, item => item == masterObject);
        }

        /// <summary>
        /// Removes a drone from the spawn pool for the enemies.
        /// </summary>
        /// <param name="masterObject">CharacterMaster GameObject of the drone.</param>
        /// <returns>True if the drone is removed from the pool. False if it is not in the pool.</returns>
        public bool RemoveEnemyDroneType(GameObject masterObject)
        {
            return EnemyDrones.ConditionalRemove(masterObject);
        }
    }
}