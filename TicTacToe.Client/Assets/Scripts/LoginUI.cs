using Assets.Scripts;
using Assets.Scripts.Packets.ClientServer;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class LoginUI : MonoBehaviour
{
    private Button _loginButton;
    private bool _isConnectedToServer;

    private void Start()
    {
        _loginButton = transform.Find("Login").GetComponent<Button>();
        _loginButton.onClick.AddListener(Login);
        NetworkClient.Instance.OnServerConnected += Instance_OnServerConnected;
    }

    private void Instance_OnServerConnected()
    {
        SetIsConnected(true);
    }

    private void Login()
    {
        StopCoroutine(LoginRoutine());
        StartCoroutine(LoginRoutine());
    }

    IEnumerator LoginRoutine()
    {
        NetworkClient.Instance.Connect();

        // dido: I need to wait for OnPeerConnected(), "[CLIENT] We connected to server " + peer.EndPoint ....  to complete first. Then we can send AuthRequest.
        // I can do that using Event.

        // I can try to login using Coroutine
        // 1. Connect to the server
        // 2. wait for server to response on OnPeerConnected
        // 3. Send AuthRequest
        // 4. Wait for server to response with OnAuthRequest
        // 5. Load another scene!

        while (!_isConnectedToServer)
        {
            Debug.Log("WAITING!!!");
            // TODO: Block the login button and add Loading/Spinner
            yield return null;
        }


        Debug.Log("Sending Auth Request!!");

        var authRequest = new Net_AuthRequest
        {
            Username = "dido",
            Password = "1234"
        };
        NetworkClient.Instance.SendServer(authRequest);
    }

    public void SetIsConnected(bool isConnected)
    {
        _isConnectedToServer = isConnected;
    }
}
