using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Random=UnityEngine.Random;

public class RoomControl : MonoBehaviour {
	public GameObject frog;
	public GameObject pellet;
	public FrogMove[] frogs;
	public List<Transform> pellets;
	int lastScore = 1;
	int frogCount = 10 * 4;
	int pelletCount = 90 * 4;
	float roomSize = 20 * 2f;
	float genTime = 60f;
	float sortTime = 5f;

	void Start() {
		Debug.Log(FrogMove.PerfJump +","+ FrogMove.NegJump);
		Time.timeScale = 3;
		Application.runInBackground = true;
		Debug.Log(Random.seed);
		frogs = new FrogMove[frogCount];
		pellets = new List<Transform>();
		for (int i = 0; i < frogCount; i++) { //Instantiate the object in a random position and get its frogmove2
			FrogMove fm = ((GameObject)Instantiate(frog, new Vector3(Random.Range(-roomSize, roomSize), Random.Range(-roomSize, roomSize)), Quaternion.identity)).GetComponent<FrogMove>();
			NeuralNetwork nn = new NeuralNetwork(this, fm);
			fm.Setup(this, roomSize, nn);
			frogs [i] = fm;
		}
		for (int i = 0; i < pelletCount; i++) { //Instantiate the object in a random position and get its transform
			Transform tf = ((GameObject)Instantiate(pellet, new Vector3(Random.Range(-roomSize, roomSize), Random.Range(-roomSize, roomSize)), Quaternion.identity)).transform;
			pellets.Add(tf);
		}
		Invoke("Sort", sortTime);
		Invoke("doGeneration", genTime); //In case we need to cancel the invocation under RemovePellet, this is a one time thing
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.UpArrow)) {
			Time.timeScale += 2f;
		}
		if (Input.GetKeyDown(KeyCode.DownArrow)) {
			Time.timeScale -= 2f;
		}
		if (Input.GetKeyDown(KeyCode.Space)) {
			Time.timeScale = 1f;
		}
	}

	public Vector2 GetNearestPellet(FrogMove frog) {
		Vector3 position = frog.transform.position;
		Vector3 potentialPosition = Vector3.zero; //Potential so that we do not need to make a new float annd Vector3 each time
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

	void doGeneration() {
		List<NeuralNetwork> networks = new List<NeuralNetwork>(pelletCount * 5); //Max upper bounds for total points is number of (pellets * 10) / 2
		for (int i = 0; i < frogs.Length; i++) {
			for (int x = 0; x < frogs[i].GetPoints()+1; x++) { //Make sure even the laziest frog has some chance of breeding
				networks.Add(frogs [i].nn);
			}
		}
		FrogMove[] newFrogs = new FrogMove[frogCount];
		//Debug.Log(frogCount);
		for (int i = 0; i < frogCount; i++) {
			int lIndex = (int)(Random.value * networks.Count);
			int rIndex = (int)(Random.value * networks.Count);
			while (rIndex==lIndex) { //Make sure we are not breeding with ourselves
				rIndex = (int)(Random.value * networks.Count);
			}
			FrogMove fm = ((GameObject)Instantiate(frog, new Vector3(Random.Range(-roomSize, roomSize), Random.Range(-roomSize, roomSize)), Quaternion.identity)).GetComponent<FrogMove>();
			//Debug.Log(fm.gameObject.transform.position);
			Neuron[] neurons = NeuralNetwork.Breed(networks [lIndex].GetNeurons(), networks [rIndex].GetNeurons());
			NeuralNetwork nn = new NeuralNetwork(this, fm, neurons);
			fm.Setup(this, roomSize, nn);
			newFrogs [i] = fm;
		}
		Debug.Log(pelletCount - pellets.Count + ":" + (float)(pelletCount - pellets.Count) / lastScore);
		lastScore = pelletCount - pellets.Count;
		while (pellets.Count<pelletCount) {
			Transform tf = ((GameObject)Instantiate(pellet, new Vector3(Random.Range(-roomSize, roomSize), Random.Range(-roomSize, roomSize)), Quaternion.identity)).transform;
			pellets.Add(tf);
		}

		for (int i = 0; i < frogCount; i++) {

			Destroy(frogs [i].gameObject);
		}
		frogs = newFrogs;
		Invoke("doGeneration", genTime);
	}

	public void RemovePellet(Transform pellet) {
		pellets.Remove(pellet);
		Destroy(pellet.gameObject);
		if (pellets.Count == 0) {
			CancelInvoke();
			doGeneration();
			Invoke("Sort", sortTime);
			Debug.Log("ATE ALL THE PELLETS! OH GOD!");
		}
	}

	void Sort() {
		Array.Sort(frogs);
		Invoke("Sort", sortTime);
	}

	void OnGUI() {
		for (int i = 0; i < 5; i++) {
			GUI.Box(new Rect(0, i * 50, Screen.width, 50), frogs [i].GetPoints() + "\n" + frogs [i].nn.ToString());
		}
	}
}
