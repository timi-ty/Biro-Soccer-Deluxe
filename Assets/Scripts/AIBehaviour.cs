using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIBehaviour : PlayerBehaviour
{
    public void PlayTurn(List<PlayerBehaviour> players)
    {
        int victimIndex = Random.Range(1, players.Count);
        int weaponDecider = Random.Range(0, 3);
        int forceMagnitude = Random.Range(50, 75);

        PlayerBehaviour victim = players[victimIndex];

        Vector3 direction;

        switch (weaponDecider)
        {
            case 0:
                direction = (victim.transform.position - middleCollider.bounds.center).normalized;
                middleBody.AddForce(direction * forceMagnitude, ForceMode.Impulse);
                break;
            case 1:
                direction = (victim.transform.position - leftCollider.bounds.center).normalized;
                leftBody.AddForce(direction * forceMagnitude, ForceMode.Impulse);
                break;
            case 2:
                direction = (victim.transform.position - rightCollider.bounds.center).normalized;
                rightBody.AddForce(direction * forceMagnitude, ForceMode.Impulse);
                break;
        }

        FinishTargettingAndPlay(true);
    }
}
