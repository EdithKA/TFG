using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPuzzleObjective 
{
    bool isComplete { get; }
    event System.Action onCompleted;
}
