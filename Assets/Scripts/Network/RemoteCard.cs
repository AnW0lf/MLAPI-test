using Assets.Scripts.Lobby;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class RemoteCard : Remote
    {
        [SerializeField] private PlayerCard _card = null;

        #region Remote Variables

        private RemoteString _nickname = null;
        private RemoteInt _iconIndex = null;
        private RemoteBool _isReady = null;

        #endregion Remote Variables

        #region Remote Actions

        private NetworkVariableString.OnValueChangedDelegate _nicknameChanged;
        private NetworkVariableInt.OnValueChangedDelegate _iconIndexChanged;
        private NetworkVariableBool.OnValueChangedDelegate _isReadyChanged;

        #endregion Remote Actions

        protected override void InitializeVariables()
        {
            CardArchitector cardArchitector = Architector as CardArchitector;

            _card.Style = PlayerCardStyle.NOTOWNER;

            _nickname = new RemoteString(cardArchitector.Nickname.Value)
            {
                IsEquals = (target) => target == _card.Nickname
            };
            _nickname.SetValue += (value) => _card.Nickname = value;

            _iconIndex = new RemoteInt(cardArchitector.IconIndex.Value, 0)
            {
                IsEquals = (target) => target == _card.IconIndex,
                IsOverOffset = (target, offset) => Mathf.Abs(target - _card.IconIndex) > offset
            };
            _iconIndex.SetValue += (value) => _card.IconIndex = value;
            _iconIndex.UpdateValue += (value) => _card.IconIndex = value;

            _isReady = new RemoteBool(cardArchitector.IsReady.Value)
            {
                IsEquals = (target) => target == _card.IsReady
            };
            _isReady.SetValue += (value) => _card.IsReady = value;
        }

        protected override void InitializeArchitectorActions()
        {
            _nicknameChanged = (oldValue, newValue) => _nickname.Target = newValue;
            _iconIndexChanged = (oldValue, newValue) => _iconIndex.Target = newValue;
            _isReadyChanged = (oldValue, newValue) => _isReady.Target = newValue;
        }

        protected override void Subscribe()
        {
            CardArchitector cardArchitector = Architector as CardArchitector;

            cardArchitector.Nickname.OnValueChanged += _nicknameChanged;
            cardArchitector.IconIndex.OnValueChanged += _iconIndexChanged;
            cardArchitector.IsReady.OnValueChanged += _isReadyChanged;
        }

        protected override void Unsubscribe()
        {
            CardArchitector cardArchitector = Architector as CardArchitector;

            cardArchitector.Nickname.OnValueChanged -= _nicknameChanged;
            cardArchitector.IconIndex.OnValueChanged -= _iconIndexChanged;
            cardArchitector.IsReady.OnValueChanged -= _isReadyChanged;
        }
    }
}