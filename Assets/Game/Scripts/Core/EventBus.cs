using System;
using System.Collections.Generic;

namespace HexaClash.Game.Scripts.Core
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, Delegate> EventTable = new();

        public static void Subscribe<T>(Action<T> listener) where T : class
        {
            if (!EventTable.ContainsKey(typeof(T)))
            {
                EventTable[typeof(T)] = null;
            }

            EventTable[typeof(T)] = (Action<T>)EventTable[typeof(T)] + listener;
        }

        public static void Unsubscribe<T>(Action<T> listener) where T : class
        {
            if (EventTable.ContainsKey(typeof(T)))
            {
                EventTable[typeof(T)] = (Action<T>)EventTable[typeof(T)] - listener;
            }
        }

        public static void Publish<T>(T eventData) where T : class
        {
            if (EventTable.ContainsKey(typeof(T)) && EventTable[typeof(T)] != null)
            {
                ((Action<T>)EventTable[typeof(T)]).Invoke(eventData);
            }
        }
    }
}