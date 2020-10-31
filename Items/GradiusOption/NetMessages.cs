using EntityStates;
using EntityStates.BeetleGuardMonster;
using EntityStates.Drone.DroneWeapon;
using EntityStates.TitanMonster;
using R2API.Networking.Interfaces;
using RoR2;
using UnityEngine;
using UnityEngine.Networking;
using MageWeapon = EntityStates.Mage.Weapon;

namespace Chen.GradiusMod
{
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
                Log.Warning($"SyncFlamethrowerEffectForClients: bodyObject is null.");
                return;
            }
            OptionTracker tracker = bodyObject.GetComponent<OptionTracker>();
            if (!tracker)
            {
                Log.Warning($"SyncFlamethrowerEffectForClients: tracker is null.");
                return;
            }
            GameObject option = tracker.existingOptions[optionNumbering - 1];
            OptionBehavior behavior = option.GetComponent<OptionBehavior>();
            if (!behavior)
            {
                Log.Warning($"SyncFlamethrowerEffectForClients: behavior is null.");
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
                    if (GradiusOption.instance.flamethrowerSoundCopy) Util.PlaySound(MageWeapon.Flamethrower.endAttackSoundString, option);
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

    public class SyncAurelioniteEffectsForClients : INetMessage
    {
        private MessageType messageType;
        private NetworkInstanceId ownerBodyId;
        private short optionNumbering;
        private float duration;
        private Vector3 point;
        private float indicatorSize;

        public SyncAurelioniteEffectsForClients()
        {
        }

        public SyncAurelioniteEffectsForClients(MessageType messageType, NetworkInstanceId id, short numbering, float duration, Vector3 point, float indicatorSize)
        {
            this.messageType = messageType;
            ownerBodyId = id;
            optionNumbering = numbering;
            this.duration = duration;
            this.point = point;
            this.indicatorSize = indicatorSize;
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write((byte)messageType);
            writer.Write(ownerBodyId);
            writer.Write(optionNumbering);
            writer.Write(duration);
            writer.Write(point);
            writer.Write(indicatorSize);
        }

        public void Deserialize(NetworkReader reader)
        {
            messageType = (MessageType)reader.ReadByte();
            ownerBodyId = reader.ReadNetworkId();
            optionNumbering = reader.ReadInt16();
            duration = reader.ReadSingle();
            point = reader.ReadVector3();
            indicatorSize = reader.ReadSingle();
        }

        public void OnReceived()
        {
            if (NetworkServer.active) return;
            GameObject goldObject = Util.FindNetworkObject(ownerBodyId);
            if (!goldObject)
            {
                Log.Warning("SyncAurelioniteEffectsForClients: goldObject is null.");
                return;
            }
            OptionTracker tracker = goldObject.GetComponent<OptionTracker>();
            if (!tracker)
            {
                Log.Warning($"SyncAurelioniteEffectsForClients: tracker is null.");
                return;
            }
            GameObject option = tracker.existingOptions[optionNumbering - 1];
            OptionBehavior behavior = option.GetComponent<OptionBehavior>();
            if (!behavior)
            {
                Log.Warning($"SyncAurelioniteEffectsForClients: behavior is null.");
                return;
            }

            Transform transform = option.transform;
            Vector3 position = transform.position;
            switch (messageType)
            {
                //case MessageType.CreateLaserCharge:
                //    if (GradiusOption.laserChargeEffectPrefab)
                //    {
                //        behavior.laserChargeEffect = Object.Instantiate(GradiusOption.laserChargeEffectPrefab, position, transform.rotation);
                //        behavior.laserChargeEffect.transform.parent = transform;
                //        ScaleParticleSystemDuration component = behavior.laserChargeEffect.GetComponent<ScaleParticleSystemDuration>();
                //        if (component) component.newDuration = duration;
                //    }
                //    else Log.Warning("NULL");
                //    if (GradiusOption.laserChargeLaserPrefab)
                //    {
                //        behavior.laserFireEffect = Object.Instantiate(GradiusOption.laserChargeLaserPrefab, position, transform.rotation);
                //        behavior.laserFireEffect.transform.parent = transform;
                //        behavior.laserLineEffect = behavior.laserFireEffect.GetComponent<LineRenderer>();
                //    }
                //    else Log.Warning("NULL");
                //    break;

                //case MessageType.DestroyLaserCharge:
                //    if (behavior.laserChargeEffect) EntityState.Destroy(behavior.laserChargeEffect);
                //    if (behavior.laserFireEffect) EntityState.Destroy(behavior.laserFireEffect);
                //    if (behavior.laserLineEffect) EntityState.Destroy(behavior.laserLineEffect);
                //    break;

                case MessageType.UpdateLaserCharge:
                    if (behavior.laserFireEffect && behavior.laserLineEffect)
                    {
                        behavior.laserLineEffect.SetPosition(0, position);
                        behavior.laserLineEffect.SetPosition(1, point);
                        behavior.laserLineEffect.startWidth = indicatorSize;
                        behavior.laserLineEffect.endWidth = indicatorSize;
                    }
                    break;

                //case MessageType.CreateLaserFire:
                //    if (!GradiusOption.laserFireLaserPrefab) return;
                //    if (GradiusOption.instance.aurelioniteMegaLaserSoundCopy)
                //    {
                //        Util.PlaySound(FireMegaLaser.playAttackSoundString, option);
                //        Util.PlaySound(FireMegaLaser.playLoopSoundString, option);
                //    }
                //    behavior.laserFire = Object.Instantiate(GradiusOption.laserFireLaserPrefab, position, transform.rotation);
                //    behavior.laserFire.transform.parent = transform;
                //    behavior.laserChildLocator = behavior.laserFireEffect.GetComponent<ChildLocator>();
                //    behavior.laserFireEnd = behavior.laserChildLocator.FindChild("LaserEnd");
                //    break;

                //case MessageType.DestroyLaserFire:
                //    if (GradiusOption.instance.aurelioniteMegaLaserSoundCopy) Util.PlaySound(FireMegaLaser.stopLoopSoundString, option);
                //    if (behavior.laserFire) EntityState.Destroy(behavior.laserFire);
                //    if (behavior.laserChildLocator) EntityState.Destroy(behavior.laserChildLocator);
                //    if (behavior.laserFireEnd) EntityState.Destroy(behavior.laserFireEnd);
                //    break;

                case MessageType.FixedUpdateGoldLaserFire:
                    behavior.laserFire.transform.rotation = Util.QuaternionSafeLookRotation(point - position);
                    behavior.laserFireEnd.transform.position = point;
                    break;

                case MessageType.CreateFist:
                    if (!GradiusOption.fistChargeEffectPrefab) return;
                    behavior.fistChargeEffect = Object.Instantiate(GradiusOption.fistChargeEffectPrefab, transform);
                    break;

                case MessageType.DestroyFist:
                    if (behavior.fistChargeEffect) EntityState.Destroy(behavior.fistChargeEffect);
                    break;
            }
        }

        public enum MessageType : byte
        {
            CreateLaserCharge,
            DestroyLaserCharge,
            UpdateLaserCharge,
            CreateLaserFire,
            DestroyLaserFire,
            FixedUpdateGoldLaserFire,
            CreateFist,
            DestroyFist
        }
    }

    public class SyncBeetleGuardEffectsForClients : INetMessage
    {
        private MessageType messageType;
        private NetworkInstanceId ownerBodyId;
        private short optionNumbering;

        public SyncBeetleGuardEffectsForClients()
        {
        }

        public SyncBeetleGuardEffectsForClients(MessageType messageType, NetworkInstanceId id, short numbering)
        {
            this.messageType = messageType;
            ownerBodyId = id;
            optionNumbering = numbering;
        }

        public void Serialize(NetworkWriter writer)
        {
            writer.Write((byte)messageType);
            writer.Write(ownerBodyId);
            writer.Write(optionNumbering);
        }

        public void Deserialize(NetworkReader reader)
        {
            messageType = (MessageType)reader.ReadByte();
            ownerBodyId = reader.ReadNetworkId();
            optionNumbering = reader.ReadInt16();
        }

        public void OnReceived()
        {
            if (NetworkServer.active) return;
            GameObject beetleObject = Util.FindNetworkObject(ownerBodyId);
            if (!beetleObject)
            {
                Log.Warning("SyncBeetleGuardEffectsForClients: beetleObject is null.");
                return;
            }
            OptionTracker tracker = beetleObject.GetComponent<OptionTracker>();
            if (!tracker)
            {
                Log.Warning($"SyncBeetleGuardEffectsForClients: tracker is null.");
                return;
            }
            GameObject option = tracker.existingOptions[optionNumbering - 1];
            OptionBehavior behavior = option.GetComponent<OptionBehavior>();
            if (!behavior)
            {
                Log.Warning($"SyncBeetleGuardEffectsForClients: behavior is null.");
                return;
            }
            switch (messageType)
            {
                case MessageType.Create:
                    if (GradiusOption.instance.beetleGuardChargeSoundCopy) Util.PlaySound(FireSunder.initialAttackSoundString, option);
                    if (FireSunder.chargeEffectPrefab) behavior.sunderEffect = Object.Instantiate(FireSunder.chargeEffectPrefab, option.transform);
                    break;

                case MessageType.Destroy:
                    if (behavior.sunderEffect) EntityState.Destroy(behavior.sunderEffect);
                    break;
            }
        }

        public enum MessageType : byte
        {
            Create,
            Destroy
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