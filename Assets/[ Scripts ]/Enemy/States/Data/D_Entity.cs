using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "newEntityData", menuName = "Data/Entity Data/Base Data")]
public class D_Entity : ScriptableObject
{
    public float maxHealth = 30f;

    public float damageHopSpeed = 3;

    public float wallCheckDistance = 0.2f;
    public float ledgeCheckDistance = 0.4f;
    public float minAgroDist = 2f;
    public float maxAgroDist = 4f;

    public float closeRangeActionDistance = 1f;

    public LayerMask goundLayer;
    public LayerMask playerLayer;
}
