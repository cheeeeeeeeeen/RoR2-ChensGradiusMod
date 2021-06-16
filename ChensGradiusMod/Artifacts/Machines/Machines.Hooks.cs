using Chen.Helpers.GeneralHelpers;
using RoR2;
using TILER2;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.GradiusModPlugin;
using DeathState = EntityStates.Drone.DeathState;
using Random = UnityEngine.Random;

namespace Chen.GradiusMod.Artifacts.Machines
{
    public partial class Machines : Artifact<Machines>
    {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

        public override void Install()
        {
            base.Install();
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            On.RoR2.CharacterMaster.OnBodyDeath += CharacterMaster_OnBodyDeath;
            On.EntityStates.Drone.DeathState.OnImpactServer += DeathState_OnImpactServer;
        }

        public override void Uninstall()
        {
            base.Uninstall();
            CharacterBody.onBodyStartGlobal -= CharacterBody_onBodyStartGlobal;
            On.RoR2.CharacterMaster.OnBodyDeath -= CharacterMaster_OnBodyDeath;
            On.EntityStates.Drone.DeathState.OnImpactServer -= DeathState_OnImpactServer;
        }

#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member

        private void DeathState_OnImpactServer(On.EntityStates.Drone.DeathState.orig_OnImpactServer orig, DeathState self, Vector3 contactPoint)
        {
            if (!IsActiveAndEnabled()) orig(self, contactPoint);
        }

        private void CharacterMaster_OnBodyDeath(On.RoR2.CharacterMaster.orig_OnBodyDeath orig, CharacterMaster self, CharacterBody body)
        {
            orig(self, body);
            if (!NetworkServer.active || !IsActiveAndEnabled() || !self || !self.minionOwnership || self.minionOwnership.ownerMaster) return;
            self.LoopMinions(minion =>
            {
                CharacterBody minionBody = minion.GetBody();
                if (minionBody && minionBody.healthComponent) minionBody.healthComponent.Suicide();
            });
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody obj)
        {
            if (!NetworkServer.active || !IsActiveAndEnabled() || !obj || !obj.master || !obj.master.minionOwnership) return;
            if (obj.master.minionOwnership.ownerMaster || !obj.teamComponent) return;
            switch (obj.teamComponent.teamIndex)
            {
                case TeamIndex.Monster:
                    if (!Util.CheckRoll(hasDroneChance, obj.master)) return;
                    int droneNumber = Random.Range(minimumEnemyDroneCount, maximumEnemyDroneCount + 1 + Run.instance.loopClearCount);
                    for (int i = 1; i <= droneNumber; i++) SummonDrone(obj, RandomlySelectDrone(), i);
                    return;

                case TeamIndex.Player:
                    if (IsPrototypeCountUncapped(obj.master)) SummonDrone(obj, tc280DroneMaster);
                    return;

                default:
                    return;
            }
        }
    }
}