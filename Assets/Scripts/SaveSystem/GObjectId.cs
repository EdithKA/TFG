using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GObjectId : MonoBehaviour
{
    public string id;
    private void Start()
    {
        id = this.gameObject.name;
    }
}
