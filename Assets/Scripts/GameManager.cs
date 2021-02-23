using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public enum ClickMode
    {
        add,
        remove,
        fire
    }
    
    public enum GameState
    {
        play,
        pause
    }

    public GameState State = GameState.play;
    public ClickMode clickMode = ClickMode.add;

    public Slider sliderDirection;
    public Slider sliderSpeed;
    public GameObject treePrefab;
    private GameObject forest;

    private int TreeNumber;
    public int windSpeedTime = 1;

    int layerMask = 1 << 8;

    void Start()
    {
        Screen.fullScreen = !Screen.fullScreen;
        Screen.SetResolution(1024, 768, true);
    }

    public void Generate()
    {
        Clear();
        forest = new GameObject();
        forest.name = "forest";
        int numberOfObjects = Random.Range(6000, 8000);
        int numberOfObject = 0;
        GameObject clone;
        Tree Treescript; 
        changeWindSpeed((int)sliderSpeed.value);
        for (int i = 0; i < numberOfObjects; i++)
        {
            float x = Random.Range(0, 150);
            float z = Random.Range(0, 100);
            Vector3 pos = new Vector3(x, Terrain.activeTerrain.SampleHeight(new Vector3(x,0,z)), z);

            if (Physics.CheckSphere(pos, 0.01f, layerMask))
            {
                numberOfObjects--;
            }
            else
            {
                numberOfObject++;
                clone = Instantiate(treePrefab, pos, Quaternion.identity);
                clone.name = "Tree" + numberOfObject.ToString();
                clone.transform.SetParent(forest.transform);

                Treescript = clone.GetComponent<Tree>();
                Treescript.ignitionTime = Random.Range(2, 3);
                Treescript.burningTime = Random.Range(5, 7);
                Treescript.status = Tree.TreeStatus.green;
                Treescript.sliderDirection = sliderDirection;
                Treescript.sliderSpeed = sliderSpeed;
            }

        }
        TreeNumber = numberOfObjects - 1;
    }

    public void addTree(Vector3 position)
    {
        if (GameObject.Find("forest") == null)
        {
            forest = new GameObject();
            forest.name = "forest";
        }
        GameObject clone;
        Tree Treescript; 
        TreeNumber++;
        clone = Instantiate(treePrefab, position, Quaternion.identity);
        clone.name = "Tree" + TreeNumber;
        clone.transform.SetParent(forest.transform);
        Treescript = clone.GetComponent<Tree>();
        Treescript.ignitionTime = Random.Range(5, 7);
        Treescript.burningTime = Random.Range(5, 7);
        Treescript.status = Tree.TreeStatus.green;
        Treescript.sliderDirection = sliderDirection;
        Treescript.sliderSpeed = sliderSpeed;
    }

    public void Clear()
    {
        TreeNumber = 0;
        Destroy(forest);
    }

    public void changeGameState() {
        if (State == GameManager.GameState.play)
        {
            State = GameManager.GameState.pause;
            Time.timeScale = 0;
        }
        else {
            State = GameManager.GameState.play;
            Time.timeScale = windSpeedTime;
        }
    }

    public void changeWindSpeed(int Value)
    {
        if (Value < 10)
         Value = 10; 
        windSpeedTime = Value / 10;
        Time.timeScale = windSpeedTime;

    }

    public void randomFire()
    {
        if (TreeNumber == 0)
            return;
        int numberSparx;
        int[] Sparx = new int[10];
        numberSparx = Random.Range(8, 10);
        for (int i = 0; i < numberSparx; i++) {
            GameObject Tree = null;
            while (Tree == null) {
                Tree = GameObject.Find("Tree" + Random.Range(1, TreeNumber));
            }
            Tree treeScript = Tree.gameObject.GetComponent<Tree>();
            treeScript.ignite();
        }
    }

    // Update is called once per frame

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                 Application.Quit();
        #endif
    }


}
