using System;
using UnityEngine;
using UnityEngine.AI;

namespace FPS
{
    [RequireComponent(typeof(EntityMovementBase))]
    public class EntityBase : MonoBehaviour, IEntity
    {
        #region Variables
        protected NavMeshAgent NavMeshAgent => entityMovementBase.navMeshAgent;
        protected EntityMovementBase entityMovementBase;
        protected EntityBehaviorData entityBehaviorData;
        #endregion

        #region Actions and delegates
        protected Action onAgentTrackingBegin;
        protected Action onAgentTrackingEnd;

        #endregion

        #region  Initialization
        internal void InitializeEntity(EntityBehaviorData behaviorData)
        {
            entityBehaviorData = behaviorData;

            entityMovementBase = GetComponent<EntityMovementBase>();

            entityMovementBase.Initialize(entityBehaviorData.movementData);
            //add further as calss grows or requirments
        }


        #endregion

        #region Virtual Methods
        public virtual void OnSpotted()
        {
            //added in case we expand furhter of attack base, currently not used. Inhertis interface here as well no need to inherit on subclass 
        }

        #endregion
    }
}
