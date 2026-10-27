using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Assets._Project.Code.Runtime.UI
{
    public class GameOverPanel : MonoBehaviour
    {
        [SerializeField] private GameObject gameOverPanel;

        void OnEnable() => GameEvents.PlayerDied += RaiseGameOverPanel;
        void OnDisable() => GameEvents.PlayerDied -= RaiseGameOverPanel;

        private void RaiseGameOverPanel()
        {
            gameOverPanel.SetActive(true);
            Time.timeScale = 0f;
        }
    }
}
