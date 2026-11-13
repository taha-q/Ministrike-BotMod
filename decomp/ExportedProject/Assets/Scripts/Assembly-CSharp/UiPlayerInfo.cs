using AnimationOrTween;
using UnityEngine;

public class UiPlayerInfo : MonoBehaviour
{
	public UISprite flag;

	public UILabel nameLabel;

	public UISlider lifeSlider;

	public TweenAlpha tweener;

	private Player target;

	private float currentLife;

	private void Update()
	{
		if (target != null && currentLife != (float)target.GetLife())
		{
			currentLife = target.GetLife();
			float num = Mathf.Min(currentLife / 100f, 1f);
			lifeSlider.Set(num);
			lifeSlider.foregroundWidget.color = new Color(1f, num, num);
		}
	}

	public void Show(Player localPlayer, Player targetPlayer)
	{
		if (!(targetPlayer == null))
		{
			target = targetPlayer;
			flag.spriteName = target.data.country;
			nameLabel.text = ((localPlayer.GetTeam() != target.GetTeam()) ? "[FF0000]" : "[00FF00]");
			UILabel uILabel = nameLabel;
			uILabel.text = uILabel.text + target.name + "[-]";
			if (tweener.direction == Direction.Reverse)
			{
				tweener.Play(true);
			}
		}
	}

	public void Hide()
	{
		currentLife = 0f;
		target = null;
		if (tweener.direction == Direction.Forward)
		{
			tweener.Play(false);
		}
	}
}
