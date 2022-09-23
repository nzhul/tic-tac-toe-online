using Assets.Scripts.Games;
using NetworkShared.Models;
using NetworkShared.Packets.ClientServer;
using NetworkShared.Packets.ServerClient;
using TMPro;
using TTT.PacketHandlers;
using UnityEngine;
using UnityEngine.UI;

namespace TTT.Game
{
    public class GameUI : MonoBehaviour
    {
        private Transform _surrenderBtn;
        private Transform _endRoundPanel;
        private TextMeshProUGUI _xUsername;
        private TextMeshProUGUI _xScoreText;
        private TextMeshProUGUI _oUsername;
        private TextMeshProUGUI _oScoreText;

        private int _xScore = 0;
        private int _oScore = 0;

        private void Start()
        {
            var header = transform.Find("Header");
            _xUsername = header.Find("xUsername").GetComponent<TextMeshProUGUI>();
            _xScoreText = header.Find("xScore").GetComponent<TextMeshProUGUI>();
            _oUsername = header.Find("oUsername").GetComponent<TextMeshProUGUI>();
            _oScoreText = header.Find("oScore").GetComponent<TextMeshProUGUI>();

            _surrenderBtn = transform.Find("Footer").Find("SurrenderBtn");
            _surrenderBtn.GetComponent<Button>().onClick.AddListener(Surrender);
            _endRoundPanel = transform.Find("EndRound");

            OnEndRoundHandler.OnEndRound += DisplayEndRoundUI;

            InitHeader();
        }

        private void InitHeader()
        {
            var game = GameManager.Instance.ActiveGame;
            _xUsername.text = game.XUser;
            _oUsername.text = game.OUser;
        }

        private void OnDestroy()
        {
            OnEndRoundHandler.OnEndRound -= DisplayEndRoundUI;
        }

        private void DisplayEndRoundUI(Net_OnEndRound msg)
        {
            _endRoundPanel.gameObject.SetActive(true);

            var isWin = GameManager.Instance.MyUsername == msg.Winner;
            _endRoundPanel.GetComponent<EndRoundUI>().Init(isWin);

            var playerType = GameManager.Instance.ActiveGame.GetPlayerType(msg.Winner);
            if (playerType == PlayerType.X)
            {
                _xScore++;
                _xScoreText.text = _xScore.ToString();
            }
            else
            {
                _oScore++;
                _oScoreText.text = _oScore.ToString();
            }
        }

        private void Surrender()
        {
            var msg = new Net_SurrenderRequest();
            NetworkClient.Instance.SendServer(msg);
        }
    }
}
