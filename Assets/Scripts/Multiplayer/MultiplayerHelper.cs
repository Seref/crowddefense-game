using Assets.Scripts.Multiplayer;
using System;
using System.IO;
using System.IO.Compression;
using System.Text;
using UnityEngine;

public static class MultiplayerHelper
{
	public static DataPackage CreateGameState(DataGameStates dataGameStates)
	{
		var dataPackage = new DataPackage
		{
			type = DataType.DataGameState
		};
				
		var towerData = new DataGameState()
		{
			state = dataGameStates
		};

		dataPackage.data = JsonUtility.ToJson(towerData).ToString();
		return dataPackage;
	}

	public static DataPackage CreateAutoTowerData(GameObject gameObject)
	{
		var dataPackage = new DataPackage
		{
			type = DataType.DataAutoTower
		};

		var isActive = gameObject.activeSelf;
		if (isActive)
		{
			var autoTower = gameObject.GetComponent<AutoTower>();
			isActive = autoTower.Dropped;
		}

		var towerData = new DataAutoTower()
		{
			objectID = gameObject.GetInstanceID(),
			active = gameObject.activeSelf,
			position = gameObject.transform.position,
			rotation = gameObject.transform.rotation
		};

		dataPackage.data = JsonUtility.ToJson(towerData).ToString();
		return dataPackage;
	}

	public static DataPackage CreateTowerDummyData(GameObject gameObject)
	{
		var dataPackage = new DataPackage
		{
			type = DataType.DataTowerDummy
		};

		var towerData = new DataTowerDummy()
		{
			objectID = gameObject.GetInstanceID(),
			active = gameObject.activeSelf,
			position = gameObject.transform.position,
			rotation = gameObject.transform.rotation.eulerAngles.z
		};

		dataPackage.data = JsonUtility.ToJson(towerData).ToString();
		return dataPackage;
	}

	public static DataPackage CreateBulletData(GameObject gameObject)
	{
		var dataPackage = new DataPackage
		{
			type = DataType.DataBullet
		};

		var positionData = new DataBullet
		{
			objectID = gameObject.GetInstanceID(),
			active = gameObject.gameObject.activeSelf,
			position = gameObject.transform.position,
			rotation = gameObject.transform.rotation
		};

		dataPackage.data = JsonUtility.ToJson(positionData).ToString();

		return dataPackage;
	}

	public static DataPackage CreateEnemyData(GameObject gameObject)
	{
		var dataPackage = new DataPackage
		{
			type = DataType.DataEnemy
		};

		var positionData = new DataEnemy
		{
			objectID = gameObject.GetInstanceID(),
			active = gameObject.gameObject.activeSelf,
			position = gameObject.transform.position,
			rotation = gameObject.transform.rotation
		};

		dataPackage.data = JsonUtility.ToJson(positionData).ToString();

		return dataPackage;
	}

	public static string Decompress(string input)
	{
		byte[] compressed = Convert.FromBase64String(input);
		byte[] decompressed = Decompress(compressed);
		return Encoding.UTF8.GetString(decompressed);
	}

	public static string Compress(string input)
	{
		byte[] encoded = Encoding.UTF8.GetBytes(input);
		byte[] compressed = Compress(encoded);
		return Convert.ToBase64String(compressed);
	}

	public static byte[] Decompress(byte[] input)
	{
		using (var source = new MemoryStream(input))
		{
			byte[] lengthBytes = new byte[4];
			source.Read(lengthBytes, 0, 4);

			var length = BitConverter.ToInt32(lengthBytes, 0);
			using (var decompressionStream = new GZipStream(source,
				CompressionMode.Decompress))
			{
				var result = new byte[length];
				decompressionStream.Read(result, 0, length);
				return result;
			}
		}
	}

	public static byte[] Compress(byte[] input)
	{
		using (var result = new MemoryStream())
		{
			var lengthBytes = BitConverter.GetBytes(input.Length);
			result.Write(lengthBytes, 0, 4);

			using (var compressionStream = new GZipStream(result,
				CompressionMode.Compress))
			{
				compressionStream.Write(input, 0, input.Length);
				compressionStream.Flush();

			}
			return result.ToArray();
		}
	}
}
