using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace SimWork.Core.Properties
{
    public abstract class Property<T>  : MonoBehaviour where T : Property<T>
    {

        private static T _instance;
        public static T Instance
        {
            get { return _instance; }
        }

        void Awake()
        {
            _instance = this as T;
        }
    }
}
