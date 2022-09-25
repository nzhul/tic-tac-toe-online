using Assets.Scripts.Games;
using Assets.Scripts.PacketHandlers;
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

            OnSurrenderHandler.OnSurrender += HandleSurrender;
            OnQuitGameHandler.OnQuitGame += HandleOpponentLeft;
            OnMarkCellHandler.OnMarkCell += HandleMarkCell;

            InitHeader();
        }

        private void HandleOpponentLeft(Net_OnQuitGame msg)
        {
            if (!_endRoundPanel.gameObject.activeSelf)
            {
                _endRoundPanel.gameObject.SetActive(true);
                _endRoundPanel.GetComponent<EndRoundUI>().HandleOpponentLeft(msg);
            }
        }

        private void InitHeader()
        {
            var game = GameManager.Instance.ActiveGame;
            _xUsername.text = "[X] " + game.XUser;
            _oUsername.text = "[O] " + game.OUser;
        }

        private void OnDestroy()
        {
            OnSurrenderHandler.OnSurrender -= HandleSurrender;
            OnQuitGameHandler.OnQuitGame -= HandleOpponentLeft;
            OnMarkCellHandler.OnMarkCell -= HandleMarkCell;
        }

        private void HandleMarkCell(Net_OnMarkCell msg)
        {
            if (msg.Outcome != MarkOutcome.None)
            {

                // TODO: Trigger WIN ANIMATION! Display EndScreen Only after Delay!
                var isDraw = msg.Outcome == MarkOutcome.Draw;
                DisplayEndRoundUI(msg.Actor, isDraw);
            }
        }

        private void HandleSurrender(Net_OnSurrender msg)
        {
            DisplayEndRoundUI(msg.Winner, false);
        }

        private void DisplayEndRoundUI(string winnerId, bool isDraw)
        {
            _endRoundPanel.gameObject.SetActive(true);
            _endRoundPanel.GetComponent<EndRoundUI>().Init(winnerId, isDraw);

            var playerType = GameManager.Instance.ActiveGame.GetPlayerType(winnerId);
            if (playerType == MarkType.X)
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
