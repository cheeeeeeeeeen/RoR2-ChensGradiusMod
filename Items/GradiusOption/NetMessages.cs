using EntityStates.Drone.DroneWeapon;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;

namespace Chen.GradiusMod
{
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
                Log.Warning("SyncOptionTargetForClients: ownerObject is null.");
                return;
            }
            GameObject targetObject = Util.FindNetworkObject(targetId);
            if (!targetObject)
            {
                Log.Warning("SyncOptionTargetForClients: targetObject is null.");
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
                        Log.Warning("SpawnOptionsForClients: ownerMaster is null.");
                        return;
                    }
                    GameObject bodyObject = ownerMaster.GetBodyObject();
                    if (!bodyObject)
                    {
                        Log.Warning("SpawnOptionsForClients: bodyObject is null.");
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

        public enum GameObjectType : byte
        {
            Master,
            Body
        }
    }

    public class SyncSimpleSound : INetMessage
    {
        private NetworkInstanceId ownerBodyId;
        private short optionNumbering;
        private string soundString;
        private float scale;

        public SyncSimpleSound()
        {
        }

        public SyncSimpleSound(NetworkInstanceId ownerBodyId, short optionNumbering, string soundString, float scale = 0)
        {
            this.ownerBodyId = ownerBodyId;
            this.optionNumbering = optionNumbering;
            this.soundString = soundString;
            this.scale = scale;
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write(ownerBodyId);
            writer.Write(optionNumbering);
            writer.Write(soundString);
            writer.Write(scale);
        }

        public void Deserialize(NetworkReader reader)
        {
            ownerBodyId = reader.ReadNetworkId();
            optionNumbering = reader.ReadInt16();
            soundString = reader.ReadString();
            scale = reader.ReadSingle();
        }

        public void OnReceived()
        {
            if (NetworkServer.active) return;
            GameObject bodyObject = Util.FindNetworkObject(ownerBodyId);
            if (!bodyObject)
            {
                Log.Warning($"SyncSimpleSound: bodyObject is null.");
                return;
            }
            OptionTracker tracker = bodyObject.GetComponent<OptionTracker>();
            if (!tracker)
            {
                Log.Warning($"SyncSimpleSound: tracker is null.");
                return;
            }
            GameObject option = tracker.existingOptions[optionNumbering - 1];
            if (soundString == FireGatling.fireGatlingSoundString && !GradiusOption.instance.gatlingSoundCopy) return;
            if (soundString == FireTurret.attackSoundString && !GradiusOption.instance.gunnerSoundCopy) return;
            if (soundString == FireMegaTurret.attackSoundString && !GradiusOption.instance.tc280SoundCopy) return;

            if (scale <= 0) Util.PlaySound(soundString, option);
            else Util.PlayScaledSound(soundString, option, scale);
        }
    }
}