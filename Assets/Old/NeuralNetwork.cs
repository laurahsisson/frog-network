using UnityEngine;
using System.Collections;

public class NeuralNetwork : MonoBehaviour {
	public FrogMove parent;
	int numInputs = 4; //x nearestPellet, y nearestPellet, x facingDirection, y facingDirection, lastHop
	// Use this for initialization
	Neuron hop;
	Neuron rotationX;
	Neuron rotationY;
	Neuron power;
	Neuron[] hiddenLayer = new Neuron[6];
	static public GameObject frogPrefab;

	void Start() {
		for (int i = 0; i < hiddenLayer.Length; i++) {
			hiddenLayer [i] = new Neuron(this, numInputs, Random.Range(0, 3));
		}
		hop = new Neuron(this, hiddenLayer.Length, 0);
		rotationX = new Neuron(this, hiddenLayer.Length, 1);
		rotationY = new Neuron(this, hiddenLayer.Length, 1);
		power = new Neuron(this, hiddenLayer.Length, 2);
	}
	
	// Update is called once per frame
	void Update() {
		float[] values = new float[hiddenLayer.Length + 1];
		for (int i = 0; i < hiddenLayer.Length; i++) {
			values [i] = hiddenLayer [i].getSignal(getInputs());	
		}
		values [hiddenLayer.Length] = -1;
		//if (hop.getSignal (values) == 1) {
		//parent.jump (rotationX.getSignal (values), rotationY.getSignal (values), power.getSignal (values));
		//}
		//Debug.Log (rotation.Signal (values));
		if (hop.getSignal(values) == 1) {
			parent.rotation(rotationX.getSignal(values), rotationY.getSignal(values));
			//Debug.Log("Hopping @ " + power.getSignal(values));
			parent.hop(power.getSignal(values));
		}
	}

	public float[] getInputs() { //Size is numInputs +1 to account for the -1 bias
		float[] values = new float[numInputs + 1];
		values [0] = parent.getNearestPelletVector().x;
		values [1] = parent.getNearestPelletVector().y;
		values [2] = parent.transform.position.x;//parent.getLookAtVector ().x;
		values [3] = parent.transform.position.y;//parent.getLookAtVector ().y;
		//values [4] = parent.getLastHop ();
		values [4] = -1f;
		return values;
	}

	public void mutate(float mutationChance) {
		hop.mutate(mutationChance);
		power.mutate(mutationChance);
		rotationX.mutate(mutationChance);
		rotationY.mutate(mutationChance);
		for (int i = 0; i < hiddenLayer.Length; i++) {
			hiddenLayer [i].mutate(mutationChance);	
		}
	}

	public static void Breed(NeuralNetwork baby, NeuralNetwork lnn, NeuralNetwork rnn) {
		float[] hopWeights = new float[lnn.hiddenLayer.Length + 1];
		int index = Random.Range(0, hopWeights.Length);
		lnn.hop.CopyWeights(hopWeights, 0, index);
		rnn.hop.CopyWeights(hopWeights, index, hopWeights.Length - index);
		baby.hop = new Neuron(baby, hopWeights, 0);

		float[] powerWeights = new float[lnn.hiddenLayer.Length + 1];
		index = Random.Range(0, powerWeights.Length);
		lnn.power.CopyWeights(powerWeights, 0, index);
		rnn.power.CopyWeights(powerWeights, index, powerWeights.Length - index);
		baby.power = new Neuron(baby, powerWeights, 0);

		float[] rotationXWeights = new float[lnn.hiddenLayer.Length + 1];
		index = Random.Range(0, rotationXWeights.Length);
		lnn.rotationX.CopyWeights(rotationXWeights, 0, index);
		rnn.rotationX.CopyWeights(rotationXWeights, index, rotationXWeights.Length - index);
		baby.rotationX = new Neuron(baby, rotationXWeights, 1);

		string s = "" + lnn.parent.getPoints() + " and " + rnn.parent.getPoints() + " to ";
		for (int i = 0; i < hopWeights.Length; i++) {
			s += i + ": " + hopWeights [i] + ". ";	
		}
		//Debug.Log (s);

		float[] rotationYWeights = new float[lnn.hiddenLayer.Length + 1];
		index = Random.Range(0, rotationYWeights.Length);
		lnn.rotationY.CopyWeights(rotationYWeights, 0, index);
		rnn.rotationY.CopyWeights(rotationYWeights, index, rotationYWeights.Length - index);
		baby.rotationY = new Neuron(baby, rotationYWeights, 1);
		
		float[][] hiddenLayerWeights = new float[lnn.hiddenLayer.Length][];
		for (int i = 0; i < hiddenLayerWeights.Length; i++) {
			hiddenLayerWeights [i] = new float[lnn.numInputs + 1];
			index = Random.Range(0, hiddenLayerWeights [i].Length);
			lnn.hiddenLayer [i].CopyWeights(hiddenLayerWeights [i], 0, index);
			rnn.hiddenLayer [i].CopyWeights(hiddenLayerWeights [i], index, hiddenLayerWeights [i].Length - index);
			baby.hiddenLayer [i] = new Neuron(baby, hiddenLayerWeights [i], Random.Range(0, 3));
			//Debug.Log(hiddenLayerWeights[i]);
		}
	}
}
