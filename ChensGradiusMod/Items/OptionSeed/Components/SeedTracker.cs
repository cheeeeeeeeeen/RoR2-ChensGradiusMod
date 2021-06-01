using Chen.Helpers.UnityHelpers;
using RoR2;
using RoR2.UI;
using System.Collections.Generic;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;
using SystemObject = System.Object;
using UnityObject = UnityEngine.Object;

namespace Chen.GradiusMod.Items.OptionSeed.Components
{
    /// <summary>
    /// A component attached to a Character Body owning Option Seeds.
    /// The mod handles attaching the component when necessary.
    /// </summary>
    public class SeedTracker : MonoBehaviour
    {
        /// <summary>
        /// Useful for storing prefabs, components, scriptable objects that needs to be saved from one state to another of the owner.
        /// Utilizing this means that one does not need to create and attach a component for storing these objects.
        /// Using this dictionary allows objects here to be checked using the Unity way. The dictionary is separated for each seed.
        /// e.g. unityData[seedObject]["myEffect"] is the way to access data from a seed.
        /// </summary>
        public Dictionary<GameObject, Dictionary<string, UnityObject>> unityData = new Dictionary<GameObject, Dictionary<string, UnityObject>>();

        /// <summary>
        /// Shorthand for the Unity data dictionary.
        /// </summary>
        public Dictionary<GameObject, Dictionary<string, UnityObject>> U
        {
            get => unityData;
            private set => unityData = value;
        }

        /// <summary>
        /// Useful for native objects or class instances that needs to be saved from one state to another of the owner.
        /// Utilizing this means that one does not need to create and attach a component for storing these objects.
        /// Casting is required when the object is accessed. The dictionary is separated for each seed.
        /// e.g. objectData[seedObject]["myEffect"] is the way to access data from a seed.
        /// </summary>
        public Dictionary<GameObject, Dictionary<string, object>> objectData = new Dictionary<GameObject, Dictionary<string, object>>();

        /// <summary>
        /// Shorthand for the Object data dictionary.
        /// </summary>
        public Dictionary<GameObject, Dictionary<string, object>> O
        {
            get => objectData;
            private set => objectData = value;
        }

        /// <summary>
        /// Character Master of this Game Object.
        /// </summary>
        public CharacterMaster characterMaster { get; private set; }

        /// <summary>
        /// Character Body of this Game Object.
        /// </summary>
        public CharacterBody characterBody { get; private set; }

        /// <summary>
        /// Input Bank Test of this Game Object.
        /// </summary>
        public InputBankTest inputBankTest { get; private set; }

        internal int attackCounter { get; set; } = 0;
        internal GameObject leftSeed { get; private set; }
        internal GameObject rightSeed { get; private set; }
        internal float distanceAxis { get; private set; } = .4f;
        internal float rotateSeedAngleSpeed { get; private set; } = 10f;
        internal float seedLookRate { get; private set; } = .5f;

        private float currentOptionAngle = 0f;
        private float invalidCheckTimer = 0f;

        private const float invalidThreshold = 3f;

        private void Awake()
        {
            leftSeed = Instantiate(OptionSeed.optionSeedPrefab, transform.position, transform.rotation);
            rightSeed = Instantiate(OptionSeed.optionSeedPrefab, transform.position, transform.rotation);
            O[leftSeed] = new Dictionary<string, object>();
            U[leftSeed] = new Dictionary<string, UnityObject>();
            O[rightSeed] = new Dictionary<string, object>();
            U[rightSeed] = new Dictionary<string, UnityObject>();
            AkSoundEngine.PostEvent(OptionSeed.getOptionEventId, gameObject);
            Log.Message($"SeedTracker.Awake: 2 new Option Seeds for {gameObject.name}.");
        }

        private void FixedUpdate()
        {
            if (PauseScreenController.paused) return;
            if (invalidCheckTimer >= invalidThreshold)
            {
                Log.Warning($"Invalid SeedTracker: Cannot find the values through the threshold time. Destroying the tracker from GameObject {gameObject.name}.");
                Destroy(this);
                return;
            }
            if (!characterMaster || !inputBankTest)
            {
                characterBody = gameObject.GetComponent<CharacterBody>();
                if (!characterBody)
                {
                    IncreaseInvalidity("SeedTracker Initialization: characterBody does not exist!");
                    return;
                }
                characterMaster = characterBody.master;
                if (!characterMaster)
                {
                    IncreaseInvalidity("SeedTracker Initialization: characterMaster does not exist!");
                    return;
                }
                inputBankTest = gameObject.GetComponent<InputBankTest>();
                if (!inputBankTest)
                {
                    IncreaseInvalidity("SeedTracker Initialization: inputBankTest does not exist!");
                    return;
                }
            }
            if (invalidCheckTimer > 0f) invalidCheckTimer = 0f;
            currentOptionAngle += rotateSeedAngleSpeed;
            if (currentOptionAngle >= 360f) currentOptionAngle = 360f - currentOptionAngle;
            MoveLeftSeed();
            MoveRightSeed();
        }

        private void OnDestroy()
        {
            Destroy(leftSeed);
            Destroy(rightSeed);
            AkSoundEngine.PostEvent(OptionSeed.loseOptionEventId, gameObject);
            Log.Message($"SeedTracker.OnDestroy: Destroying Seeds of {gameObject.name}.");
        }

        private void IncreaseInvalidity(SystemObject obj)
        {
            Log.Warning(obj);
            invalidCheckTimer += Time.fixedDeltaTime;
        }

        private void MoveLeftSeed()
        {
            Vector3 newPosition = DecidePosition(currentOptionAngle, Vector3.up) * distanceAxis;
            newPosition = transform.position - Vector3.Cross(inputBankTest.aimDirection, Vector3.up) + newPosition;
            MoveSeed(leftSeed, newPosition);
        }

        private void MoveRightSeed()
        {
            Vector3 newPosition = DecidePosition(-currentOptionAngle, Vector3.down) * distanceAxis;
            newPosition = transform.position + Vector3.Cross(inputBankTest.aimDirection, Vector3.up) + newPosition;
            MoveSeed(rightSeed, newPosition);
        }

        private Vector3 DecidePosition(float baseAngle, Vector3 axis)
        {
            Vector3 relativePosition = Quaternion.AngleAxis(baseAngle, inputBankTest.aimDirection) * Vector3.Cross(inputBankTest.aimDirection, axis);
            return relativePosition.normalized;
        }

        private void MoveSeed(GameObject seedObject, Vector3 newPosition)
        {
            seedObject.transform.position = Vector3.Lerp(seedObject.transform.position, newPosition, seedLookRate);
            if (OptionSeed.instance.includeModelInsideOrb)
            {
                seedObject.transform.rotation = Quaternion.Lerp(seedObject.transform.rotation, Util.QuaternionSafeLookRotation(inputBankTest.aimDirection), seedLookRate);
            }
        }

        internal static SeedTracker SpawnSeeds(GameObject owner) => owner.GetOrAddComponent<SeedTracker>();

        internal static void DestroySeeds(GameObject owner)
        {
            SeedTracker tracker = owner.GetComponent<SeedTracker>();
            if (tracker) Destroy(tracker);
        }
    }
}