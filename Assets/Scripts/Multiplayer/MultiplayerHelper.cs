using Assets.Scripts.Multiplayer;
using UnityEngine;

public static class MultiplayerHelper
{
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
}
