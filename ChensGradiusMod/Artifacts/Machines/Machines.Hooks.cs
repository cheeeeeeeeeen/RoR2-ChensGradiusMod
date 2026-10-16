using RoR2;
using RoR2.CharacterAI;
using UnityEngine.Networking;
using Random = UnityEngine.Random;

namespace Chen.GradiusMod.Artifacts.Machines
{
    public partial class Machines
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public override void Install()
        {
            base.Install();
            RunArtifactManager.onArtifactEnabledGlobal += RunArtifactManager_onArtifactEnabledGlobal;
            RunArtifactManager.onArtifactDisabledGlobal += RunArtifactManager_onArtifactDisabledGlobal;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            RunArtifactManager.onArtifactEnabledGlobal -= RunArtifactManager_onArtifactEnabledGlobal;
            RunArtifactManager.onArtifactDisabledGlobal -= RunArtifactManager_onArtifactDisabledGlobal;
            UnhookRunBehavior();
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private void RunArtifactManager_onArtifactEnabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifact)
        {
            if (!NetworkServer.active || artifact != artifactDef) return;
            UnhookRunBehavior();
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
        }

        private void RunArtifactManager_onArtifactDisabledGlobal(RunArtifactManager runArtifactManager, ArtifactDef artifact)
        {
            if (artifact != artifactDef) return;
            UnhookRunBehavior();
        }

        private void UnhookRunBehavior()
        {
            CharacterBody.onBodyStartGlobal -= CharacterBody_onBodyStartGlobal;
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody body)
        {
            if (!NetworkServer.active || !IsActiveAndEnabled() || !body || !body.teamComponent) return;
            CharacterMaster master = body.master;
            if (!master || !master.minionOwnership || master.minionOwnership.ownerMaster) return;
            // The artifact only processes a master once so that revived characters and respawned survivors do not stack drones.
            if (master.GetComponent<MachinesProcessed>()) return;

            TeamIndex team = body.teamComponent.teamIndex;
            if (team == TeamIndex.Player)
            {
                if (!body.isPlayerControlled) return;
                if (maxPrototypePlayerCount > 0 && IsPrototypeCountUncapped(master)) SummonDrone(body, GradiusModPlugin.tc280DroneMaster, false);
                return;
            }
            if (team == TeamIndex.None || team == TeamIndex.Neutral) return;
            if ((body.bodyFlags & CharacterBody.BodyFlags.Drone) == CharacterBody.BodyFlags.Drone) return;
            if (!master.GetComponent<BaseAI>()) return;

            master.gameObject.AddComponent<MachinesProcessed>();
            if (!Util.CheckRoll(hasDroneChance, 0f, null)) return;
            int loopClearCount = Run.instance ? Run.instance.loopClearCount : 0;
            int droneNumber = Random.Range(minimumEnemyDroneCount, maximumEnemyDroneCount + 1 + loopClearCount);
            for (int i = 1; i <= droneNumber; i++) SummonDrone(body, RandomlySelectDrone(), true, i);
        }

        /// <summary>
        /// Checks if the drone should not leave a repurchasable broken drone behind upon death.
        /// Drones owned by enemies, and drones that were given by this artifact, are never repurchasable.
        /// </summary>
        /// <param name="droneBody">The body of the drone that died.</param>
        /// <returns>True if the broken drone should not spawn.</returns>
        public bool ShouldSuppressBrokenDrone(CharacterBody droneBody)
        {
            if (!droneBody) return false;
            if (droneBody.teamComponent && droneBody.teamComponent.teamIndex != TeamIndex.Player) return true;
            return droneBody.master && droneBody.master.GetComponent<MachinesDrone>();
        }
    }
}
