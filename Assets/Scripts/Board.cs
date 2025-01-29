using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    public static Board I;
    public Camera mainCamera;
    //The edge length value for a square grid.
    public int sideLength;
    public GameObject tilePrefab;
    //A two-dimensional array created for the object at that column and row.
    public GameObject[,] AllXmarksPos;

    private void Start() {
        I = this;
        //Assign a value based on the value in the input field.
        if (UIManager.storedValue > 0) sideLength = UIManager.storedValue;
        AllXmarksPos = new GameObject[sideLength, sideLength];
        CameraPos();
        SetUp();
    }


    //Setup background tiles according to side length.
    void SetUp() {
        for (int i = 0; i < sideLength; i++) {
            for (int j = 0; j < sideLength; j++) {
                Vector2 tempPos = new Vector2(i, j);
                Instantiate(tilePrefab, tempPos, Quaternion.identity, transform);
            }
        }
    }

    //Determine camera position and size.
    void CameraPos() {
        float constantValue = ((float)sideLength - 1) / 2;
        //Determine camera pos.
        mainCamera.transform.position = new Vector3(constantValue, constantValue, -10);
        //Determine camera size.
        mainCamera.orthographicSize = sideLength;
    }

    //Return the object in that row and column.
    public Transform XMarkRowColumn(int column, int row) {
        if (AllXmarksPos[column, row] != null) {
            return AllXmarksPos[column, row].transform;
        }

        return null;
    }
    
}
