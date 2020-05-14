using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    GameOrganizer gameOrganizer;

    List<PlayerBehaviour> FFAPlayers;
    List<PlayerBehaviour> TEAM1Players;
    List<PlayerBehaviour> TEAM2Players;

    public LineRenderer targetGizmo;

    CameraController cameraController;

    private bool isTurnUsed;

    private void Start()
    {
        gameOrganizer = GetComponent<GameOrganizer>();
        cameraController = Camera.main.GetComponent<CameraController>();

        GameObject theHuman = gameOrganizer.SpawnPlayers(out FFAPlayers, out TEAM1Players, out TEAM2Players);

        if (theHuman)
        {
            cameraController.lookAt = theHuman;
            foreach(PlayerBehaviour player in FFAPlayers)
            {
                if(!player.gameObject.Equals(theHuman))
                {
                    cameraController.lookTowards = player.gameObject;
                    break;
                }
            }
        }

        NextTurn();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && !isTurnUsed)
        {
            if (!FFAPlayers[0].GetComponent<AIBehaviour>())
            {
                FFAPlayers[0].PlayTurn(targetGizmo);
                isTurnUsed = true;
            }
            
        }
    }

    private void NextTurn()
    {
        Invoke("NextTurn", 10);

        PlayerBehaviour lastTurn = FFAPlayers[0];
        lastTurn.FinishTargettingAndPlay(false);

        FFAPlayers.RemoveAt(0);
        FFAPlayers.Add(lastTurn);

        AIBehaviour ai = FFAPlayers[0].GetComponent<AIBehaviour>();

        if (ai)
        {
            cameraController.lookTowards = ai.gameObject;
            ai.PlayTurn(FFAPlayers);
            isTurnUsed = true;
        }
        else
        {
            isTurnUsed = false;
        }
    }

    public void TurnPlayed()
    {
        CancelInvoke("NextTurn");
        Invoke("NextTurn", 4);
    }

    public void Respawn(PlayerBehaviour deadPlayer, Vector3 spawnLocation)
    {
        PlayerBehaviour revivedPlayer = gameOrganizer.SpawnPlayer(deadPlayer, spawnLocation);
        revivedPlayer.Score = deadPlayer.Score;

        if (deadPlayer.gameObject.Equals(cameraController.lookAt))
        {
            cameraController.lookAt = revivedPlayer.gameObject;
        }
        if (deadPlayer.gameObject.Equals(cameraController.lookTowards))
        {
            cameraController.lookTowards = revivedPlayer.gameObject;
        }

        int index = FFAPlayers.IndexOf(deadPlayer);
        FFAPlayers.Insert(index, revivedPlayer);
        FFAPlayers.Remove(deadPlayer);
        revivedPlayer.SetManager(this);
        Destroy(deadPlayer.gameObject);
    }
}
