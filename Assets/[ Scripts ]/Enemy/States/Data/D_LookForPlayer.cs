using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newLookForPlayerStateData", menuName = "Data/State Data/Look For Player State")]
public class D_LookForPlayer : ScriptableObject
{
    public float timeBetweenTurns = 0.5f;
    public int amountsOfTurn = 5;
}
