using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GemGrid : MonoBehaviour
{
    public GameObject[] m_gemPrefab;
    public int m_width = 6;
    public int m_height = 6;
    public float m_gridSize = 50.0f;
    public GameObject m_gemPopSound;
    public GameObject m_pauseMenu;
    public GameObject gameover;
    public Text m_text;

    Gem[,] m_grid;
    float[] m_yOffset;
    int moves;
    bool m_isAnimating = true;
    bool m_isPaused = false;

    // Start is called before the first frame update
    void Start()
    {
        m_grid = new Gem[m_width, m_height];
        m_yOffset = new float[m_width];
        moves = 10;
        m_text.text = "Move: " + moves.ToString();
        gameover.SetActive(false);
        SetPause(false);
        FillGrid();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < m_width; ++i)
        {
            m_yOffset[i] = 0.0f;
        }
        // wait for the falling to stop
        bool falling = false;
        for (int y = 0; y < m_height; ++y)
        {
            for (int x = 0; x < m_width; ++x)
            {
                if (null != m_grid[x, y] && m_grid[x, y].IsFalling())
                {
                    falling = true;
                    m_isAnimating = true;
                }
            }
        }

        if (false == falling)
        {
            if (false == CheckMatch())
            {
                m_isAnimating = false;
            }
        }

        // keys
        if (Input.GetKeyDown(KeyCode.Escape))
        {   // this doubles as the option key in the android navigation bar
            SetPause(!m_isPaused);
        }
    }

    void breakMap(List<Gem> output){
        for (int i = 0; i < m_width; i++){
            for (int j = 0; j < m_height; j++){
                output.Add(m_grid[i,j]);
            }
        }
    }

    bool CheckMatch()
    {
        // a list of gems to be broken
        List<Gem> breakGems = new List<Gem>();

            // check for matches of 3 or more gems in the vertical direction
            for (int i = 0; i < m_width; i++){
                for (int j = 0; j < m_height; j++){
                    Gem gem1= m_grid[i,j];

                    List<Gem> xtemp = new List<Gem>();
                    List<Gem> ytemp = new List<Gem>();

                    xtemp.Add(gem1);ytemp.Add(gem1);

                    // check row
                    int x_inc = 1; 
                    while (i+x_inc < m_width){
                        if (m_grid[i+x_inc,j].m_gemType == gem1.m_gemType){
                            xtemp.Add(m_grid[i+x_inc,j]);
                        }
                        else {
                            break;
                        }
                        x_inc++;
                    }

                    // check column 
                    int y_inc = 1; 
                    while (j-y_inc >= 0){
                        if (m_grid[i,j-y_inc].m_gemType == gem1.m_gemType){
                            ytemp.Add(m_grid[i,j-y_inc]);
                        }
                        else {
                            break;
                        }
                        y_inc++;
                    }

                    // 5 or more, break the map
                    if (xtemp.Count >= 5 || ytemp.Count >= 5){
                        breakMap(breakGems);
                        Debug.Log("5 or more");
                        goto End;

                    }

                    // L-shape
                    if (xtemp.Count >= 3 && ytemp.Count >= 3){
                        for (int x = i; x < i + xtemp.Count; x++){
                            for (int y = j; y > j - ytemp.Count; y--){
                                breakGems.Add(m_grid[x,y]);
                            }
                        }                   
                        Debug.Log("L Shape");
                        goto End;
                    }

                    // 4 match in a line
                    else if (xtemp.Count == 4){
                        // break row
                        for (int x = 0; x < m_width; x++){
                            breakGems.Add(m_grid[x,j]);
                        }
                        Debug.Log("4 match in a row");
                        goto End;
                    }
                    else if (ytemp.Count == 4){
                        for (int y = 0; y < m_height; y++){
                            breakGems.Add(m_grid[i,y]);
                        }
                        Debug.Log("4 match in a column");
                        goto End;
                    }

                    // pop
                    else if (xtemp.Count == 3){
                        breakGems.AddRange(xtemp); 
                        Debug.Log("Row pop");

                    }
                    else if (ytemp.Count == 3){
                        breakGems.AddRange(ytemp); 
                        Debug.Log("Column pop");
                    }                
                }
            }
            End:
        

        {   // TODO call BreakGem() on all the gems in your list of gems to break
            // If there are any, play the gem popping sound
            // If any gems broke, return true to indicate we need to re-enter the "falling" stage
            if (breakGems.Count != 0){
                for (int i = 0; i < breakGems.Count; i++){
                    breakGems[i].BreakGem();
                }
                GameObject pop = Instantiate(m_gemPopSound);
                return true;
            }
        }
        return false;   // returning false indicates everything is static
    }

    void SpawnGem(int x, int y)
    {
        GameObject gem = Instantiate(m_gemPrefab[Random.Range(0, m_gemPrefab.Length)]);
        gem.transform.SetParent(transform);
        gem.transform.localScale = Vector3.one;
        Vector2 gemPos = GetGemPos(x, y);
        gemPos.y = m_yOffset[x] + 0.5f * m_height * m_gridSize;
        gem.transform.localPosition = gemPos;
        m_grid[x, y] = gem.GetComponent<Gem>();
        m_grid[x, y].SetSlot(this, x, y);
        m_yOffset[x] += 50.0f;
    }

    void FillGrid()
    {
        for (int y = m_height - 1; y >= 0; --y)
        {
            for (int x = 0; x < m_width; ++x)
            {
                SpawnGem(x, y);
            }
        }
    }

    public void BreakGem(int x, int y)
    {
        if (null != m_grid[x, y])
        {
            Destroy(m_grid[x, y].gameObject);
            for (int row = y; row > 0; --row)
            {
                m_grid[x, row] = m_grid[x, row - 1];
                m_grid[x, row].SetSlot(this, x, row);
            }
            SpawnGem(x, 0);
        }
    }

    public bool IsAnimating()
    {
        return m_isAnimating;
    }

    public void Swap(int x1, int y1, int x2, int y2)

    {
        Gem gem1 = m_grid[x1, y1];
        Gem gem2 = m_grid[x2, y2];
        m_grid[x1, y1] = gem2;
        m_grid[x2, y2] = gem1;
        gem1.SetSlot(this, x2, y2);
        gem2.SetSlot(this, x1, y1);

        moves--;
        m_text.text = "Move: " + moves.ToString();
        
        if (moves == 0){
            GameOver();
        }
    }

    public Vector2 GetGemPos(int x, int y)
    {
        return new Vector2(m_gridSize * (x - 0.5f * m_width + 0.5f),
            m_gridSize * (0.5f * m_height - y - 0.5f));
    }

    public void SetPause(bool setPause)
    {
        if (setPause)
        {
            Time.timeScale = 0.0f;
        }
        else
        {
            Time.timeScale = 1.0f;
        }
        m_pauseMenu.SetActive(setPause);
        m_isPaused = setPause;
    }

    IEnumerator gg(){
        gameover.SetActive(true);
        yield return new WaitForSecondsRealtime(2.0f);
        SceneManager.LoadScene("Game");
    }

    public void GameOver()
    {
        {   // TODO change this to kick-off a coroutine
            // Unhide the "Game Over" text
            // Delay 2 seconds
            // Then load the scene in the coroutine
            StartCoroutine(gg());
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
