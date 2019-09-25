using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMarkerController : MonoBehaviour {

  private GameObject firstBlockMarker; //最初にドラッグしたボール
  private GameObject lastBlockMarker; //最後にドラッグしたボール
  GameObject block;
  BlockController blockScript;
  private string currentName; //名前判定用のstring変数

  // Use this for initialization
  void Start () {
    block = GameObject.Find("Stone1");
    blockScript = block.GetComponent<BlockController>();
  }

  // Update is called once per frame
  void Update()
  {
    //画面をクリックし、firstBallがnullの時実行
    if (Input.GetMouseButtonDown(0) && firstBlockMarker == null)
    {
      OnDragStart();
    }
    else if (Input.GetMouseButtonUp(0))
    {
      //クリックを終えた時
      OnDragEnd();
    }
    else if (firstBlockMarker != null)
    {
      OnDragging();
    }
  }

  public void OnDragStart()
  {
    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
    // ヒットしたオブジェクトがマーカーだった場合、キャラとの距離を計算しブロック１つ分の距離だった場合、オブジェウトの1ます下にブロックが存在していればtrue、してなければfalse
    if (hit.collider != null)
    {
      GameObject hitObj = hit.collider.gameObject;
      //オブジェクトの名前を前方一致で判定
      string blockName = hitObj.name;
      // キャラの座標取得
      // タップしたブロックがキャラの左下、真下、右下の場合処理を行う
      if (blockName.StartsWith("Stone"))
      {
        print("STONE");
        firstBlockMarker = hitObj;
        lastBlockMarker = hitObj;
        currentName = hitObj.name;
      }
    }
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
}
