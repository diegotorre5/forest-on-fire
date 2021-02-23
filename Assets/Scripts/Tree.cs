using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System;
using UnityEngine.EventSystems;

public class Tree : MonoBehaviour
{
    public enum TreeStatus
    {
       green,
       ignition,
       burning,
       burned
    }

    public TreeStatus status;
    private IEnumerator coroutineBurning;
    private IEnumerator coroutineSpreadFireForward;
    private IEnumerator coroutineSpreadFireRadious;
    public GameObject windDirection;
    public Slider sliderDirection;
    public Slider sliderSpeed;
    public float ignitionTime;
    public float burningTime;
    private GameManager gameManagerScript;

    void Start()
    {
        gameManagerScript = GameObject.Find("GameManager").GetComponent<GameManager>();

    }

    void plant()
    {
        status = TreeStatus.green;
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>()){
            renderer.material.color = Color.green;
        }
    }

    IEnumerator igniteProcess()
    {
        status = TreeStatus.ignition;
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>()){
            renderer.material.color = Color.yellow;
        }
        yield return new WaitForSeconds(ignitionTime);
            status = TreeStatus.burning;
            coroutineBurning = burningProcess();
            StartCoroutine(coroutineBurning);
        yield return null;
    }

    IEnumerator burningProcess()
    {
        status = TreeStatus.burning;
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>()){
            renderer.material.color = Color.red;
        }
        coroutineSpreadFireForward = spreadFireForwardProcess();
        StartCoroutine(coroutineSpreadFireForward);
        coroutineSpreadFireRadious = spreadFireRadiousProcess();
        StartCoroutine(coroutineSpreadFireRadious);
        yield return new WaitForSeconds(burningTime);
        makeAshes();
        yield return null;
    }

    void makeAshes() {
        status = TreeStatus.burned;
        foreach(Renderer renderer in GetComponentsInChildren<Renderer>()){
            renderer.material.color = Color.black;
        }

    }

    IEnumerator spreadFireForwardProcess()
    {
        float minDistance = 0;
        RaycastHit[] hits = new RaycastHit[0];
        windDirection.transform.localRotation = Quaternion.Euler(0, sliderDirection.value, 0);
        var dicHits = new Dictionary<RaycastHit, float>();
            hits = Physics.RaycastAll(windDirection.transform.position, windDirection.transform.forward, 100.0f);
        if (hits.Length > 1)
        {
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].transform.name.Contains("Tree")) { 
                    dicHits.Add(hits[i], Vector3.Distance(windDirection.transform.position, hits[i].transform.position));
                }

            }
            minDistance = dicHits.Values.Min();
            RaycastHit min = dicHits.FirstOrDefault(x => x.Value == minDistance).Key;
            if (minDistance < 15 )
            {
                Tree treeScript = min.transform.GetComponent<Tree>();
                if (treeScript.status == TreeStatus.green)
                {
                    treeScript.status = TreeStatus.ignition;
                    StartCoroutine(treeScript.igniteProcess());
                    
                }
            }
        }

        dicHits = null;
        yield return null;
    }

    IEnumerator spreadFireRadiousProcess()
    {
        Collider[] hitColliders = new Collider[10];
       int numColiders = Physics.OverlapSphereNonAlloc(this.gameObject.transform.position, 1, hitColliders);
        for (int i = 0; i < numColiders; i++) {
            if (hitColliders[i].name.Contains("Tree")) {
                Tree treeScript = hitColliders[i].transform.GetComponent<Tree>();
                if (treeScript.status == TreeStatus.green)
                {
                    treeScript.status = TreeStatus.ignition;
                    StartCoroutine(treeScript.igniteProcess());
                }
            }
        }
        hitColliders = null;
        yield return null;
    }

    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

            RaycastHit hit;
        var ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            float x = hit.point.x;
            float z = hit.point.z;
            Vector3 pos = new Vector3(x, 0, z);

            switch(gameManagerScript.clickMode)
            {
                case GameManager.ClickMode.add:
                    Debug.Log("Add error message Tree already in place");
                    break;
                case GameManager.ClickMode.remove:
                    Destroy(gameObject);
                    break;
                case GameManager.ClickMode.fire:
                    if (status == TreeStatus.burning || status == TreeStatus.ignition) {
                        plant();
                        StopCoroutine(coroutineBurning);
                    }
                    else {
                        ignite();
                    }
                    break;
            }
        }
   
    }

    public void ignite() {
        status = TreeStatus.ignition;
        coroutineBurning = igniteProcess();
        StartCoroutine(coroutineBurning);
    }

}
