using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using System;

namespace MIKA {

    public class ServerManager : NetworkManager {
        private static ServerManager instance;
        public static ServerManager Instance { get { return instance; } }

        public string ConnectionIP = "localhost";
        public int ConnectionPort = 7777;
        public DiscoveryMode mode = DiscoveryMode.Server;
        public bool isServer, isClient;

        public GameObject IK_Model;
        public GameObject ballPrefab;

        #region unity callbacks
        private void Awake() {
            instance = this;
        }

        void Start() {
            NW_Discovery.EventStartServer += OnEventStartServer;
            NW_Discovery.EventFoundServer += OnEventFoundServer;
        }

        void Update() {

        }

        private void OnDestroy() {
            NW_Discovery.EventStartServer -= OnEventStartServer;
            NW_Discovery.EventFoundServer -= OnEventFoundServer;
        }


        public override void OnClientDisconnect(NetworkConnection conn) {
            base.OnClientConnect(conn);
            StopClient();
            SceneManager.LoadScene(0);
        }

        #endregion


        private void OnEventStartServer() {
            StartCoroutine(StartUpServer());
        }
        private void OnEventFoundServer(string ip, string data) {
            if (IsClientConnected())
                return;
            networkAddress = ip;
            JoinClient();
        }

        private IEnumerator StartUpServer() {
            yield return new WaitForSeconds(1);

            //SetPort();
            StartServer();
            isServer = true;


            GameObject go = Instantiate(ballPrefab);

            while (true) {
                NetworkPlayer np = FindObjectOfType<NetworkPlayer>();
                if (np != null) {
                    NetworkServer.SpawnWithClientAuthority(go, np.gameObject);
                    break;
                }
                print("Search for player");
                yield return new WaitForSeconds(1);
            }

        }

        private void JoinClient() {
            StartClient();
            isClient = true;
        }
    }
}

//private void ConnectViaSceneName() {
//    if (SceneManager.GetActiveScene().name.ToLower().Contains("server")) {
//        StartCoroutine(StartUpServer());
//    }
//    else {
//        JoinClient();
//        //StartCoroutine(CheckAndJoinClient());
//    }
//}
//private IEnumerator CheckAndJoinClient() {
//    bool conn = false;
//    while (!conn) {
//        if (!IsClientConnected()) {
//            JoinClient();
//            conn = true;
//        }
//        yield return new WaitForSeconds(2);
//    }
//}

