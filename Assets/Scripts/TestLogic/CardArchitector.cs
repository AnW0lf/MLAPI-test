using MLAPI.NetworkVariable;
using System;

namespace Assets.Scripts.TestLogic
{
    public class CardArchitector : Architector
    {
        #region Network Variables

        public readonly NetworkVariableString Nickname = new NetworkVariableString(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, string.Empty);

        public readonly NetworkVariableInt IconIndex = new NetworkVariableInt(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, 0);

        public readonly NetworkVariableBool IsReady = new NetworkVariableBool(new NetworkVariableSettings
        {
            WritePermission = NetworkVariablePermission.OwnerOnly,
            ReadPermission = NetworkVariablePermission.Everyone
        }, false);

        #endregion Network Variables

        #region OnLocalChanged Actions

        private Action<string> _nicknameChanged = null;
        private Action<int> _iconIndexChanged = null;
        private Action<bool> _isReadyChanged = null;

        #endregion OnLocalChanged Actions

        protected override void InitializeActions()
        {
            _nicknameChanged = (value) => Nickname.Value = value;
            _iconIndexChanged = (value) => IconIndex.Value = value;
            _isReadyChanged = (value) => { IsReady.Value = value; };
        }

        protected override void Synchronize()
        {
            LocalCard localCard = Local as LocalCard;

            Nickname.Value = localCard.Nickname.Value;
            IconIndex.Value = localCard.IconIndex.Value;
            IsReady.Value = localCard.IsReady.Value;
        }

        protected override void Subscribe()
        {
            LocalCard localCard = Local as LocalCard;

            localCard.Nickname.ValueChanged += _nicknameChanged;
            localCard.IconIndex.ValueChanged += _iconIndexChanged;
            localCard.IsReady.ValueChanged += _isReadyChanged;
        }

        protected override void Unsubscribe()
        {
            LocalCard localCard = Local as LocalCard;

            localCard.Nickname.ValueChanged -= _nicknameChanged;
            localCard.IconIndex.ValueChanged -= _iconIndexChanged;
            localCard.IsReady.ValueChanged -= _isReadyChanged;
        }
    }
}