using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{

		// The target the camera is following (the player, probably)
		public GameObject target;
		// The distance along the X axis that the camera should stay from target
		public float distance_x = 5;
		// The distance along the Z axis that the camera should stay from target
		public float distance_z = 5;
		// The modifier for distance along Y axis (>1 = looking down at target, 1 = level, < 1 = looking up)
		public float heightMultiplier = 1.5f;
		// The current distance from target, used to smooth movement
		private float currentDistance = 5;
		private bool fov30 = false;
		private float targetFov = 60;

		// Use this for initialization
		void Start ()
		{
				Camera.main.fieldOfView = targetFov;
		}
	
		// Update is called once per frame
		void Update ()
		{
				// Set the camera's position and make it look at target
				transform.position = new Vector3 ((target.transform.position.x + currentDistance), (target.transform.position.y + (currentDistance * heightMultiplier)), (target.transform.position.z + distance_z));
				transform.LookAt (target.transform.position);

				// Use scroll wheel to increase or decrease desired distance
				if (Input.GetAxis ("Mouse ScrollWheel") < 0 && distance_x < 10) {
						distance_x++;
				} else if (Input.GetAxis ("Mouse ScrollWheel") > 0 && distance_x > 3) {
						distance_x--;
				}

				if (Input.GetKeyDown (KeyCode.Tab)) {
						fov30 = !fov30;
						if (!fov30) {
								targetFov = 60;
						} else {
								targetFov = 30;
						}
				}

				if (distance_x != currentDistance || Camera.main.fieldOfView != targetFov) {
						interpolate ();
				}

		}

		void interpolate ()
		{
				// Move camera smoothly towards desired distance
				// The camera slows down as it approaches the desired distance
				float diff = Mathf.Abs (distance_x - currentDistance);
				float amt = (Time.deltaTime * diff * diff);

				if (amt < 0.1f)
						amt = 0.1f;

				if (currentDistance > distance_x) {
						currentDistance -= amt;
				} else if (currentDistance < distance_x) {
						currentDistance += amt;
				}
		
				// Fixes jitters when camera never quite makes it to 0 diff
				if (diff <= 0.1f) {
						currentDistance = distance_x;
				}

				if (Camera.main.fieldOfView < targetFov) {
						Camera.main.fieldOfView += 1;
				} else if (Camera.main.fieldOfView > targetFov) {
						Camera.main.fieldOfView -= 1;
				}
		}
}