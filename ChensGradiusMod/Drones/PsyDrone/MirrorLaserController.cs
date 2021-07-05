using RoR2;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class MirrorLaserController : MonoBehaviour
    {
        private const uint FireSoundEffect = 108801625;
        private const int MaxPositions = 100;
        private const float CurveAge = .5f;
        private const float MaxDetectionRange = 70f;
        private const float MaxAngleDetection = 180f;
        private const float LifeTime = 5f;
        private const float LaserSpeed = 2f;
        private const float HitCooldown = .1f;
        private const float HitAdjustmentMultiplier = .02f;
        private const float Force = 1f;
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

        public Vector3 direction { get => _direction.normalized; set => _direction = value.normalized; }
        public bool spawnEffects { get; set; }
        public bool complete { get; private set; }
        public bool curved { get; private set; }
        public bool expired { get; private set; }

        private TeamComponent teamComponent { get => _teamComponent ? _teamComponent : owner.teamComponent; set => _teamComponent = value; }

        private CharacterBody _owner;
        private TeamComponent _teamComponent;
        private Vector3 _direction;
        private LineRenderer lineRenderer;
        private ParticleSystem spawnerEffect;
        private ParticleSystem muzzleEffect;
        private List<Vector3> vertices;
        private float fixedAge;
        private int maxPositions;
        private float hitTimer;
        private bool foundTargetAlready;

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
        }

        private void Start()
        {
            if (spawnEffects)
            {
                muzzleEffect.Play();
                spawnerEffect.Play();
                AkSoundEngine.PostEvent(FireSoundEffect, gameObject);
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
        }

        private void GetTarget(Vector3 origin)
        {
            BullseyeSearch enemyFinder = new BullseyeSearch
            {
                maxDistanceFilter = MaxDetectionRange,
                maxAngleFilter = MaxAngleDetection,
                searchOrigin = origin,
                searchDirection = direction,
                filterByLoS = false,
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
            hit = Physics.Raycast(origin, direction, out RaycastHit raycastHit, LaserSpeed,
                                  LayerIndex.world.mask | LayerIndex.entityPrecise.mask | LayerIndex.defaultLayer.mask, QueryTriggerInteraction.UseGlobal);
            if (hit)
            {
                newEndPosition = raycastHit.point - (HitAdjustmentMultiplier * LaserSpeed * direction);
                curved = true;
            }
            else newEndPosition = origin + (direction * LaserSpeed);

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
            else if (!expired) vertices.Insert(0, newEndPosition);
            if (complete) vertices.RemoveAt(vertices.Count - 1);
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
                        new BlastAttack
                        {
                            attacker = owner.gameObject,
                            inflictor = owner.gameObject,
                            teamIndex = teamComponent.teamIndex,
                            baseDamage = owner.damage,
                            baseForce = Force,
                            position = vertex,
                            radius = Radius,
                            attackerFiltering = AttackerFiltering.NeverHit,
                            damageType = DamageType.Stun1s,
                            falloffModel = BlastAttack.FalloffModel.None
                        }.Fire();
                        interval = DamageAuraInterval;
                    }
                    interval--;
                }
                hitTimer += HitCooldown;
            }
        }
    }
}