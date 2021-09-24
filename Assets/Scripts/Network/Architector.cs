using MLAPI;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class Architector : NetworkBehaviour
    {
        [SerializeField] protected Local _localPrefab = null;
        [SerializeField] protected Remote _remotePrefab = null;

        public Local Local { get; protected set; } = null;
        public Remote Remote { get; protected set; } = null;
        public Transform Body
        {
            get
            {
                if (IsLocalSpawned.Value == false) return null;
                if (Local != null) return Local.transform;
                if (Remote != null) return Remote.transform;
                return null;
            }
        }

        public readonly NetworkVariableBool IsLocalSpawned = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);

        #region Local

        public void SpawnLocal(Transform parent = null)
        {
            DeleteLocal();

            Local = Instantiate(_localPrefab, parent);
            Local.Architector = this;
            Local.Initialize();
            SubscribeToLocal();
            SynchronizeWithLocal();

            IsLocalSpawned.Value = Local != null;
        }

        public void DeleteLocal()
        {
            if (Local == null) { return; }

            UnsubscribeFromLocal();
            Destroy(Local.gameObject);
            Local = null;

            IsLocalSpawned.Value = Local != null;
        }

        protected virtual void InitializeActions() { }

        private bool _localActionsInitialized = false;
        private void InitializeLocalActions()
        {
            if (_localActionsInitialized) { return; }

            InitializeActions();

            _localActionsInitialized = true;
        }

        public void Initialize()
        {
            InitializeLocalActions();
        }

        protected virtual void Synchronize() { }

        private void SynchronizeWithLocal()
        {
            if (Local == null) { return; }

            Synchronize();
        }

        protected virtual void Subscribe() { }
        protected virtual void Unsubscribe() { }

        private bool _subscribedToLocal = false;
        private void SubscribeToLocal()
        {
            if (Local == null) { return; }
            if (_subscribedToLocal) { return; }

            Subscribe();

            _subscribedToLocal = true;
        }

        private void UnsubscribeFromLocal()
        {
            if (Local == null) { return; }
            if (_subscribedToLocal == false) { return; }

            Unsubscribe();

            _subscribedToLocal = false;
        }

        #endregion Local

        #region Remote

        public void SpawnRemote(Transform container = null)
        {
            DeleteRemote();

            Remote = Instantiate(_remotePrefab, container);
            Remote.Architector = this;
            Remote.Initialize();
        }

        public void DeleteRemote()
        {
            if (Remote == null) { return; }

            Remote.Architector = null;
            Destroy(Remote.gameObject);
            Remote = null;
        }

        #endregion Remote
    }
}