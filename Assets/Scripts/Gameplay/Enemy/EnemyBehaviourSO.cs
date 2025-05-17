using System;
using UnityEngine;

namespace FPS
{
    [CreateAssetMenu(fileName = "EnemyBehaviour", menuName = "FPS/EnemyBehaviour", order = 1)]
    public class EnemyBehaviourSO : ScriptableObject
    {
        #region Properties
        [SerializeField] private EnemyType _enemyType;
        [SerializeField] private EntityBehaviorData _entityBehaviorData;

        #endregion

        #region Methods and Getters
        //add custom methdos as per need if any
        internal EntityBehaviorData GetEntityData()
        {
            return _entityBehaviorData;
        }
        

        internal EnemyType GetEnemyType()
        {
            return _enemyType;
        }

        #endregion
    }

    //expand furhter as you make system to define personalities etc and how  and what data it fetch froom enemy brain

    public enum EnemyType
    {
        Brute,
        SoundBug,
        MainEntity,
    }
}