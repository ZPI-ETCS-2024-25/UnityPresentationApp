using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAlongPath : MonoBehaviour
{
    // List of waypoints to follow
    public Transform[] waypoints;

    // Speed of movement
    public float moveSpeed = 5f;

    // How close the object needs to be to consider it has "reached" the waypoint
    public float waypointRadius = 0.1f;

    private int currentWaypointIndex = 0;

    void Update()
    {
        // Check if waypoints are assigned
        if (waypoints.Length == 0) return;

        // Move towards the current waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = targetWaypoint.position - transform.position;
        transform.position += direction.normalized * moveSpeed * Time.deltaTime;

        // Check if we are close enough to the waypoint to move to the next one
        if (Vector3.Distance(transform.position, targetWaypoint.position) < waypointRadius)
        {
            currentWaypointIndex++;
            if (currentWaypointIndex >= waypoints.Length)
            {
                currentWaypointIndex = 0;  // Loop back to the first waypoint
            }
        }
    }

    // Optional: Draw the path in the editor
    private void OnDrawGizmos()
    {
        if (waypoints != null && waypoints.Length > 1)
        {
            for (int i = 0; i < waypoints.Length - 1; i++)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
            }
        }
    }
}
