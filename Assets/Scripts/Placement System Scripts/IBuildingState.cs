﻿using UnityEngine;

public interface IBuildingState
{
    void EndState();
    void OnAction(Vector3Int gridPosition, Vector3 rotation);
    void UpdateState(Vector3Int gridPosition, Vector3 rotation);
}