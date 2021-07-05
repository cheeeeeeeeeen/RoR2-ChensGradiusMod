using RoR2;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

namespace Chen.GradiusMod.Drones.PsyDrone
{
    internal class SearchLaserController : MonoBehaviour
    {
        private const float DefaultSpeed = 1f;
        private const float DecelerationStateDuration = 1f;
        private const float AccelerationRate = .03f;
        private const float DecelerateLeastSpeed = .1f;
        private const float MaxDetectionRange = 200f;
        private const float MaxAngleDetection = 180f;
        private const float MaxSpeed = 1f;
        private const float MaxSmoothCurveRate = 1f;
        private const float SmoothCurveRate = .005f;
        private const float Radius = 1f;
        private const float Force = 1f;

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

        private void Awake()
        {
            computedPosition = transform.position;
            direction = transform.forward;
            currentState = States.StraightDecelerate;
            timer = 0f;
            currentSpeed = DefaultSpeed;
            target = null;
            smoothCurveValue = 0f;
        }

        private void FixedUpdate()
        {
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
                new BlastAttack
                {
                    attacker = owner.gameObject,
                    inflictor = owner.gameObject,
                    teamIndex = teamComponent.teamIndex,
                    baseDamage = owner.damage,
                    baseForce = Force,
                    position = computedPosition,
                    radius = Radius,
                    attackerFiltering = AttackerFiltering.NeverHit,
                    falloffModel = BlastAttack.FalloffModel.None
                }.Fire();
            }
            currentState = States.DestroySelf;
            Destroy(transform.Find("Sphere").gameObject);
        }

        private void PerformDestroySelf()
        {
            if (!GetComponentInChildren<TrailRenderer>()) Destroy(gameObject);
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