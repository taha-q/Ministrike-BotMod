using UnityEngine;

public class UiStatItem : MonoBehaviour
{
	public UISprite flag;

	public UILabel nick;

	public UILabel status;

	public UILabel score;

	public UILabel deaths;

	public UILabel ping;

	public void UpdateStatus(Player player)
	{
		flag.spriteName = player.data.country;
		nick.text = player.name;
		status.text = ((!player.IsAlive()) ? "DEAD" : string.Empty);
		score.text = string.Empty + player.GetKills();
		deaths.text = string.Empty + player.GetDeaths();
		ping.text = string.Empty + player.GetPing();
		nick.color = ((!player.isMine) ? Color.white : ((player.GetLife() <= 0) ? Color.red : Color.green));
		status.color = ((!player.isMine) ? Color.white : ((player.GetLife() <= 0) ? Color.red : Color.green));
		score.color = ((!player.isMine) ? Color.white : ((player.GetLife() <= 0) ? Color.red : Color.green));
		deaths.color = ((!player.isMine) ? Color.white : ((player.GetLife() <= 0) ? Color.red : Color.green));
		ping.color = ((!player.isMine) ? Color.white : ((player.GetLife() <= 0) ? Color.red : Color.green));
	}
}
