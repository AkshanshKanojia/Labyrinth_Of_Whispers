using System;
using UnityEngine;

namespace FPS
{
    [CreateAssetMenu(fileName = "EnemyBehaviour", menuName = "FPS/EnemyBehaviour", order = 1)]
    public class EnemyBehaviourSO : ScriptableObject
    {
        #region Prperties
        [SerializeField] private EnemyType _enemyType;
        [SerializeField] private EnemyMovementData _movementData;
        [SerializeField] private EnemyAttackData _attackData;
        [SerializeField] private EnemyHealthData _healthData;

        #endregion

        #region Methods and Getters
        //add custom methdos as per need if any
        internal EnemyMovementData GetMovementData()
        {
            return _movementData;
        }

        internal EnemyAttackData GetAttackData()
        {
            return _attackData;
        }

        internal EnemyHealthData GetHealthData()
        {
            return _healthData;
        }

        internal EnemyType GetEnemyType()
        {
            return _enemyType;
        }

        #endregion
    }

    [Serializable]
    public class EnemyMovementData
    {
        public float patrolSpeed =2f;
        public float chaseSpeed = 4f;

        //add properties as required
    }

    [Serializable]
    public class EnemyAttackData
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
    public class EnemyHealthData
    {
        public bool isUnkillable = false;
        public float initialHealth = 100f;
        public bool recoverOvertime = false;
        public bool recoverOnHit = false;
        public float recoveryAmount = 5f;
        public float recoveryOvertimeDelay = 2f;//for example recover 5 hp per 2 seconds. Use theese props for special entities if any.
    }

    //expand furhter as you make system to define personalities etc and how  and what data it fetch froom enemy brain

    public enum EnemyType
    {
        Brute,
        SoundBug,
        MainEntity,
    }
}