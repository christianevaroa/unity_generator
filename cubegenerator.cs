/*cubegenerator.cs
 * 
 * Script for generating dungeon floor layouts.
 * 
 * Christian Evaroa
 */

/* TODO Use corner tiles where walls meet at right angles
 * TODO Use wall variations to make tiling a bit less obvious
 * TODO Place torches nicely
 * Place rocks - DONE
 * TODO Place chests
 * TODO Place traps
 */
using UnityEngine;
using System.Collections;

public class cubegenerator : MonoBehaviour
{
	
		public int WIDTH = 60;
		public int HEIGHT = 40;
		public int MIN_WID = 8;
		public int MAX_WID = 12;
		public float PERCENT_ROCK_CHANCE = 0.02f;
		public readonly float CUBESCALE = 2.5f;
		public readonly int CUBEHEIGHT = 8;
		bool[,] bools;
		Rect[] rooms;
		public GameObject player;
		public GameObject torch;

		// Public fields for wall objects
	
		// Single faces
		public GameObject Floor_Tile;
		public GameObject Wall_North_A;
		public GameObject Wall_North_B;
		public GameObject Wall_North_C;
		public GameObject Wall_North_D;
		public GameObject Wall_East_A;
		public GameObject Wall_East_B;
		public GameObject Wall_East_C;
		public GameObject Wall_East_D;
		public GameObject Wall_South_A;
		public GameObject Wall_South_B;
		public GameObject Wall_South_C;
		public GameObject Wall_South_D;
		public GameObject Wall_West_A;
		public GameObject Wall_West_B;
		public GameObject Wall_West_C;
		public GameObject Wall_West_D;
	
		// Two faces
		public GameObject Wall_NorthEast;
		public GameObject Wall_NorthWest;
		public GameObject Wall_SouthEast;
		public GameObject Wall_SouthWest;
		public GameObject Wall_NorthSouth;
		public GameObject Wall_EastWest;
	
		// Three Faces
		public GameObject Wall_NorthEastWest;
		public GameObject Wall_EastNorthSouth;
		public GameObject Wall_SouthEastWest;
		public GameObject Wall_WestNorthSouth;
	
		// Four Faces
		public GameObject Wall_NESW;
		public GameObject Cube;

		// Corner blocks
		public GameObject Corner_NorthEast;
		public GameObject Corner_SouthEast;
		public GameObject Corner_SouthWest;
		public GameObject Corner_NorthWest;

		// Primes for faces
		private int NORTH = 2;
		private int EAST = 31;
		private int SOUTH = 73;
		private int WEST = 127;

		// Floor texture duh
		public Material Floor_Texture;

		// Rocks duh
		public GameObject Rock_A;
		public GameObject Rock_B;
		public GameObject Rock_C;
		public GameObject Rock_D;
		public GameObject Rock_E;
	
		// Use this for initialization
		void Start ()
		{
				// Number of rooms we will generate
				int numRooms = Random.Range (8, 12);
				//int numRooms = 3;
				rooms = new Rect[numRooms];
		
				// Create bool[W,H] and initialise to true
				bools = new bool[WIDTH, HEIGHT];
				for (int col = 0; col < WIDTH; col++) {
						for (int row = 0; row < HEIGHT; row++) {
								bools [col, row] = true;
						}
				}
		
				// Carve rooms
				for (int i = 0; i < numRooms; i++) {
						Rect room = makeRect ();
			
						// Check if new room intersects another and remake if so
						for (int j = 0; j < i; j++) {
								if (Intersect (room, rooms [j])) {
										room = makeRect ();
										j = 0;
								}
						}
						rooms [i] = room;
			
			
						for (int col = (int)room.xMin; col < room.xMin+room.width; col++) {
								for (int row = (int)room.yMin; row < room.yMin+room.height; row++) {
										bools [col, row] = false;
								}
						}
			
						// Make a light source
						GameObject torchy = (GameObject)GameObject.Instantiate (torch);
						torchy.transform.position = new Vector3 ((room.xMin * CUBESCALE) + (room.width * CUBESCALE / 2), 1, (room.yMin * CUBESCALE) + (room.height * CUBESCALE / 2));
				}
		
				// Carve hallways
				for (int i = 0; i < numRooms-1; i++) {
						Rect first = rooms [i];
						Rect second = rooms [i + 1];
			
						int firstX = (int)Random.Range ((int)first.xMin, (int)first.xMin + first.width);
						int firstY = (int)Random.Range ((int)first.yMin, (int)first.yMin + first.height);
						int secondX = (int)Random.Range ((int)second.xMin, (int)second.xMin + second.width);
						int secondY = (int)Random.Range ((int)second.yMin, (int)second.yMin + second.height);
			
						Vector2 startPoint = new Vector2 (firstX, firstY);
						Vector2 endPoint = new Vector2 (secondX, secondY);
			
						carveHallway (startPoint, endPoint);
				}

				// Create Walls
				createWalls ();
		
				// Create floor
//				GameObject floor = GameObject.CreatePrimitive (PrimitiveType.Cube);
//				floor.transform.position = new Vector3 (WIDTH * CUBESCALE / 2, -1, HEIGHT * CUBESCALE / 2);
//				floor.transform.localScale = new Vector3 (WIDTH * CUBESCALE, 1, HEIGHT * CUBESCALE);
//				floor.renderer.material = Floor_Texture;

				player.transform.position = new Vector3 (rooms [0].xMin * CUBESCALE, 1, rooms [0].yMin * CUBESCALE);
		}
	
		Rect makeRect ()
		{
				int rmWidth = Random.Range (MIN_WID, MAX_WID);
				int rmHeight = Random.Range (MIN_WID, MAX_WID);
				int left = Random.Range (1, WIDTH - rmWidth - 1);
				int top = Random.Range (1, HEIGHT - rmHeight - 1);
				return new Rect ((float)left, (float)top, (float)rmWidth, (float)rmHeight);
		}


		/* Carve a big dumb hallway between two points
		 * TODO Change this to make more interesting hallways instead of just a big right angle
		 */
		void carveHallway (Vector2 start, Vector2 end)
		{
				Vector2 midpoint = new Vector2 (start.x, end.y);
				for (int i = (int)start.y; i != end.y;) {
						bools [(int)start.x, i] = false;
						if (i < end.y)
								i++;
						else
								i--;
				}
				for (int i = (int)end.x; i != start.x;) {
						bools [i, (int)end.y] = false;
						if (i < start.x)
								i++;
						else
								i--;
				}
				bools [(int)midpoint.x, (int)midpoint.y] = false;
		}

		/* Check if rects intersect
		 */
		public static bool Intersect (Rect a, Rect b)
		{
				bool c1 = a.x < b.xMax;
				bool c2 = a.xMax > b.x;
				bool c3 = a.y < b.yMax;
				bool c4 = a.yMax > b.y;
				return c1 && c2 && c3 && c4;
		}

		/* Put the walls, floors and rocks in
		 */
		private void createWalls ()
		{
				for (int col = 0; col < WIDTH; col++) {
						for (int row = 0; row < HEIGHT; row++) {
								if (bools [col, row]) {
										GameObject currentWall = chooseWall (col, row);
										GameObject cw = (GameObject)GameObject.Instantiate (currentWall);
										cw.transform.position = new Vector3 (col * CUBESCALE, 0, row * CUBESCALE);
								} else {
										GameObject floor = (GameObject)GameObject.Instantiate (Floor_Tile);
										Vector3 pos = new Vector3 (col * CUBESCALE, 0, row * CUBESCALE);
										floor.transform.position = pos;
										if (Random.Range (0.0f, 1.0f) < PERCENT_ROCK_CHANCE) {
												int m = 0;
												if (bools [col - 1, row])
														m++;
												if (bools [col + 1, row])
														m++;
												if (bools [col, row - 1])
														m++;
												if (bools [col, row + 1])
														m++;
												if (m < 2)
														placeRock (pos);
										}
								}
						}
				}
		}

		/* Choose which wall should go in this cell based on the cells around it.
		 * TODO This needs to check diagonal neighbours so it can fill in the gaps that appear when walls meet at right angles
		 */
		private GameObject chooseWall (int col, int row)
		{
				// Select current wall model based on surrounding walls
				
				int seed = getPrime (col, row);

				float m = Random.Range (0.0f, 1.0f);

				switch (seed) {
				case 2:
						if (m < 0.25f)
								return Wall_North_A;
						if (m < 0.5f)
								return Wall_North_B;
						if (m < 0.75f)
								return Wall_North_C;
						return Wall_North_D;
				case 33:
						return Wall_NorthEast;
				case 75:
						return Wall_NorthSouth;
				case 129:
						return Wall_NorthWest;
				case 160:
						return Wall_NorthEastWest;
				case 202:
						return Wall_EastNorthSouth;
				case 106:
						return Wall_WestNorthSouth;
				case 233:
						return Wall_NESW;
				case 31:
						if (m < 0.25f)
								return Wall_East_A;
						if (m < 0.5f)
								return Wall_East_B;
						if (m < 0.75f)
								return Wall_East_C;
						return Wall_East_D;
				case 104:
						return Wall_SouthEast;
				case 231:
						return Wall_SouthEastWest;
				case 158:
						return Wall_EastWest;
				case 73:
						if (m < 0.25f)
								return Wall_South_A;
						if (m < 0.5f)
								return Wall_South_B;
						if (m < 0.75f)
								return Wall_South_C;
						return Wall_South_D;
				case 200:
						return Wall_SouthWest;
				case 127:
						if (m < 0.25f)
								return Wall_West_A;
						if (m < 0.5f)
								return Wall_West_B;
						if (m < 0.75f)
								return Wall_West_C;
						return Wall_West_D;
				default:
						return Cube;
				}

				return Wall_NESW; // Dead code
		}

		/* Return a prime number that describes which cells surrounding the given
	 	 * co-ordinates are empty
	 	 */
		private int getPrime (int col, int row)
		{
				int above = row - 1;
				int below = row + 1;
				int left = col - 1;
				int right = col + 1;

				int seed = 0;

				// Check cell above
				if (above >= 0 && !bools [col, above])
						seed += NORTH;
				// Check cell below
				if (below < HEIGHT && !bools [col, below])
						seed += SOUTH;
				// Check cell to left
				if (left >= 0 && !bools [left, row])
						seed += WEST;
				// Check cell to right
				if (right < WIDTH && !bools [right, row])
						seed += EAST;

				return seed;
		}

		/* PLACE A ROCK
		 */
		private void placeRock (Vector3 pos)
		{
				Vector3 rotationVector = new Vector3 (0, Random.Range (0.0f, 360.0f), 0);
				int m = Random.Range (0, 4);
				GameObject rock = (GameObject)GameObject.Instantiate (Rock_E);

				switch (m) {
				case 0:
						rock = (GameObject)GameObject.Instantiate (Rock_A);
						break;
				case 1:
						rock = (GameObject)GameObject.Instantiate (Rock_B);
						break;
				case 2:
						rock = (GameObject)GameObject.Instantiate (Rock_C);
						break;
				case 3:
						rock = (GameObject)GameObject.Instantiate (Rock_D);
						break;
				}

				rock.transform.position = pos;
				rock.transform.Rotate (0.0f, 0.0f, Random.Range (0.0f, 360.0f));

		}
}
