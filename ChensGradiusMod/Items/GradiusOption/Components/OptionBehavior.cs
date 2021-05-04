using Chen.Helpers.MathHelpers;
using RoR2;
using RoR2.UI;
using System.Collections.Generic;
using UnityEngine;
using static Chen.GradiusMod.GradiusModPlugin;
using UnityObject = UnityEngine.Object;

namespace Chen.GradiusMod.Items.GradiusOption.Components
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

        /// <summary>
        /// Useful for storing prefabs, components, scriptable objects that needs to be saved from one state to another of the owner.
        /// Utilizing this means that one does not need to create and attach a component for storing these objects.
        /// Using this dictionary allows objects here to be checked using the Unity way.
        /// </summary>
        public Dictionary<string, UnityObject> unityData = new Dictionary<string, UnityObject>();

        /// <summary>
        /// Shorthand for the data dictionary.
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
        /// Shorthand for the data dictionary.
        /// </summary>
        public Dictionary<string, object> O
        {
            get => objectData;
            private set => objectData = value;
        }

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

        private void FixedUpdate()
        {
            if (PauseScreenController.paused) return;
            if (init && owner)
            {
                init = false;
                ownerT = owner.transform;
                ownerIbt = owner.GetComponent<InputBankTest>();
                ownerOt = owner.GetComponent<OptionTracker>();
                ownerBody = owner.GetComponent<CharacterBody>();
                ownerMaster = ownerBody.master;
            }
            else if (!init)
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
                    Log.Warning($"OptionBehavior.FixedUpdate: Lost owner or one of its components. Destroying this Option. numbering = {numbering}");
                    Destroy(gameObject);
                }
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
}