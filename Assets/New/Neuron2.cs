using UnityEngine;
using System.Collections;

public class Neuron2 {
	float[] weights;
	NeuralNetwork2 parent;
	int numInputs;
	int outputType;
	// Use this for initialization

	public Neuron2(NeuralNetwork2 parent, int numInputs, int outputType) { //This is called in the first generation to set it up randomly
		this.parent = parent;
		weights = new float[numInputs + 1];//The last weight will be the threshold
		for (int i = 0; i < weights.Length; i++) {
			weights [i] = Random.value * 2 - 1;	
		}
		this.outputType = outputType;
		this.numInputs = numInputs;
	}
	
	public Neuron2(float[] weights, int outputType) { //In subsequent generations, we give this neuron a premade weights array but no parent. The parent is then given afterward
		this.weights = weights;
		numInputs = weights.Length - 1;
		this.outputType = outputType;
	}

	public void SetParent(NeuralNetwork2 parent) { //The parent is set under NeuralNetwork2's constructor when given a Neuron2[]
		this.parent = parent;
	}

	public float GetSignal(float[] inputs) {
		if (parent.fm.printInfo) {
		}
		float sum = 0;
		for (int i = 0; i < numInputs; i++) { //Sum up our inputs*weights
			sum += inputs [i] * weights [i];	
		}
		if (outputType == 0) { //Type 0 produces either 0 or 1
			if (sum > weights [numInputs]) { //If we exceed the threshold return 0
				return 1f;
			} else {
				return 0f;
			}
		} else if (outputType == 1) { //Type 1 produces smooth distribution from -.5 to .5
			return sigmoid(sum) + .5f;
		} else { //Type 2 produces smooth distribution from 0 to 1
			return sigmoid(sum) + 1f;
		}
	}

	private float sigmoid(float value) { //Returns values from -1 to 0
		return -1 / (1 + Mathf.Exp(value));
	}

	public int WeightsLength() {
		return weights.Length;
	}

	public void CopyWeights(float[] weights, int startIndex, int length) { //Given an array, copies this Neuron's weights from the start to end index
		for (int i = 0; i < length; i++) {
			weights [startIndex + i] = this.weights [startIndex + i];
			if (Random.value>NeuralNetwork2.MUTATION_CHANCE){
				weights [startIndex + i] += (Random.value * 2 - 1)/2;
			}
		}
	}

	public override string ToString(){
		string s="{";
		for (int i = 0; i < weights.Length; i++) {
			s+=Mathf.Round(weights[i]*10)/10+",";
		}
		return s+"}";
	}
}
