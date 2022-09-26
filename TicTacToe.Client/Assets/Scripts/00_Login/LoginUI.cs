using Assets.Scripts.Games;
using NetworkShared.Packets.ClientServer;
using NetworkShared.Packets.ServerClient;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using TTT.PacketHandlers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TTT.Login
{
    public class LoginUI : MonoBehaviour
    {
        [SerializeField] int _maxUsernameLength = 10;
        [SerializeField] int _maxPasswordLength = 10;

        private TMP_InputField _usernameInput;
        private Transform _usernameError;
        private TMP_InputField _passwordInput;
        private Transform _passwordError;
        private Transform _loginButton;
        private TextMeshProUGUI _loginText;
        private Transform _loadingUI;
        private Transform _loginError;

        private bool _isConnected;

        private string _username = string.Empty;
        private string _password = string.Empty;

        private string inputSelection;

        private void Start()
        {
            _loginButton = transform.Find("Login");
            _loginButton.GetComponent<Button>().onClick.AddListener(Login);
            _loginText = _loginButton.transform.Find("Text").GetComponent<TextMeshProUGUI>();

            _usernameInput = transform.Find("UsernameInput").GetComponent<TMP_InputField>();
            _usernameInput.onValueChanged.AddListener(UpdateUsername);
            _usernameError = _usernameInput.transform.Find("Error");

            _passwordInput = transform.Find("PasswordInput").GetComponent<TMP_InputField>();
            _passwordInput.onValueChanged.AddListener(UpdatePassword);
            _passwordError = _passwordInput.transform.Find("Error");

            _loginError = transform.Find("LoginError");
            _loadingUI = transform.Find("Loading");

            NetworkClient.Instance.OnServerConnected += SetIsConnected;
            OnAuthFailHandler.OnAuthFail += ShowLoginError;

            inputSelection = "password";
            IterateInputFields();
        }

        private void OnDestroy()
        {
            NetworkClient.Instance.OnServerConnected -= SetIsConnected;
            OnAuthFailHandler.OnAuthFail -= ShowLoginError;
        }

        private void Update()
        {
            HandleTabSelectFields();
        }

        void UpdateUsername(string value)
        {
            _username = value;
            ValidateAndUpdateUI();
        }

        void UpdatePassword(string value)
        {
            _password = value;
            ValidateAndUpdateUI();
        }

        private void SetIsConnected()
        {
            _isConnected = true;
        }

        private void ValidateAndUpdateUI()
        {
            var usernameRegex = Regex.Match(_username, "^[a-zA-Z0-9]+$");

            var interactable = 
                (!string.IsNullOrWhiteSpace(_username) && 
                !string.IsNullOrWhiteSpace(_password))&& 
                (_username.Length <= _maxUsernameLength && _password.Length <= _maxPasswordLength) &&
                usernameRegex.Success;

            EnableLoginButton(interactable);

            if (_password != null)
            {
                var passwordTooLong = _password.Length > _maxPasswordLength;
                _passwordError.gameObject.SetActive(passwordTooLong);
            }

            if (_username != null)
            {
                var usernameTooLong = _username.Length > _maxUsernameLength || !usernameRegex.Success;
                _usernameError.gameObject.SetActive(usernameTooLong);
            }
        }

        private void EnableLoginButton(bool interactable)
        {
            _loginButton.GetComponent<Button>().interactable = interactable;
            var color = _loginButton.GetComponent<Button>().interactable ? Color.white : Color.gray;
            _loginText.color = color;
        }

        private void Login()
        {
            StopCoroutine(LoginRoutine());
            StartCoroutine(LoginRoutine());
        }

        IEnumerator LoginRoutine()
        {
            HideLoginError();
            EnableLoginButton(false);
            LeanTween.cancelAll();
            LeanTween.reset(); // because of bug when restarting the scene!
            _loadingUI.gameObject.SetActive(true);

            NetworkClient.Instance.Connect();

            while (!_isConnected)
            {
                Debug.Log("WAITING!");
                yield return null;
            }

            var authRequest = new Net_AuthRequest
            {
                Username = _username,
                Password = _password
            };

            NetworkClient.Instance.SendServer(authRequest);
            GameManager.Instance.MyUsername = _username;
        }

        private void HandleTabSelectFields()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.Tab))
            {
                IterateInputFields();
            }
        }

        private void IterateInputFields()
        {
            if (inputSelection == "username")
            {
                EventSystem.current.SetSelectedGameObject(_passwordInput.gameObject, null);
                inputSelection = "password";
            }
            else if (inputSelection == "password")
            {
                EventSystem.current.SetSelectedGameObject(_usernameInput.gameObject, null);
                inputSelection = "username";
            }
        }

        private void ShowLoginError(Net_OnAuthFail msg)
        {
            EnableLoginButton(true);
            _loadingUI.gameObject.SetActive(false);
            _loginError.gameObject.SetActive(true);
        }

        private void HideLoginError()
        {
            _loginError.gameObject.SetActive(false);
        }
    }

}