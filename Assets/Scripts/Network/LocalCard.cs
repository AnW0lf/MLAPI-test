using Assets.Scripts.Lobby;
using UnityEngine;

namespace Assets.Scripts.Network
{
    public class LocalCard : Local
    {
        [Header("Card")]
        [SerializeField] private PlayerCard _card = null;
        public PlayerCard Card => _card;

        #region Local Variables

        public LocalString Nickname = null;
        public LocalInt IconIndex = null;
        public LocalBool IsReady = null;

        #endregion Local Variables

        protected override void InitializeVariables()
        {
            _card.Style = PlayerCardStyle.OWNER;

            Nickname = new LocalString(_card.Nickname);
            IconIndex = new LocalInt(_card.IconIndex, 0);
            IsReady = new LocalBool(_card.IsReady);
        }

        protected override void UpdateVariables()
        {
            Nickname.Value = _card.Nickname;
            IconIndex.Value = _card.IconIndex;
            IsReady.Value = _card.IsReady;
        }
    }
}