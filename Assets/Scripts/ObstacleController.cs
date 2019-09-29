using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk
{
	public GameObject[] obstacles;
}

public class ObstacleController : MonoBehaviour
{
	public GameObject prefabOstacle;
	public Transform parent;
	public bool ignoreObstacles = false;

	public float tesGrav = 10f;

	private int CHUNK_WIDTH = 20;
	private int CHUNK_HEIGHT = 20;
	private int CHUNKS_SPAWN_X = 5;
	private int CHUNKS_SPAWN_Y = 5;
	private Dictionary<string, Chunk> obstaclesMap;

	void Start()
	{
		obstaclesMap = new Dictionary<string, Chunk>();
	}

	public void UpdateObstacle(Vector3 center)
	{
		float posStartX = center.x - CHUNKS_SPAWN_X * CHUNK_WIDTH;
		float posStartY = center.y - CHUNKS_SPAWN_Y * CHUNK_HEIGHT;
		float spawnWidth = CHUNKS_SPAWN_X * 2 * CHUNK_WIDTH;
		float spawnHeight = CHUNKS_SPAWN_Y * 2 * CHUNK_HEIGHT;

		for (float posX = posStartX; posX < posStartX + spawnWidth; posX += CHUNK_WIDTH)
		{
			for (float posY = posStartY; posY < posStartY + spawnHeight; posY += CHUNK_HEIGHT)
			{
				int col = Mathf.RoundToInt(posX / CHUNK_WIDTH);
				int row = Mathf.RoundToInt(posY / CHUNK_HEIGHT);
				string key = col + "_" + row;

				if (!obstaclesMap.ContainsKey(key))
				{
					obstaclesMap.Add(key, AddChunk(col, row));
				}
			}
		}
	}

	public GravityBody[] GetObstacles()
	{
		GravityBody[] obstacles = FindObjectsOfType<GravityBody>();
		return obstacles;
	}

	protected Chunk AddChunk(int col, int row)
	{
		Chunk chunk = new Chunk();

		float posOriginX = col * CHUNK_WIDTH;
		float posOriginY = row * CHUNK_HEIGHT;

		if (!ignoreObstacles)
		{
			GameObject body = Instantiate(prefabOstacle, parent);
			body.transform.parent = parent;
			body.transform.position = new Vector3(posOriginX, posOriginY, 0);
			body.transform.localScale = new Vector3(1, 1, 1);
			body.GetComponent<GravityBody>().mass = tesGrav;

			chunk.obstacles = new GameObject[1];
			chunk.obstacles[0] = body;
		} else
			chunk.obstacles = new GameObject[0];

		return chunk;
	}
}
