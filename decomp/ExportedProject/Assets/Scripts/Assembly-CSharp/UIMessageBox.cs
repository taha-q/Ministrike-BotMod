using System;
using AnimationOrTween;
using UnityEngine;

[ExecuteInEditMode]
public class UIMessageBox : MonoBehaviour
{
	public enum UIMessageType
	{
		Info = 0,
		Choice = 1,
		Infiny = 2
	}

	public enum UIAction
	{
		Confirm = 0,
		Cancel = 1,
		Close = 2
	}

	public enum UITween
	{
		Alpha = 0,
		Position = 1,
		Scale = 2
	}

	public UIAtlas atlas;

	public string slicedSpriteBig;

	public string slicedSpriteSmall;

	public Font font;

	public int fontSize = 32;

	public Vector2 fontSpacing = Vector2.zero;

	public int fontSizeTitle = 32;

	public float margin = 0.1f;

	public string cancelText = "Cancel";

	public string confirmText = "Ok";

	public UITween tweenMode = UITween.Position;

	public float tweenDuration = 0.5f;

	private UISprite _backgroundSprite;

	private UIPanel _panel;

	private UILabel _titleLabel;

	private UILabel _textLabel;

	private UISprite _cancelSprite;

	private UILabel _cancelLabel;

	private UISprite _confirmSprite;

	private UILabel _confirmLabel;

	private UITweener _tweener;

	private Action<UIAction> _action;

	private void Awake()
	{
		Init();
		UpdateRes();
		Reset();
	}

	private void Init()
	{
		if (_panel == null)
		{
			_panel = GetComponent<UIPanel>();
		}
		_panel.enabled = true;
		if (_backgroundSprite == null)
		{
			_backgroundSprite = base.transform.FindChild("UIMBackground").GetComponent<UISprite>();
		}
		if (_titleLabel == null)
		{
			_titleLabel = base.transform.FindChild("UIMTitle").GetComponent<UILabel>();
		}
		if (_textLabel == null)
		{
			_textLabel = base.transform.FindChild("UIMText").GetComponent<UILabel>();
		}
		if (_cancelSprite == null)
		{
			_cancelSprite = base.transform.FindChild("UIMCancel").GetComponent<UISprite>();
		}
		if (_cancelLabel == null)
		{
			_cancelLabel = _cancelSprite.transform.FindChild("UIMCancelLabel").GetComponent<UILabel>();
			_cancelSprite.GetComponent<UIButton>().onClick.Clear();
			_cancelSprite.GetComponent<UIButton>().onClick.Add(new EventDelegate(this, "onButtonCancel"));
		}
		if (_confirmSprite == null)
		{
			_confirmSprite = base.transform.FindChild("UIMConfirm").GetComponent<UISprite>();
			_confirmSprite.GetComponent<UIButton>().onClick.Clear();
			_confirmSprite.GetComponent<UIButton>().onClick.Add(new EventDelegate(this, "onButtonConfirm"));
		}
		if (_confirmLabel == null)
		{
			_confirmLabel = _confirmSprite.transform.FindChild("UIMConfirmLabel").GetComponent<UILabel>();
		}
	}

	public void SetTextLabel(string text)
	{
		_textLabel.text = text;
	}

	public void SetConfirmLabel(string text)
	{
		_confirmLabel.text = text;
	}

	public void SetCancelLabel(string text)
	{
		_cancelLabel.text = text;
	}

	public void Show(string title, string text)
	{
		Show(UIMessageType.Info, title, text, fontSize, NGUIText.Alignment.Center, null);
	}

	public void Show(UIMessageType type, string title, string text)
	{
		Show(type, title, text, fontSize, NGUIText.Alignment.Center, null);
	}

	public void Show(UIMessageType type, string title, string text, int fontSize)
	{
		Show(type, title, text, fontSize, NGUIText.Alignment.Center, null);
	}

	public void Show(UIMessageType type, string title, string text, int fontSize, NGUIText.Alignment alignement)
	{
		Show(type, title, text, fontSize, alignement, null);
	}

	public void Show(UIMessageType type, string title, string text, Action<UIAction> uiAction)
	{
		Show(type, title, text, fontSize, NGUIText.Alignment.Center, uiAction);
	}

	public void Show(UIMessageType type, string title, string text, int fontSize, Action<UIAction> uiAction)
	{
		Show(type, title, text, fontSize, NGUIText.Alignment.Center, uiAction);
	}

	public void Show(UIMessageType type, string title, string text, int fontSize, NGUIText.Alignment alignement, Action<UIAction> uiAction)
	{
		Reset();
		NGUITools.SetActive(_panel.gameObject, true);
		NGUITools.SetActive(_confirmSprite.gameObject, type != UIMessageType.Infiny);
		NGUITools.SetActive(_cancelSprite.gameObject, type == UIMessageType.Choice);
		switch (type)
		{
		case UIMessageType.Choice:
			_confirmSprite.leftAnchor.relative = 0.55f;
			_confirmSprite.rightAnchor.relative = 0.85f;
			break;
		case UIMessageType.Info:
			_confirmSprite.leftAnchor.relative = 0.45f;
			_confirmSprite.rightAnchor.relative = 0.55f;
			break;
		}
		_titleLabel.text = title;
		_textLabel.alignment = alignement;
		_textLabel.fontSize = fontSize;
		_textLabel.text = text;
		_action = uiAction;
		if (tweenMode == UITween.Alpha)
		{
			_tweener = _panel.gameObject.AddComponent<TweenAlpha>();
			((TweenAlpha)_tweener).from = 0f;
			((TweenAlpha)_tweener).to = 1f;
			_tweener.AddOnFinished(new EventDelegate(this, "onHide"));
			_tweener.method = UITweener.Method.Linear;
		}
		else if (tweenMode == UITween.Position)
		{
			_tweener = _backgroundSprite.gameObject.AddComponent<TweenPosition>();
			((TweenPosition)_tweener).from.y = _panel.GetViewSize().y * 1.6f;
			((TweenPosition)_tweener).to.y = 0f;
			_tweener.AddOnFinished(new EventDelegate(this, "onHide"));
			_tweener.method = UITweener.Method.BounceIn;
		}
		else if (tweenMode == UITween.Scale)
		{
			_tweener = _backgroundSprite.gameObject.AddComponent<TweenScale>();
			((TweenScale)_tweener).from = Vector3.zero;
			((TweenScale)_tweener).to = Vector3.one;
			_tweener.AddOnFinished(new EventDelegate(this, "onHide"));
			_tweener.method = UITweener.Method.Linear;
		}
		else
		{
			_tweener = _backgroundSprite.gameObject.AddComponent<TweenPosition>();
			((TweenPosition)_tweener).from.y = _panel.GetViewSize().y * 1.6f;
			((TweenPosition)_tweener).to.y = 0f;
			_tweener.AddOnFinished(new EventDelegate(this, "onHide"));
			_tweener.method = UITweener.Method.BounceIn;
		}
		_tweener.duration = tweenDuration;
		_tweener.Play(true);
	}

	public bool IsShown()
	{
		return _panel.gameObject.activeSelf;
	}

	public void Hide()
	{
		if (_tweener != null && _tweener.direction == Direction.Forward)
		{
			_tweener.method = UITweener.Method.Linear;
			_tweener.Play(false);
		}
	}

	private void UpdateRes()
	{
		UILabel titleLabel = _titleLabel;
		Font font = this.font;
		_confirmLabel.trueTypeFont = font;
		font = font;
		_cancelLabel.trueTypeFont = font;
		font = font;
		_textLabel.trueTypeFont = font;
		titleLabel.trueTypeFont = font;
		_titleLabel.fontSize = fontSizeTitle;
		_textLabel.spacingX = (int)fontSpacing.x;
		_textLabel.spacingY = (int)fontSpacing.y;
		UILabel textLabel = _textLabel;
		int num = fontSize;
		_confirmLabel.fontSize = num;
		num = num;
		_cancelLabel.fontSize = num;
		textLabel.fontSize = num;
		_backgroundSprite.atlas = atlas;
		_backgroundSprite.spriteName = slicedSpriteBig;
		_cancelSprite.atlas = atlas;
		_cancelSprite.spriteName = slicedSpriteSmall;
		_cancelSprite.GetComponent<UIButton>().normalSprite = slicedSpriteSmall;
		_confirmSprite.atlas = atlas;
		_confirmSprite.spriteName = slicedSpriteSmall;
		_confirmSprite.GetComponent<UIButton>().normalSprite = slicedSpriteSmall;
	}

	private void Reset()
	{
		_textLabel.alignment = NGUIText.Alignment.Center;
		_textLabel.fontSize = fontSize;
		if ((bool)_tweener)
		{
			UnityEngine.Object.DestroyImmediate(_tweener);
		}
		_action = null;
		_backgroundSprite.leftAnchor.relative = margin;
		_backgroundSprite.rightAnchor.relative = 1f - margin;
		_backgroundSprite.bottomAnchor.relative = margin;
		_backgroundSprite.topAnchor.relative = 1f - margin;
		_backgroundSprite.keepAspectRatio = ((!(_panel.GetViewSize().x > _panel.GetViewSize().y)) ? UIWidget.AspectRatioSource.BasedOnWidth : UIWidget.AspectRatioSource.BasedOnHeight);
		_backgroundSprite.aspectRatio = 1f;
		_backgroundSprite.ResetAnchors();
		_backgroundSprite.UpdateAnchors();
		if (Application.isPlaying)
		{
			NGUITools.SetActive(_panel.gameObject, false);
		}
	}

	private void onHide()
	{
		if (_tweener.direction == Direction.Reverse)
		{
			Reset();
		}
	}

	private void onButtonConfirm()
	{
		if (_action != null)
		{
			_action(UIAction.Confirm);
		}
		else
		{
			Hide();
		}
	}

	private void onButtonCancel()
	{
		if (_action != null)
		{
			_action(UIAction.Cancel);
		}
		else
		{
			Hide();
		}
	}

	private void onButtonClose()
	{
		if (_action != null)
		{
			_action(UIAction.Close);
		}
		else
		{
			Hide();
		}
	}
}
