using Chen.Helpers.MathHelpers;
using Chen.Helpers.UnityHelpers;
using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.GradiusModPlugin;
using static Chen.GradiusMod.SyncOptionTargetForClients;
using static Chen.Helpers.MathHelpers.Wave;
using Object = System.Object;

namespace Chen.GradiusMod
{
    /// <summary>
    /// A component attached to the Options/Multiples for their behavioral functions.
    /// </summary>
    public class OptionBehavior : MonoBehaviour
    {
        /// <summary>
        /// The Character Body Game Object of this Option's owner.
        /// </summary>
        public GameObject owner;

        /// <summary>
        /// The number that represents the identification of the Option scoped under the owner.
        /// </summary>
        public int numbering = 0;

        internal GameObject flamethrower;
        internal HealBeamController healBeamController;
        internal GameObject laserChargeEffect;
        internal GameObject laserFire;
        internal ChildLocator laserChildLocator;
        internal Transform laserFireEnd;
        internal GameObject fistChargeEffect;
        internal GameObject sunderEffect;
        internal GameObject target;
        internal OptionTracker ownerOt;

        private Transform t;
        private Transform ownerT;
        private InputBankTest ownerIbt;
        private CharacterMaster ownerMaster;
        private CharacterBody ownerBody;
        private bool init = true;

        private void Awake()
        {
            t = gameObject.transform;
        }

        private void Update()
        {
            if (!PauseScreenController.paused && !init)
            {
                if (owner && ownerBody && ownerMaster && ownerOt)
                {
                    if (ownerOt.IsRotateUser())
                    {
                        Vector3 newPosition = DecidePosition(ownerOt.currentOptionAngle) * ownerOt.distanceAxis * ownerOt.GetRotateMultiplier();
                        newPosition = ownerT.position + ownerOt.GetRotateOffset() + newPosition;
                        t.position = Vector3.Lerp(t.position, newPosition, ownerOt.optionLookRate);
                    }
                    else t.position = ownerOt.flightPath[numbering * ownerOt.distanceInterval - 1];
                    if (GradiusOption.instance.includeModelInsideOrb)
                    {
                        Vector3 direction;
                        if (target) direction = (target.transform.position - t.position).normalized;
                        else if (ownerIbt) direction = ownerIbt.aimDirection;
                        else direction = (ownerT.position - t.position).normalized;
                        t.rotation = Quaternion.Lerp(t.rotation, Util.QuaternionSafeLookRotation(direction), ownerOt.optionLookRate);
                    }
                }
                else
                {
                    Log.Warning($"OptionBehavior.Update: Lost owner or one of its components. Destroying this Option. numbering = {numbering}");
                    Destroy(gameObject);
                }
            }
        }

        private void FixedUpdate()
        {
            if (!PauseScreenController.paused && init && owner)
            {
                init = false;
                ownerT = owner.transform;
                ownerIbt = owner.GetComponent<InputBankTest>();
                ownerOt = owner.GetComponent<OptionTracker>();
                ownerBody = owner.GetComponent<CharacterBody>();
                ownerMaster = ownerBody.master;
            }
        }

        /// <summary>
        /// Computes for the actual position of the Option based on the owner's rotational variables and its numbering.
        /// </summary>
        /// <param name="baseAngle">The angle to compute from</param>
        /// <returns>Normalized position</returns>
        public Vector3 DecidePosition(float baseAngle)
        {
            Vector3 relativePosition = Quaternion.AngleAxis(baseAngle, ownerT.up) * -ownerT.forward;
            float angleDifference = 360f.SafeDivide(ownerOt.existingOptions.Count);
            relativePosition = Quaternion.AngleAxis(angleDifference * numbering, ownerT.up) * relativePosition;
            return relativePosition.normalized;
        }
    }

    /// <summary>
    /// A component attached to a Character Body that may own Options/Multiples.
    /// The mod handles attaching the component when necessary.
    /// </summary>
    public class OptionTracker : MonoBehaviour
    {
        /// <summary>
        /// Property that stores the current positional angle from the owner.
        /// Useful for determining patterns relative to the Option's angle.
        /// </summary>
        public float currentOptionAngle { get; private set; } = 0f;

        /// <summary>
        /// The Character Master of this Game Object's owner through Minion Ownership component.
        /// </summary>
        public CharacterMaster masterCharacterMaster { get; private set; }

        /// <summary>
        /// Character Master of this Game Object.
        /// </summary>
        public CharacterMaster characterMaster { get; private set; }

        /// <summary>
        /// Character Body of this Game Object.
        /// </summary>
        public CharacterBody characterBody { get; private set; }

        internal List<Vector3> flightPath { get; private set; } = new List<Vector3>();
        internal List<GameObject> existingOptions { get; private set; } = new List<GameObject>();
        internal int distanceInterval { get; private set; } = 20;
        internal float distanceAxis { get; private set; } = 1.2f;
        internal float rotateOptionAngleSpeed { get; private set; } = 2f;
        internal float optionLookRate { get; private set; } = .15f;
        internal OptionMasterTracker masterOptionTracker { get; private set; }

        internal List<Tuple<GameObjectType, NetworkInstanceId, short, NetworkInstanceId>> targetIds { get; private set; } =
            new List<Tuple<GameObjectType, NetworkInstanceId, short, NetworkInstanceId>>();

        private Vector3 previousPosition = new Vector3();
        private bool init = true;
        private int previousOptionItemCount = 0;
        private bool? isRotate = null;
        private float rotateMultiplier = 0f;
        private Vector3? rotateOffset = null;
        private float invalidCheckTimer = 0f;

        private Transform t;

        private const float invalidThreshold = 3f;

        private void Awake()
        {
            t = gameObject.transform;
        }

        private void Update()
        {
            if (PauseScreenController.paused) return;
            if (!init && masterOptionTracker && characterMaster)
            {
                if (IsRotateUser() && masterOptionTracker.optionItemCount > 0)
                {
                    if (masterOptionTracker.optionItemCount % 2 == 0) currentOptionAngle += rotateOptionAngleSpeed;
                    else currentOptionAngle -= rotateOptionAngleSpeed;
                    if (currentOptionAngle >= 360f) currentOptionAngle = 360f - currentOptionAngle;
                    else if (currentOptionAngle <= 0f) currentOptionAngle += 360f;
                }
                else if (previousPosition != t.position)
                {
                    flightPath.Insert(0, t.position);
                    if (flightPath.Count > masterOptionTracker.optionItemCount * distanceInterval) flightPath.RemoveAt(flightPath.Count - 1);
                }
                previousPosition = t.position;
            }
        }

        private void FixedUpdate()
        {
            if (PauseScreenController.paused) return;
            if (invalidCheckTimer >= invalidThreshold)
            {
                Log.Warning($"Invalid OptionTracker: Cannot find the values through the threshold time. Destroying the tracker from GameObject {gameObject.name}.");
                Destroy(this);
                return;
            }
            if (!masterOptionTracker)
            {
                characterBody = gameObject.GetComponent<CharacterBody>();
                if (!characterBody)
                {
                    IncreaseInvalidity("OptionTracker Initialization: characterBody does not exist!");
                    return;
                }
                characterMaster = characterBody.master;
                if (!characterMaster)
                {
                    IncreaseInvalidity("OptionTracker Initialization: characterMaster does not exist!");
                    return;
                }
                masterCharacterMaster = characterMaster.minionOwnership.ownerMaster;
                if (!masterCharacterMaster)
                {
                    IncreaseInvalidity("OptionTracker Initialization: masterCharacterMaster does not exist!");
                    return;
                }
                masterOptionTracker = masterCharacterMaster.gameObject.GetComponent<OptionMasterTracker>();
                if (!masterOptionTracker)
                {
                    IncreaseInvalidity("OptionTracker Initialization: masterOptionTracker is null.");
                    return;
                }
            }
            if (invalidCheckTimer > 0f) invalidCheckTimer = 0f;
            if (init && masterOptionTracker.optionItemCount > 0)
            {
                init = false;
                previousPosition = t.position;
                ManageFlightPath(masterOptionTracker.optionItemCount);
            }
            else if (!init && masterOptionTracker.optionItemCount > 0)
            {
                int diff = masterOptionTracker.optionItemCount - previousOptionItemCount;
                if (diff > 0 || diff < 0)
                {
                    previousOptionItemCount = masterOptionTracker.optionItemCount;
                    ManageFlightPath(diff);
                }
            }
            else if (!init && masterOptionTracker.optionItemCount <= 0)
            {
                init = true;
                flightPath.Clear();
                previousOptionItemCount = 0;
            }
            SyncTargets();
        }

        private void SyncTargets()
        {
            if (GradiusOption.instance.includeModelInsideOrb && NetworkServer.active && NetworkUser.AllParticipatingNetworkUsersReady() && targetIds.Count > 0)
            {
                Tuple<GameObjectType, NetworkInstanceId, short, NetworkInstanceId>[] listCopy = new Tuple<GameObjectType, NetworkInstanceId, short, NetworkInstanceId>[targetIds.Count];
                targetIds.CopyTo(listCopy);
                targetIds.Clear();
                for (int i = 0; i < listCopy.Length; i++)
                {
                    GameObjectType gameObjectType = listCopy[i].Item1;
                    NetworkInstanceId netId = listCopy[i].Item2;
                    short numbering = listCopy[i].Item3;
                    NetworkInstanceId targetId = listCopy[i].Item4;
                    new SyncOptionTargetForClients(gameObjectType, netId, numbering, targetId).Send(NetworkDestination.Clients);
                }
            }
        }

        private void ManageFlightPath(int difference)
        {
            if (IsRotateUser()) return;
            int flightPathCap = masterOptionTracker.optionItemCount * distanceInterval;
            if (difference > 0) while (flightPath.Count < flightPathCap) flightPath.Add(t.position);
            else if (difference < 0) while (flightPath.Count >= flightPathCap) flightPath.RemoveAt(flightPath.Count - 1);
        }

        private void IncreaseInvalidity(Object obj)
        {
            Log.Warning(obj);
            invalidCheckTimer += Time.fixedDeltaTime;
        }

        internal bool IsRotateUser()
        {
            if (isRotate == null) isRotate = GradiusOption.instance.IsRotateUser(characterMaster.name);
            return (bool)isRotate;
        }

        /// <summary>
        /// Fetches the rotational distance and speed multiplier for this object's Options.
        /// </summary>
        /// <returns>Rotational speed and distance multiplier</returns>
        public float GetRotateMultiplier()
        {
            if (rotateMultiplier <= 0f) rotateMultiplier = GradiusOption.instance.GetRotateMultiplier(characterMaster.name);
            return rotateMultiplier;
        }

        /// <summary>
        /// Fetches the rotational central offset for this object's Options.
        /// </summary>
        /// <returns>Offset</returns>
        public Vector3 GetRotateOffset()
        {
            if (rotateOffset == null) rotateOffset = GradiusOption.instance.GetRotateOffset(characterMaster.name);
            return (Vector3)rotateOffset;
        }
    }

    internal class OptionMasterTracker : MonoBehaviour
    {
        public int optionItemCount = 0;

        public static void SpawnOption(GameObject owner, int itemCount)
        {
            OptionTracker ownerOptionTracker = owner.GetOrAddComponent<OptionTracker>();
            GameObject option = Instantiate(GradiusOption.gradiusOptionPrefab, owner.transform.position, owner.transform.rotation);
            OptionBehavior behavior = option.GetComponent<OptionBehavior>();
            behavior.owner = owner;
            behavior.numbering = itemCount;
            ownerOptionTracker.existingOptions.Add(option);
        }

        public static void DestroyOption(OptionTracker optionTracker, int optionNumber)
        {
            int index = optionNumber - 1;
            GameObject option = optionTracker.existingOptions[index];
            optionTracker.existingOptions.RemoveAt(index);
            Destroy(option);
        }
    }

    internal class Flicker : MonoBehaviour
    {
        // Child Objects in Order:
        // 0. sphere1:     Light
        // 1. sphere2:     Light
        // 2. sphere3:     Light
        // 3. sphere4:     MeshRenderer, MeshFilter (only in OptionOrb)
        // 4. sphere5:     MeshRenderer, MeshFilter (only in OptionOrbWithModel)
        // 5. OptionModel: The option model (only in OptionOrbWithModel)

        private readonly float baseValue = 1f;
        private readonly float amplitude = .25f;
        private readonly float phase = 0f;
        private readonly float frequency = 1f;

        private readonly Light[] lightObjects = new Light[3];
        private readonly float[] originalRange = new float[3];
        private readonly float[] ampMultiplier = new float[4] { 1.2f, 1f, .8f, .4f };
        private Vector3 originalLocalScale;
        private GameObject meshObject;

        private void Awake()
        {
            for (int i = 0; i < transform.childCount; i++)
            {
                GameObject child = transform.GetChild(i).gameObject;
                Light childLight = child.GetComponent<Light>();
                switch (child.name)
                {
                    case "sphere1":
                        originalRange[0] = childLight.range;
                        lightObjects[0] = childLight;
                        break;

                    case "sphere2":
                        originalRange[1] = childLight.range;
                        lightObjects[1] = childLight;
                        break;

                    case "sphere3":
                        originalRange[2] = childLight.range;
                        lightObjects[2] = childLight;
                        break;

                    case "sphere4":
                        originalLocalScale = child.transform.localScale;
                        meshObject = child;
                        break;

                    case "sphere5":
                        child.transform.localScale *= 1.5f;
                        break;

                    case "option":
                        child.transform.localScale = new Vector3(2f, 2f, 2f);
                        break;
                }
            }
        }

        private void Update()
        {
            if (PauseScreenController.paused) return;
            for (int i = 0; i < lightObjects.Length; i++)
            {
                lightObjects[i].range = originalRange[i] * Sine(phase, frequency, amplitude * ampMultiplier[i], baseValue);
            }
            if (meshObject && originalLocalScale != null)
            {
                meshObject.transform.localScale = originalLocalScale * Sine(phase, frequency, amplitude * ampMultiplier[3], baseValue);
            }
        }
    }
}