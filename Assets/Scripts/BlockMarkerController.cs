using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockMarkerController : MonoBehaviour {

  private GameObject firstBlockMarker; //最初にドラッグしたボール
  private GameObject lastBlockMarker; //最後にドラッグしたボール

  // Use this for initialization
  void Start () {
		
	}

  // Update is called once per frame
  void Update()
  {
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
        firstBlock = hitObj;
        lastBlock = hitObj;
        currentName = hitObj.name;
        //削除対象オブジェクトリストの初期化
        removableBlockList = new List<GameObject>();
        //ドラッグ中のオブジェクトの座標を取得
        int matrixX = getMatrix_X(hitObj.transform.position.x);
        int matrixY = getMatrix_Y(hitObj.transform.position.y);
        //print("(x,y)=" + "(" + matrixY + ", " + matrixX + ")");
        isExistence[matrixX, matrixY] = false;
        removableBlockX.Add(matrixX);
        removableBlockY.Add(matrixY);
        //削除対象のオブジェクトを格納
        PushToList(hitObj);
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
