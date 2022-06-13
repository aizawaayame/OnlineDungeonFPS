using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Assets
{
    public static class Resloader
    {
        public static T Load<T>(string path) where T : Object
        {
            return Resources.Load<T>(path);
        }
    }
}
