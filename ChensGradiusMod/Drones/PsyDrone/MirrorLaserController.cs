using Chen.GradiusMod.Compatibility;
using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.Helpers.GeneralHelpers.BlastAttack;
using static RoR2.BlastAttack;

using BlastAttack = Chen.Helpers.GeneralHelpers.BlastAttack;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class MirrorLaserController : MonoBehaviour
    {
        public const float BaseForce = 1f;

        private const int MaxPositions = 100;
        private const float CurveAge = .5f;
        private const float MaxDetectionRange = 70f;
        private const float MaxAngleDetection = 180f;
        private const float LifeTime = 5f;
        private const float LaserSpeed = 2f;
        private const float HitCooldown = .15f;
        private const float HitAdjustmentMultiplier = .02f;
        private const float Radius = 3f;
        private const int DamageAuraInterval = 3;

        public CharacterBody owner
        {
            get => _owner;
            set
            {
                _owner = value;
                teamComponent = _owner.teamComponent;
            }
        }

        public float damage
        {
            get
            {
                float value = 0f;
                if (_damage == null)
                {
                    if (owner) _damage = value = owner.damage;
                }
                else value = (float)_damage;
                return value;
            }
            set => _damage = value;
        }

        public float force
        {
            get
            {
                if (_force == null) _force = BaseForce;
                return (float)_force;
            }
            set => _force = value;
        }

        public float speed
        {
            get
            {
                if (_speed == null) _speed = LaserSpeed;
                return (float)_speed;
            }
            set => _speed = value;
        }

        public Vector3 direction { get => _direction.normalized; set => _direction = value.normalized; }
        public bool spawnEffects { get; set; }
        public bool complete { get; private set; }
        public bool curved { get; private set; }
        public bool expired { get; private set; }

        private TeamComponent teamComponent { get => _teamComponent ? _teamComponent : owner.teamComponent; set => _teamComponent = value; }

        private CharacterBody _owner;
        private TeamComponent _teamComponent;
        private Vector3 _direction;
        private float? _damage;
        private float? _speed;
        private float? _force;
        private LineRenderer lineRenderer;
        private ParticleSystem spawnerEffect;
        private ParticleSystem muzzleEffect;
        private List<Vector3> vertices;
        private float fixedAge;
        private int maxPositions;
        private float hitTimer;
        private bool foundTargetAlready;
        private List<GameObject> bodyEffects;

        private void Awake()
        {
            lineRenderer = GetComponentInChildren<LineRenderer>();
            lineRenderer.positionCount = 0;
            vertices = new List<Vector3>();
            spawnerEffect = transform.Find("Spawn").GetComponent<ParticleSystem>();
            muzzleEffect = transform.Find("Laser Emission").GetComponent<ParticleSystem>();
            fixedAge = 0f;
            maxPositions = MaxPositions;
            hitTimer = 0f;
            foundTargetAlready = false;
            bodyEffects = new List<GameObject>();
        }

        private void Start()
        {
            if (spawnEffects)
            {
                muzzleEffect.Play();
                spawnerEffect.Play();
            }
        }

        private void FixedUpdate()
        {
            ManageLine();
            DamagingAura();
            if (vertices.Count <= 0 || maxPositions <= 0) Destroy(gameObject);
        }

        private void Update()
        {
            lineRenderer.positionCount = vertices.Count;
            lineRenderer.SetPositions(vertices.ToArray());
            for (int i = 0; i < vertices.Count; i++)
            {
                bodyEffects[i].transform.position = vertices[i];
            }
        }

        private void GetTarget(Vector3 origin)
        {
            BullseyeSearch enemyFinder = new BullseyeSearch
            {
                maxDistanceFilter = MaxDetectionRange,
                maxAngleFilter = MaxAngleDetection,
                searchOrigin = origin,
                searchDirection = direction,
                filterByLoS = true,
                sortMode = BullseyeSearch.SortMode.Distance,
                teamMaskFilter = TeamMask.allButNeutral
            };
            if (teamComponent) enemyFinder.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
            enemyFinder.RefreshCandidates();
            HurtBox exists = enemyFinder.GetResults().FirstOrDefault();
            if (exists)
            {
                foundTargetAlready = true;
                direction = exists.transform.position - origin;
            }
        }

        private Vector3 CheckHits(out bool hit, Vector3 origin)
        {
            Vector3 newEndPosition;
            hit = Physics.Raycast(origin, direction, out RaycastHit raycastHit, speed,
                                  LayerIndex.world.mask | LayerIndex.entityPrecise.mask | LayerIndex.defaultLayer.mask, QueryTriggerInteraction.UseGlobal);
            if (hit)
            {
                newEndPosition = raycastHit.point - (HitAdjustmentMultiplier * speed * direction);
                curved = true;
            }
            else newEndPosition = origin + (direction * speed);

            return newEndPosition;
        }

        private void ManageLine()
        {
            fixedAge += Time.fixedDeltaTime;
            if (fixedAge >= CurveAge) curved = true;
            if (fixedAge >= LifeTime) expired = true;
            Vector3 newEndPosition;
            bool hit = false;
            if (vertices.Count <= 0) newEndPosition = transform.position;
            else
            {
                Vector3 endPosition = vertices[0];
                if (curved && !foundTargetAlready) GetTarget(endPosition);
                newEndPosition = CheckHits(out hit, endPosition);
            }
            if (hit)
            {
                vertices[0] = newEndPosition;
                maxPositions--;
            }
            else if (!expired)
            {
                vertices.Insert(0, newEndPosition);
                bodyEffects.Insert(0, Instantiate(PsyDrone.mirrorLaserBodyEffect, newEndPosition, Quaternion.identity));
            }
            if (complete)
            {
                int index = vertices.Count - 1;
                vertices.RemoveAt(index);
                Destroy(bodyEffects[index]);
                bodyEffects.RemoveAt(index);
            }
            else if (vertices.Count >= maxPositions)
            {
                complete = true;
                spawnerEffect.Stop();
                muzzleEffect.Stop();
            }
        }

        private void DamagingAura()
        {
            if (!NetworkServer.active || !owner) return;
            if (hitTimer > 0) hitTimer -= Time.fixedDeltaTime;
            if (hitTimer <= 0)
            {
                int interval = 0;
                foreach (var vertex in vertices)
                {
                    if (interval <= 0)
                    {
                        HitPointAndResult result = new BlastAttack
                        {
                            attacker = owner.gameObject,
                            inflictor = owner.gameObject,
                            teamIndex = teamComponent.teamIndex,
                            baseDamage = damage,
                            baseForce = force,
                            position = vertex,
                            radius = Radius,
                            attackerFiltering = AttackerFiltering.NeverHit,
                            damageType = DamageType.Stun1s,
                            falloffModel = FalloffModel.None
                        }.InformativeFire();
                        ApplyHitEffect(result);
                        TriggerArmsRace();
                        interval = DamageAuraInterval;
                    }
                    interval--;
                }
                hitTimer += HitCooldown / owner.attackSpeed;
            }
        }

        private void ApplyHitEffect(HitPointAndResult result)
        {
            foreach (var victim in result.hitPoints)
            {
                HurtBox hurtBox = victim.hurtBox;
                if (hurtBox)
                {
                    HealthComponent healthComponent = hurtBox.healthComponent;
                    if (healthComponent)
                    {
                        GameObject body = healthComponent.gameObject;
                        if (PsyDrone.instance.hitSoundEffect) AkSoundEngine.PostEvent(PsyDrone.HitEffectEventId, body);
                        Instantiate(PsyDrone.mirrorLaserHitEffect, body.transform.position, Quaternion.identity);
                    }
                }
            }
        }

        private void TriggerArmsRace()
        {
            if (ChensClassicItems.enabled)
            {
                ChensClassicItems.TriggerArtillery(owner, damage, owner.RollCrit());
            }
        }
    }
}