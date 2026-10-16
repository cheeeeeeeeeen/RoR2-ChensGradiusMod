using EntityStates;
using RoR2;
using RoR2.Projectile;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace Chen.GradiusMod.Mirroring
{
    /// <summary>
    /// A point where a mirrored attack is copied from, such as an Option Seed or an Option.
    /// </summary>
    public struct MirrorPoint
    {
        /// <summary>
        /// The object the copied attack comes out of.
        /// </summary>
        public GameObject origin;

        /// <summary>
        /// The damage multiplier of the copied attack.
        /// </summary>
        public float multiplier;

        /// <summary>
        /// The direction the copied attack is aimed at. If null, the direction of the original attack is used.
        /// </summary>
        public Vector3? direction;
    }

    /// <summary>
    /// Provides the points where attacks of a character are copied from.
    /// </summary>
    public interface IMirrorProvider
    {
        /// <summary>
        /// Adds the mirror points of the attacker to the list. Add nothing if the attacker's attacks should not be copied by this provider.
        /// </summary>
        /// <param name="attacker">The character that performed the attack.</param>
        /// <param name="points">The list to add the mirror points to.</param>
        void CollectMirrorPoints(CharacterBody attacker, List<MirrorPoint> points);
    }

    /// <summary>
    /// Copies attacks automatically by hooking into the basic attack types of the game: bullets, projectiles, melee hitboxes and explosions.
    /// This is how Option Seeds and Options support characters that do not have dedicated support, such as modded survivors and drones.
    /// Only attacks that come from skills are copied. Item procs, equipment and attacks that are spawned by projectiles are ignored.
    /// </summary>
    public static class AttackMirror
    {
        private static readonly List<IMirrorProvider> providers = new List<IMirrorProvider>();
        private static readonly HashSet<Type> excludedStates = new HashSet<Type>();
        private static readonly HashSet<string> excludedProjectiles = new HashSet<string>();
        private static readonly ConditionalWeakTable<OverlapAttack, OverlapAttack[]> overlapCopies = new ConditionalWeakTable<OverlapAttack, OverlapAttack[]>();
        private static readonly List<MirrorPoint> pointBuffer = new List<MirrorPoint>();
        private static readonly Func<object, object> memberwiseClone =
            (Func<object, object>)Delegate.CreateDelegate(typeof(Func<object, object>),
                typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic));

        private static bool installed;
        private static bool mirroring;

        /// <summary>
        /// Prevents attacks from being copied while the character is in the given state. Useful for skills that should not be duplicated,
        /// such as skills that throw something unique.
        /// </summary>
        /// <param name="stateType">The type of the entity state.</param>
        public static void ExcludeEntityState(Type stateType)
        {
            if (stateType != null) excludedStates.Add(stateType);
        }

        /// <summary>
        /// Prevents a projectile from being copied.
        /// </summary>
        /// <param name="projectilePrefab">The projectile prefab.</param>
        public static void ExcludeProjectile(GameObject projectilePrefab)
        {
            if (projectilePrefab) excludedProjectiles.Add(projectilePrefab.name);
        }

        internal static void RegisterProvider(IMirrorProvider provider)
        {
            if (!providers.Contains(provider)) providers.Add(provider);
            Install();
        }

        private static void Install()
        {
            if (installed) return;
            installed = true;
            On.RoR2.BulletAttack.Fire_FireArgs += BulletAttack_Fire;
            On.RoR2.Projectile.ProjectileManager.FireProjectile_FireProjectileInfo += ProjectileManager_FireProjectile;
            On.RoR2.OverlapAttack.Fire += OverlapAttack_Fire;
            On.RoR2.BlastAttack.Fire += BlastAttack_Fire;
        }

        private static void BulletAttack_Fire(On.RoR2.BulletAttack.orig_Fire_FireArgs orig, BulletAttack self, BulletAttack.FireArgs args)
        {
            orig(self, args);
            if (self.weapon && self.weapon.GetComponent<ProjectileController>()) return;
            if (!TryGetMirrorPoints(self.owner, self.damageType, self.procChainMask, out List<MirrorPoint> points)) return;
            RunMirrored(() =>
            {
                foreach (MirrorPoint point in points)
                {
                    BulletAttack copy = (BulletAttack)memberwiseClone(self);
                    copy.origin = point.origin.transform.position;
                    copy.aimVector = point.direction ?? self.aimVector;
                    copy.weapon = point.origin;
                    copy.damage *= point.multiplier;
                    copy.force *= point.multiplier;
                    copy.FilterOutOwnerFromAttack();
                    copy.Fire();
                }
            });
        }

        private static void ProjectileManager_FireProjectile(On.RoR2.Projectile.ProjectileManager.orig_FireProjectile_FireProjectileInfo orig, ProjectileManager self, FireProjectileInfo info)
        {
            orig(self, info);
            if (!info.projectilePrefab || excludedProjectiles.Contains(info.projectilePrefab.name)) return;
            // Projectiles that skills look up later, such as a blade that the survivor dashes to, are part of the skill's mechanics. Copies would confuse them.
            if (info.projectilePrefab.GetComponent<ProjectileLocator>()) return;
            DamageTypeCombo damageType = info.damageTypeOverride ?? GetProjectileDamageType(info.projectilePrefab);
            if (!TryGetMirrorPoints(info.owner, damageType, info.procChainMask, out List<MirrorPoint> points)) return;
            if (IsCalledFromProjectile()) return;
            RunMirrored(() =>
            {
                foreach (MirrorPoint point in points)
                {
                    FireProjectileInfo copy = info;
                    copy.position = point.origin.transform.position;
                    if (point.direction.HasValue) copy.rotation = Util.QuaternionSafeLookRotation(point.direction.Value);
                    copy.damage *= point.multiplier;
                    copy.force *= point.multiplier;
                    self.FireProjectile(copy);
                }
            });
        }

        private static bool OverlapAttack_Fire(On.RoR2.OverlapAttack.orig_Fire orig, OverlapAttack self, List<HurtBox> hitResults)
        {
            bool result = orig(self, hitResults);
            if (self.inflictor && self.inflictor != self.attacker) return result;
            if (!TryGetMirrorPoints(self.attacker, self.damageType, self.procChainMask, out List<MirrorPoint> points)) return result;
            // The copies are kept per original attack so that each copy remembers who it already hit, just like the original.
            if (!overlapCopies.TryGetValue(self, out OverlapAttack[] copies) || copies.Length != points.Count)
            {
                copies = new OverlapAttack[points.Count];
                for (int i = 0; i < copies.Length; i++) copies[i] = CloneOverlapAttack(self);
                overlapCopies.Remove(self);
                overlapCopies.Add(self, copies);
            }
            RunMirrored(() =>
            {
                for (int i = 0; i < copies.Length; i++)
                {
                    OverlapAttack copy = copies[i];
                    copy.hitBoxGroup = self.hitBoxGroup;
                    copy.isCrit = self.isCrit;
                    copy.damage = self.damage * points[i].multiplier;
                    copy.forceVector = self.forceVector * points[i].multiplier;
                    copy.pushAwayForce = self.pushAwayForce * points[i].multiplier;
                    copy.Fire();
                }
            });
            return result;
        }

        private static BlastAttack.Result BlastAttack_Fire(On.RoR2.BlastAttack.orig_Fire orig, BlastAttack self)
        {
            BlastAttack.Result result = orig(self);
            if (self.inflictor && self.inflictor != self.attacker) return result;
            if (!TryGetMirrorPoints(self.attacker, self.damageType, self.procChainMask, out List<MirrorPoint> points)) return result;
            RunMirrored(() =>
            {
                foreach (MirrorPoint point in points)
                {
                    BlastAttack copy = (BlastAttack)memberwiseClone(self);
                    copy.baseDamage *= point.multiplier;
                    copy.baseForce *= point.multiplier;
                    copy.bonusForce *= point.multiplier;
                    copy.Fire();
                }
            });
            return result;
        }

        private static bool TryGetMirrorPoints(GameObject attackerObject, DamageTypeCombo damageType, ProcChainMask procChainMask, out List<MirrorPoint> points)
        {
            points = null;
            if (mirroring || providers.Count == 0 || !attackerObject) return false;
            if ((damageType.damageSource & DamageSource.Equipment) != 0) return false;
            // Skills mark their damage with a skill source. Older modded skills may not, so an empty proc chain is accepted as well.
            // Item procs always carry the proc that triggered them.
            bool fromSkill = (damageType.damageSource & DamageSource.SkillMask) != 0;
            if (!fromSkill && !procChainMask.Equals(default(ProcChainMask))) return false;
            CharacterBody attacker = attackerObject.GetComponent<CharacterBody>();
            if (!attacker || IsInExcludedState(attacker)) return false;

            pointBuffer.Clear();
            foreach (IMirrorProvider provider in providers) provider.CollectMirrorPoints(attacker, pointBuffer);
            pointBuffer.RemoveAll(point => !point.origin);
            if (pointBuffer.Count == 0) return false;
            points = new List<MirrorPoint>(pointBuffer);
            return true;
        }

        private static bool IsInExcludedState(CharacterBody body)
        {
            if (excludedStates.Count == 0) return false;
            foreach (EntityStateMachine stateMachine in body.GetComponents<EntityStateMachine>())
            {
                EntityState state = stateMachine.state;
                if (state == null) continue;
                foreach (Type excludedType in excludedStates)
                {
                    if (excludedType.IsInstanceOfType(state)) return true;
                }
            }
            return false;
        }

        private static bool IsCalledFromProjectile()
        {
            // Projectiles that split or spawn other projectiles fire them with the same owner. Those should not be copied again,
            // since the copied parent projectile already spawns its own children.
            StackFrame[] frames = new StackTrace(2, false).GetFrames();
            if (frames == null) return false;
            foreach (StackFrame frame in frames)
            {
                Type declaringType = frame.GetMethod()?.DeclaringType;
                if (declaringType == null || declaringType == typeof(ProjectileManager)) continue;
                if (declaringType.Namespace == "RoR2.Projectile") return true;
            }
            return false;
        }

        private static DamageTypeCombo GetProjectileDamageType(GameObject projectilePrefab)
        {
            ProjectileDamage projectileDamage = projectilePrefab.GetComponent<ProjectileDamage>();
            return projectileDamage ? projectileDamage.damageType : DamageTypeCombo.Generic;
        }

        private static OverlapAttack CloneOverlapAttack(OverlapAttack source)
        {
            return new OverlapAttack
            {
                attacker = source.attacker,
                inflictor = source.inflictor,
                teamIndex = source.teamIndex,
                attackerFiltering = source.attackerFiltering,
                forceVector = source.forceVector,
                pushAwayForce = source.pushAwayForce,
                physForceFlags = source.physForceFlags,
                damage = source.damage,
                isCrit = source.isCrit,
                procChainMask = source.procChainMask,
                procCoefficient = source.procCoefficient,
                hitBoxGroup = source.hitBoxGroup,
                hitEffectPrefab = source.hitEffectPrefab,
                impactSound = source.impactSound,
                damageColorIndex = source.damageColorIndex,
                damageType = source.damageType,
                maximumOverlapTargets = source.maximumOverlapTargets,
                attackOriginPreference = source.attackOriginPreference,
                retriggerTimeout = source.retriggerTimeout,
                isSphereOverlap = source.isSphereOverlap,
                filterCallback = source.filterCallback,
                hasDamageProcessingHurtboxes = source.hasDamageProcessingHurtboxes,
                modifyOutgoingOverlapInfoCallback = source.modifyOutgoingOverlapInfoCallback
            };
        }

        private static void RunMirrored(Action action)
        {
            mirroring = true;
            try
            {
                action();
            }
            finally
            {
                mirroring = false;
            }
        }
    }
}
