using Assets.Scripts.Lobby;
using MLAPI.NetworkVariable;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class RemoteCard : Remote
    {
        [SerializeField] private PlayerCard _card = null;

        CardArchitector cardArchitector = null;

        #region Remote Variables

        private RemoteString _nickname = null;
        private RemoteInt _iconIndex = null;
        private RemoteBool _isReady = null;

        #endregion Remote Variables

        protected override void InitializeVariables()
        {
            _nickname = new RemoteString(cardArchitector.Nickname.Value)
            {
                IsEquals = (target) => target == _card.Nickname
            };
            _nickname.SetValue += (value) => _card.Nickname = value;
            _card.Nickname = _nickname.Target;

            _iconIndex = new RemoteInt(cardArchitector.IconIndex.Value, 0)
            {
                IsEquals = (target) => target == _card.IconIndex,
                IsOverOffset = (target, offset) => Mathf.Abs(target - _card.IconIndex) > offset
            };
            _iconIndex.SetValue += (value) => _card.IconIndex = value;
            _iconIndex.UpdateValue += (value) => _card.IconIndex = value;
            _card.IconIndex = _iconIndex.Target;

            _isReady = new RemoteBool(cardArchitector.IsReady.Value)
            {
                IsEquals = (target) => target == _card.IsReady
            };
            _isReady.SetValue += (value) => _card.IsReady = value;
            _card.IsReady = _isReady.Target;
        }

        protected override void InitializeArchitector()
        {
            if (Architector == null) cardArchitector = null;
            else cardArchitector = Architector as CardArchitector;
        }

        protected override void UpdateVariables()
        {
            _nickname.Target = cardArchitector.Nickname.Value;
            _iconIndex.Target = cardArchitector.IconIndex.Value;
            _isReady.Target = cardArchitector.IsReady.Value;
        }
    }
}