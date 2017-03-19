using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

namespace MIKA {
    public class ServerManager : NetworkManager {
        public string ConnectionIP = "localhost";
        public int ConnectionPort = 7777;
        public bool isServer, isClient;

        public GameObject IK_Model;

        // Use this for initialization
        void Start() {
            if (SceneManager.GetActiveScene().name.ToLower().Contains("server"))
                StartCoroutine(StartUpServer());
            else
                JoinClient();

        }

        // Update is called once per frame
        void Update() {

        }


        private IEnumerator StartUpServer() {
            yield return new WaitForSeconds(1);

            //SetPort();
            StartServer();
            isServer = true;
        }

        private void JoinClient() {
            StartClient();
            isClient = true;
        }


        void SetIPAddress() {
            networkAddress = ConnectionIP;
        }
        void SetPort() {
            networkPort = ConnectionPort;
        }
    }
}