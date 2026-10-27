using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.Core
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField] private float smoothTime = 0.15f;
        private Transform player;

        private Vector3 velocity;

        private void Start() => player = GameObject.FindGameObjectWithTag("Player").transform;

        private void LateUpdate()
        {
            if (player == null) return;

            Vector3 goal = new Vector3(player.position.x, player.position.y, transform.position.z);

            transform.position = Vector3.SmoothDamp(transform.position, goal, ref velocity, smoothTime);
        }
    }
}
