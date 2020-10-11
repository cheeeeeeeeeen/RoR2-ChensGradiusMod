using R2API.Networking;
using R2API.Networking.Interfaces;
using RoR2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.SpawnOptionsForClients;
using static Chen.GradiusMod.SyncFlamethrowerEffectForClients;

namespace Chen.GradiusMod
{
    public class OptionBehavior : MonoBehaviour
    {
        public GameObject owner;
        public int numbering = 0;
        public GameObject flamethrower;
        public HealBeamController healBeamController;
        public GameObject target;

        private Transform t;
        private Transform ownerT;
        private InputBankTest ownerIbt;
        private OptionTracker ownerOt;
        private Vector3 axis;
        private Quaternion oldRotation;
        private bool init = true;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by UnityEngine")]
        private void Awake()
        {
            t = gameObject.transform;
            axis = Vector3.zero;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by UnityEngine")]
        private void Update()
        {
            if (!init)
            {
                if (owner && ownerOt)
                {
                    oldRotation = t.rotation;
                    if (owner.name.Contains("Turret1"))
                    {
                        t.position = ownerT.position + (t.position - ownerT.position).normalized * DecideDistance();
                        t.RotateAround(ownerT.position, DecideAxis(), ownerOt.rotateOptionSpeed * Time.deltaTime);
                    }
                    else t.position = ownerOt.flightPath[numbering * ownerOt.distanceInterval - 1];
                    if (GradiusOption.instance.includeModelInsideOrb)
                    {
                        Vector3 direction;
                        if (target) direction = (target.transform.position - t.position).normalized;
                        else direction = ownerIbt.aimDirection;
                        t.rotation = Quaternion.Lerp(oldRotation, Util.QuaternionSafeLookRotation(direction), ownerOt.optionLookSpeed);
                    }
                }
                else
                {
                    GradiusModPlugin._logger.LogWarning($"OptionBehavior.Update: Lost owner or ownerOt. Destroying this Option. numbering = {numbering}");
                    Destroy(gameObject);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by UnityEngine")]
        private void FixedUpdate()
        {
            if (init && owner)
            {
                init = false;
                ownerT = owner.transform;
                ownerIbt = owner.GetComponent<InputBankTest>();
                ownerOt = owner.GetComponent<OptionTracker>();
                if (owner.name.Contains("Turret1")) t.position += DecideAxis(true) * DecideDistance();
            }
        }

        private float DecideDistance()
        {
            return Mathf.CeilToInt(numbering / 3f) * ownerOt.distanceAxis;
        }

        private Vector3 DecideAxis(bool positioning = false)
        {
            if ((numbering + 2) % 3 == 0)
            {
                if (numbering % 2 == 0)
                {
                    if (positioning) axis = Vector3.up;
                    else axis = Vector3.forward;
                }
                else
                {
                    if (positioning) axis = Vector3.down;
                    else axis = Vector3.back;
                }
            }
            else if ((numbering + 1) % 3 == 0)
            {
                if (numbering % 2 == 0)
                {
                    if (positioning) axis = Vector3.left;
                    else axis = Vector3.up;
                }
                else
                {
                    if (positioning) axis = Vector3.right;
                    else axis = Vector3.down;
                }
            }
            else if (numbering % 3 == 0)
            {
                if (numbering % 2 == 0)
                {
                    if (positioning) axis = Vector3.forward;
                    else axis = Vector3.left;
                }
                else
                {
                    if (positioning) axis = Vector3.back;
                    else axis = Vector3.right;
                }
            }
            return axis;
        }
    }

    public class OptionTracker : MonoBehaviour
    {
        public List<Vector3> flightPath { get; private set; } = new List<Vector3>();
        public List<GameObject> existingOptions { get; private set; } = new List<GameObject>();
        public int distanceInterval { get; private set; } = 20;
        public float distanceAxis { get; private set; } = .3f;
        public float rotateOptionSpeed { get; private set; } = 200f;
        public float optionLookSpeed { get; private set; } = .15f;
        public CharacterMaster masterCharacterMaster { get; private set; }
        public OptionMasterTracker masterOptionTracker { get; private set; }
        public CharacterMaster characterMaster { get; private set; }
        public CharacterBody characterBody { get; private set; }

        public List<Tuple<MessageType, NetworkInstanceId, short, float, Vector3>> netIds { get; private set; } =
            new List<Tuple<MessageType, NetworkInstanceId, short, float, Vector3>>();
        public List<Tuple<GameObjectType, NetworkInstanceId, short, NetworkInstanceId>> targetIds { get; private set; } =
            new List<Tuple<GameObjectType, NetworkInstanceId, short, NetworkInstanceId>>();

        private Vector3 previousPosition = new Vector3();
        private bool init = true;
        private int previousOptionItemCount = 0;

        private Transform t;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by UnityEngine")]
        private void Awake()
        {
            t = gameObject.transform;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by UnityEngine")]
        private void Update()
        {
            if (!init && masterOptionTracker && characterMaster)
            {
                if (!characterMaster.name.Contains("Turret1") && previousPosition != t.position)
                {
                    flightPath.Insert(0, t.position);
                    if (flightPath.Count > masterOptionTracker.optionItemCount * distanceInterval) flightPath.RemoveAt(flightPath.Count - 1);
                }
                previousPosition = t.position;
            }
            SyncFlamethrowerEffects();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by UnityEngine")]
        private void FixedUpdate()
        {
            if (!masterOptionTracker)
            {
                characterBody = gameObject.GetComponent<CharacterBody>();
                if (!characterBody)
                {
                    GradiusModPlugin._logger.LogWarning("OptionTracker Initialization: characterBody does not exist!");
                    return;
                }
                characterMaster = characterBody.master;
                if (!characterMaster)
                {
                    GradiusModPlugin._logger.LogWarning("OptionTracker Initialization: characterMaster does not exist!");
                    return;
                }
                masterCharacterMaster = characterMaster.minionOwnership.ownerMaster;
                if (!masterCharacterMaster)
                {
                    GradiusModPlugin._logger.LogWarning("OptionTracker Initialization: masterCharacterMaster does not exist!");
                    return;
                }
                masterOptionTracker = masterCharacterMaster.gameObject.GetComponent<OptionMasterTracker>();
                if (!masterOptionTracker)
                {
                    GradiusModPlugin._logger.LogWarning("OptionTracker Initialization: masterOptionTracker is null.");
                    return;
                }
            }
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

        private void SyncFlamethrowerEffects()
        {
            if (NetworkServer.active && NetworkUser.AllParticipatingNetworkUsersReady() && netIds.Count > 0)
            {
                Tuple<MessageType, NetworkInstanceId, short, float, Vector3>[] listCopy = new Tuple<MessageType, NetworkInstanceId, short, float, Vector3>[netIds.Count];
                netIds.CopyTo(listCopy);
                netIds.Clear();
                for (int i = 0; i < listCopy.Length; i++)
                {
                    MessageType messageType = listCopy[i].Item1;
                    NetworkInstanceId netId = listCopy[i].Item2;
                    short numbering = listCopy[i].Item3;
                    float duration = listCopy[i].Item4;
                    Vector3 direction = listCopy[i].Item5;
                    new SyncFlamethrowerEffectForClients(messageType, netId, numbering, duration, direction).Send(NetworkDestination.Clients);
                }
            }
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
            if (characterMaster.name.Contains("Turret1")) return;
            int flightPathCap = masterOptionTracker.optionItemCount * distanceInterval;
            if (difference > 0) while (flightPath.Count < flightPathCap) flightPath.Add(t.position);
            else if (difference < 0) while (flightPath.Count >= flightPathCap) flightPath.RemoveAt(flightPath.Count - 1);
        }

        public static OptionTracker GetOrCreateComponent(GameObject me)
        {
            OptionTracker tracker = me.GetComponent<OptionTracker>() ?? me.AddComponent<OptionTracker>();
            return tracker;
        }

        public static OptionTracker GetOrCreateComponent(CharacterBody me)
        {
            return GetOrCreateComponent(me.gameObject);
        }
    }

    public class OptionMasterTracker : MonoBehaviour
    {
        public int optionItemCount = 0;
        public List<Tuple<GameObjectType, NetworkInstanceId, short>> netIds { get; private set; } = new List<Tuple<GameObjectType, NetworkInstanceId, short>>();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by UnityEngine")]
        private void FixedUpdate()
        {
            if (NetworkServer.active && NetworkUser.AllParticipatingNetworkUsersReady() && netIds.Count > 0)
            {
                Tuple<GameObjectType, NetworkInstanceId, short>[] listCopy = new Tuple<GameObjectType, NetworkInstanceId, short>[netIds.Count];
                netIds.CopyTo(listCopy);
                netIds.Clear();
                for (int i = 0; i < listCopy.Length; i++)
                {
                    GameObjectType bodyOrMaster = listCopy[i].Item1;
                    NetworkInstanceId netId = listCopy[i].Item2;
                    short numbering = listCopy[i].Item3;
                    StartCoroutine(SpawnOptionForClient(bodyOrMaster, netId, numbering));
                }
            }
        }

        private IEnumerator SpawnOptionForClient(GameObjectType bodyOrMaster, NetworkInstanceId netId, short numbering)
        {
            yield return new WaitForSeconds(GradiusOption.instance.spawnSyncSeconds);
            new SpawnOptionsForClients(bodyOrMaster, netId, numbering).Send(NetworkDestination.Clients);
        }

        public static OptionMasterTracker GetOrCreateComponent(CharacterMaster me)
        {
            return GetOrCreateComponent(me.gameObject);
        }

        public static OptionMasterTracker GetOrCreateComponent(GameObject me)
        {
            OptionMasterTracker tracker = me.GetComponent<OptionMasterTracker>() ?? me.AddComponent<OptionMasterTracker>();
            return tracker;
        }

        public static void SpawnOption(GameObject owner, int itemCount)
        {
            OptionTracker ownerOptionTracker = OptionTracker.GetOrCreateComponent(owner);
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

    public class Flicker : MonoBehaviour
    {
        // Child Objects in Order:
        // 0. sphere1:     Light
        // 1. sphere2:     Light
        // 2. sphere3:     Light
        // 3. sphere4:     MeshRenderer, MeshFilter
        // 4. OptionModel: The option model

        private readonly float baseValue = 1f;
        private readonly float amplitude = .25f;
        private readonly float phase = 0f;
        private readonly float frequency = 1f;

        private readonly Light[] lightObjects = new Light[3];
        private readonly float[] originalRange = new float[3];
        private readonly float[] ampMultiplier = new float[4] { 1.2f, 1f, .8f, .4f };
        private Vector3 originalLocalScale;
        private GameObject meshObject;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by UnityEngine")]
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

                    case "OptionModel":
                        child.transform.localScale = new Vector3(.06f, .06f, .06f);
                        child.transform.Rotate(Vector3.up, -90);
                        break;
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Code Quality", "IDE0051:Remove unused private members", Justification = "Used by UnityEngine")]
        private void Update()
        {
            for (int i = 0; i < lightObjects.Length; i++)
            {
                lightObjects[i].range = originalRange[i] * Wave(ampMultiplier[i]);
            }
            meshObject.transform.localScale = originalLocalScale * Wave(ampMultiplier[3]);
        }

        private float Wave(float ampMultiplier)
        {
            float x = (Time.time + phase) * frequency;
            x -= Mathf.Floor(x);
            float y = Mathf.Sin(x * 2 * Mathf.PI);

            return (y * amplitude * ampMultiplier) + baseValue;
        }
    }
}