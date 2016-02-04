using UnityEngine;
using System.Collections;
using System.Linq;

public class NeuralNetwork {
	// Use this for initialization
	static int numInputs = 3; //nearestX, nearestY, lastJump
	static int numOutputs = 4; //xForce, yForce, jump, jumpSpeed
	RoomControl rc;
	public FrogMove fm;
	float[] inputs;
	Neuron[] topLayer;
	Neuron[] hiddenLayer;
	static int hiddenSize = 6;
	public static readonly float MUTATION_CHANCE = .05f; //5% chance to mutate
	private float[] getInputs() {
		float[] inputs = new float[numInputs];
		Vector2 nearest = rc.GetNearestPellet(fm);
		nearest -= (Vector2)fm.transform.position; //Turn nearest into a vector pointing from us to the nearest pellet
		inputs [0] = nearest.x;
		inputs [1] = nearest.y;
		inputs [2] = fm.getLastJump();
		return inputs;
	}

	private float[] getHidden() { //Get the results from the hidden layer w/ input getInputs()
		float[] values = new float[hiddenLayer.Length];
		for (int i = 0; i < hiddenLayer.Length; i++) {
			values [i] = hiddenLayer [i].GetSignal(getInputs());	
		}
		return values;
	}

	public NeuralNetwork(RoomControl rc, FrogMove fm) {//Inputs are fed to the hidden layer which are in turn fed into the visible layer
		hiddenLayer = new Neuron[hiddenSize];
		this.rc = rc;
		this.fm = fm;
		for (int i = 0; i < hiddenSize; i++) {
			hiddenLayer [i] = new Neuron(this, numInputs, 1);
		}
		topLayer = new Neuron[numOutputs];
		topLayer [0] = new Neuron(this, hiddenSize, 1);
		topLayer [1] = new Neuron(this, hiddenSize, 1);
		topLayer [2] = new Neuron(this, hiddenSize, 0);
		topLayer [3] = new Neuron(this, hiddenSize, 2);
	}

	public NeuralNetwork(RoomControl rc, FrogMove fm, Neuron[] neurons) {//We already have the whole neuron array (called in generations after the 1st)
		hiddenLayer = new Neuron[hiddenSize];
		this.rc = rc;
		this.fm = fm;
		int i = 0;
		for (; i < hiddenSize; i++) {
			hiddenLayer [i] = neurons [i];
			hiddenLayer [i].SetParent(this);
		}
		topLayer = new Neuron[numOutputs];
		for (int x=0; x<numOutputs; x++) {
			topLayer [x] = neurons [i + x];
			topLayer [x].SetParent(this);
		}
	}

	public float getXForce() {
		return topLayer [0].GetSignal(getHidden());
	}

	public float getYForce() {
		return topLayer [1].GetSignal(getHidden());
	}

	public bool getJump() { //GetSignal will return either a 1 or 0 and we want a bool.
		return topLayer [2].GetSignal(getHidden()) == 1;
	}

	public float getSpeed() {
		return topLayer [3].GetSignal(getHidden());
	}

	public Neuron[] GetNeurons() {//Concatenate hiddenLayer and topLayer and return them as an array,
		return hiddenLayer.Concat(topLayer).ToArray(); //this array contains all important info for our network
	}

	public static Neuron[] Breed(Neuron[] lnn, Neuron[] rnn) { //Mutation happens under CopyWeight
		Neuron[] baby = new Neuron[numOutputs + hiddenSize];
		for (int i=0; i<baby.Length; i++) { //For every Neuron, cross lnn and rnn and add it to the array
			int length = lnn [i].WeightsLength();
			int index = Random.Range(0, length);
			float[] weights = new float[length];
			lnn [i].CopyWeights(weights, 0, index);
			rnn [i].CopyWeights(weights, index, weights.Length - index);
			if (i == baby.Length - 2) { //Making the jump neuron
				baby [i] = new Neuron(weights, 0);
			} else if (i == baby.Length - 1) {  //Making the speed neuron
				baby [i] = new Neuron(weights, 2);
			} else {
				baby [i] = new Neuron(weights, 1);
				
			}
		}
		return baby;
	}

	public string ToString() {
		string s = "";
		int i = 0;
		foreach (Neuron neur in GetNeurons()) {
			s += i + ": " + neur.ToString();
			i++;
			if (i == 6) {
				s += "\n";
			}
		}
		return s;
	}
}
