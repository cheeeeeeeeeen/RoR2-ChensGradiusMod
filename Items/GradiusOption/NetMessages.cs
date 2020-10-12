using EntityStates;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.GradiusMod.SpawnOptionsForClients;
using MageWeapon = EntityStates.Mage.Weapon;

namespace Chen.GradiusMod
{
    public class SpawnOptionsForClients : INetMessage
    {
        private GameObjectType bodyOrMaster;
        private NetworkInstanceId ownerId;
        private short numbering;

        public SpawnOptionsForClients()
        {
        }

        public SpawnOptionsForClients(GameObjectType bodyOrMaster, NetworkInstanceId ownerId, short numbering)
        {
            this.bodyOrMaster = bodyOrMaster;
            this.ownerId = ownerId;
            this.numbering = numbering;
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write((byte)bodyOrMaster);
            writer.Write(ownerId);
            writer.Write(numbering);
        }

        public void Deserialize(NetworkReader reader)
        {
            bodyOrMaster = (GameObjectType)reader.ReadByte();
            ownerId = reader.ReadNetworkId();
            numbering = reader.ReadInt16();
        }

        public void OnReceived()
        {
            if (NetworkServer.active) return;
            GameObject ownerObject = Util.FindNetworkObject(ownerId);
            if (!ownerObject)
            {
                GradiusModPlugin._logger.LogWarning("SpawnOptionsForClients: ownerObject is null.");
                return;
            }
            switch (bodyOrMaster)
            {
                case GameObjectType.Body:
                    TrySpawnOption(ownerObject.GetComponent<CharacterBody>());
                    break;

                case GameObjectType.Master:
                    CharacterMaster ownerMaster = ownerObject.GetComponent<CharacterMaster>();
                    if (!ownerMaster)
                    {
                        GradiusModPlugin._logger.LogWarning("SpawnOptionsForClients: ownerMaster is null.");
                        return;
                    }
                    TrySpawnOption(ownerMaster.GetBody());
                    break;
            }
        }

        private void TrySpawnOption(CharacterBody ownerBody)
        {
            if (!ownerBody)
            {
                GradiusModPlugin._logger.LogWarning("SpawnOptionsForClients: ownerBody is null.");
                return;
            }
            OptionMasterTracker.SpawnOption(ownerBody.gameObject, numbering);
        }

        public enum GameObjectType : byte
        {
            Master,
            Body
        }
    }

    public class SyncFlamethrowerEffectForClients : INetMessage
    {
        private MessageType messageType;
        private NetworkInstanceId ownerBodyId;
        private short optionNumbering;
        private float duration;
        private Vector3 direction;

        public SyncFlamethrowerEffectForClients()
        {
        }

        public SyncFlamethrowerEffectForClients(MessageType messageType, NetworkInstanceId id, short numbering, float duration, Vector3 direction)
        {
            this.messageType = messageType;
            ownerBodyId = id;
            optionNumbering = numbering;
            this.duration = duration;
            this.direction = direction;
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write((byte)messageType);
            writer.Write(ownerBodyId);
            writer.Write(optionNumbering);
            writer.Write(duration);
            writer.Write(direction);
        }

        public void Deserialize(NetworkReader reader)
        {
            messageType = (MessageType)reader.ReadByte();
            ownerBodyId = reader.ReadNetworkId();
            optionNumbering = reader.ReadInt16();
            duration = reader.ReadSingle();
            direction = reader.ReadVector3();
        }

        public void OnReceived()
        {
            if (NetworkServer.active) return;
            GameObject bodyObject = Util.FindNetworkObject(ownerBodyId);
            if (!bodyObject)
            {
                GradiusModPlugin._logger.LogWarning($"SyncFlamethrowerEffectForClients: bodyObject is null.");
                return;
            }
            OptionTracker tracker = bodyObject.GetComponent<OptionTracker>();
            if (!tracker)
            {
                GradiusModPlugin._logger.LogWarning($"SyncFlamethrowerEffectForClients: tracker is null.");
                return;
            }
            GameObject option = tracker.existingOptions[optionNumbering - 1];
            OptionBehavior behavior = option.GetComponent<OptionBehavior>();
            if (!behavior)
            {
                GradiusModPlugin._logger.LogWarning($"SyncFlamethrowerEffectForClients: behavior is null.");
                return;
            }
            switch (messageType)
            {
                case MessageType.Create:
                    if (GradiusOption.instance.flamethrowerSoundCopy) Util.PlaySound(MageWeapon.Flamethrower.startAttackSoundString, option);
                    behavior.flamethrower = Object.Instantiate(GradiusOption.flamethrowerEffectPrefab, option.transform);
                    behavior.flamethrower.GetComponent<ScaleParticleSystemDuration>().newDuration = duration;
                    break;

                case MessageType.Destroy:
                    if (behavior.flamethrower) EntityState.Destroy(behavior.flamethrower);
                    break;

                case MessageType.Redirect:
                    if (behavior.flamethrower) behavior.flamethrower.transform.forward = direction;
                    break;

                default:
                    break;
            }
        }

        public enum MessageType : byte
        {
            Create,
            Destroy,
            Redirect
        };
    }

    public class SyncOptionTargetForClients : INetMessage
    {
        private GameObjectType bodyOrMaster;
        private NetworkInstanceId ownerId;
        private short numbering;
        private NetworkInstanceId targetId;

        public SyncOptionTargetForClients()
        {
        }

        public SyncOptionTargetForClients(GameObjectType bodyOrMaster, NetworkInstanceId ownerId, short numbering, NetworkInstanceId targetId)
        {
            this.bodyOrMaster = bodyOrMaster;
            this.ownerId = ownerId;
            this.numbering = numbering;
            this.targetId = targetId;
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write((byte)bodyOrMaster);
            writer.Write(ownerId);
            writer.Write(numbering);
            writer.Write(targetId);
        }

        public void Deserialize(NetworkReader reader)
        {
            bodyOrMaster = (GameObjectType)reader.ReadByte();
            ownerId = reader.ReadNetworkId();
            numbering = reader.ReadInt16();
            targetId = reader.ReadNetworkId();
        }

        public void OnReceived()
        {
            if (NetworkServer.active) return;
            GameObject ownerObject = Util.FindNetworkObject(ownerId);
            if (!ownerObject)
            {
                GradiusModPlugin._logger.LogWarning("SyncOptionTargetForClients: ownerObject is null.");
                return;
            }
            GameObject targetObject = Util.FindNetworkObject(targetId);
            if (!targetObject)
            {
                GradiusModPlugin._logger.LogWarning("SyncOptionTargetForClients: targetObject is null.");
                return;
            }
            OptionTracker tracker = null;
            switch (bodyOrMaster)
            {
                case GameObjectType.Body:
                    tracker = ownerObject.GetComponent<OptionTracker>();
                    break;

                case GameObjectType.Master:
                    CharacterMaster ownerMaster = ownerObject.GetComponent<CharacterMaster>();
                    if (!ownerMaster)
                    {
                        GradiusModPlugin._logger.LogWarning("SpawnOptionsForClients: ownerMaster is null.");
                        return;
                    }
                    GameObject bodyObject = ownerMaster.GetBodyObject();
                    if (!bodyObject)
                    {
                        GradiusModPlugin._logger.LogWarning("SpawnOptionsForClients: bodyObject is null.");
                        return;
                    }
                    tracker = bodyObject.GetComponent<OptionTracker>();
                    break;
            }
            if (!tracker) return;
            SetTarget(tracker, targetObject);
        }

        private void SetTarget(OptionTracker tracker, GameObject target)
        {
            GameObject option = tracker.existingOptions[numbering - 1];
            OptionBehavior behavior = option.GetComponent<OptionBehavior>();
            if (!behavior) return;
            behavior.target = target;
        }
    }
}