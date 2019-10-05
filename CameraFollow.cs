/* Authors: Edwin Zhang, Sepehr Hosseini
 * Last Updated: 2019-08-07
 * 
 * Description: 
 * Camera script to follow player and to keep camera in bounds
 * 
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform playerPos;
    public bool bounds;
    public Vector3 minCameraPos;
    public Vector3 maxCameraPos;
    void LateUpdate()
    {
        Vector3 ytemp = transform.position;

        ytemp.y = playerPos.position.y;

        transform.position = ytemp;

        if (bounds)
        {
            transform.position = new Vector3(Mathf.Clamp(transform.position.x, minCameraPos.x, maxCameraPos.x), Mathf.Clamp(transform.position.y, minCameraPos.y, maxCameraPos.y), Mathf.Clamp(transform.position.z, minCameraPos.z, maxCameraPos.z));
        }
    }
}

