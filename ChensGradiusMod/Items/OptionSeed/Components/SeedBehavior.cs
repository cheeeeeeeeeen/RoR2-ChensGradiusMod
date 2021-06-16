using RoR2;
using RoR2.UI;
using System.Collections.Generic;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;
using UnityObject = UnityEngine.Object;

namespace Chen.GradiusMod.Items.OptionSeed.Components
{
    /// <summary>
    /// A component attached to the Options/Multiples for their behavioral functions.
    /// </summary>
    public class SeedBehavior : MonoBehaviour
    {
        /// <summary>
        /// The Character Body Game Object of this Option's owner.
        /// </summary>
        public GameObject owner;

        /// <summary>
        /// The number that represents the identification of the Option scoped under the owner.
        /// </summary>
        public int numbering = 0;

        /// <summary>
        /// Useful for storing prefabs, components, scriptable objects that needs to be saved from one state to another of the owner.
        /// Utilizing this means that one does not need to create and attach a component for storing these objects.
        /// Using this dictionary allows objects here to be checked using the Unity way.
        /// </summary>
        public Dictionary<string, UnityObject> unityData = new Dictionary<string, UnityObject>();

        /// <summary>
        /// Shorthand for the Unity data dictionary.
        /// </summary>
        public Dictionary<string, UnityObject> U
        {
            get => unityData;
            private set => unityData = value;
        }

        /// <summary>
        /// Useful for native objects or class instances that needs to be saved from one state to another of the owner.
        /// Utilizing this means that one does not need to create and attach a component for storing these objects.
        /// Casting is required when the object is accessed.
        /// </summary>
        public Dictionary<string, object> objectData = new Dictionary<string, object>();

        /// <summary>
        /// Shorthand for the Object data dictionary.
        /// </summary>
        public Dictionary<string, object> O
        {
            get => objectData;
            private set => objectData = value;
        }

        internal GameObject target;
        internal SeedTracker ownerSt;
        internal CharacterMaster ownerMaster;
        internal CharacterBody ownerBody;

        private Transform t;
        private Transform ownerT;
        private InputBankTest ownerIbt;
        private bool init = true;

        private void Awake()
        {
            t = gameObject.transform;
        }

        private void FixedUpdate()
        {
            if (PauseScreenController.paused) return;
            if (init && owner)
            {
                init = false;
                ownerIbt = owner.GetComponent<InputBankTest>();
                ownerSt = owner.GetComponent<SeedTracker>();
                ownerBody = owner.GetComponent<CharacterBody>();
                ownerMaster = ownerBody.master;
                ownerT = owner.transform;
            }
            else if (!init)
            {
                if (owner && ownerBody && ownerMaster && ownerSt && ownerIbt)
                {
                    t.position = Vector3.Lerp(t.position, DecidePosition(), ownerSt.seedLookRate);
                    if (OptionSeed.instance.includeModelInsideOrb)
                    {
                        t.rotation = Quaternion.Lerp(t.rotation, Util.QuaternionSafeLookRotation(ownerIbt.aimDirection), ownerSt.seedLookRate);
                    }
                }
                else
                {
                    Log.Warning($"SeedBehavior.FixedUpdate: Lost owner or one of its components. Destroying this Option Seed. numbering = {numbering}");
                    Destroy(gameObject);
                }
            }
        }

        private Vector3 DecidePosition()
        {
            Vector3 relativePosition = Vector3.zero;
            if (!OptionSeed.instance.staticPositions)
            {
                relativePosition = Quaternion.AngleAxis(ownerSt.currentOptionAngle * numbering, ownerIbt.aimDirection) * Vector3.Cross(ownerIbt.aimDirection, Vector3.down * numbering);
                relativePosition = (relativePosition.normalized * ownerSt.distanceAxis);
            }
            relativePosition += Util.QuaternionSafeLookRotation(ownerIbt.aimDirection) * (ownerSt.horizontalOffsetMultiplier * numbering * Vector3.right);
            relativePosition += Util.QuaternionSafeLookRotation(ownerIbt.aimDirection) * (ownerSt.verticalOffsetMultiplier * Vector3.up);
            Vector3 newPosition = ownerT.position + relativePosition;
            return newPosition;
        }
    }
}