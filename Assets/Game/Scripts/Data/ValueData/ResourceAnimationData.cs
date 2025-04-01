using System;
using UnityEngine;

namespace HexaClash.Game.Scripts.Data.ValueData
{
    public struct ResourceAnimationData
    {
        public readonly Sprite Sprite;
        public readonly Vector2 StartPosition;
        public readonly Vector2 TargetPosition;
        public readonly int Count;
        public readonly float StartScale;
        public readonly float MidScale;
        public readonly float TargetScale;
        public Action OnComplete;

        public ResourceAnimationData(Sprite sprite, Vector2 startPos, Vector2 targetPos, int count, float startScale, float midScale, float targetScale, Action onComplete = null)
        {
            Sprite = sprite;
            StartPosition = startPos;
            TargetPosition = targetPos;
            Count = count;
            StartScale = startScale;
            MidScale = midScale;
            TargetScale = targetScale;
            OnComplete = onComplete;
        }

        public ResourceAnimationData SetOnComplete(Action completeCallback)
        {
            OnComplete = completeCallback;
            return this;
        }
    }
}