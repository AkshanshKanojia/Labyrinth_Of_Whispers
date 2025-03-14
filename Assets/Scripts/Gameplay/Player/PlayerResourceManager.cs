using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FPS
{
    public class PlayerResourceManager : Singleton<PlayerResourceManager>
    {
        private PlayerResourceData _playerResourceData;

        private const string KEY_RESOURCES = "PlayerResources";

        #region Initialization
        private void Start()
        {
            Initialize();//todo: remove later once game flow is finalized
        }

        internal void Initialize()
        {
            LoadPlayerResources();//load default resources if any
        }

        #endregion

        #region Resource Methods
        internal void LoadPlayerResources()
        {
            string playerResourceData = PlayerPrefs.GetString(KEY_RESOURCES);
            if (!string.IsNullOrEmpty(playerResourceData))
            {
                PlayerResourceData playerResourceSaveData = JsonUtility.FromJson<PlayerResourceData>(playerResourceData);

                _playerResourceData = playerResourceSaveData;
            }
            else
            {
                //generate default resources
                _playerResourceData = new PlayerResourceData
                {
                    Resources = new List<PlayerResource>()
                };

                foreach (PlayerResourceType resourceType in Enum.GetValues(typeof(PlayerResourceType)))
                {
                    _playerResourceData.Resources.Add(new PlayerResource { ResourceType = resourceType, AmountOwned = 0 });
                }
            }
        }

        internal void SavePlayerResources()
        {
            string playerResourceData = JsonUtility.ToJson(_playerResourceData);
            PlayerPrefs.SetString(KEY_RESOURCES, playerResourceData);
        }

        internal void AddResource(PlayerResourceType resourceType, int amount)
        {
            PlayerResource resource = _playerResourceData.Resources.FirstOrDefault(x => x.ResourceType == resourceType);

            if (resource != null)
            {
                resource.AmountOwned += amount;
            }
            else
            {
                _playerResourceData.Resources.Add(new PlayerResource { ResourceType = resourceType, AmountOwned = amount });
            }

            SavePlayerResources();
        }

        internal bool HasSufficientResource(PlayerResourceType resourceType, int amount)
        {
            PlayerResource resource = _playerResourceData.Resources.FirstOrDefault(x => x.ResourceType == resourceType);
            return resource != null && resource.AmountOwned >= amount;
        }

        internal bool RemoveResource(PlayerResourceType resourceType, int amount)
        {
            PlayerResource resource = _playerResourceData.Resources.FirstOrDefault(x => x.ResourceType == resourceType);

            bool canRemove = HasSufficientResource(resourceType, amount);
            if (canRemove)
            {
                resource.AmountOwned -= amount;
                SavePlayerResources();
            }

            return canRemove;
        }
        #endregion
    }

    [Serializable]
    public class PlayerResourceData
    {
        public List<PlayerResource> Resources;
    }

    [Serializable]
    public class PlayerResource
    {
        public PlayerResourceType ResourceType;
        public int AmountOwned;
    }

    public enum PlayerResourceType
    {
        Scrap,
        Nails,
        DuctTape,
        GunPowder,
        Alcahol,
        Herb,
        Cloth,
        //modify as per need later
    }
}
