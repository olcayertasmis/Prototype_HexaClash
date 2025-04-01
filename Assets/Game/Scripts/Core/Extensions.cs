using System;

namespace HexaClash.Game.Scripts.Core
{
    public static class Extensions
    {
        public static void RemoveAt<T>(ref T[] arr, int index)
        {
            for (int a = index; a < arr.Length - 1; a++)
            {
                arr[a] = arr[a + 1];
            }

            Array.Resize(ref arr, arr.Length - 1);
        }

        public static void UnsubscribeAction<T>(Action<T> action) where T : class
        {
            if (action == null) return;

            foreach (var activeDelegate in action.GetInvocationList())
            {
                action -= (Action<T>)activeDelegate;
            }
        }

        public static void UnsubscribeAction(Action action)
        {
            if (action == null) return;

            foreach (var activeDelegate in action.GetInvocationList())
            {
                action -= (Action)activeDelegate;
            }
        }
    }
}