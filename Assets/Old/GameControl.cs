using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameControl : MonoBehaviour {
	public GameObject frog;
	public GameObject pellet;
	public FrogMove[] frogs;
	public List<Transform> pellets;
	float mutationChance = .1f;
	float roomSize = 20f;
	int genCount = 0;
	int pelCount = 0;
	// Use this for initialization
	void Start() {
		frogs = new FrogMove[10];
		pellets = new List<Transform>();
		for (int i = 0; i < frogs.Length; i++) {
			GameObject go = (GameObject)Instantiate(frog, new Vector3(Random.Range(-roomSize, roomSize), Random.Range(-roomSize, roomSize)), Quaternion.identity);
			frogs [i] = go.GetComponent<FrogMove>();
			frogs [i].setParent(this);
		}
		for (int i = 0; i < 120; i++) {
			GameObject go = (GameObject)Instantiate(pellet, new Vector3(Random.Range(-roomSize, roomSize), Random.Range(-roomSize, roomSize)), Quaternion.identity);
			pellets.Add(go.transform);
		}
		Invoke("Cull", 20);
	}

	private void Cull() {
		int lowestPoints = frogs [getWeakest()].getPoints();
		//int currentPoints = lowestPoints;
		int removal = 0;
		while (removal<3) {
			removal++;
			int tFrogs = 0; //Frogs that scores more than zero
			//Debug.Log (frogs [getWeakest ()].getPoints());
			int index = getWeakest();
			int roundPoints = frogs [index].getPoints();
			int sum = 0;
			for (int i = 0; i < frogs.Length; i++) {
				if (i != index) {
					frogs [i].mutate(mutationChance);
					sum += frogs [i].getPoints();
					if (frogs [i].getPoints() > 0) {
						tFrogs++;
					}
					//sum++; //Every frog gets 1 chance, even if they didnt eat
				}

			}

			FrogMove[] wFrogs = new FrogMove[sum];
			int loc = 0;
			for (int i = 0; i < frogs.Length; i++) {
				if (i != index) {
					int weight = frogs [i].getPoints();
					frogs [i].subtractPoints(roundPoints); //Keep it fresh
					for (int j = 0; j < weight; j++) {
						wFrogs [loc] = frogs [i];
						loc++;
					}
				}
			}
			if (tFrogs > 0) {
				Destroy(frogs [index].gameObject);
				frogs [index] = null;
				//Debug.Log(wFrogs.Length);
				FrogMove lfr = wFrogs [Random.Range(0, wFrogs.Length - 1)];
				FrogMove rfr = wFrogs [Random.Range(0, wFrogs.Length - 1)];
				if (tFrogs > 1) {
					while (lfr.Equals(rfr)) {
						rfr = wFrogs [Random.Range(0, wFrogs.Length - 1)];
					}
				}
				GameObject go = (GameObject)Instantiate(frog, new Vector3(Random.Range(-roomSize, roomSize), Random.Range(-roomSize, roomSize)), Quaternion.identity);
				frogs [index] = go.GetComponent<FrogMove>();
				frogs [index].setParent(this);
				frogs [index].subtractPoints(-roundPoints - 1); //So that the baby does not die immediately
				FrogMove.Breed(frogs [index], lfr, rfr);
			} else {
				Debug.Log("Everyone starved!");
			}
		}
		//currentPoints = frogs [getWeakest ()].getPoints ();
		genCount++;
		/*pelCount += 120 - pellets.Count;
				if (genCount > 5) {
						Debug.Log (pelCount / 5);
						genCount = 0;
						pelCount = 0;
				}*/
		Debug.Log(120 - pellets.Count);
		while (pellets.Count<120) {
			GameObject pel = (GameObject)Instantiate(pellet, new Vector3(Random.Range(-roomSize, roomSize), Random.Range(-roomSize, roomSize)), Quaternion.identity);
			pellets.Add(pel.transform);
		}
		Invoke("Cull", 20);
	}
	// Update is called once per frame
	void Update() {
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			Time.timeScale = 10f;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			Time.timeScale -= .5f;
		}
		if (Input.GetKeyDown(KeyCode.Space)) {
			Time.timeScale = 1f;
		}
	}

	int getWeakest() {
		int i = -1;
		int current = 0;
		int points = 2147483647;
		for (i=0; i < frogs.Length; i++) {
			if (frogs [i] && frogs [i].getPoints() <= points) {

				points = frogs [i].getPoints();
				current = i;
			}
		}
		return current;
	}

	public Vector2 getNearestPelletVector(FrogMove frog) {
		Vector3 position = frog.transform.position;
		Vector3 potentialPosition = Vector3.zero;
		float sqrDistance = Mathf.Infinity;
		float potentialSqrDistance = -1;
		foreach (Transform p in pellets) {
			potentialSqrDistance = (p.transform.position - position).sqrMagnitude;
			if (potentialSqrDistance < sqrDistance) {
				sqrDistance = potentialSqrDistance;
				potentialPosition = p.transform.position;				
			}
		}
		return (Vector2)(potentialPosition);
	}

	public void respawnPellet(Transform transform) {
		pellets.Remove(transform);
		Destroy(transform.gameObject);

	}
}
