using System.Collections;
using System.Collections.Generic;
using HexaClash.Game.Scripts.Core;
using HexaClash.Game.Scripts.Gameplay.HexagonSystem;
using UnityEngine;

namespace HexaClash.Game.Scripts.Managers
{
    public class MergeManager : ManagerBase
    {
        private readonly Queue<(HexagonArea, HexagonBlock)> _mergeQueue = new();
        private Coroutine _mergeProcessCoroutine;

        #region Override Methods

        public override void InitializeSystem()
        {
            InitializeSystemBody();
        }

        #endregion

        #region Public Methods

        public void RequestMerge(HexagonArea area, HexagonBlock block)
        {
            if (block.IsMerging) return;

            _mergeQueue.Enqueue((area, block));

            _mergeProcessCoroutine ??= StartCoroutine(ProcessMergeQueue());
        }

        #endregion

        #region Private Methods

        private IEnumerator ProcessMergeQueue()
        {
            while (_mergeQueue.Count > 0)
            {
                var (area, block) = _mergeQueue.Dequeue();

                block.StartMerge();
                yield return area.ProcessMerge(block);
                block.EndMerge();

                yield return null;
            }

            _mergeProcessCoroutine = null;
        }

        private void OnDestroy()
        {
            if (_mergeProcessCoroutine != null) StopCoroutine(_mergeProcessCoroutine);
        }

        #endregion
    }
}