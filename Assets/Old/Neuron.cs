using UnityEngine;
using System.Collections;

public class Neuron
{
		NeuralNetwork parent;
		float[] weights;
		int numInputs;
		int outputType;

		public Neuron (NeuralNetwork parent, int numInputs, int outputType)
		{
				this.numInputs = numInputs;
				weights = new float[numInputs + 1];
				this.parent = parent;
				for (int i = 0; i < weights.Length; i++) {
						weights [i] = Random.Range (-1f, 1f);	
						//Debug.Log("Called");
				}
				this.outputType = outputType;
		}

		public Neuron (NeuralNetwork parent, float[] weights, int outputType)
		{
				this.weights = weights;
				numInputs = weights.Length - 1;
				this.parent = parent;
				this.outputType = outputType;
				//Debug.Log("Me too");


		}

		public float getSignal (float[] values)
		{
				float sum = 0;
				for (int i = 0; i < numInputs; i++) {
						sum += values [i] * weights [i];	
				}
				//Debug.Log (sum);
				if (outputType == 1) {
						return sigmoid (sum) - .5f;
				} else if (outputType == 2) {

						return sigmoid (sum);
				} else {
						//Debug.Log(sum - (values[numInputs]*weights[numInputs]));
						if (sum + (values [numInputs] * weights [numInputs]) > 0) {
								return 1;
						} else {
								return 0;
						}
				}
		}

		private float sigmoid (float value)
		{
				return 1 / (1 + Mathf.Exp (value));
		}

		public void mutate (float mutationChance)
		{
				for (int i = 0; i < weights.Length; i++) {
						if (Random.value < mutationChance) {
								weights [i] += Random.Range (-.2f, .2f);
						}
				}
		}

		public void CopyWeights (float[] weights, int startIndex, int length)
		{
				for (int i = 0; i < length; i++) {
						weights [startIndex + i] = this.weights [startIndex + i];
				}
		}
}
