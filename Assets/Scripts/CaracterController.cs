using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaracterController : MonoBehaviour {

	// Use this for initialization
	void Start () {
    createCharacter();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

  void createCharacter()
  {
    print("キャr生成");
    // プレファブ取得
    GameObject charaPrefab = GameObject.Find("Character");
    // オブジェクトのポジション設定
    float posX = 0;
    float posY = 2.5f; // (3.0f - posY) / 0.5f
    Vector2 charaPosition = new Vector2(posX, posY);
    GameObject character = Instantiate(charaPrefab, charaPosition, Quaternion.AngleAxis(Random.Range(-0, 0), Vector3.up)) as GameObject;
    //print("(x, y)=" + "(" + i + ", " + j + ")");
    //print(blocks[i, j]);
  }
}
