using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AngleToPlayer : MonoBehaviour
{

    Transform player;
    Vector3 targetPos;
    Vector3 targetDir;

    float angle;
    public int lastIndex;

    SpriteRenderer spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerMove>().transform;
        spriteRenderer= GetComponentInChildren<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        targetPos = new Vector3(player.position.x, transform.position.y, player.position.z);
        targetDir = targetPos - transform.position;

        angle = Vector3.SignedAngle(targetDir, transform.forward, Vector3.up);

        //Voltear el sprite si es necesario
        Vector3 tempScale = Vector3.one;
        if(angle > 0)
        {
            tempScale.x = -1f;
        }
        lastIndex = GetIndex(angle);
    }

    int GetIndex(float angle)
    {

        //Front
        if (angle > -22.5f && angle < 22.6f)
            return 0;
        if (angle >= 22.5f && angle < 67.5f)
            return 7;
        if (angle >= 67.5f && angle < 112.5f)
            return 6;
        if (angle >= 112.5f && angle < 157.5f)
            return 5;

        //back
        if (angle <= -157.5f || angle >= 157.5f)
            return 4;
        if (angle >= -157.4f && angle < -112.5f)
            return 3;
        if (angle >= -112.5f && angle < -67.5f)
            return 2;
        if (angle >= 67.5f && angle <= -22.5f)
            return 1;

        return lastIndex;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawRay(transform.position, targetPos);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(transform.position, targetPos);
    }
}
