using UnityEngine;
using UnityEngine.Events;

namespace CubeToss.Gameplay
{
    /// <summary>
    /// Simple version of asset based event channel or system module for basic event handling.
    /// </summary>
    [CreateAssetMenu(fileName = "ScoreModule", menuName = "Scriptable Objects/ScoreModule")]
    public class ScoreModule : ScriptableObject
    {
        [SerializeField] private int impactScore = 1;
        [SerializeField] private int chainBonus = 4;
        [SerializeField] private int maxChain = 4;

        public UnityEvent<int> UpdateScore;

        private int _score;
        public int Score
        {
            get => _score;
            private set
            {
                _score = value;
                UpdateScore?.Invoke(_score);
            }
        }
        
        public void Init()
        {
            _score = 0;
        }

        // TODO: Text popups and other FX.
        public void ScoreImpact(Vector3 impactVelocity, int chainLevel)
        {
            var score = (int)impactVelocity.magnitude * impactScore;
            score += chainBonus * chainLevel;
            if (chainLevel >= maxChain)
                score += chainBonus * (chainLevel - maxChain + 1);
            
            Score += score;
        }
        
        public void ResetScore()
        {
            Score = 0;
            Debug.Log("Score reset.");
        }
    }
}
