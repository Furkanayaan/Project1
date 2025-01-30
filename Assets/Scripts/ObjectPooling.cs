using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour {
    static public ObjectPooling I;
    
    
    [Serializable]
    public class ObjectPool {
        public GameObject XMark;
        public int initializePoolSize = 5;
        public Transform activeChild;
        public Transform deactiveChild;

        //A function that allows us to instantiate a specific number of X markers at the start if desired.
        public void InitializePool() {
            for (int i = 0; i < initializePoolSize; i++) {
                Instantiate(XMark, Vector2.zero, Quaternion.identity, deactiveChild);
            }
        }
        
        //A function that allows us to fetch an X mark from the deactivated parent.
        public Transform GetPooledObject() {
            if (deactiveChild.childCount <= 0) {
                Instantiate(XMark, Vector2.zero, Quaternion.identity, deactiveChild);
                return GetPooledObject();
            }
            
            Transform obj = deactiveChild.GetChild(0);
            obj.transform.SetParent(activeChild.transform);
            return obj;
        }
        //The function where we assign the X marker as a child of the deactivated parent and deactivate it
        public void ReturnToPool(GameObject obj) {
            obj.transform.SetParent(deactiveChild.transform);
        }
    }

    public ObjectPool CobjectPool = new();

    private void Start() {
        I = this;
        CobjectPool.InitializePool();
    }
}
