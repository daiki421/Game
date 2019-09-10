using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour {
  public GameObject[] blockPrefab;
  private int BLOCK_LINE = 30;
  private int BLOCK_ROW = 9;
  private GameObject firstBlock; //最初にドラッグしたボール
  private GameObject lastBlock; //最後にドラッグしたボール
  private string currentName; //名前判定用のstring変数
  // 削除するブロックのリスト
  List<GameObject> removableBlockList = new List<GameObject>();
  // 削除するブロックの配列
  List<int> removableBlockX = new List<int>();
  List<int> removableBlockY = new List<int>();
  // 削除するブロックをブロックの座標で管理する配列
  bool[,] isExistence;
  // 現在のオブジェクトの有無を管理する配列
  GameObject[,] blocks;
  // ブロック落下用のマーカーを管理する配列
  GameObject[,] blockMarkers;
  int[] deleteBlockCount;

  void Start () {
    blocks = new GameObject[BLOCK_ROW, BLOCK_LINE];
    blockMarkers = new GameObject[BLOCK_ROW, BLOCK_LINE];
    deleteBlockCount = new int[BLOCK_ROW];
    createBlock(BLOCK_ROW, BLOCK_LINE);
    createSideWall(23);
    createFloorStone(BLOCK_LINE);
    createBlockMarker(BLOCK_ROW, BLOCK_LINE);
    isExistence = new bool[BLOCK_ROW, BLOCK_LINE];
    for(int i=0;i< BLOCK_ROW; i++)
    {
      for (int j = 2; j < BLOCK_LINE; j++)
      {
        isExistence[i, j] = true;
        //print("("+i+", "+j+")="+isExistence[i, j]);
      }
    }
    for (int j = 0; j < BLOCK_ROW; j++)
    {
      deleteBlockCount[j] = 0;
    }
  }

  void Update()
  {
    //画面をクリックし、firstBallがnullの時実行
    if (Input.GetMouseButtonDown(0) && firstBlock == null)
    {
      OnDragStart();
    } else if (Input.GetMouseButtonUp(0)) {
      //クリックを終えた時
      OnDragEnd();
      dropBlock();
    } else if (firstBlock != null) {
      OnDragging();
    }
  }

  private void OnDragStart()
  {
    //print("START");
    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);

    if (hit.collider != null)
    {
      GameObject hitObj = hit.collider.gameObject;
      //オブジェクトの名前を前方一致で判定
      string blockName = hitObj.name;
      if (blockName.StartsWith("Stone"))
      {
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

  private void OnDragging()
  {
    //print("DRAG");
    RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
    if (hit.collider != null) {
      GameObject hitObj = hit.collider.gameObject;
      //同じ名前のブロックをクリック＆lastBallとは別オブジェクトである時
      if (hitObj.name == currentName && lastBlock != hitObj) {
        //２つのオブジェクトの距離を取得
        float distance = Vector2.Distance(hitObj.transform.position, lastBlock.transform.position);
        if (distance < 1.0f) {
          // 座標が保存してある配列にドラッグ中のブロックの座標が存在する場合は最新の配列の中身を削除
          // ブロックの数だけループ回す
          for (int i = 0; i < BLOCK_ROW * BLOCK_LINE; i++)
          {
            if (removableBlockX[i] == getMatrix_X(hitObj.transform.position.x) && removableBlockY[i] == getMatrix_Y(hitObj.transform.position.y)) {
              // 配列の最新を削除
              removableBlockX.RemoveAt(removableBlockX.Count - 1);
              removableBlockY.RemoveAt(removableBlockY.Count - 1);
            } else {
              // ドラッグ中のブロックの座標がある場合は配列の最も新しい要素を削除する
              //削除対象のオブジェクトを格納
              lastBlock = hitObj;
              //ドラッグ中のオブジェクトの座標を取得
              int matrixX = getMatrix_X(hitObj.transform.position.x);
              int matrixY = getMatrix_Y(hitObj.transform.position.y);
              //print("(x,y)="+"("+matrixY+", "+matrixX+")");
              isExistence[matrixX, matrixY] = false;
              removableBlockX.Add(matrixX);
              removableBlockY.Add(matrixY);
              PushToList(hitObj);
            }
          }
        }
      }
    }
  }

  private void OnDragEnd()
  {
    //print("DRAG END");
    int remove_cnt = removableBlockList.Count;
    if (remove_cnt >= 1) {
      for (int i = 0; i < remove_cnt; i++) {
        Destroy(removableBlockList[i]);
      }
    }
    firstBlock = null;
    lastBlock = null;
  }

  void PushToList(GameObject obj)
  {
    removableBlockList.Add(obj);
    //色の透明度を50%に落とす
    ChangeColor(obj, 0.5f);
  }

  void dropBlock()
  {
    // 消す時に削除フラグを立てたisExistenceを使う
    // 列ごとに消えたブロックの個数をカウント
    // 同時に落下させるブロックはそのブロックよりも下にいくつ消えたブロックがあるかを取得し落下させる

    //for (int i = 0; i < BLOCK_ROW; i++)
    //{
    //  for (int j = 2; j < BLOCK_LINE; j++)
    //  {
    //    print("("+i+", "+j+")="+isExistence[i, j]);
    //  }
    //}
    for (int j = 0; j < BLOCK_ROW; j++)
    {
      deleteBlockCount[j] = 0;
    }
    for (int i = 0; i < BLOCK_ROW; i++)
    {
      for (int j = BLOCK_LINE - 1; j > 1; j--)
      {
        //print("(" + i + ", " + j + ")=" + isExistence[j, i]);
        if (!isExistence[i, j])
        {
          deleteBlockCount[i]++;
          //print("-------------------------------------");
          //print("isExistence=" + isExistence[j, i]);
          //print("(x, y)=" + "(" + i + ", " + j + ")");
          //print("deleteBlockCount=" + deleteBlockCount[i]);
          //print("-------------------------------------");
        } else if (isExistence[i, j] && deleteBlockCount[i] != 0) {
          print("-------------------------------------");
          print("(x, y)=" + "(" + i + ", " + j + ")");
          print("isExistence=" + isExistence[i, j]);
          print("deleteBlockCount=" + deleteBlockCount[i]);
          print("-------------------------------------");
          // 落下させるオブジェクトを求める
          GameObject block = blocks[i, j];
          print("block=" + block);
          // 落下予定地のY座標
          int matrixY = j + deleteBlockCount[i];
          print("blockMarker=" + "(" + i + ", " + matrixY + ")");
          // 落下予定地のマーカーを取得
          GameObject blockMarker = blockMarkers[i, matrixY];
          // 落下させる
          iTween.MoveTo(block, iTween.Hash("y", blockMarker.transform.position.y));
          // isExistenceの更新
          isExistence[i, j] = false;
          isExistence[i, matrixY] = true;
          // ブロックを落下地点の座標に入れ直す
          blocks[i, matrixY] = block;
        }
      }
    }
    //for (int i = 0; i < BLOCK_ROW; i++)
    //{
    //  for (int j = 2; j < BLOCK_LINE; j++)
    //  {
    //    print("(" + i + ", " + j + ")=" + isExistence[i, j]);
    //    print("(" + i + ", " + j + ")=" + blocks[i, j]);
    //  }
    //}
  }

  // 連結用の線を引く
  void createLine()
  {

  }

  int getMatrix_X(float posX)
  {
    float matrix = (posX + 2.0f) / 0.5f;
    return (int)matrix;
  }

  int getMatrix_Y(float posY)
  {
    float matriy = (3.0f - posY) / 0.5f;
    return (int)matriy;
  }

  void ChangeColor(GameObject obj, float transparency)
  {
    //SpriteRendererコンポーネントを取得
    SpriteRenderer blockTexture = obj.GetComponent<SpriteRenderer>();
    //Colorプロパティのうち、透明度のみ変更する
    blockTexture.color = new Color(blockTexture.color.r, blockTexture.color.g, blockTexture.color.b, transparency);
  }

  void createBlock(int row, int line) {
    for (int i = 0; i < row; i++) {
      for (int j = 2; j < line; j++) {
        int objectNum = Random.Range(1, 5);
        // プレファブ取得
        GameObject blockPrefab = GameObject.Find("Stone"+objectNum);
        // オブジェクトのポジション設定
        float posX = -2.0f + 0.5f * i; // (posX + 2.0f) / 0.5f
        float posY = 3.0f - 0.5f * j; // (3.0f - posY) / 0.5f
        Vector2 blockPosition = new Vector2(posX, posY);
        GameObject block = Instantiate(blockPrefab, blockPosition, Quaternion.AngleAxis(Random.Range(-0, 0), Vector3.up)) as GameObject;
        //print("(x, y)=" + "(" + i + ", " + j + ")");
        blocks[i, j] = block;
        //print(blocks[i, j]);
      }
    }
  }

  void createBlockMarker(int row, int line)
  {
    for (int i = 0; i < row; i++)
    {
      for (int j = 0; j < line; j++)
      {
        //int objectNum = Random.Range(1, 5);
        // プレファブ取得
        GameObject blockPrefab = GameObject.Find("BlockMarker");
        // オブジェクトのポジション設定
        float posX = -2.0f + 0.5f * i;
        float posY = 3.0f - 0.5f * j;
        Vector2 blockPosition = new Vector2(posX, posY);
        GameObject blockMarker = Instantiate(blockPrefab, blockPosition, Quaternion.AngleAxis(Random.Range(-0, 0), Vector3.up)) as GameObject;
        blockMarkers[i, j] = blockMarker;
      }
    }
  }

  void createFloorStone(int tall)
  {
    GameObject blockPrefab = GameObject.Find("StoneWalls");
    // オブジェクトのポジション設定
    for (int i = 0; i < 10; i++)
    {
      float posX = 0;
      float posY = 3.0f - 0.5f * (BLOCK_LINE + i);
      Vector2 blockPosition = new Vector2(posX, posY);
      GameObject block = Instantiate(blockPrefab, blockPosition, Quaternion.AngleAxis(Random.Range(-0, 0), Vector3.up)) as GameObject;
    }
  }

  void createSideWall(int tall) {
    // プレファブ取得
    for (int i = 0; i < tall; i++) {
      GameObject blockPrefab = GameObject.Find("StoneSideWall");
      // オブジェクトのポジション設定
      float posX = -2.5f;
      float posY = -5.0f + 0.5f * i;
      Vector2 blockPosition = new Vector2(posX, posY);
      GameObject block = Instantiate(blockPrefab, blockPosition, Quaternion.AngleAxis(Random.Range(-0, 0), Vector3.up)) as GameObject;
    } 
    for (int i = 0; i < tall; i++) {
      GameObject blockPrefab = GameObject.Find("StoneSideWall");
      // オブジェクトのポジション設定
      float posX = 2.5f;
      float posY = -5.0f + 0.5f * i;
      Vector2 blockPosition = new Vector2(posX, posY);
      GameObject block = Instantiate(blockPrefab, blockPosition, Quaternion.AngleAxis(Random.Range(-0, 0), Vector3.up)) as GameObject;
    }
    for (int i = 0; i < tall; i++) {
      GameObject blockPrefab = GameObject.Find("StoneSideWall");
      // オブジェクトのポジション設定
      float posX = 3.0f;
      float posY = -5.0f + 0.5f * i;
      Vector2 blockPosition = new Vector2(posX, posY);
      GameObject block = Instantiate(blockPrefab, blockPosition, Quaternion.AngleAxis(Random.Range(-0, 0), Vector3.up)) as GameObject;
    } 
    for (int i = 0; i < tall; i++) {
      GameObject blockPrefab = GameObject.Find("StoneSideWall");
      // オブジェクトのポジション設定
      float posX = -3.0f;
      float posY = -5.0f + 0.5f * i;
      Vector2 blockPosition = new Vector2(posX, posY);
      GameObject block = Instantiate(blockPrefab, blockPosition, Quaternion.AngleAxis(Random.Range(-0, 0), Vector3.up)) as GameObject;
    } 
  } 
}