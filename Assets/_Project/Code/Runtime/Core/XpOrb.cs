using LastWick.Player;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

namespace Assets._Project.Code.Runtime.Core
{
    public class XpOrb : MonoBehaviour
    {
        private static PlayerRunStats player;

        private int amount;

        public void Init(int xp) => amount = xp;

        private void Awake()
        {
            if (player == null)
            {
                GameObject found = GameObject.FindWithTag("Player");
                if (found != null)
                    player = found.GetComponent<PlayerRunStats>();
            }
        }

        void Update()
        {
            // O player é destruído ao morrer, e os orbes continuam na arena.
            if (player == null) return;

            Vector2 offset = player.transform.position - transform.position;
            float radius = player.XpPickupRadius;

            if (offset.sqrMagnitude <= radius * radius)
            {
                int speed = 7;

                transform.position = Vector2.MoveTowards(
                    transform.position,
                    player.transform.position,
                    speed * Time.deltaTime);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player")) return;

            GameEvents.RaiseXpGained(amount);
            Destroy(gameObject);
        }
    }
}
