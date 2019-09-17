using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

  // キャラをタップした時の処理
  // BlockMarkerにDragが入ったらキャラとマーカーを連結
  // 移動ロジック
  public void PointerDown()
  {
    print("PointerDown");
  }

  // DragしているオブジェクトがMarkerだった場合、処理に入る
  public void Drag()
  {
    // 今ドラッグしているオブジェクトを取得
    // そのオブジェクトとキャラを線でつなぐ
    // オブジェクトの座標を保存
    print("Drag");
  }

  public void PointerUp()
  {
    // 保存したマーカーの座標を順番に移動する
    print("PointerUp");
  }

  public void createCharacter()
  {
    // プレファブ取得
    GameObject charaPrefab = GameObject.Find("Character");
    // オブジェクトのポジション設定
    float posX = 0;
    float posY = 2.5f; // (3.0f - posY) / 0.5f
    Vector2 charaPosition = new Vector2(posX, posY);
    GameObject character = Instantiate(charaPrefab, charaPosition, Quaternion.AngleAxis(Random.Range(-0, 0), Vector3.up)) as GameObject;
  }
}
