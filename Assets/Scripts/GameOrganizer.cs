using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOrganizer : MonoBehaviour
{
    [Header("Parameters")]
    public int FFAPlayerCount;
    public int AIPlayerCount;
    public int teamSize;
    public int platformId;
    public enum GameTypes{FFA, TEAM};
    public GameTypes gameType;

    [Header("Prefabs")]
    public List<GameObject> ProPenPrefabs;
    public List<GameObject> PlatformPrefabs;

    PlatformMarkers platformMarkers;

    [Header("Parents")]
    public Transform FFAParent;
    public Transform TEAM1Parent;
    public Transform TEAM2Parent;

    private void OnEnable()
    {
        platformMarkers = Instantiate(PlatformPrefabs[platformId], Vector3.zero,
            Quaternion.Euler(-90, 90, 0)).GetComponent<PlatformMarkers>();

        if(AIPlayerCount > FFAPlayerCount)
        {
            throw new UnityException("AI Players must be less than or equal to total players");
        }
    }

    public GameObject SpawnPlayers(out List<PlayerBehaviour> FFAPlayers, out List<PlayerBehaviour> TEAM1Players, out List<PlayerBehaviour> TEAM2Players)
    {
        FFAPlayers = new List<PlayerBehaviour>();
        TEAM1Players = new List<PlayerBehaviour>();
        TEAM2Players = new List<PlayerBehaviour>();

        GameObject theHuman = null;
        switch (gameType)
        {
            case GameTypes.FFA:
                for (int i = 0; i < FFAPlayerCount; i++)
                {
                    try
                    {
                        bool exists = platformMarkers.FFAMarkers[i] != null;
                    }
                    catch
                    {
                        throw new UnityException("Number of players must not exceed number of markers");
                    }
                    int index = i % ProPenPrefabs.Count;
                    GameObject player = Instantiate(ProPenPrefabs[index],
                        platformMarkers.FFAMarkers[i].position, Quaternion.Euler(-90, 0, 0), FFAParent);

                    if(i < AIPlayerCount)
                    {
                        player.AddComponent<AIBehaviour>();
                        player.name = "AI: Player " + (i + 1);
                    }
                    else
                    {
                        player.AddComponent<PlayerBehaviour>();
                        theHuman = player;
                        player.name = "HUMAN: Player " + (i + 1);
                    }

                    player.GetComponent<PlayerBehaviour>().SetManager(GetComponent<GameManager>());
                    FFAPlayers.Add(player.GetComponent<PlayerBehaviour>());
                }
                break;
            case GameTypes.TEAM:
                for (int i = 0; i < teamSize; i++)
                {
                    GameObject player = Instantiate(ProPenPrefabs[0], 
                        platformMarkers.TEAM1Markers[i].position, Quaternion.Euler(-90, 0, 0), TEAM1Parent);
                    player.GetComponent<PlayerBehaviour>().SetManager(GetComponent<GameManager>());
                    TEAM1Players.Add(player.GetComponent<PlayerBehaviour>());

                    player.name = "TEAM 1: P" + (i + 1);
                }
                for (int i = 0; i < teamSize; i++)
                {
                    GameObject player = Instantiate(ProPenPrefabs[1], 
                        platformMarkers.TEAM2Markers[i].position, Quaternion.Euler(-90, 0, 0), TEAM2Parent);
                    player.GetComponent<PlayerBehaviour>().SetManager(GetComponent<GameManager>());
                    TEAM2Players.Add(player.GetComponent<PlayerBehaviour>());

                    player.name = "TEAM 2: P" + (i + 1);
                }
                break;
        }
        return theHuman;
    }

    public PlayerBehaviour SpawnPlayer(PlayerBehaviour pen, Vector3 spawnLocation)
    {
        PlayerBehaviour spawnedPen = Instantiate(pen.gameObject, spawnLocation, 
            Quaternion.Euler(-90, 0, 0), FFAParent).GetComponent<PlayerBehaviour>();
        spawnedPen.name = pen.name;
        return spawnedPen;
    }
}