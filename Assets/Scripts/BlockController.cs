using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockController : MonoBehaviour {
  public GameObject[] blockPrefab;
  private int BLOCK_LINE = 15;
  private int BLOCK_ROW = 9;
  private GameObject firstBlock; //最初にドラッグしたボール
  private GameObject lastBlock; //最後にドラッグしたボール
  private string currentName; //名前判定用のstring変数
  //削除するボールのリスト
  List<GameObject> removableBlockList = new List<GameObject>();
  // 削除するブロックをブロックの座標で管理する配列
  bool[,] isExistence;
  // 現在のオブジェクトの有無を管理する配列
  GameObject[,] blocks;
  // ブロック落下用のマーカーを管理する配列
  GameObject[,] blockMarkers;
  int[] deleteBlockCount;

  void Start () {
    blocks = new GameObject[BLOCK_LINE, BLOCK_ROW];
    blockMarkers = new GameObject[BLOCK_LINE, BLOCK_ROW];
    createBlock(BLOCK_LINE, BLOCK_ROW);
    createSideWall(23);
    createFloorStone(BLOCK_LINE);
    createBlockMarker(BLOCK_LINE, BLOCK_ROW);
    isExistence = new bool[BLOCK_LINE, BLOCK_ROW];
    for(int i=0;i< BLOCK_LINE; i++)
    {
      for (int j = 0; j < BLOCK_ROW; j++)
      {
        isExistence[i, j] = true;
        //print("("+i+", "+j+")="+isExistence[i, j]);
      }
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
        print("(x,y)=" + "(" + matrixY + ", " + matrixX + ")");
        isExistence[matrixY, matrixX] = false;
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
          //削除対象のオブジェクトを格納
          lastBlock = hitObj;
          //ドラッグ中のオブジェクトの座標を取得
          int matrixX = getMatrix_X(hitObj.transform.position.x);
          int matrixY = getMatrix_Y(hitObj.transform.position.y);
          print("(x,y)="+"("+matrixY+", "+matrixX+")");
          isExistence[matrixY, matrixX] = false;
          PushToList(hitObj);
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
    dropBlock();
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
    for (int i = 0; i < BLOCK_ROW; i++)
    {
      deleteBlockCount[i] = 0;
    }
    for (int i = 0; i < BLOCK_ROW; i++)
    {
      for (int j = BLOCK_LINE - 1; j > 0; j--)
      {
        if (isExistence[i, j])
        {
          deleteBlockCount[i]++;
        } else if (!isExistence[i, j] && deleteBlockCount[i] != 0) {
          // 落下させる
          // 二点間の距離を取得
          // 落下させるオブジェクトとブロックマーカーの距離を測る

          // 落下させるオブジェクトを求める
          Transform dropBlock = blocks[i, j].transform;
          // 落下予定地のマーカーを取得
          // 落下予定地のY座標
          int matrixY = j + deleteBlockCount[i];
          Transform blockMarker = blockMarkers[i, matrixY].transform;
          // 二点間の距離を求める
          float distance = Vector3.Distance(dropBlock.position, blockMarker.position);
          // Leapで距離算出
          // updateに設定できないのでwhileで回す
          float present_Location = (Time.time * 1.0f) / distance;
          while (present_Location < 1)
          {
            dropBlock.transform.position = Vector3.Lerp(dropBlock.position, blockMarker.position, present_Location);
            distance = Vector3.Distance(dropBlock.position, blockMarker.position);
            present_Location = (Time.time * 1.0f) / distance;
          }
        }
      }
    }
  }

  int getMatrix_X(float posX)
  {
    float matrix = (posX + 2.0f) / 0.5f;
    return (int)matrix;
  }

  int getMatrix_Y(float posY)
  {
    float matrix = (posY + 4.5f) / 0.5f;
    return (int)matrix;
  }

  void ChangeColor(GameObject obj, float transparency)
  {
    //SpriteRendererコンポーネントを取得
    SpriteRenderer blockTexture = obj.GetComponent<SpriteRenderer>();
    //Colorプロパティのうち、透明度のみ変更する
    blockTexture.color = new Color(blockTexture.color.r, blockTexture.color.g, blockTexture.color.b, transparency);
  }

  void createBlock(int line, int row) {
    for (int i = 0; i < row; i++) {
      for (int j = 0; j < line; j++) {
        int objectNum = Random.Range(1, 5);
        // プレファブ取得
        GameObject blockPrefab = GameObject.Find("Stone"+objectNum);
        // オブジェクトのポジション設定
        float posX = -2.0f + 0.5f * i; // i = (posX + 2.0f) / 0.5f
        float posY = -4.5f + 0.5f * j; // j = (posY + 4.5f) / 0.5f
        Vector2 blockPosition = new Vector2(posX, posY);
        GameObject block = Instantiate(blockPrefab, blockPosition, Quaternion.AngleAxis(Random.Range(-0, 0), Vector3.up)) as GameObject;
        blocks[j, i] = block; 
      }
    }
  }

  void createBlockMarker(int line, int row)
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
        float posY = -4.5f + 0.5f * j;
        Vector2 blockPosition = new Vector2(posX, posY);
        GameObject blockMarker = Instantiate(blockPrefab, blockPosition, Quaternion.AngleAxis(Random.Range(-0, 0), Vector3.up)) as GameObject;
        blockMarkers[j, i] = blockMarker;
      }
    }
  }

  void createFloorStone(int tall)
  {
    GameObject blockPrefab = GameObject.Find("StoneWalls");
    // オブジェクトのポジション設定
    float posX = 0;
    float posY = 0 + (tall - 5) * -0.5f;
    Vector2 blockPosition = new Vector2(posX, posY);
    GameObject block = Instantiate(blockPrefab, blockPosition, Quaternion.AngleAxis(Random.Range(-0, 0), Vector3.up)) as GameObject;
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