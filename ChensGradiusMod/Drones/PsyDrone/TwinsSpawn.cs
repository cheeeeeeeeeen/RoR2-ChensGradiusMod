using Chen.Helpers.UnityHelpers;
using RoR2;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class TwinsSpawn : MonoBehaviour
    {
        private CharacterBody body { get => master.GetBody(); }
        private CharacterMaster master { get => GetComponent<CharacterMaster>(); }

        private void Awake()
        {
            CharacterMaster twinCharacterMaster = new MasterSummon
            {
                masterPrefab = PsyDrone.droneMasterGreen,
                position = body.transform.position + Vector3.up,
                rotation = body.transform.rotation,
                summonerBodyObject = master.minionOwnership.ownerMaster.GetBodyObject(),
                ignoreTeamMemberLimit = true,
                useAmbientLevel = true
            }.Perform();
            if (twinCharacterMaster)
            {
                GameObject bodyObject = twinCharacterMaster.GetBodyObject();
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
                        temporaryOverlay.originalMaterial = summonDroneMaterial;
                        temporaryOverlay.AddToCharacerModel(component.modelTransform.GetComponent<CharacterModel>());
                    }
                    body.GetOrAddComponent<TwinsDeath>().twin = bodyObject;
                    bodyObject.GetOrAddComponent<TwinsDeath>().twin = body.gameObject;
                }
            }
        }
    }
}