using System;
using UnityEngine;
using CubeToss.Gameplay;

namespace CubeToss.UI
{
    public class ScoreBoard : MonoBehaviour
    {
        [SerializeField] private ScoreModule scoreModule;
        [SerializeField] private TMPro.TextMeshProUGUI scoreText;
        [SerializeField] private TMPro.TextMeshProUGUI bonusText;

        [SerializeField] private float countDuration = 1.0f;

        private float _startTime;
        private float _startScore;
        
        private int _currentScore;
        private int _targetScore;

        private void Start()
        {
            scoreModule.Init();
            
            _currentScore = 0;
            _targetScore = 0;
            
            scoreText.text = "0";
            bonusText.text = null;
        }

        private void OnEnable()
        {
            scoreModule.UpdateScore.AddListener(OnScoreUpdated);
        }

        private void OnDisable()
        {
            scoreModule.UpdateScore.RemoveListener(OnScoreUpdated);
        }

        private void Update()
        {
            if (_currentScore == _targetScore)
                return;

            var t = Mathf.Clamp01((Time.time - _startTime) / countDuration);
            _currentScore = (int)Mathf.Lerp(_startScore, _targetScore, t);
            
            scoreText.text = _currentScore.ToString("N0");
        }

        private void OnScoreUpdated(int newScore)
        {
            _startTime = Time.time;
            _startScore = _currentScore;
            _targetScore = newScore;
        }
    }
}