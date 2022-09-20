using Assets.Scripts.PacketHandlers;
using NetworkShared.Packets.ClientServer;
using NetworkShared.Packets.ServerClient;
using NetworkShared.Shared.Packets.ClientServer;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TTT.Lobby
{
    public class LobbyUI : MonoBehaviour
    {
        [SerializeField] GameObject _playerRowPrefab;

        private TextMeshProUGUI _playersOnlineLabel;
        private Button _findOpponentBtn;
        private Transform _loadingUI;
        private Transform _cancelBtn;
        private Transform _topPlayersContainer;

        private void Start()
        {
            _topPlayersContainer = transform.Find("TopPlayersContainer");
            _playersOnlineLabel = transform.Find("playersOnlineLbl").GetComponent<TextMeshProUGUI>();
            _findOpponentBtn = transform.Find("FindOpponentBtn").GetComponent<Button>();
            _findOpponentBtn.onClick.AddListener(FindOpponent);
            _loadingUI = transform.Find("Loading");
            _cancelBtn = _loadingUI.Find("CancelBtn");

            OnServerStatusRequestHandler.OnServerStatus += RefreshUI;
            RequestServerStatus();
        }

        private void FindOpponent()
        {
            // Disable and Hide FindOpponent button
            // Show loading spinner

            _findOpponentBtn.gameObject.SetActive(false);
            _loadingUI.gameObject.SetActive(true);

            var msg = new Net_FindOpponentRequest();
            NetworkClient.Instance.SendServer(msg);

            // wait for OnGameStart
        }

        private void RefreshUI(Net_OnServerStatus msg)
        {
            _playersOnlineLabel.text = $"{msg.PlayersCount} players online";
            for (int i = 0; i < msg.TopPlayers.Length; i++)
            {
                var player = msg.TopPlayers[i];
                var instance = Instantiate(_playerRowPrefab, _topPlayersContainer).GetComponent<PlayerRowUI>();
                instance.Init(player);
            }
        }

        private void RequestServerStatus()
        {
            var msg = new Net_ServerStatusRequest();
            NetworkClient.Instance.SendServer(msg);
        }
    }
}
