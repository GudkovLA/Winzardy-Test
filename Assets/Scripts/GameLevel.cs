using System;
using UnityEngine;

namespace Game
{
    public class GameLevel : IDisposable
    {
        public Transform Root { private set; get; }

        public GameLevel(Transform root)
        {
            Root = root;
        }

        public void Dispose()
        {
            Root =  null;
        }
    }
}