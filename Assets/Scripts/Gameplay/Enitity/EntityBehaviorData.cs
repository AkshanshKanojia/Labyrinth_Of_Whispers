using System;
using UnityEngine;

namespace FPS
{
    [Serializable]
    public sealed class EntityBehaviorData
    {
        public EntityMovementData movementData;
        public EntityAttackData attackData;
        public EntityHealthData healthData;

        //add properties as per need
    }

    [Serializable]
    public class EntityMovementData
    {
        public EntityMovementState defualtMovementState = EntityMovementState.Idle;

        public float patrolSpeed = 2f;
        public float chaseSpeed = 4f;
        public float fleeSpeed = 6f;

        //add properties as required
    }

    [Serializable]
    public class EntityAttackData
    {
        public float attackRange = 2f;
        public float attackDamage = 10f;
        public float attackCooldown = 1f;
        public float attackDuration = 0.5f;//use for attacks like continuos ranged/active deflay for attack activation on hit etc
        public float attackChargeDelay = 0.2f;
        public float selfStunDurationAfterAttck = 0.5f;
        //add properties as required
    }

    [Serializable]
    public class EntityHealthData
    {
        public bool isUnkillable = false;
        public float initialHealth = 100f;
        public bool recoverOvertime = false;
        public bool recoverOnHit = false;
        public float recoveryAmount = 5f;
        public float recoveryOvertimeDelay = 2f;//for example recover 5 hp per 2 seconds. Use theese props for special entities if any.
    }

    public enum EntityMovementState
    {
        Idle,
        Patrol,
        Chase,
        Flee,
    }
}
