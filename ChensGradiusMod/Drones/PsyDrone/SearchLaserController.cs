using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;
using static Chen.Helpers.GeneralHelpers.BlastAttack;
using static RoR2.BlastAttack;

using BlastAttack = Chen.Helpers.GeneralHelpers.BlastAttack;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class SearchLaserController : MonoBehaviour
    {
        private const float DefaultSpeed = 1f;
        private const float DecelerationStateDuration = 1f;
        private const float AccelerationRate = .03f;
        private const float DecelerateLeastSpeed = .1f;
        private const float MaxDetectionRange = 100f;
        private const float MaxAngleDetection = 180f;
        private const float MaxSpeed = 1f;
        private const float MaxSmoothCurveRate = 1f;
        private const float SmoothCurveRate = .005f;
        private const float Radius = 2f;
        private const float Force = 1f;
        private const float DamageMultiplier = 2f;
        private const float SubRadius = 8f;
        private const float SubForce = 8f;
        private const float Expiration = 10f;

        public CharacterBody owner
        {
            get => _owner;
            set
            {
                _owner = value;
                teamComponent = _owner.teamComponent;
            }
        }

        private TeamComponent teamComponent { get => _teamComponent ? _teamComponent : owner.teamComponent; set => _teamComponent = value; }
        private Vector3 direction { get => _direction.normalized; set => _direction = value.normalized; }

        private CharacterBody _owner;
        private TeamComponent _teamComponent;
        private Vector3 _direction;
        private Vector3 computedPosition;
        private States currentState;
        private float timer;
        private float currentSpeed;
        private HurtBox target;
        private float smoothCurveValue;
        private TrailRenderer trailRenderer;
        private GameObject endBall;
        private Vector3 computedEndPosition;
        private float age;
        private bool expired;
        private bool destroyTrailRenderer;

        private void Awake()
        {
            computedPosition = transform.position;
            direction = transform.forward;
            currentState = States.StraightDecelerate;
            timer = 0f;
            currentSpeed = DefaultSpeed;
            target = null;
            smoothCurveValue = 0f;
            trailRenderer = transform.Find("Trail").gameObject.GetComponent<TrailRenderer>();
            endBall = null;
            computedEndPosition = computedPosition;
            age = 0f;
            expired = false;
            destroyTrailRenderer = false;
        }

        private void FixedUpdate()
        {
            age += Time.fixedDeltaTime;
            if (!expired && age >= Expiration)
            {
                currentState = States.DamageEnemy;
                expired = true;
            }
            switch (currentState)
            {
                case States.StraightDecelerate:
                    PerformStraightDecelerate();
                    break;

                case States.FindEnemy:
                    PerformFindEnemy();
                    break;

                case States.HuntEnemy:
                    PerformHuntEnemy();
                    break;

                case States.DamageEnemy:
                    PerformDamageEnemy();
                    break;

                case States.DestroySelf:
                    PerformDestroySelf();
                    break;
            }
            DetectHits();
            ManageEndBall();
        }

        private void Update()
        {
            transform.position = computedPosition;
        }

        private void PerformStraightDecelerate()
        {
            currentSpeed = Mathf.Max(currentSpeed - AccelerationRate, DecelerateLeastSpeed);
            computedPosition += direction * currentSpeed;
            if (currentSpeed <= DecelerateLeastSpeed)
            {
                timer += Time.fixedDeltaTime;
                if (timer >= DecelerationStateDuration)
                {
                    currentState = States.FindEnemy;
                    timer = 0f;
                }
            }
        }

        private void PerformFindEnemy()
        {
            BullseyeSearch enemyFinder = new BullseyeSearch
            {
                maxDistanceFilter = MaxDetectionRange,
                maxAngleFilter = MaxAngleDetection,
                searchOrigin = computedPosition,
                searchDirection = direction,
                filterByLoS = true,
                sortMode = BullseyeSearch.SortMode.Angle,
                teamMaskFilter = TeamMask.allButNeutral
            };
            if (teamComponent) enemyFinder.teamMaskFilter.RemoveTeam(teamComponent.teamIndex);
            enemyFinder.RefreshCandidates();
            target = enemyFinder.GetResults().FirstOrDefault();
            if (target) currentState = States.HuntEnemy;
            computedPosition += direction * currentSpeed;
        }

        private void PerformHuntEnemy()
        {
            if (target)
            {
                currentSpeed = Mathf.Min(currentSpeed + AccelerationRate, MaxSpeed);
                Vector3 directionToEnemy = (target.transform.position - computedPosition).normalized;
                direction = Vector3.Lerp(direction, directionToEnemy, smoothCurveValue);
                smoothCurveValue = Mathf.Min(smoothCurveValue + SmoothCurveRate, MaxSmoothCurveRate);
                bool hit = Physics.Raycast(computedPosition, direction, out RaycastHit raycastHit, currentSpeed,
                                           LayerIndex.world.mask | LayerIndex.entityPrecise.mask | LayerIndex.defaultLayer.mask, QueryTriggerInteraction.UseGlobal);
                if (hit)
                {
                    computedPosition = raycastHit.point;
                    currentState = States.DamageEnemy;
                }
                else computedPosition += direction * currentSpeed;
            }
            else
            {
                timer = 0f;
                smoothCurveValue = 0f;
                currentState = States.StraightDecelerate;
                PerformStraightDecelerate();
            }
        }

        private void PerformDamageEnemy()
        {
            if (NetworkServer.active && owner)
            {
                HitPointAndResult result = new BlastAttack
                {
                    attacker = owner.gameObject,
                    inflictor = owner.gameObject,
                    teamIndex = teamComponent.teamIndex,
                    baseDamage = owner.damage * DamageMultiplier,
                    baseForce = Force,
                    position = computedPosition,
                    radius = Radius,
                    attackerFiltering = AttackerFiltering.NeverHit,
                    falloffModel = FalloffModel.None
                }.InformativeFire();
                ApplyHitEffects(result);
            }
            currentState = States.DestroySelf;
            Destroy(transform.Find("Sphere").gameObject);
        }

        private void PerformDestroySelf()
        {
            if (!destroyTrailRenderer && trailRenderer)
            {
                Destroy(trailRenderer.gameObject, trailRenderer.time);
                destroyTrailRenderer = true;
            }
            if (!trailRenderer)
            {
                Destroy(gameObject);
                Destroy(endBall);
                new BlastAttack
                {
                    attacker = owner.gameObject,
                    inflictor = owner.gameObject,
                    teamIndex = teamComponent.teamIndex,
                    baseDamage = owner.damage,
                    baseForce = SubForce,
                    position = computedPosition,
                    radius = SubRadius,
                    attackerFiltering = AttackerFiltering.NeverHit,
                    falloffModel = FalloffModel.None
                }.Fire();
                Instantiate(PsyDrone.searchLaserSubExplosion, computedPosition, Quaternion.identity);
            }
        }

        private void ManageEndBall()
        {
            if (trailRenderer && trailRenderer.positionCount > 0)
            {
                computedEndPosition = trailRenderer.GetPosition(0);
                if (!endBall) endBall = Instantiate(PsyDrone.searchLaserSubPrefab, computedEndPosition, transform.rotation);
                if (endBall) endBall.transform.position = computedEndPosition;
            }
        }

        private void ApplyHitEffects(HitPointAndResult result)
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
                        Instantiate(PsyDrone.searchLaserHitEffect, body.transform.position, Quaternion.identity);
                    }
                }
            }
        }

        private void DetectHits()
        {
            float distance = Vector3.Distance(transform.position, computedPosition);
            bool hit = Physics.Raycast(transform.position, direction, out RaycastHit raycastHit, distance,
                                       LayerIndex.world.mask | LayerIndex.entityPrecise.mask | LayerIndex.defaultLayer.mask, QueryTriggerInteraction.UseGlobal);
            if (hit)
            {
                computedPosition = raycastHit.point;
                currentState = States.DamageEnemy;
            }
        }

        private enum States
        {
            StraightDecelerate,
            FindEnemy,
            HuntEnemy,
            DamageEnemy,
            DestroySelf
        }
    }
}