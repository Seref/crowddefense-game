using UnityEngine;

public class TankyEnemy : Enemy
{
	public SpriteRenderer sr;
	public SpriteRenderer sr1;
	public SpriteRenderer sr2;

	public override void SetHealth(float health)
	{
		sr.color = new Color(0.2f + 0.8f * ((10 - health) / 10), 0f + 1.0f * ((10 - health) / 10), 0.3f + 0.7f * ((10 - health) / 10), 1f);
		sr1.color = new Color(0.2f + 0.8f * ((10 - health) / 10), 0f + 1.0f * ((10 - health) / 10), 0.3f + 0.7f * ((10 - health) / 10), 1f);
		sr2.color = new Color(0.2f + 0.8f * ((10 - health) / 10), 0f + 1.0f * ((10 - health) / 10), 0.3f + 0.7f * ((10 - health) / 10), 1f);		
		Health = health;
	}


	void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.tag == "Bullet")
		{
			if (collision.gameObject.GetComponent<Bullet>().ShotBy == Bullet.BULLETUSER.PLAYER)
			{
				Health -= collision.gameObject.GetComponent<Bullet>().Damage;

				SetHealth(Health);

				if (Health <= 0.0f)
					Die();
			}
		}
	}




}
