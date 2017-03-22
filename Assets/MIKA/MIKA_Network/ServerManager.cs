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

        void Start() {
            if (SceneManager.GetActiveScene().name.ToLower().Contains("server")) {
                StartCoroutine(StartUpServer());
            }
            else {
                JoinClient(); 
                //StartCoroutine(CheckAndJoinClient());
            }
        }

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


        void SetIPAddress() {
            networkAddress = ConnectionIP;
        }
        void SetPort() {
            networkPort = ConnectionPort;
        }
    }
}