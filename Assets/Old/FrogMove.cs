using UnityEngine;
using System.Collections;

public class FrogMove : MonoBehaviour
{

		// Use this for initialization
		float speed;
		float direction;
		float lastSpeed;
		float roomSize = 20;
		float lastHop = 0;
		GameControl parent;
		int points = 0;
		public NeuralNetwork network;

		void Start ()
		{
				direction = Random.Range (0, 360);

		}

		// Update is called once per frame
		void Update ()
		{
				lastHop += Time.deltaTime;
				if (direction > 360) {
						direction %= 360;
				}
				transform.rotation = Quaternion.Euler (0, 0, direction);
				if (speed > 0) { //Slow down
						speed -= lastSpeed * Time.deltaTime;
				}
				if (speed < 0) { //Dont overshoot
						speed = 0;
				}
				transform.position = new Vector3 (transform.position.x + Mathf.Cos (Mathf.Deg2Rad * direction) * speed * Time.deltaTime, transform.position.y + Mathf.Sin (Mathf.Deg2Rad * direction) * speed * Time.deltaTime);
				if (transform.position.x > roomSize) { //Wrap
						transform.position = new Vector3 (-roomSize, transform.position.y);
				}
				if (transform.position.x < -roomSize) {
						transform.position = new Vector3 (roomSize, transform.position.y);
				}
				if (transform.position.y > roomSize) {
						transform.position = new Vector3 (transform.position.x, -roomSize);
				}
				if (transform.position.y < -roomSize) {
						transform.position = new Vector3 (transform.position.x, roomSize);
				}

		}

		public void setParent (GameControl parent)
		{
				this.parent = parent;
		}

		public void resetPoints ()
		{
				points = 0;
		}

		public void subtractPoints (int subtraction)
		{
				points -= subtraction;
		}

		public int getPoints ()
		{
				return points;
		}

		public void rotation (float x, float y)
		{
				if (speed == 0) {
//			Debug.Log(x + " , " + y + " : " + Mathf.Rad2Deg * (Mathf.Atan(y/x)));
						direction = Mathf.Rad2Deg * (Mathf.Atan2 (y, x));

				}
		}

		public void jump (float x, float y, float power)
		{
				if (speed == 0) {
						rotation (x, y);
						hop (10);
				}

		}

		public void hop (float speed)
		{
				if (this.speed == 0) {
						lastHop = 0;
						this.speed = speed * 10;
						lastSpeed = this.speed;
				}
		}

		public float getLastHop ()
		{
				return lastHop;
		}

		void OnTriggerEnter2D (Collider2D co)
		{
				//Debug.Log (co.gameObject.name);
				if (co.gameObject.name == "Pellet(Clone)") {
						points++;
						parent.respawnPellet (co.gameObject.transform);
				}
		}

		public Vector2 getLookAtVector ()
		{
				return new Vector2 (Mathf.Cos (Mathf.Deg2Rad * direction), Mathf.Sin (Mathf.Deg2Rad * direction));
		}

		public Vector2 getNearestPelletVector ()
		{
				return parent.getNearestPelletVector (this);
		}

		public void mutate (float mutationChance)
		{
				network.mutate (mutationChance);
		}

		public static void Breed (FrogMove baby, FrogMove lfr, FrogMove rfr)
		{
				NeuralNetwork.Breed (baby.network, lfr.network, rfr.network);
		}

		public void SetNetwork (NeuralNetwork network)
		{
				this.network = network;
		}
}

