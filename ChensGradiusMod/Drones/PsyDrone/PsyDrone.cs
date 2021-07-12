#undef DEBUG

using Chen.Helpers.CollectionHelpers;
using Chen.Helpers.GeneralHelpers;
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
using static R2API.DirectorAPI;
using static RoR2.CharacterAI.AISkillDriver;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class PsyDrone : Drone<PsyDrone>
    {
        public const uint HitEffectEventId = 1227075968;

        public int spawnWeight { get; private set; } = 1;
        public bool hitSoundEffect { get; private set; } = true;

        public override bool canHaveOptions => true;

        public static InteractableSpawnCard iSpawnCard { get; private set; }
        public static GameObject brokenObject { get; private set; }
        public static DirectorCardHolder iDirectorCardHolder { get; private set; }
        public static DirectorCardHolder iHeavyDirectorCardHolder { get; private set; }
        public static GameObject droneBodyRed { get; private set; }
        public static GameObject droneMasterRed { get; private set; }
        public static GameObject droneBodyGreen { get; private set; }
        public static GameObject droneMasterGreen { get; private set; }
        public static GameObject mirrorLaserPrefab { get; private set; }
        public static GameObject searchLaserPrefab { get; private set; }
        public static GameObject mirrorLaserBodyEffect { get; private set; }
        public static GameObject searchLaserSubPrefab { get; private set; }
        public static GameObject searchLaserSubExplosion { get; private set; }
        public static GameObject mirrorLaserHitEffect { get; private set; }
        public static GameObject searchLaserHitEffect { get; private set; }
        public static GameObject mirrorLaserMuzzleEffect { get; private set; }
        public static GameObject searchLaserMuzzleEffect { get; private set; }

        protected override GameObject DroneCharacterMasterObject => droneMasterRed;

        protected override void SetupConfig()
        {
            spawnWeightWithMachinesArtifact = 0;
            base.SetupConfig();

            spawnWeight = config.Bind(configCategory,
                "SpawnWeight", spawnWeight,
                "The weight for which the Director is biased towards spawning this drone."
            ).Value;
            hitSoundEffect = config.Bind(configCategory,
                "HitSoundEffect", hitSoundEffect,
                "Allow playing of a sound effect when victims are hit by the drones' attacks."
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
            brokenObject.ReplaceModel(customBrokenModel, DebugCheck());
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
            DestroySkillDrivers(droneMasterRed);
            DestroySkillDrivers(droneMasterGreen);
            AISkillDriver skillDriver = droneMasterRed.AddComponent<AISkillDriver>();
            skillDriver.customName = "Attack";
            skillDriver.skillSlot = SkillSlot.Primary;
            skillDriver.moveTargetType = TargetType.CurrentLeader;
            skillDriver.movementType = MovementType.ChaseMoveTarget;
            skillDriver.aimType = AimType.AtCurrentEnemy;
            skillDriver.buttonPressType = ButtonPressType.Hold;
            skillDriver = droneMasterRed.AddComponent<AISkillDriver>();
            skillDriver.customName = "LeashToLeader";
            skillDriver.moveTargetType = TargetType.CurrentLeader;
            skillDriver.movementType = MovementType.ChaseMoveTarget;
            skillDriver.aimType = AimType.AtCurrentEnemy;
            skillDriver.buttonPressType = ButtonPressType.Hold;
            skillDriver.resetCurrentEnemyOnNextDriverSelection = true;
            skillDriver.minDistance = 20f;
            skillDriver = droneMasterRed.AddComponent<AISkillDriver>();
            skillDriver.customName = "Standby";
            skillDriver.maxDistance = 20f;
            skillDriver.moveTargetType = TargetType.CurrentLeader;
            skillDriver.movementType = MovementType.StrafeMovetarget;
            skillDriver.aimType = AimType.AtCurrentEnemy;
            skillDriver.buttonPressType = ButtonPressType.Hold;
            skillDriver = droneMasterRed.AddComponent<AISkillDriver>();
            skillDriver.customName = "RoamAttack";
            skillDriver.skillSlot = SkillSlot.Primary;
            skillDriver.moveTargetType = TargetType.CurrentEnemy;
            skillDriver.movementType = MovementType.ChaseMoveTarget;
            skillDriver.aimType = AimType.AtCurrentEnemy;
            skillDriver.buttonPressType = ButtonPressType.Hold;
            skillDriver = droneMasterRed.AddComponent<AISkillDriver>();
            skillDriver.customName = "RoamMove";
            skillDriver.moveTargetType = TargetType.CurrentEnemy;
            skillDriver.movementType = MovementType.ChaseMoveTarget;
            skillDriver.aimType = AimType.AtCurrentEnemy;
            skillDriver.buttonPressType = ButtonPressType.Hold;
            skillDriver.resetCurrentEnemyOnNextDriverSelection = true;
            droneMasterGreen.DeepCopyComponentsFrom<AISkillDriver>(droneMasterRed);
        }

        private void ModifyDroneBody()
        {
            CharacterBody bodyRed = droneBodyRed.GetComponent<CharacterBody>();
            float baseDamage = bodyRed.baseDamage;
            float levelDamage = bodyRed.levelDamage;
            bodyRed.baseNameToken = "PSI_BIT_NAME";
            bodyRed.baseMoveSpeed *= 1.3f;
            bodyRed.baseAcceleration *= 4f;
            bodyRed.baseMaxHealth *= 1.2f;
            bodyRed.baseRegen *= 1.2f;
            bodyRed.baseCrit = 0f;
            bodyRed.baseDamage = baseDamage * .7f;
            bodyRed.levelMaxHealth *= 1.2f;
            bodyRed.levelRegen *= 1.2f;
            bodyRed.levelCrit = 0f;
            bodyRed.levelMoveSpeed *= 1.3f;
            bodyRed.levelDamage = levelDamage * .7f;
            bodyRed.portraitIcon = assetBundle.LoadAsset<Texture>("Assets/Drones/PsiBits/Icon/PsiBitRed.png");
            CharacterBody bodyGreen = droneBodyGreen.GetComponent<CharacterBody>();
            bodyGreen.baseNameToken = bodyRed.baseNameToken;
            bodyGreen.baseMoveSpeed = bodyRed.baseMoveSpeed;
            bodyGreen.baseAcceleration = bodyRed.baseAcceleration;
            bodyGreen.baseMaxHealth = bodyRed.baseMaxHealth;
            bodyGreen.baseRegen = bodyRed.baseRegen;
            bodyGreen.baseCrit = bodyRed.baseCrit;
            bodyGreen.baseDamage = baseDamage * 1.4f;
            bodyGreen.levelMaxHealth = bodyRed.levelMaxHealth;
            bodyGreen.levelRegen = bodyRed.levelRegen;
            bodyGreen.levelCrit = bodyRed.levelCrit;
            bodyGreen.levelMoveSpeed = bodyRed.levelMoveSpeed;
            bodyGreen.levelDamage = levelDamage * 1.4f;
            bodyGreen.portraitIcon = assetBundle.LoadAsset<Texture>("Assets/Drones/PsiBits/Icon/PsiBitGreen.png");
            ModifyDroneModel(bodyRed, bodyGreen);
            ModifySkill();
            InitializeEffects();
            CharacterDeathBehavior death = bodyRed.GetOrAddComponent<CharacterDeathBehavior>();
            death.deathState = new SerializableEntityStateType(typeof(DeathState));
            death = bodyGreen.GetOrAddComponent<CharacterDeathBehavior>();
            death.deathState = new SerializableEntityStateType(typeof(DeathState));
        }

        private void ModifyDroneModel(CharacterBody bodyRed, CharacterBody bodyGreen)
        {
            GameObject customModel = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/mdlPsiBitRed.prefab");
            droneBodyRed.ReplaceModel(customModel, DebugCheck());
            customModel.transform.localRotation = Util.QuaternionSafeLookRotation(Vector3.left);
            customModel.InitializeDroneModelComponents(bodyRed, 3f);
            customModel.transform.Find("Core").gameObject.AddComponent<CoreFlicker>();
            BodyRotation rotationComponent = customModel.transform.parent.gameObject.AddComponent<BodyRotation>();
            rotationComponent.rotationDirection = -1;
            rotationComponent.rotationSpeed = 6f;
            customModel = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/mdlPsiBitGreen.prefab");
            droneBodyGreen.ReplaceModel(customModel, DebugCheck());
            customModel.transform.localRotation = Util.QuaternionSafeLookRotation(Vector3.left);
            customModel.InitializeDroneModelComponents(bodyGreen, 3f);
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
        }

        private void InitializeEffects()
        {
            mirrorLaserPrefab = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/MirrorLaser.prefab");
            mirrorLaserPrefab.GetOrAddComponent<NetworkIdentity>();
            searchLaserPrefab = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/SearchLaser.prefab");
            searchLaserPrefab.GetOrAddComponent<NetworkIdentity>();
            searchLaserPrefab.transform.Find("Sphere").Find("Point Light").gameObject.AddComponent<SearchLaserBallFlicker>();
            mirrorLaserBodyEffect = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/MirrorLaserBodyEffect.prefab");
            searchLaserSubPrefab = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/SearchLaserSub.prefab");
            searchLaserSubExplosion = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/SearchLaserSubExplosion.prefab");
            searchLaserSubExplosion.GetOrAddComponent<TemporaryParticleSystemWithSound>();
            searchLaserHitEffect = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/SearchLaserHitEffect.prefab");
            searchLaserHitEffect.GetOrAddComponent<TemporaryParticleSystem>();
            mirrorLaserHitEffect = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/MirrorLaserHitEffect.prefab");
            mirrorLaserHitEffect.GetOrAddComponent<TemporaryParticleSystem>();
            mirrorLaserMuzzleEffect = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/MirrorLaserMuzzleEffect.prefab");
            searchLaserMuzzleEffect = assetBundle.LoadAsset<GameObject>("Assets/Drones/PsiBits/Model/SearchLaserMuzzleEffect.prefab");
        }

        private void ModifyInteractableSpawnCard()
        {
            iSpawnCard.name = $"iscBroken{name}";
            iSpawnCard.prefab = brokenObject;
            iSpawnCard.slightlyRandomizeOrientation = false;
            iSpawnCard.orientToFloor = true;
        }

        private void DestroySkillDrivers(GameObject masterObject)
        {
            AISkillDriver[] skillDrivers = masterObject.GetComponents<AISkillDriver>();
            foreach (var skillDriver in skillDrivers)
            {
                Object.DestroyImmediate(skillDriver);
            }
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
            if (!NetworkServer.active || obj.isPlayerControlled || !obj.name.Contains("PsyDroneRed")) return;
            obj.master.GetOrAddComponent<TwinsSpawn>();
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