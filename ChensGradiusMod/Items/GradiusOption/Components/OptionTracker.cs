using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using RoR2.UI;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.GradiusModPlugin;
using static Chen.GradiusMod.Items.GradiusOption.SyncOptionTarget;
using SystemObject = System.Object;

namespace Chen.GradiusMod.Items.GradiusOption.Components
{
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
        internal float positionSmoothRate { get; private set; } = .5f;
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

        private void FixedUpdate()
        {
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
            SyncTargets();
            if (init && masterOptionTracker.optionItemCount > 0)
            {
                init = false;
                previousPosition = t.position;
                ManageFlightPath(masterOptionTracker.optionItemCount);
            }
            else if (!init)
            {
                if (masterOptionTracker.optionItemCount > 0)
                {
                    int diff = masterOptionTracker.optionItemCount - previousOptionItemCount;
                    if (diff > 0 || diff < 0)
                    {
                        previousOptionItemCount = masterOptionTracker.optionItemCount;
                        ManageFlightPath(diff);
                    }
                }
                else
                {
                    init = true;
                    flightPath.Clear();
                    previousOptionItemCount = 0;
                }
                if (PauseScreenController.paused) return;
                if (masterOptionTracker && characterMaster)
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
        }

        private void OnDestroy()
        {
            foreach (var option in existingOptions) Destroy(option);
            AkSoundEngine.PostEvent(GradiusOption.loseOptionEventId, gameObject);
            Log.Message($"OptionTracker.OnDestroy: Destroying all Options of {gameObject.name}.");
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
                    new SyncOptionTarget(gameObjectType, netId, numbering, targetId).Send(NetworkDestination.Clients);
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

        private void IncreaseInvalidity(SystemObject obj)
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
}