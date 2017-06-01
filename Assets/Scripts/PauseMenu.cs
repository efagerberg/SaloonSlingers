using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PauseMenu : MonoBehaviour
{
    public static bool IsOn = false;

    private NetworkManager networkManager;

    void Start()
    {
        networkManager = NetworkManager.singleton;
    }

    public void LeaveRoom()
    {
        MatchInfo _matchInfo = networkManager.matchInfo;
        networkManager.matchMaker.DropConnection(_matchInfo.networkId,
                                                 _matchInfo.nodeId,
                                                 _matchInfo.domain,
                                                 networkManager.OnDropConnection);
        networkManager.StopHost();
    }

}
