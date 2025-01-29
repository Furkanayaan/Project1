using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public Camera mainCamera;
    public int sideLength;
    public GameObject tilePrefab;

    private void Start() {
        if (UIManager.storedValue > 0) sideLength = UIManager.storedValue;
        CameraPos();
        SetUp();
    }


    void SetUp() {
        for (int i = 0; i < sideLength; i++) {
            for (int j = 0; j < sideLength; j++) {
                Vector2 tempPos = new Vector2(i, j);
                GameObject tiles = Instantiate(tilePrefab, tempPos, Quaternion.identity, transform);
            }
        }
    }

    void CameraPos() {
        float constantValue = ((float)sideLength - 1) / 2;
        mainCamera.transform.position = new Vector3(constantValue, constantValue, -10);
        mainCamera.orthographicSize = sideLength;
    }
    
}
