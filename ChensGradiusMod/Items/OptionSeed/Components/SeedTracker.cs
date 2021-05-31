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
        internal float distanceAxis { get; private set; } = 1.1f;
        internal float rotateSeedAngleSpeed { get; private set; } = 4f;
        internal float seedLookRate { get; private set; } = .15f;

        private float currentOptionAngle = 0f;
        private float invalidCheckTimer = 0f;

        private const float invalidThreshold = 3f;

        private void Awake()
        {
            leftSeed = Instantiate(OptionSeed.optionSeedPrefab, transform.position, transform.rotation);
            rightSeed = Instantiate(OptionSeed.optionSeedPrefab, transform.position, transform.rotation);
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
            Vector3 newPosition = DecidePosition(currentOptionAngle) * distanceAxis;
            newPosition = transform.position + inputBankTest.aimDirection - transform.right + newPosition;
            MoveSeed(leftSeed, newPosition);
        }

        private void MoveRightSeed()
        {
            Vector3 newPosition = DecidePosition(-currentOptionAngle) * distanceAxis;
            newPosition = transform.position + inputBankTest.aimDirection + transform.right + newPosition;
            MoveSeed(rightSeed, newPosition);
        }

        private Vector3 DecidePosition(float baseAngle)
        {
            Vector3 relativePosition = Quaternion.AngleAxis(baseAngle, (inputBankTest.aimDirection + Vector3.up).normalized) * -inputBankTest.aimDirection;
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