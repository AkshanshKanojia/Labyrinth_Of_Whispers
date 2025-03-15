using Extensions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FPS
{
    public class PlayerResourceManager : Singleton<PlayerResourceManager>
    {
        private PlayerResourceData _playerResourceData;

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
            _playerResourceData = SaveManager.GetResourceData();

            if (_playerResourceData == null)
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
            SaveManager.SetResourceData(_playerResourceData);
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
