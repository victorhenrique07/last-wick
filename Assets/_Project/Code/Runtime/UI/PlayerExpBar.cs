using Assets._Project.Code.Runtime.Run;
using LastWick.Combat;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.UI
{
    public class PlayerExpBar : MonoBehaviour
    {
        [SerializeField] private PlayerLevel playerLevel;
        [SerializeField] private RectTransform fill;

        [Tooltip("Velocidade de preenchimento, em barras cheias por segundo.")]
        [SerializeField, Min(0.01f)] private float fillSpeed = 1f;

        private float displayed;

        private void Start() => displayed = playerLevel.Progress;

        private void Update()
        {
            if (playerLevel == null || fill == null) return;

            displayed = Mathf.MoveTowards(displayed, playerLevel.Progress, fillSpeed * Time.unscaledDeltaTime);
            fill.anchorMax = new Vector2(displayed - Mathf.Floor(displayed), 1f);
        }
    }
}
