using Chen.GradiusMod.Compatibility;
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
        public const float BaseForce = 1f;

        private const float DefaultSpeed = 1f;
        private const float DecelerationStateDuration = 1f;
        private const float AccelerationRate = .03f;
        private const float DecelerateLeastSpeed = .1f;
        private const float MaxDetectionRange = 100f;
        private const float MaxAngleDetection = 180f;
        private const float MaxSpeed = 1f;
        private const float MaxSmoothCurveRate = 1f;
        private const float DefaultSmoothCurveRate = .005f;
        private const float Radius = 2f;
        private const float DamageMultiplier = 2f;
        private const float SubRadius = 8f;
        private const float SubForceMultiplier = 15f;
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

        public float smoothCurveRate
        {
            get
            {
                if (_smoothCurveRate == null) _smoothCurveRate = DefaultSmoothCurveRate;
                return (float)_smoothCurveRate;
            }
            set => _smoothCurveRate = value;
        }

        public float acceleration
        {
            get
            {
                if (_acceleration == null) _acceleration = AccelerationRate;
                return (float)_acceleration;
            }
            set => _acceleration = value;
        }

        public float maximumSpeed
        {
            get
            {
                if (_maximumSpeed == null) _maximumSpeed = MaxSpeed;
                return (float)_maximumSpeed;
            }
            set => _maximumSpeed = value;
        }

        private TeamComponent teamComponent { get => _teamComponent ? _teamComponent : owner.teamComponent; set => _teamComponent = value; }
        private Vector3 direction { get => _direction.normalized; set => _direction = value.normalized; }

        private CharacterBody _owner;
        private TeamComponent _teamComponent;
        private Vector3 _direction;
        private float? _damage;
        private float? _force;
        private float? _smoothCurveRate;
        private float? _acceleration;
        private float? _maximumSpeed;
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
            if (!expired && age >= Expiration) SetState(States.DamageEnemy);
            switch (currentState)
            {
                case States.StraightDecelerate:
                    PerformStraightDecelerate();
                    DetectHits();
                    break;

                case States.FindEnemy:
                    PerformFindEnemy();
                    DetectHits();
                    break;

                case States.HuntEnemy:
                    PerformHuntEnemy();
                    DetectHits();
                    break;

                case States.DamageEnemy:
                    PerformDamageEnemy();
                    break;

                case States.DestroySelf:
                    PerformDestroySelf();
                    break;
            }
            ManageEndBall();
        }

        private void Update()
        {
            transform.position = computedPosition;
            if (endBall) endBall.transform.position = computedEndPosition;
        }

        private void PerformStraightDecelerate()
        {
            currentSpeed = Mathf.Max(currentSpeed - acceleration, DecelerateLeastSpeed);
            computedPosition += direction * currentSpeed;
            if (currentSpeed <= DecelerateLeastSpeed)
            {
                timer += Time.fixedDeltaTime;
                if (timer >= DecelerationStateDuration) SetState(States.FindEnemy);
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
            if (target) SetState(States.HuntEnemy);
            computedPosition += direction * currentSpeed;
        }

        private void PerformHuntEnemy()
        {
            if (target)
            {
                HealthComponent targetHealth = target.healthComponent;
                if (targetHealth && targetHealth.alive)
                {
                    currentSpeed = Mathf.Min(currentSpeed + acceleration, maximumSpeed);
                    Vector3 directionToEnemy = (target.transform.position - computedPosition).normalized;
                    direction = Vector3.Lerp(direction, directionToEnemy, smoothCurveValue);
                    smoothCurveValue = Mathf.Min(smoothCurveValue + smoothCurveRate, MaxSmoothCurveRate);
                    int layerMask = LayerIndex.world.mask | LayerIndex.entityPrecise.mask | LayerIndex.defaultLayer.mask;
                    bool hit = Physics.Raycast(computedPosition, direction, out RaycastHit raycastHit, currentSpeed,
                                               layerMask, QueryTriggerInteraction.UseGlobal);
                    if (hit)
                    {
                        computedPosition = raycastHit.point;
                        SetState(States.DamageEnemy);
                    }
                    else computedPosition += direction * currentSpeed;
                    return;
                }
            }
            timer = 0f;
            smoothCurveValue = 0f;
            SetState(States.StraightDecelerate);
            PerformStraightDecelerate();
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
                    baseDamage = damage * DamageMultiplier,
                    baseForce = force,
                    position = computedPosition,
                    radius = Radius,
                    attackerFiltering = AttackerFiltering.NeverHitSelf,
                    falloffModel = FalloffModel.None
                }.InformativeFire();
                ApplyHitEffects(result);
                TriggerArmsRace();
            }
            SetState(States.DestroySelf);
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
                if (NetworkServer.active && owner)
                {
                    new BlastAttack
                    {
                        attacker = owner.gameObject,
                        inflictor = owner.gameObject,
                        teamIndex = teamComponent.teamIndex,
                        baseDamage = damage,
                        baseForce = force * SubForceMultiplier,
                        position = computedPosition,
                        radius = SubRadius,
                        attackerFiltering = AttackerFiltering.NeverHitSelf,
                        falloffModel = FalloffModel.None
                    }.Fire();
                    TriggerArmsRace();
                }
                Instantiate(PsyDrone.searchLaserSubExplosion, computedPosition, Quaternion.identity);
            }
        }

        private void ManageEndBall()
        {
            if (trailRenderer && trailRenderer.positionCount > 0)
            {
                computedEndPosition = trailRenderer.GetPosition(0);
                if (!endBall) endBall = Instantiate(PsyDrone.searchLaserSubPrefab, computedEndPosition, transform.rotation);
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
                SetState(States.DamageEnemy);
            }
        }

        private void TriggerArmsRace()
        {
            if (ChensClassicItems.enabled)
            {
                ChensClassicItems.TriggerArtillery(owner, damage, owner.RollCrit());
            }
        }

        private void SetState(States newState)
        {
            switch (newState)
            {
                case States.FindEnemy:
                    timer = 0;
                    break;

                case States.DamageEnemy:
                    expired = true;
                    break;

                case States.StraightDecelerate:
                case States.HuntEnemy:
                case States.DestroySelf:
                    break;
            }
            currentState = newState;
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