﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterController : MonoBehaviour {

  List<int> chainBlockX = new List<int>();
  List<int> chainBlockY = new List<int>();

  // Use this for initialization
  void Start () {

	}
	
	// Update is called once per frame
	void Update () {
    //画面をクリックし、firstBallがnullの時実行
    if (Input.GetMouseButtonDown(0) && firstBlock == null)
    {
      OnDragStart();
    }
    else if (Input.GetMouseButtonUp(0))
    {
      //クリックを終えた時
      OnDragEnd();
    }
    else if (firstBlock != null)
    {
      OnDragging();
    }
  }

  public void OnDragStart()
  {
    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
    // ヒットしたオブジェクトがマーカーだった場合、キャラとの距離を計算しブロック１つ分の距離だった場合、オブジェウトの1ます下にブロックが存在していればtrue、してなければfalse
  }

  public void OnDragging()
  {
    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
    // ドラッグしているオブジェクトがマーカーだった場合、オブジェウトの1ます下にブロックが存在していればtrue、してなければfalse
    // 線でつなげる

  }

  public void OnDragEnd()
  {
    // 移動開始
    // コルーチンで遅延を発生させながら移動させる
  }

  // キャラをタップした時の処理
  // BlockMarkerにDragが入ったらキャラとマーカーを連結
  // 移動ロジック
  //public void PointerDown()
  //{
  //  print("PointerDown");
  //}

  // DragしているオブジェクトがMarkerだった場合、処理に入る
  //public void Drag()
  //{
  //  // ドラッグ中のオブジェクトを取得
  //  // 調べる
  //  // raycastを使う
  //  // rayが当たったオブジェクトの座標を取得

  //  // オブジェクトの座標を保存
  //  //chainBlockX.Add(this.transform.position.x);
  //  //chainBlockY.Add(this.transform.position.y);
  //  // そのオブジェクトとキャラを線でつなぐ
  //  LineRenderer renderer = gameObject.GetComponent<LineRenderer>();
  //  // 線の幅
  //  renderer.SetWidth(0.1f, 0.1f);
  //  // 頂点の数
  //  renderer.SetVertexCount(2);
  //  // 頂点を設定
  //  // １つ前のオブジェクトと現在ドラッグしているオブジェクトを設定
  //  //renderer.SetPosition(0, new Vector3(chainBlockX[chainBlockX.Count - 1], chainBlockY[chainBlockY.Count - 1], 0f));
  //  //renderer.SetPosition(1, new Vector3(this.transform.position.x, this.transform.position.y, 0f));

  //  print("Drag");
  //}

  //public void PointerUp()
  //{
  //  // 保存したマーカーの座標を順番に移動する
  //  print("PointerUp");
  //}

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
