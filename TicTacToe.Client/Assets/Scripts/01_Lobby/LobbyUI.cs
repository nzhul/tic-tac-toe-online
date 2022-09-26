using NetworkShared.Packets.ClientServer;
using NetworkShared.Packets.ServerClient;
using TMPro;
using TTT.PacketHandlers;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        private Transform _logoutBtn;

        private void Start()
        {
            _topPlayersContainer = transform.Find("TopPlayersContainer");
            _playersOnlineLabel = transform.Find("playersOnlineLbl").GetComponent<TextMeshProUGUI>();
            _findOpponentBtn = transform.Find("FindOpponentBtn").GetComponent<Button>();
            _findOpponentBtn.onClick.AddListener(FindOpponent);
            _loadingUI = transform.Find("Loading");
            _cancelBtn = _loadingUI.Find("CancelBtn");
            _cancelBtn.GetComponent<Button>().onClick.AddListener(CancelFindOpponent);
            _logoutBtn = transform.Find("Footer").Find("LogoutBtn");
            _logoutBtn.GetComponent<Button>().onClick.AddListener(Logout);

            OnServerStatusRequestHandler.OnServerStatus += RefreshUI;
            RequestServerStatus();
        }

        private void OnDestroy()
        {
            OnServerStatusRequestHandler.OnServerStatus -= RefreshUI;
        }

        private void FindOpponent()
        {
            // Disable and Hide FindOpponent button
            // Show loading spinner

            LeanTween.cancelAll();
            LeanTween.reset(); // because of bug when restarting the scene!
            _findOpponentBtn.gameObject.SetActive(false);
            _loadingUI.gameObject.SetActive(true);

            var msg = new Net_FindOpponentRequest();
            NetworkClient.Instance.SendServer(msg);

            // wait for OnGameStart
        }

        private void CancelFindOpponent()
        {
            _findOpponentBtn.gameObject.SetActive(true);
            _loadingUI.gameObject.SetActive(false);

            var msg = new Net_CancelFindOpponentRequest();
            NetworkClient.Instance.SendServer(msg);
        }

        private void Logout()
        {
            NetworkClient.Instance.Disconnect();
            SceneManager.LoadScene("00_Login");
        }

        private void RefreshUI(Net_OnServerStatus msg)
        {
            while (_topPlayersContainer.childCount > 0)
            {
                DestroyImmediate(_topPlayersContainer.GetChild(0).gameObject);
            }

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
