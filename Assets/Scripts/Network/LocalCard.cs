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

        private void OnDestroy()
        {
            UnsubscribeFromCard();
        }

        protected override void InitializeVariables()
        {
            _card.Style = PlayerCardStyle.OWNER;

            Nickname = new LocalString(_card.Nickname);
            IconIndex = new LocalInt(_card.IconIndex, 0);
            IsReady = new LocalBool(_card.IsReady);

            SubscribeToCard();
        }

        private bool _subscribedToCard = false;
        private void SubscribeToCard()
        {
            if (_card == null) { return; }
            if (_subscribedToCard) { return; }

            _card.NicknameChanged += OnNicknameChanged;
            _card.IconIndexChanged += OnIconIndexChanged;
            _card.IsReadyChanged += OnIsReadyChanged;

            _subscribedToCard = true;
        }

        private void UnsubscribeFromCard()
        {
            if (_card == null) { return; }
            if (_subscribedToCard == false) { return; }

            _card.NicknameChanged -= OnNicknameChanged;
            _card.IconIndexChanged -= OnIconIndexChanged;
            _card.IsReadyChanged -= OnIsReadyChanged;

            _subscribedToCard = false;
        }

        private void OnNicknameChanged(string nickname)
        {
            Nickname.Value = nickname;
        }

        private void OnIconIndexChanged(int iconIndex)
        {
            IconIndex.Value = iconIndex;
        }

        private void OnIsReadyChanged(bool isReady)
        {
            IsReady.Value = isReady;
        }
    }
}