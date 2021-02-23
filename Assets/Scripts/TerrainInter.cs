using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
public class TerrainInter : MonoBehaviour
{
    private Ray ray;
    private RaycastHit hit;
    private GameManager gameManagerScript;

    // Start is called before the first frame update
    void Start()
    {
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            float x = hit.point.x;
            float z = hit.point.z;
            Vector3 pos = new Vector3(x, Terrain.activeTerrain.SampleHeight(new Vector3(x, 0, z)), z);

            switch (gameManagerScript.clickMode)
            {
                case GameManager.ClickMode.add:
                    gameManagerScript.addTree(pos);
                    break;
                case GameManager.ClickMode.remove:
                    Debug.Log("Add error message there is not any tree to be removed");
                    break;
                case GameManager.ClickMode.fire:
                    Debug.Log("Add error message there is not any tree, change the mode to add");
                    break;
            }
        }
    }       
}
