using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    /// <summary>
    /// Lives on the CharacterMaster of the red Psy Drone. It summons the green twin and links both drones together.
    /// The link is kept on the masters because drone bodies are recreated every stage.
    /// </summary>
    internal class TwinsSpawn : MonoBehaviour
    {
        public CharacterMaster twinMaster { get; private set; }

        private CharacterMaster master;

        /// <summary>
        /// Finds the body of the other twin.
        /// </summary>
        /// <param name="psyDroneMaster">The master of either of the twins.</param>
        /// <returns>The body of the other twin, or null if it cannot be found.</returns>
        public static CharacterBody FindTwinBody(CharacterMaster psyDroneMaster)
        {
            if (!psyDroneMaster) return null;
            TwinsSpawn leader = psyDroneMaster.GetComponent<TwinsSpawn>();
            if (leader) return leader.twinMaster ? leader.twinMaster.GetBody() : null;
            TwinsDeath follower = psyDroneMaster.GetComponent<TwinsDeath>();
            return follower && follower.leader && follower.leader.master ? follower.leader.master.GetBody() : null;
        }

        private void Awake()
        {
            master = GetComponent<CharacterMaster>();
            if (NetworkServer.active) SpawnTwin();
        }

        private void FixedUpdate()
        {
            if (!NetworkServer.active) return;
            CharacterBody body = master.GetBody();
            bool isAlive = IsAlive(body);
            if (!twinMaster)
            {
                if (isAlive) SpawnTwin();
                return;
            }
            CharacterBody twinBody = twinMaster.GetBody();
            if (isAlive && IsDying(twinBody)) body.healthComponent.Suicide();
            else if (IsAlive(twinBody) && IsDying(body)) twinBody.healthComponent.Suicide();
            SyncDroneUpgrades();
        }

        private void OnDestroy()
        {
            // The red drone was removed without dying, e.g. by the Drone Scrapper or the Drone Combiner. The green twin goes with it.
            if (!NetworkServer.active || !twinMaster) return;
            CharacterBody twinBody = twinMaster.GetBody();
            if (IsAlive(twinBody)) NetworkServer.Destroy(twinBody.gameObject);
            NetworkServer.Destroy(twinMaster.gameObject);
        }

        private void SpawnTwin()
        {
            CharacterBody body = master.GetBody();
            if (!body || !master.minionOwnership) return;
            CharacterMaster ownerMaster = master.minionOwnership.ownerMaster;
            CharacterMaster newTwinMaster = new MasterSummon
            {
                masterPrefab = PsyDrone.droneMasterGreen,
                position = body.transform.position + Vector3.up,
                rotation = body.transform.rotation,
                summonerBodyObject = ownerMaster ? ownerMaster.GetBodyObject() : null,
                teamIndexOverride = master.teamIndex,
                ignoreTeamMemberLimit = true,
                useAmbientLevel = true
            }.Perform();
            if (!newTwinMaster) return;
            if (ownerMaster && newTwinMaster.minionOwnership.ownerMaster != ownerMaster) newTwinMaster.minionOwnership.SetOwner(ownerMaster);
            twinMaster = newTwinMaster;
            twinMaster.gameObject.AddComponent<TwinsDeath>().leader = this;
            SyncDroneUpgrades();
            PlaySummonEffect(twinMaster.GetBody());
        }

        private void SyncDroneUpgrades()
        {
            if (!master.inventory || !twinMaster.inventory) return;
            ItemDef upgradeItem = DLC3Content.Items.DroneUpgradeHidden;
            int difference = master.inventory.GetItemCountPermanent(upgradeItem) - twinMaster.inventory.GetItemCountPermanent(upgradeItem);
            if (difference > 0) twinMaster.inventory.GiveItemPermanent(upgradeItem, difference);
            else if (difference < 0) twinMaster.inventory.RemoveItemPermanent(upgradeItem, -difference);
        }

        private static void PlaySummonEffect(CharacterBody twinBody)
        {
            if (!twinBody || !twinBody.modelLocator || !twinBody.modelLocator.modelTransform) return;
            CharacterModel characterModel = twinBody.modelLocator.modelTransform.GetComponent<CharacterModel>();
            if (!characterModel) return;
            TemporaryOverlayInstance overlay = TemporaryOverlayManager.AddOverlay(twinBody.modelLocator.modelTransform.gameObject);
            overlay.duration = 0.5f;
            overlay.alphaCurve = AnimationCurve.EaseInOut(0f, 1f, 1f, 0f);
            overlay.animateShaderAlpha = true;
            overlay.destroyComponentOnEnd = true;
            overlay.originalMaterial = summonDroneMaterial;
            overlay.AddToCharacterModel(characterModel);
        }

        private static bool IsAlive(CharacterBody body) => body && body.healthComponent && body.healthComponent.alive;

        private static bool IsDying(CharacterBody body) => body && body.healthComponent && !body.healthComponent.alive;
    }
}
