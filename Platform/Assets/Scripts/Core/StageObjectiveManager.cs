using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OCaminhoDoPeregrino.Core
{
    public class StageObjectiveManager : MonoBehaviour
    {
        [Serializable]
        public class IntEvent : UnityEvent<int> { }

        public static StageObjectiveManager Instance { get; private set; }

        [SerializeField] private int requiredScore;
        [SerializeField] private IntEvent onScoreChanged = new();
        [SerializeField] private UnityEvent onObjectiveCompleted = new();

        private int score;
        private readonly HashSet<string> collectedIds = new();

        public int Score => score;
        public int RequiredScore => requiredScore;
        public bool IsCompleted => score >= requiredScore;
        public IntEvent OnScoreChanged => onScoreChanged;
        public UnityEvent OnObjectiveCompleted => onObjectiveCompleted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public bool RegisterScore(string uniqueId, int amount)
        {
            if (string.IsNullOrWhiteSpace(uniqueId) || amount <= 0)
            {
                return false;
            }

            if (!collectedIds.Add(uniqueId))
            {
                return false;
            }

            score += amount;
            onScoreChanged.Invoke(score);

            if (IsCompleted)
            {
                onObjectiveCompleted.Invoke();
            }

            return true;
        }
    }
}
