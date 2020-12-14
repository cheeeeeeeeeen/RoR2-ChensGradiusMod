using Chen.GradiusMod.Items.GradiusOption.Components;
using Chen.Helpers.CollectionHelpers;
using Chen.Helpers.GeneralHelpers;
using RoR2;
using RoR2.CharacterAI;
using System;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.Items.GradiusOption.SyncOptionTarget;

namespace Chen.GradiusMod.Items.GradiusOption
{
    public partial class GradiusOption
    {
        /// <summary>
        /// Adds a support for a minion for them to gain Options.
        /// </summary>
        /// <param name="masterName">The CharacterMaster name of the minion.</param>
        /// <returns>True if the minion is supported. False if it is already supported.</returns>
        public bool SupportMinionType(string masterName)
        {
            return MinionsList.ConditionalAdd(masterName, item => item == masterName);
        }

        /// <summary>
        /// Removes support for a minion so that they do not acquire Options.
        /// </summary>
        /// <param name="masterName">The CharacterMaster name of the minion.</param>
        /// <returns>True if the minion is not supported anymore. False if it is already unsupported.</returns>
        public bool UnsupportMinionType(string masterName)
        {
            return MinionsList.ConditionalRemove(masterName);
        }

        /// <summary>
        /// Lets the minion use Rotate Options.
        /// </summary>
        /// <param name="masterName">The CharacterMaster name of the minion.</param>
        /// <returns>True if the minion is successfully set to use Rotate Options. False if it is already using Rotate Options.</returns>
        public bool SetToRotateOptions(string masterName)
        {
            return RotateUsers.ConditionalAdd(masterName, item => item == masterName);
        }

        /// <summary>
        /// Sets the rotation multiplier for a minion type. This multiplier affects the distance and speed of rotation.
        /// </summary>
        /// <param name="masterName">The CharacterMaster name of the minion.</param>
        /// <param name="newValue">The multiplier value.</param>
        /// <returns>True if the values are set. False if not.</returns>
        public bool SetRotateOptionMultiplier(string masterName, float newValue)
        {
            if (IsRotateUser(masterName))
            {
                RotateMultipliers[masterName] = newValue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Sets the offset center position for a minion type. Options will rotate around the offset.
        /// </summary>
        /// <param name="masterName">The CharacterMaster name of the minion.</param>
        /// <param name="newValue">The offset value.</param>
        /// <returns>True if the values are set. False if not.</returns>
        public bool SetRotateOptionOffset(string masterName, Vector3 newValue)
        {
            if (IsRotateUser(masterName))
            {
                RotateOffsets[masterName] = newValue;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Lets the minion use Regular Options.
        /// </summary>
        /// <param name="masterName">The CharacterMaster name of the minion.</param>
        /// <returns>True if the minion is successfully set to use Regular Options. False if it is already using Regular Options.</returns>
        public bool SetToRegularOptions(string masterName)
        {
            bool result = RotateUsers.ConditionalRemove(masterName);
            if (result)
            {
                RotateMultipliers.Remove(masterName);
                RotateOffsets.Remove(masterName);
            }
            return result;
        }

        /// <summary>
        /// Loops through the all the minions of the owner.
        /// </summary>
        /// <param name="ownerMaster">The owner of the minions.</param>
        /// <param name="actionToRun">An action to execute for each minion. The minion's CharacterBody GameObject is given as the input.</param>
        public void LoopAllMinions(CharacterMaster ownerMaster, Action<GameObject> actionToRun)
        {
            ownerMaster.LoopMinions((minionMaster) =>
            {
                if (FilterMinions(minionMaster))
                {
                    CharacterBody minionBody = minionMaster.GetBody();
                    if (minionBody)
                    {
                        GameObject minion = minionBody.gameObject;
                        actionToRun(minion);
                    }
                }
            });
        }

        /// <summary>
        /// Loops through all the Options of the minion. The action has 4 useful parameters to use.
        /// The first parameter refers to the Option/Multiple itself. It is a GameObject.
        /// The second parameter refers to the OptionBehavior component in the Option/Multiple.
        /// The third parameter refers to the target of the Option/Multiple owner. It is also a GameObject.
        /// The last parameter refers to the direction from the option to the target. It is a normalized Vector3.
        /// </summary>
        /// <param name="optionOwner">The owner of the option.</param>
        /// <param name="actionToRun">An action to execute for each Option. The inputs are as follows:
        /// GameObject option, OptionBehavior behavior, GameObject target, Vector3 direction.</param>
        public void FireForAllOptions(CharacterBody optionOwner, Action<GameObject, OptionBehavior, GameObject, Vector3> actionToRun)
        {
            if (!optionOwner) return;
            OptionTracker optionTracker = optionOwner.GetComponent<OptionTracker>();
            if (!optionTracker) return;

            GameObject target = null;
            GameObject masterObject = optionOwner.masterObject;
            if (masterObject)
            {
                BaseAI ai = masterObject.GetComponent<BaseAI>();
                BaseAI.Target mainTarget = ai.currentEnemy;
                if (mainTarget != null && mainTarget.gameObject)
                {
                    target = mainTarget.gameObject;
                }
            }

            foreach (GameObject option in optionTracker.existingOptions)
            {
                OptionBehavior behavior = option.GetComponent<OptionBehavior>();
                if (behavior)
                {
                    Vector3 direction;
                    if (target)
                    {
                        direction = (target.transform.position - option.transform.position).normalized;
                        if (includeModelInsideOrb)
                        {
                            behavior.target = target;
                            NetworkIdentity targetNetworkIdentity = target.GetComponent<NetworkIdentity>();
                            if (targetNetworkIdentity)
                            {
                                OptionSync(optionOwner, (networkIdentity, nullTracker) =>
                                {
                                    optionTracker.targetIds.Add(Tuple.Create(
                                        GameObjectType.Body, networkIdentity.netId,
                                        (short)behavior.numbering, targetNetworkIdentity.netId
                                    ));
                                }, false);
                            }
                        }
                    }
                    else direction = optionOwner.inputBank.GetAimRay().direction.normalized;
                    actionToRun(option, behavior, target, direction);
                }
            }
        }

        /// <summary>
        /// Method that provides the Network Identity and Option Tracker for easier syncing. Sync logic should be provided in actionToRun.
        /// </summary>
        /// <param name="optionOwner">The owner of the option.</param>
        /// <param name="actionToRun">The sync action to perform. Inputs are as follows: NetworkIdentity optionIdentity, OptionTracker tracker.</param>
        /// <param name="queryTracker">If true, the Option tracker is automatically queried. If false, the Option tracker will not be queried.</param>
        public void OptionSync(CharacterBody optionOwner, Action<NetworkIdentity, OptionTracker> actionToRun, bool queryTracker = true)
        {
            if (!NetworkServer.active) return;
            NetworkIdentity networkIdentity = optionOwner.gameObject.GetComponent<NetworkIdentity>();
            if (!networkIdentity) return;
            OptionTracker tracker = null;
            if (queryTracker) tracker = optionOwner.gameObject.GetComponent<OptionTracker>();
            if (!queryTracker || tracker) actionToRun(networkIdentity, tracker);
        }

        /// <summary>
        /// Method that provides an easy way of displaying the effect prefab on Options.
        /// </summary>
        /// <param name="prefab">Effect prefab to display</param>
        /// <param name="option">The Option Game Object</param>
        /// <param name="transmit">Determines whether this effect should be networked</param>
        public void OptionMuzzleEffect(GameObject prefab, GameObject option, bool transmit)
        {
            EffectData data = new EffectData
            {
                origin = option.transform.position,
                rotation = option.transform.rotation,
                rootObject = option
            };
            EffectManager.SpawnEffect(prefab, data, transmit);
        }

        /// <summary>
        /// Loops through all the Options of the minion. Always do a null check on the target parameter of actionToRun.
        /// </summary>
        /// <param name="optionOwner">The owner of the option.</param>
        /// <param name="actionToRun">An action to execute for each Option. The inputs are as follows: GameObject option, OptionBehavior behavior, GameObject target.</param>
        [Obsolete("Use the other overload with an Action that has 4 parameters. This one is old and only has an Action that has 3 parameters.")]
        public void FireForAllOptions(CharacterBody optionOwner, Action<GameObject, OptionBehavior, GameObject> actionToRun)
        {
            if (!optionOwner) return;
            OptionTracker optionTracker = optionOwner.GetComponent<OptionTracker>();
            if (!optionTracker) return;

            GameObject target = null;
            GameObject masterObject = optionOwner.masterObject;
            if (masterObject)
            {
                BaseAI ai = masterObject.GetComponent<BaseAI>();
                BaseAI.Target mainTarget = ai.currentEnemy;
                if (mainTarget != null && mainTarget.gameObject)
                {
                    target = mainTarget.gameObject;
                }
            }

            foreach (GameObject option in optionTracker.existingOptions)
            {
                OptionBehavior behavior = option.GetComponent<OptionBehavior>();
                if (behavior)
                {
                    if (includeModelInsideOrb && target)
                    {
                        behavior.target = target;
                        NetworkIdentity targetNetworkIdentity = target.GetComponent<NetworkIdentity>();
                        if (targetNetworkIdentity)
                        {
                            OptionSync(optionOwner, (networkIdentity, nullTracker) =>
                            {
                                optionTracker.targetIds.Add(Tuple.Create(
                                    GameObjectType.Body, networkIdentity.netId, (short)behavior.numbering, targetNetworkIdentity.netId
                                ));
                            }, false);
                        }
                    }
                    actionToRun(option, behavior, target);
                }
            }
        }
    }
}