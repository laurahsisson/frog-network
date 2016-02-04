using UnityEngine;
using System.Collections;
using System.Linq;

public class NeuralNetwork2 {
	// Use this for initialization
	static int numInputs = 3; //nearestX, nearestY, lastJump
	static int numOutputs = 4; //xForce, yForce, jump, jumpSpeed
	Room_Control rc;
	public FrogMove2 fm;
	float[] inputs;
	Neuron2[] topLayer;
	Neuron2[] hiddenLayer;
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

	private float[] getHidden() {
		float[] values = new float[hiddenLayer.Length];
		for (int i = 0; i < hiddenLayer.Length; i++) {
			values [i] = hiddenLayer [i].GetSignal(getInputs());	
		}
		return values;
	}

	public NeuralNetwork2(Room_Control rc, FrogMove2 fm) {//Inputs are fed to the hidden layer which are in turn fed into the visible layer
		hiddenLayer = new Neuron2[hiddenSize];
		this.rc = rc;
		this.fm = fm;
		for (int i = 0; i < hiddenSize; i++) {
			hiddenLayer [i] = new Neuron2(this, numInputs, 1);
		}
		topLayer = new Neuron2[numOutputs];
		topLayer [0] = new Neuron2(this, hiddenSize, 1);
		topLayer [1] = new Neuron2(this, hiddenSize, 1);
		topLayer [2] = new Neuron2(this, hiddenSize, 0);
		topLayer [3] = new Neuron2(this, hiddenSize, 2);
	}

	public NeuralNetwork2(Room_Control rc, FrogMove2 fm, Neuron2[] neurons) {//We already have the whole neuron array
		hiddenLayer = new Neuron2[hiddenSize];
		this.rc = rc;
		this.fm = fm;
		int i = 0;
		for (; i < hiddenSize; i++) {
			hiddenLayer [i] = neurons [i];
			hiddenLayer[i].SetParent(this);
		}
		topLayer = new Neuron2[numOutputs];
		for (int x=0; x<numOutputs; x++) {
			topLayer [x] = neurons [i + x];
			topLayer[x].SetParent(this);
		}
	}

	public float getXForce() {
		return topLayer [0].GetSignal(getHidden());
	}

	public float getYForce() {
		return topLayer [1].GetSignal(getHidden());
	}

	public bool getJump() {
		return topLayer [2].GetSignal(getHidden()) == 1;
	}
	public float getSpeed(){
		return topLayer [3].GetSignal(getHidden());
	}
	public Neuron2[] GetNeurons() {//Concatenate hiddenLayer and topLayer and return them as an array,
		return hiddenLayer.Concat(topLayer).ToArray(); //this array contains all important info for our network
	}

	public static Neuron2[] Breed(Neuron2[] lnn, Neuron2[] rnn) { //Mutation happens under CopyWeight
		Neuron2[] baby = new Neuron2[numOutputs + hiddenSize];
		for (int i=0; i<baby.Length; i++) {
			int length = lnn [i].WeightsLength();
			int index = Random.Range(0, length);
			float[] weights = new float[length];
			lnn [i].CopyWeights(weights, 0, index);
			rnn [i].CopyWeights(weights, index, weights.Length - index);
			if (i == baby.Length - 2) { //Making the jump neuron
				baby[i] = new Neuron2(weights,0);
			} else if (i == baby.Length - 1){  //Making the speed neuron
				baby[i] = new Neuron2(weights,2);
			} else {
				baby[i] = new Neuron2(weights,1);
				
			}
		}
		return baby;
	}
	public string ToString(){
		string s="";
		int i=0;
		foreach (Neuron2 neur in GetNeurons()) {
			s+=i + ": " + neur.ToString();
			i++;
			if (i==6){
				s+="\n";
			}
		}
		return s;
	}
}
