using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Defender
{
    public static class Game
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            m_state = State.INITIALISE;
            onStateChanged = null; // Clear callbacks, in case we're in editor
        }

        public enum State
        {
            INITIALISE,
            MENU,
            NEWGAME,
            GAME,
            GAMEOVER
        }

        public static State Current
        {
            get => m_state;
            set
            {
                if(value != m_state)
                {
                    m_state = value;
                    onStateChanged?.Invoke(m_state);
                }
            }
        }

        private static State m_state;

        private static event Action<State> onStateChanged;

        public static void Subscribe(Action<State> callback)
        {
            onStateChanged += callback;
        }

        public static void Unsubscribe(Action<State> callback)
        {
            onStateChanged -= callback;
        }
    }
}
