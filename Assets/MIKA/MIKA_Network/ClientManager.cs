using UnityEngine;
using System.Collections;

namespace MIKA {
    public class ClientManager : MonoBehaviour {


        void Start() {
            //InvokeRepeating("TryConnecting", 2, 2.5f);

        }

        // Update is called once per frame
        void Update() {
            //if (!ServerManager.Instance.isNetworkActive) {
            //    GetComponent<ServerManager>().JoinGame();
            //}
        }

        void TryConnecting() {
            //if (ServerManager.Instance.isNetworkActive) {
            //    //CancelInvoke("TryConnecting");
            //    return;
            //}
            //print("Try connecting");
            //GetComponent<ServerManager>().JoinGame();
        }
    }
}