using System;
using UnityEngine;
using UnityEngine.AI;

namespace FPS
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EntityMovementBase : MonoBehaviour
    {
        #region Variables
        internal NavMeshAgent navMeshAgent;
        internal bool isMoving = false;

        private EntityMovementData _entityMovementData;
        #endregion

        #region Actions and delegates
        internal Action onMovementBegin;
        internal Action onMovementEnd;

        #endregion

        #region  Initialization
        internal void Initialize(EntityMovementData movementData)
        {
            _entityMovementData = movementData;

            InitializeDefaultNavmeshBehaviour();
        }

        private void InitializeDefaultNavmeshBehaviour()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();

            navMeshAgent.speed = _entityMovementData.defualtMovementState switch
            {
                EntityMovementState.Idle => _entityMovementData.patrolSpeed,
                EntityMovementState.Patrol => _entityMovementData.patrolSpeed,
                EntityMovementState.Chase => _entityMovementData.chaseSpeed,
                EntityMovementState.Flee => _entityMovementData.fleeSpeed,
                _ => navMeshAgent.speed
            };
        }
        #endregion

        #region Unity Methods
        private void Update()
        {
            CheckForDestinationEnd();
        }
        #endregion

        #region Movement Methods
        private void CheckForDestinationEnd()
        {
            if(!isMoving)
            {
                return;
            }

            if (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance)
            {
                isMoving = false;
                onMovementEnd?.Invoke();
            }
        }
        internal void MoveTo(Vector3 targetPosition)
        {
            navMeshAgent.SetDestination(targetPosition);
            isMoving = true;
            onMovementBegin?.Invoke();
        }

        internal void UpdateMovementSpeed(float speed)
        {
            //additionally update default movement data speed if needed at any instance
            navMeshAgent.speed = speed;
        }


        //expand methods as per need such as flee behaviour etc
        #endregion
    }
}
