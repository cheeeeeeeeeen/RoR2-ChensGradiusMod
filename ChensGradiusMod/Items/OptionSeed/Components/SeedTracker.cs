using Chen.Helpers.UnityHelpers;
using RoR2;
using RoR2.UI;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;
using SystemObject = System.Object;

namespace Chen.GradiusMod.Items.OptionSeed.Components
{
    /// <summary>
    /// A component attached to a Character Body owning Option Seeds.
    /// The mod handles attaching the component when necessary.
    /// </summary>
    public class SeedTracker : MonoBehaviour
    {
        /// <summary>
        /// Property that stores the current positional angle from the owner.
        /// Useful for determining patterns relative to the Option's angle.
        /// </summary>
        public float currentOptionAngle { get; private set; } = 0f;

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

        internal GameObject leftSeed { get; private set; }
        internal GameObject rightSeed { get; private set; }
        internal SeedBehavior leftBehavior { get; private set; }
        internal SeedBehavior rightBehavior { get; private set; }

        internal float verticalOffsetMultiplier
        {
            get
            {
                if (characterBody) return OptionSeed.instance.GetVerticalOffsetMultiplier(characterBody.name);
                else return OptionSeed.DefaultVerticalOffsetMultiplier;
            }
        }

        internal float horizontalOffsetMultiplier
        {
            get
            {
                if (characterBody) return OptionSeed.instance.GetHorizontalOffsetMultiplier(characterBody.name);
                else return OptionSeed.DefaultHorizontalOffsetMultiplier;
            }
        }

        internal float distanceAxis
        {
            get
            {
                if (characterBody) return OptionSeed.instance.GetRadius(characterBody.name);
                else return OptionSeed.DefaultRotationRadius;
            }
        }

        internal float rotateSeedAngleSpeed { get => OptionSeed.instance.rotationSpeed; }
        internal float seedLookRate { get; private set; } = .1f;
        internal float positionSmoothRate { get; private set; } = .3f;

        private float invalidCheckTimer = 0f;

        private const float InvalidThreshold = 3f;

        private void Awake()
        {
            leftSeed = Instantiate(OptionSeed.optionSeedPrefab, transform.position, transform.rotation);
            leftBehavior = InitializeSeed(leftSeed, -1);
            rightSeed = Instantiate(OptionSeed.optionSeedPrefab, transform.position, transform.rotation);
            rightBehavior = InitializeSeed(rightSeed, 1);

            AkSoundEngine.PostEvent(OptionSeed.GetOptionEventId, gameObject);
            Log.Message($"SeedTracker.Awake: 2 new Option Seeds for {gameObject.name}.");
        }

        private void FixedUpdate()
        {
            if (invalidCheckTimer >= InvalidThreshold)
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
            if (PauseScreenController.paused) return;
            currentOptionAngle += rotateSeedAngleSpeed;
            if (currentOptionAngle >= 360f) currentOptionAngle = 360f - currentOptionAngle;
        }

        private void OnDestroy()
        {
            Destroy(leftSeed);
            Destroy(rightSeed);
            AkSoundEngine.PostEvent(OptionSeed.LoseOptionEventId, gameObject);
            Log.Message($"SeedTracker.OnDestroy: Destroying Seeds of {gameObject.name}.");
        }

        private SeedBehavior InitializeSeed(GameObject seed, int numbering)
        {
            SeedBehavior seedBehavior = seed.GetComponent<SeedBehavior>();
            seedBehavior.owner = gameObject;
            seedBehavior.numbering = numbering;
            return seedBehavior;
        }

        private void IncreaseInvalidity(SystemObject obj)
        {
            Log.Warning(obj);
            invalidCheckTimer += Time.fixedDeltaTime;
        }

        internal static SeedTracker SpawnSeeds(GameObject owner) => owner.GetOrAddComponent<SeedTracker>();

        internal static void DestroySeeds(GameObject owner)
        {
            SeedTracker tracker = owner.GetComponent<SeedTracker>();
            if (tracker) Destroy(tracker);
        }
    }
}