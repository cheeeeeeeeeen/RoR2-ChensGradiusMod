#undef DEBUG

using Chen.Helpers.CollectionHelpers;
using Chen.Helpers.RoR2Helpers;
using Chen.Helpers.UnityHelpers;
using EntityStates;
using R2API;
using R2API.Utils;
using RoR2;
using RoR2.CharacterAI;
using RoR2.Skills;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.GradiusModPlugin;
using static EntityStates.Drone.DeathState;
using static R2API.DirectorAPI;
using UnityObject = UnityEngine.Object;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class PsyDrone : Drone<PsyDrone>
    {
        public int spawnWeight { get; private set; } = 1;

        public override bool canHaveOptions => false;

        public static InteractableSpawnCard iSpawnCard { get; private set; }
        public static GameObject brokenObject { get; private set; }
        public static DirectorCardHolder iDirectorCardHolder { get; private set; }
        public static DirectorCardHolder iHeavyDirectorCardHolder { get; private set; }
        public static GameObject droneBodyRed { get; private set; }
        public static GameObject droneMasterRed { get; private set; }
        public static GameObject droneBodyGreen { get; private set; }
        public static GameObject droneMasterGreen { get; private set; }
        public static GameObject mirrorLaserPrefab { get; private set; }

        protected override GameObject DroneCharacterMasterObject => droneMasterRed;

        protected override void SetupConfig()
        {
            spawnWeightWithMachinesArtifact = 0;
            base.SetupConfig();

            spawnWeight = config.Bind(configCategory,
                "SpawnWeight", spawnWeight,
                "The weight for which the Director is biased towards spawning this drone."
            ).Value;
        }

        protected override void SetupComponents()
        {
            base.SetupComponents();
            AddLanguageTokens();
            InteractableSpawnCard origIsc = drone1SpawnCard;
            brokenObject = origIsc.prefab.InstantiateClone($"{name}Broken", true);
            ModifyBrokenObject();
            iSpawnCard = Object.Instantiate(origIsc);
            ModifyInteractableSpawnCard();
            InitializeDirectorCards();
        }

        protected override void SetupBehavior()
        {
            base.SetupBehavior();
            InteractableActions += DirectorAPI_InteractableActions;
            CharacterBody.onBodyStartGlobal += CharacterBody_onBodyStartGlobal;
            On.EntityStates.Drone.DeathState.RigidbodyCollisionListener.OnCollisionEnter += RigidbodyCollisionListener_OnCollisionEnter;
        }

        private void AddLanguageTokens()
        {
            LanguageAPI.Add("PSI_BIT_NAME", "Psy Drone");
            LanguageAPI.Add("PSI_BIT_CONTEXT", "Repair Psy Drones");
            LanguageAPI.Add("PSI_BIT_INTERACTABLE_NAME", "Broken Psy Drones");
        }

        private void ModifyBrokenObject()
        {
            SummonMasterBehavior summonMasterBehavior = brokenObject.GetComponent<SummonMasterBehavior>();
            droneMasterRed = summonMasterBehavior.masterPrefab.InstantiateClone($"{name}RedMaster", true);
            droneMasterGreen = summonMasterBehavior.masterPrefab.InstantiateClone($"{name}GreenMaster", true);
            contentProvider.masterObjects.Add(droneMasterRed);
            contentProvider.masterObjects.Add(droneMasterGreen);
            ModifyDroneMaster();
            CharacterMaster masterRed = droneMasterRed.GetComponent<CharacterMaster>();
            CharacterMaster masterGreen = droneMasterGreen.GetComponent<CharacterMaster>();
            droneBodyRed = masterRed.bodyPrefab.InstantiateClone($"{name}RedBody", true);
            droneBodyGreen = masterGreen.bodyPrefab.InstantiateClone($"{name}GreenBody", true);
            contentProvider.bodyObjects.Add(droneBodyRed);
            contentProvider.bodyObjects.Add(droneBodyGreen);
            ModifyDroneBody();
            masterRed.bodyPrefab = droneBodyRed;
            masterGreen.bodyPrefab = droneBodyGreen;
            summonMasterBehavior.masterPrefab = droneMasterRed;
            PurchaseInteraction purchaseInteraction = brokenObject.GetComponent<PurchaseInteraction>();
            purchaseInteraction.cost *= 10;
            purchaseInteraction.Networkcost = purchaseInteraction.cost;
            purchaseInteraction.contextToken = "PSI_BIT_CONTEXT";
            purchaseInteraction.displayNameToken = "PSI_BIT_INTERACTABLE_NAME";
            GenericDisplayNameProvider nameProvider = brokenObject.GetComponent<GenericDisplayNameProvider>();
            nameProvider.displayToken = "PSI_BIT_NAME";
            GameObject customBrokenModel = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/mdlPsiBitsBroken.prefab");
            brokenObject.ReplaceModel(customBrokenModel);
            Highlight highlight = brokenObject.GetComponent<Highlight>();
            GameObject coreObject = customBrokenModel.transform.Find("_mdlPsiBitsBroken").gameObject;
            highlight.targetRenderer = coreObject.GetComponent<MeshRenderer>();
            EntityLocator entityLocator = customBrokenModel.AddComponent<EntityLocator>();
            entityLocator.entity = brokenObject;
            EntityLocator coreEntityLocator = coreObject.AddComponent<EntityLocator>();
            coreEntityLocator.entity = brokenObject;
            AddBrokenEffects(customBrokenModel, (MeshRenderer)highlight.targetRenderer);
        }

        private void AddBrokenEffects(GameObject customBrokenModel, MeshRenderer meshRenderer)
        {
            GameObject brokenEffects = brokenObject.transform.Find("ModelBase").Find("BrokenDroneVFX").gameObject;
            brokenEffects.transform.parent = customBrokenModel.transform;
            GameObject sparks = brokenEffects.transform.Find("Small Sparks, Mesh").gameObject;
            ParticleSystem.ShapeModule sparksShape = sparks.GetComponent<ParticleSystem>().shape;
            sparksShape.shapeType = ParticleSystemShapeType.MeshRenderer;
            sparksShape.meshShapeType = ParticleSystemMeshShapeType.Edge;
            sparksShape.meshRenderer = meshRenderer;
            GameObject damagePoint = brokenEffects.transform.Find("Damage Point").gameObject;
            damagePoint.transform.localPosition = Vector3.zero;
            damagePoint.transform.localRotation = Quaternion.identity;
            damagePoint.transform.localScale = Vector3.one;
        }

        private void ModifyDroneMaster()
        {
            AISkillDriver[] skillDrivers = droneMasterRed.GetComponents<AISkillDriver>();
            skillDrivers.SetAllDriversToAimTowardsEnemies();
            skillDrivers = droneMasterGreen.GetComponents<AISkillDriver>();
            skillDrivers.SetAllDriversToAimTowardsEnemies();
        }

        private void ModifyDroneBody()
        {
            CharacterBody bodyRed = droneBodyRed.GetComponent<CharacterBody>();
            bodyRed.baseNameToken = "PSI_BIT_NAME";
            bodyRed.baseMoveSpeed *= 1.3f;
            bodyRed.baseAcceleration *= 4f;
            bodyRed.baseMaxHealth *= 1.2f;
            bodyRed.baseRegen *= 1.2f;
            bodyRed.baseCrit = 0f;
            bodyRed.levelMaxHealth *= 1.2f;
            bodyRed.levelRegen *= 1.2f;
            bodyRed.levelCrit = 0f;
            bodyRed.levelMoveSpeed *= 1.3f;
            bodyRed.portraitIcon = assetBundle.LoadAsset<Texture>("Assets/Drones/LaserDrone1/Icon/texLaserDrone1IconOld.png");
            CharacterBody bodyGreen = droneBodyGreen.GetComponent<CharacterBody>();
            bodyGreen.baseNameToken = bodyRed.baseNameToken;
            bodyGreen.baseMoveSpeed = bodyRed.baseMoveSpeed;
            bodyGreen.baseAcceleration = bodyRed.baseAcceleration;
            bodyGreen.baseMaxHealth = bodyRed.baseMaxHealth;
            bodyGreen.baseRegen = bodyRed.baseRegen;
            bodyGreen.baseCrit = bodyRed.baseCrit;
            bodyGreen.levelMaxHealth = bodyRed.levelMaxHealth;
            bodyGreen.levelRegen = bodyRed.levelRegen;
            bodyGreen.levelCrit = bodyRed.levelCrit;
            bodyGreen.levelMoveSpeed = bodyRed.levelMoveSpeed;
            bodyGreen.portraitIcon = assetBundle.LoadAsset<Texture>("Assets/Drones/LaserDrone1/Icon/texLaserDrone1IconOld.png");
            ModifyDroneModel(bodyRed, bodyGreen);
            ModifySkill();
            CharacterDeathBehavior death = bodyRed.GetOrAddComponent<CharacterDeathBehavior>();
            death.deathState = new SerializableEntityStateType(typeof(DeathState));
            death = bodyGreen.GetOrAddComponent<CharacterDeathBehavior>();
            death.deathState = new SerializableEntityStateType(typeof(DeathState));
        }

        private void ModifyDroneModel(CharacterBody bodyRed, CharacterBody bodyGreen)
        {
            GameObject customModel = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/mdlPsiBitRed.prefab");
            droneBodyRed.ReplaceModel(customModel);
            customModel.transform.localRotation = Util.QuaternionSafeLookRotation(Vector3.left);
            customModel.InitializeDroneModelComponents(bodyRed, DebugCheck());
            customModel.transform.Find("Core").gameObject.AddComponent<CoreFlicker>();
            BodyRotation rotationComponent = customModel.transform.parent.gameObject.AddComponent<BodyRotation>();
            rotationComponent.rotationDirection = -1;
            rotationComponent.rotationSpeed = 6f;
            customModel = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/mdlPsiBitGreen.prefab");
            droneBodyGreen.ReplaceModel(customModel);
            customModel.transform.localRotation = Util.QuaternionSafeLookRotation(Vector3.left);
            customModel.InitializeDroneModelComponents(bodyGreen, DebugCheck());
            customModel.transform.Find("Core").gameObject.AddComponent<CoreFlicker>();
            rotationComponent = customModel.transform.parent.gameObject.AddComponent<BodyRotation>();
            rotationComponent.rotationDirection = 1;
            rotationComponent.rotationSpeed = 6f;
        }

        private void ModifySkill()
        {
            LoadoutAPI.AddSkill(typeof(MirrorLaser));
            LoadoutAPI.AddSkill(typeof(SearchLaser));
            SkillDef newSkillDefRed = Object.Instantiate(drone1Skill);
            newSkillDefRed.activationState = new SerializableEntityStateType(typeof(MirrorLaser));
            newSkillDefRed.baseRechargeInterval = 5f;
            newSkillDefRed.beginSkillCooldownOnSkillEnd = true;
            newSkillDefRed.baseMaxStock = 1;
            newSkillDefRed.fullRestockOnAssign = false;
            LoadoutAPI.AddSkillDef(newSkillDefRed);
            SkillLocator locator = droneBodyRed.GetComponent<SkillLocator>();
            SkillFamily newSkillFamily = Object.Instantiate(locator.primary.skillFamily);
            newSkillFamily.variants = new SkillFamily.Variant[1];
            newSkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = newSkillDefRed,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node("", false, null)
            };
            locator.primary.SetFieldValue("_skillFamily", newSkillFamily);
            LoadoutAPI.AddSkillFamily(newSkillFamily);
            SkillDef newSkillDefGreen = Object.Instantiate(drone1Skill);
            newSkillDefGreen.activationState = new SerializableEntityStateType(typeof(SearchLaser));
            newSkillDefGreen.baseRechargeInterval = newSkillDefRed.baseRechargeInterval;
            newSkillDefGreen.beginSkillCooldownOnSkillEnd = newSkillDefRed.beginSkillCooldownOnSkillEnd;
            newSkillDefGreen.baseMaxStock = newSkillDefRed.baseMaxStock;
            newSkillDefGreen.fullRestockOnAssign = newSkillDefRed.fullRestockOnAssign;
            LoadoutAPI.AddSkillDef(newSkillDefGreen);
            locator = droneBodyGreen.GetComponent<SkillLocator>();
            newSkillFamily = Object.Instantiate(locator.primary.skillFamily);
            newSkillFamily.variants = new SkillFamily.Variant[1];
            newSkillFamily.variants[0] = new SkillFamily.Variant
            {
                skillDef = newSkillDefGreen,
                unlockableDef = null,
                viewableNode = new ViewablesCatalog.Node("", false, null)
            };
            locator.primary.SetFieldValue("_skillFamily", newSkillFamily);
            LoadoutAPI.AddSkillFamily(newSkillFamily);
            mirrorLaserPrefab = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/MirrorLaser.prefab");
            mirrorLaserPrefab.AddComponent<NetworkIdentity>();
        }

        private void ModifyInteractableSpawnCard()
        {
            iSpawnCard.name = $"iscBroken{name}";
            iSpawnCard.prefab = brokenObject;
            iSpawnCard.slightlyRandomizeOrientation = false;
            iSpawnCard.orientToFloor = true;
        }

        private void DirectorAPI_InteractableActions(List<DirectorCardHolder> arg1, StageInfo arg2)
        {
            arg1.ConditionalAdd(iDirectorCardHolder, card => iDirectorCardHolder == card);
        }

        private void InitializeDirectorCards()
        {
            DirectorCard directorCard = new DirectorCard
            {
                spawnCard = iSpawnCard,
#if DEBUG
                selectionWeight = 1000,
#else
                selectionWeight = spawnWeight,
#endif
                spawnDistance = DirectorCore.MonsterSpawnDistance.Close,
                allowAmbushSpawn = true,
                preventOverhead = false
            };
            iDirectorCardHolder = new DirectorCardHolder
            {
                Card = directorCard,
                MonsterCategory = MonsterCategory.None,
                InteractableCategory = InteractableCategory.Drones,
            };
        }

        private void CharacterBody_onBodyStartGlobal(CharacterBody obj)
        {
            if (!NetworkServer.active) return;
            if (!obj.name.Contains("PsyDroneRed")) return;
            CharacterMaster characterMaster = new MasterSummon
            {
                masterPrefab = droneMasterGreen,
                position = obj.transform.position + Vector3.up,
                rotation = obj.transform.rotation,
                summonerBodyObject = obj.master.minionOwnership.ownerMaster.GetBodyObject(),
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
                        temporaryOverlay.originalMaterial = summonDroneMaterial;
                        temporaryOverlay.AddToCharacerModel(component.modelTransform.GetComponent<CharacterModel>());
                    }
                    obj.GetOrAddComponent<Twins>().twin = bodyObject;
                    bodyObject.GetOrAddComponent<Twins>().twin = obj.gameObject;
                }
            }
        }

        private void RigidbodyCollisionListener_OnCollisionEnter(On.EntityStates.Drone.DeathState.RigidbodyCollisionListener.orig_OnCollisionEnter orig,
                                                                 RigidbodyCollisionListener self, Collision collision)
        {
            if (self.deathState.GetType() == typeof(DeathState))
            {
                Twins twinComponent = self.GetComponent<Twins>();
                if (twinComponent)
                {
                    orig(self, collision);
                    UnityObject.Destroy(twinComponent);
                    UnityObject.Destroy(twinComponent.twinTwinComponent);
                }
                else return;
            }
            orig(self, collision);
        }

        internal static bool DebugCheck()
        {
#if DEBUG
            return true;
#else
            return false;
#endif
        }
    }
}