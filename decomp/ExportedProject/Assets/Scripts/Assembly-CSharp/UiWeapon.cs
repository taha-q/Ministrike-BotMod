using UnityEngine;

public class UiWeapon : MonoBehaviour
{
	public UISprite uiSpriteWeapon;

	public UILabel uiLabelBullets;

	private TweenAlpha tweener;

	public void Enable(bool e)
	{
		if (tweener == null)
		{
			tweener = GetComponent<TweenAlpha>();
		}
		tweener.Play(e);
	}
}
