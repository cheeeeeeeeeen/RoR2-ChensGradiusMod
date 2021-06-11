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
            if (!NetworkServer.active || !IsActiveAndEnabled()) return;
            self.LoopMinions(minion =>
            {
                CharacterBody minionBody = minion.GetBody();
                if (minionBody.healthComponent) minionBody.healthComponent.Suicide();
                else Log.Warning($"CharacterMaster.OnBodyDeath: Minion {minion.name} of {body.name} does not have HealthComponent.");
            });
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody obj)
        {
            if (!NetworkServer.active || !IsActiveAndEnabled()) return;
            if (obj.master.minionOwnership.ownerMaster) return;
            switch (obj.teamComponent.teamIndex)
            {
                case TeamIndex.Monster:
                    if (!Util.CheckRoll(hasDroneChance, obj.master)) return;
                    int droneNumber = Random.Range(minimumEnemyDroneCount, maximumEnemyDroneCount + 1 + Run.instance.loopClearCount);
                    for (int i = 1; i <= droneNumber; i++) SummonDrone(obj, RandomlySelectDrone());
                    return;

                case TeamIndex.Player:
                    if (IsPrototypeCountUncapped(obj.master)) SummonDrone(obj, tc280DroneMaster);
                    return;

                default:
                    return;
            }
        }

        private GameObject RandomlySelectDrone()
        {
            int randomIndex = Random.Range(0, EnemyDrones.Count);
            return EnemyDrones[randomIndex];
        }

        private void SummonDrone(CharacterBody ownerBody, GameObject droneMasterPrefab)
        {
            if (!ownerBody || !droneMasterPrefab)
            {
                Log.Warning("Machines.SummonDrone: ownerBody or droneMasterPrefab is null. Cancel drone summon.");
                return;
            }
            CharacterMaster characterMaster = new MasterSummon
            {
                masterPrefab = droneMasterPrefab,
                position = ownerBody.transform.position - ownerBody.transform.forward.normalized,
                rotation = ownerBody.transform.rotation,
                summonerBodyObject = ownerBody.gameObject,
                ignoreTeamMemberLimit = true,
                useAmbientLevel = true
            }.Perform();
            if (characterMaster)
            {
                GameObject bodyObject = characterMaster.GetBodyObject();
                if (bodyObject)
                {
                    ModelLocator component = bodyObject.GetComponent<ModelLocator>();
                    if (component && component.modelTransform)
                    {
                        TemporaryOverlay temporaryOverlay = component.modelTransform.gameObject.AddComponent<TemporaryOverlay>();
                        temporaryOverlay.duration = 0.5f;
                        temporaryOverlay.animateShaderAlpha = true;
                        temporaryOverlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
                        temporaryOverlay.destroyComponentOnEnd = true;
                        temporaryOverlay.originalMaterial = Resources.Load<Material>("Materials/matSummonDrone");
                        temporaryOverlay.AddToCharacerModel(component.modelTransform.GetComponent<CharacterModel>());
                    }
                }
            }
        }

        private bool IsPrototypeCountUncapped(CharacterMaster master)
        {
            int prototypeNumber = 0;
            master.LoopMinions(minion =>
            {
                if (minion.name.Contains("MegaDroneMaster")) prototypeNumber++;
            });
            return prototypeNumber < maxPrototypePlayerCount;
        }
    }
}