using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OCaminhoDoPeregrino.Core
{
    public class VirtueManager : MonoBehaviour
    {
        [Serializable]
        public class VirtueEvent : UnityEvent<string> { }

        public static VirtueManager Instance { get; private set; }

        [SerializeField] private List<string> startingVirtues = new();
        [SerializeField] private VirtueEvent onVirtueAcquired = new();

        private readonly HashSet<string> virtues = new();

        public VirtueEvent OnVirtueAcquired => onVirtueAcquired;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);

            for (int i = 0; i < startingVirtues.Count; i++)
            {
                AcquireVirtue(startingVirtues[i]);
            }
        }

        public bool AcquireVirtue(string virtueId)
        {
            string normalized = NormalizeVirtue(virtueId);
            if (string.IsNullOrEmpty(normalized))
            {
                return false;
            }

            if (!virtues.Add(normalized))
            {
                return false;
            }

            onVirtueAcquired.Invoke(normalized);
            return true;
        }

        public bool HasVirtue(string virtueId)
        {
            string normalized = NormalizeVirtue(virtueId);
            if (string.IsNullOrEmpty(normalized))
            {
                return false;
            }

            return virtues.Contains(normalized);
        }

        public bool RemoveVirtue(string virtueId)
        {
            string normalized = NormalizeVirtue(virtueId);
            if (string.IsNullOrEmpty(normalized))
            {
                return false;
            }

            return virtues.Remove(normalized);
        }

        public List<string> GetAllVirtues()
        {
            return new List<string>(virtues);
        }

        private static string NormalizeVirtue(string virtueId)
        {
            if (string.IsNullOrWhiteSpace(virtueId))
            {
                return string.Empty;
            }

            return virtueId.Trim().ToLowerInvariant();
        }
    }
}
