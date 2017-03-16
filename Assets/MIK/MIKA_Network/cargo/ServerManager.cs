using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Xml;

public class ServerManager : NetworkManager {

    public string ConnectionIP;
    public int ConnectionPort = 7777;
    public bool ClientConnected = false;
    //public TextMesh debugTextClient;
    public static ServerManager Instance = null;              //Static instance of GameManager which allows it to be accessed by any other script.
    public bool isServer;
    public bool isClient;
    public Text debugTextServer;
    public GameObject SoundManager;
    public GameObject CanonStationLeft;
    public GameObject CanonStationRight;
    public GameObject TargetTransform;
    public GameObject RotationTransform;
    public GameObject SteeringStation;
    public GameObject PublicPlayer;
    public GameObject PickUp;

    public NetworkPlayer[] playerClients;

    //   void Awake()
    //{
    //       playerClients = new NetworkPlayer[2];
    //       //Check if instance already exists
    //       if (Instance == null)

    //		//if not, set instance to this
    //		Instance = this;

    //	//If instance already exists and it's not this:
    //	else if (Instance != this)

    //		//Then destroy this. This enforces our singleton pattern, meaning there can only ever be one instance of a GameManager.
    //		Destroy(gameObject);

    //	//Sets this to not be destroyed when reloading scene
    //	//DontDestroyOnLoad(gameObject);

    //       //Register this script to delegates
    //       Stage1_Logic.Stage1Done += OnStage1Done;
    //}

    //void Update()
    //{
    //       // !!! better not - may cause accidental shutdowns !!!
    //       //if(Input.GetKeyDown(KeyCode.Escape))
    //       //    {
    //       //        StopServer();
    //       //        NetworkServer.Reset();
    //       //        Application.Quit();
    //       //    }
    //   }

    //   public void RestartApplication()
    //   {
    //       if (!isServer)
    //           return;

    //       foreach (NetworkPlayer np in FindObjectsOfType<NetworkPlayer>())
    //       {
    //           np.RpcRestartApplication();
    //       }

    //       //Network.Disconnect();

    //       StartCoroutine(RestartDelayed());
    //   }

    //   IEnumerator RestartDelayed()
    //   {
    //       yield return new WaitForSeconds(0.5f);
    //       //StopHost();
    //       StopServer();
    //       NetworkServer.Reset();
    //       SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    //   }

    //   public void SpawnEntityAtPrefabPosition(GameObject prefab) {
    //       GameObject obj = Instantiate(prefab) as GameObject;
    //       NetworkServer.Spawn(obj);
    //   }

    //   public void SpawnEntity(GameObject prefab)
    //   {
    //       GameObject obj = Instantiate(prefab, new Vector3(-6000, Random.Range(200, 2000), Random.Range(-1500, 1500)), Quaternion.identity) as GameObject;
    //       NetworkServer.Spawn(obj);
    //   }

    //   public GameObject SpawnEntityAt(GameObject prefab, Vector3 spawnPosition, Quaternion spawnRotation)
    //   {
    //       if (!isServer)
    //           return null;
    //       if(!NetworkServer.active)
    //           return null;

    //       GameObject obj = Instantiate(prefab, spawnPosition, spawnRotation) as GameObject;
    //       NetworkServer.Spawn(obj);

    //       return obj;
    //   }


    //   public void SpawnPickUp()
    //   {
    //       SpawnPickUp(new Vector3(Random.Range(0, 9), Random.Range(3, 4), Random.Range(4, 9)));
    //   }
    //   public void SpawnPickUp(Vector3 position)
    //   {
    //       ServerManager.Instance.SpawnEntityAt(PickUp.gameObject, position, Quaternion.identity);
    //   }


    //   public void SpawnPublicPlayer(MaSineTrackedPlayer tp) {
    //       if (!isServer)
    //           return;

    //       TrackedPlayerNetworkBehaviour tpnb = tp.GetComponent<TrackedPlayerNetworkBehaviour>();
    //       if (tpnb != null) {
    //           if (tpnb.ControlledPlayer != null) {
    //               return;
    //           }
    //       }

    //       GameObject obj = Instantiate(PublicPlayer, tp.transform.position, Quaternion.identity) as GameObject;
    //       tp.PublicPlayer = obj.GetComponent<PublicPlayer>();

    //       obj.GetComponent<PublicPlayer>().ControllingPlayer = tp;

    //       if (!NetworkServer.active)  // prevent exception at restart
    //           return;
    //       NetworkServer.Spawn(obj);
    //   }


    //   void OnStage1Done()
    //   {
    //       SpawnEntityAtPrefabPosition(SteeringStation);

    //       foreach (NetworkPlayer np in FindObjectsOfType<NetworkPlayer>()) {
    //           np.RpcGameStarted();
    //       }
    //   }

    //   public void StartupHost()
    //{
    //	Admin.Instance.ButtonPlayerOne.gameObject.SetActive(false);
    //	Admin.Instance.ButtonPlayerTwo.gameObject.SetActive(false);
    //       //Instantiate(SoundManager, new Vector3(0,0,0), Quaternion.identity);

    //       // TODO: check why this doesn't work
    //       //Instantiate(TargetTransform);
    //       //Instantiate(SteeringStation);
    //       //Instantiate(CanonStation);
    //       SetPort();
    //	StartServer();
    //	isServer = true;
    //	NetworkServer.SpawnObjects();

    //       //SpawnEntityAtPrefabPosition(SteeringStation);
    //       SpawnEntityAtPrefabPosition(TargetTransform);
    //       SpawnEntityAtPrefabPosition(RotationTransform);
    //       SpawnEntityAtPrefabPosition(CanonStationLeft);
    //       SpawnEntityAtPrefabPosition(CanonStationRight);
    //       Stage1_Logic.SpawnStage1();
    //   }

    //public void StopHosting()
    //{
    //	StopServer();
    //	NetworkServer.Reset();
    //}

    void SetIPAddress() {
        networkAddress = ConnectionIP;
    }
    void SetPort() {
        networkPort = ConnectionPort;
    }
    //#region Server 
    //public override void OnStartServer()
    //{
    //	base.OnStartServer();
    //	debugTextServer.text = "Server Started";

    //       ShipManager.Instance.Initialize();
    //   }

    //public override void OnStopServer()
    //{
    //	base.OnStopServer();
    //       debugTextServer.text = "Server Stopped";
    //   }

    //   void OnDestroy()
    //   {

    //       Stage1_Logic.Stage1Done -= OnStage1Done;
    //   }

    //   public void RegisterPlayer(NetworkPlayer np)
    //   {
    //       if (!isServer)
    //           return;

    //       if (np.clientType != ClientChooser.ClientType.VRClient)
    //           return;

    //       for (int i = 0; i < 2; i++) {
    //           if (playerClients[i] == np)
    //               return;
    //       }

    //       if (playerClients[0] == null) {
    //           playerClients[0] = np;
    //           Admin.Instance.ButtonPlayerOne.gameObject.SetActive(true);
    //           Admin.Instance.ButtonPlayerOne.interactable = true;
    //           Admin.Instance.PlayerOne = np.gameObject;
    //       }
    //       else if (playerClients[1] == null) {
    //           playerClients[1] = np;
    //           Admin.Instance.ButtonPlayerTwo.gameObject.SetActive(true);
    //           Admin.Instance.ButtonPlayerTwo.interactable = true;
    //           Admin.Instance.PlayerTwo = np.gameObject;
    //       } else {
    //           // both are allready assigned
    //           return;
    //       }


    //       if (isServer)
    //       {
    //           debugTextServer.text = "Client " + np.connectionToClient.connectionId + " connected.";
    //           //print("Client " + np.connectionToClient.connectionId + " connected.");
    //       }
    //   }

    //   public void UnregisterPlayer(NetworkPlayer np)
    //   {
    //       //Unregister player from wall client Playerview
    //       foreach (PlayerView pv in FindObjectsOfType<PlayerView>())
    //       {
    //           if(pv.player != null)
    //           {
    //               if (pv.player.netId.Value == np.netId.Value)
    //               {
    //                   pv.DisconnectPlayer();
    //               }
    //           }

    //       }

    //       //Unrergister Player from Admin Singleton
    //       if (!isServer)
    //           return;

    //       if (Admin.Instance.ButtonPlayerOne == null || Admin.Instance.ButtonPlayerTwo == null)
    //           return;


    //       if (playerClients[0] == np) {
    //           Admin.Instance.ButtonPlayerOne.gameObject.SetActive(false);
    //           Admin.Instance.ButtonPlayerOne.interactable = true;
    //           Admin.Instance.PlayerTwo = null;
    //           playerClients[0] = null;
    //       }
    //       else if (playerClients[1] == np) {
    //           Admin.Instance.ButtonPlayerTwo.gameObject.SetActive(false);
    //           Admin.Instance.ButtonPlayerTwo.interactable = true;
    //           Admin.Instance.PlayerTwo = null;
    //           playerClients[1] = null;
    //       }
    //   }

    //public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    //{
    //	base.OnServerAddPlayer(conn, playerControllerId);

    //}

    //public override void OnServerRemovePlayer(NetworkConnection conn, PlayerController player)
    //{

    //	base.OnServerRemovePlayer(conn, player);
    //}

    //#endregion

    //#region Client
    public void JoinGame()
	{
        SetIPAddress();
		SetPort();
		StartClient();
		isClient = true;

	}

	//public override void OnClientConnect(NetworkConnection conn)
	//{
 //       //if (NetworkManager.singleton.IsClientConnected())
 //       //    return;
	//	base.OnClientConnect(conn);
 //       //if (np.clientType == ClientChooser.ClientType.RenderClientWall) {
 //       //    CameraToolServer cts = FindObjectOfType<CameraToolServer>();
 //       //    if (cts == null)
 //       //        return;

 //       //}

 //       //debugTextClient.text = "ClientConnected";
 //   }

 //   public override void OnStartClient(NetworkClient client)
	//{
	//	base.OnStartClient(client);
	//	//debugTextClient.text = "Client Started";
	//}

	//public override void OnStopClient()
	//{
	//	base.OnStopClient();
	//	//debugTextClient.text = "Server Stopped";
	//}

	//public void ReconnectClient()
	//{
	//	StopClient();
	//	StartCoroutine("Reconnect");
	//}

	//IEnumerator Reconnect()
	//{
	//	yield return new WaitForSeconds(1f);
	//	StartClient();

	//}
	//#endregion
}
