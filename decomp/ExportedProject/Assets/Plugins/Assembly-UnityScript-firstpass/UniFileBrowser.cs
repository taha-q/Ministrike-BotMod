using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Boo.Lang;
using Boo.Lang.Runtime;
using CompilerGenerated;
using UnityEngine;
using UnityScript.Lang;

[Serializable]
public class UniFileBrowser : MonoBehaviour
{
	[Serializable]
	public enum SortType
	{
		Name = 0,
		DateNewest = 1,
		DateOldest = 2
	}

	[Serializable]
	public enum FileType
	{
		Open = 0,
		Save = 1,
		Folder = 2
	}

	[Serializable]
	public enum MessageWindowType
	{
		Error = 0,
		Confirm = 1
	}

	[Serializable]
	public class RefBool
	{
		public bool b;

		public RefBool(bool b)
		{
			this.b = b;
		}
	}

	[Serializable]
	public class RefInt
	{
		public int i;

		public RefInt(int i)
		{
			this.i = i;
		}
	}

	[Serializable]
	public class Popup
	{
		[NonSerialized]
		public static int popupListHash = "PopupList".GetHashCode();

		public static bool List(Rect position, RefBool showList, RefInt listEntry, GUIContent buttonContent, GUIContent[] listContent, GUIStyle listStyle)
		{
			return List(position, showList, listEntry, buttonContent, listContent, "button", "box", listStyle);
		}

		public static bool List(Rect position, RefBool showList, RefInt listEntry, GUIContent buttonContent, GUIContent[] listContent, GUIStyle buttonStyle, GUIStyle boxStyle, GUIStyle listStyle)
		{
			int controlID = GUIUtility.GetControlID(popupListHash, FocusType.Passive);
			bool flag = false;
			switch (Event.current.GetTypeForControl(controlID))
			{
			case EventType.MouseDown:
				if (position.Contains(Event.current.mousePosition))
				{
					showList.b = true;
				}
				break;
			case EventType.MouseUp:
				if (showList.b)
				{
					flag = true;
				}
				break;
			}
			GUI.Label(position, buttonContent, buttonStyle);
			if (showList.b)
			{
				Rect position2 = new Rect(position.x, position.y, position.width, listStyle.CalcHeight(listContent[0], 1f) * (float)listContent.Length);
				GUI.Box(position2, string.Empty, boxStyle);
				listEntry.i = GUI.SelectionGrid(position2, listEntry.i, listContent, 1, listStyle);
			}
			if (flag)
			{
				showList.b = false;
			}
			return flag;
		}
	}

	[Serializable]
	public class FileData
	{
		public string name;

		public DateTime date;

		public FileData(string name, DateTime date)
		{
			this.name = name;
			this.date = date;
		}
	}

	[Serializable]
	internal sealed class _0024WaitForFrame_002439 : GenericGenerator<object>
	{
		[Serializable]
		internal sealed class _0024 : GenericGeneratorEnumerator<object>, IEnumerator
		{
			internal UniFileBrowser _0024self__002440;

			public _0024(UniFileBrowser self_)
			{
				_0024self__002440 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					_0024self__002440.frameDone = false;
					_0024self__002440.selectedFileNumber = -1;
					result = (YieldDefault(2) ? 1 : 0);
					break;
				case 2:
					_0024self__002440.frameDone = true;
					_0024self__002440.selectedFileNumber = -1;
					YieldDefault(1);
					goto case 1;
				case 1:
					result = 0;
					break;
				}
				return (byte)result != 0;
			}
		}

		internal UniFileBrowser _0024self__002441;

		public _0024WaitForFrame_002439(UniFileBrowser self_)
		{
			_0024self__002441 = self_;
		}

		public override IEnumerator<object> GetEnumerator()
		{
			return new _0024(_0024self__002441);
		}
	}

	[Serializable]
	internal sealed class _0024DeleteFile_002442 : GenericGenerator<object>
	{
		[Serializable]
		internal sealed class _0024 : GenericGeneratorEnumerator<object>, IEnumerator
		{
			internal System.Collections.Generic.List<string> _0024files_002443;

			internal int _0024i_002444;

			internal string _0024thisFileName_002445;

			internal UniFileBrowser _0024self__002446;

			public _0024(UniFileBrowser self_)
			{
				_0024self__002446 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					if (!_0024self__002446.showMessageWindow && !_0024self__002446.selectFileInProgress && (_0024self__002446.selectedFileNumber < 0 || _0024self__002446.selectedFileNumber >= _0024self__002446.dirList.Count))
					{
						_0024self__002446.selectFileInProgress = true;
						if (_0024self__002446._allowMultiSelect)
						{
							_0024files_002443 = _0024self__002446.GetMultiFileNames();
							_0024i_002444 = 0;
							goto IL_02b5;
						}
						if (File.Exists(_0024self__002446.filePath + _0024self__002446.fileName))
						{
							_0024self__002446.ShowConfirmMessage("Warning", "Are you sure you want to delete \"" + _0024self__002446.fileName + "\"?", "Cancel", "Delete");
							goto case 2;
						}
						_0024self__002446.selectFileInProgress = false;
					}
					goto case 1;
				case 2:
					if (_0024self__002446.showMessageWindow)
					{
						result = (YieldDefault(2) ? 1 : 0);
						break;
					}
					if (!_0024self__002446.confirm)
					{
						_0024self__002446.selectFileInProgress = false;
						goto case 1;
					}
					try
					{
						File.Delete(_0024self__002446.filePath + _0024self__002446.fileName);
					}
					catch (Exception ex)
					{
						_0024self__002446.ShowError(ex.Message);
					}
					_0024self__002446.GetCurrentFileInfo();
					_0024self__002446.selectFileInProgress = false;
					goto IL_02e2;
				case 3:
					if (_0024self__002446.showMessageWindow)
					{
						result = (YieldDefault(3) ? 1 : 0);
						break;
					}
					if (_0024self__002446.confirm)
					{
						try
						{
							File.Delete(_0024files_002443[_0024i_002444]);
						}
						catch (Exception ex2)
						{
							_0024self__002446.ShowError(ex2.Message);
							_0024self__002446.GetCurrentFileInfo();
							_0024self__002446.selectFileInProgress = false;
							goto case 1;
						}
					}
					goto IL_02a7;
				case 1:
					{
						result = 0;
						break;
					}
					IL_02b5:
					if (_0024i_002444 < _0024files_002443.Count)
					{
						if (File.Exists(_0024files_002443[_0024i_002444]))
						{
							_0024thisFileName_002445 = Path.GetFileName(_0024files_002443[_0024i_002444]);
							_0024self__002446.ShowConfirmMessage("Warning", "Are you sure you want to delete \"" + _0024thisFileName_002445 + "\"?", "Cancel", "Delete");
							goto case 3;
						}
						goto IL_02a7;
					}
					_0024self__002446.GetCurrentFileInfo();
					_0024self__002446.selectFileInProgress = false;
					goto IL_02e2;
					IL_02a7:
					_0024i_002444++;
					goto IL_02b5;
					IL_02e2:
					YieldDefault(1);
					goto case 1;
				}
				return (byte)result != 0;
			}
		}

		internal UniFileBrowser _0024self__002447;

		public _0024DeleteFile_002442(UniFileBrowser self_)
		{
			_0024self__002447 = self_;
		}

		public override IEnumerator<object> GetEnumerator()
		{
			return new _0024(_0024self__002447);
		}
	}

	[Serializable]
	internal sealed class _0024SelectFile_002448 : GenericGenerator<object>
	{
		[Serializable]
		internal sealed class _0024 : GenericGeneratorEnumerator<object>, IEnumerator
		{
			internal string _0024thisFileName_002449;

			internal int _0024i_002450;

			internal UniFileBrowser _0024self__002451;

			public _0024(UniFileBrowser self_)
			{
				_0024self__002451 = self_;
			}

			public override bool MoveNext()
			{
				int result;
				switch (_state)
				{
				default:
					if (!_0024self__002451.showMessageWindow && !_0024self__002451.selectFileInProgress)
					{
						if (_0024self__002451.selectedFileNumber >= 0 && _0024self__002451.selectedFileNumber < _0024self__002451.dirList.Count)
						{
							_0024self__002451.filePath += _0024self__002451.dirList[_0024self__002451.selectedFileNumber].name + _0024self__002451.pathChar;
							if (_0024self__002451.showVolumes && _0024self__002451.selectedFileNumber < _0024self__002451.numberOfVolumes)
							{
								if (_0024self__002451.windowsSystem)
								{
									_0024self__002451.filePath = _0024self__002451.dirList[_0024self__002451.selectedFileNumber].name + _0024self__002451.pathChar + _0024self__002451.pathChar;
								}
								else if (_0024self__002451.linuxSystem)
								{
									_0024self__002451.filePath = _0024self__002451.dirList[_0024self__002451.selectedFileNumber].name + _0024self__002451.pathChar;
								}
								else
								{
									_0024self__002451.filePath = "/Volumes/" + _0024self__002451.dirList[_0024self__002451.selectedFileNumber].name + _0024self__002451.pathChar;
								}
							}
							_0024self__002451.GetCurrentFileInfo();
						}
						else if ((_0024self__002451.fileType == FileType.Save || ((_0024self__002451._allowMultiSelect || !(_0024self__002451.fileName == string.Empty)) && (!_0024self__002451._allowMultiSelect || _0024self__002451.multiFileList.Count >= 1))) && (_0024self__002451.fileType != FileType.Save || !_0024self__002451.autoAddExtension || !(_0024self__002451.fileName == _0024self__002451.addedExtension)))
						{
							_0024self__002451.selectFileInProgress = true;
							_0024thisFileName_002449 = _0024self__002451.fileName;
							if (_0024self__002451.fileType == FileType.Save)
							{
								_0024i_002450 = 0;
								goto IL_0392;
							}
							goto IL_03ad;
						}
					}
					goto case 1;
				case 2:
					if (_0024self__002451.showMessageWindow)
					{
						_0024self__002451.fileName = _0024thisFileName_002449;
						result = (YieldDefault(2) ? 1 : 0);
						break;
					}
					if (_0024self__002451.confirm)
					{
						goto IL_0384;
					}
					_0024self__002451.selectFileInProgress = false;
					goto case 1;
				case 1:
					{
						result = 0;
						break;
					}
					IL_0384:
					_0024i_002450++;
					goto IL_0392;
					IL_0392:
					if (_0024i_002450 < _0024self__002451.fileList.Count)
					{
						if (_0024self__002451.fileList[_0024i_002450].name == _0024self__002451.fileName)
						{
							_0024self__002451.ShowConfirmMessage("Warning", "A file with that name already exists. Are you sure you want to replace it?", "Cancel", "Replace");
							goto case 2;
						}
						goto IL_0384;
					}
					goto IL_03ad;
					IL_03ad:
					_0024self__002451.selectFileInProgress = false;
					if (_0024self__002451.fileType == FileType.Open || _0024self__002451.fileType == FileType.Save)
					{
						_0024self__002451.CloseFileWindow();
						if (_0024self__002451.fileType == FileType.Open && _0024self__002451._allowMultiSelect)
						{
							_0024self__002451.multiDelegate(_0024self__002451.GetMultiFileNames().ToArray());
						}
						else
						{
							_0024self__002451.@delegate(_0024self__002451.filePath + _0024thisFileName_002449);
						}
						if (_0024self__002451.fileType == FileType.Save)
						{
							_0024self__002451.GetCurrentFileInfo();
						}
					}
					YieldDefault(1);
					goto case 1;
				}
				return (byte)result != 0;
			}
		}

		internal UniFileBrowser _0024self__002452;

		public _0024SelectFile_002448(UniFileBrowser self_)
		{
			_0024self__002452 = self_;
		}

		public override IEnumerator<object> GetEnumerator()
		{
			return new _0024(_0024self__002452);
		}
	}

	public bool filterFiles;

	public string[] filterFileExtensions;

	public bool autoAddExtension;

	public string addedExtension;

	public bool useFilterButton;

	public bool useDeleteButton;

	public string defaultFileName;

	public int maxFileNameLength;

	public bool limitToInitialFolder;

	public bool showVolumes;

	public bool volumesAreSeparate;

	public bool showDate;

	public int dateWidthAdd;

	public SortType sortType;

	public bool m_allowMultiSelect;

	public bool allowAppBundleBrowsing;

	public bool showHiddenOSXFiles;

	public bool preventIllegalCharInput;

	public string illegalChars;

	public bool allowWindowResize;

	public bool allowWindowDrag;

	public int fileWindowID;

	public int messageWindowID;

	public float doubleClickTime;

	public int fileWindowInset;

	public Rect defaultFileWindowRect;

	public int minWindowWidth;

	public int minWindowHeight;

	public Vector2 messageWindowSize;

	public Rect popupRect;

	public Vector2 buttonSize;

	public Vector2 windowTabOffset;

	public int guiDepth;

	public GUISkin guiSkin;

	public Texture highlightTexture;

	public Color highlightTextColor;

	public Texture windowTab;

	public Texture messageWindowTexture;

	public Texture driveIcon;

	public Texture folderIcon;

	public Texture fileIcon;

	private bool _allowMultiSelect;

	private Vector2 scrollPos;

	private int selectedFileNumber;

	private int oldSelectedFileNumber;

	private int anchorFileNumber;

	private System.Collections.Generic.List<FileData> dirList;

	private System.Collections.Generic.List<FileData> fileList;

	private Texture[] iconDisplayList;

	private string[] fileDisplayList;

	private string[] dateDisplayList;

	private System.Collections.Generic.List<int> multiFileList;

	private GUIStyle scrollViewStyle;

	private GUIStyle scrollViewStyleSelected;

	private GUIStyle popupListStyle;

	private GUIStyle popupButtonStyle;

	private GUIStyle popupBoxStyle;

	private GUIStyle messageWindowStyle;

	private string filePath;

	private string fileName;

	private string oldFileName;

	private bool frameDone;

	private GUIContent[] pathList;

	private RefBool showPopup;

	private RefInt selectedPath;

	private char pathChar;

	private bool windowsSystem;

	private bool linuxSystem;

	private bool osxSystem;

	private bool androidSystem;

	private int numberOfVolumes;

	private bool fileWindowOpen;

	private Rect fileWindowRect;

	private string windowTitle;

	private string[] fileWindowTitles;

	private string[] selectButtonText;

	private FileType fileType;

	private bool handleClicked;

	private Vector3 clickedPosition;

	private bool doubleClicked;

	private Rect originalWindowRect;

	private int cmdKey1;

	private int cmdKey2;

	private Vector3 mousePos;

	private int linePixelHeight;

	private int iconWidth;

	private int dateWidth;

	private bool selectFileInProgress;

	private bool showFiles;

	private __UniFileBrowser_delegate_0024callable0_0024117_24__ @delegate;

	private __UniFileBrowser_multiDelegate_0024callable1_0024118_29__ multiDelegate;

	private bool sendCloseMessage;

	private __UniFileBrowser_closeWindowFunction_0024callable2_0024120_35__ closeWindowFunction;

	private bool sendChangeMessage;

	private __UniFileBrowser_closeWindowFunction_0024callable2_0024120_35__ changeWindowFunction;

	private Rect lastRect;

	[NonSerialized]
	public static UniFileBrowser use;

	private float clickTimer;

	private bool clicked;

	private int keyboardControlID;

	private int windowOpenedCounter;

	private GUISkin defaultSkin;

	private Rect textfieldRect;

	private Rect fileBoxRect;

	private Rect fileAreaRect;

	private Rect volumeBoxRect;

	private Rect saveFileAreaRect;

	private Rect viewRect;

	private int bottomAreaSpace;

	private int xIndent;

	private int yIndent;

	private int scrollbarWidth;

	private int buttonPositionX;

	private int buttonPositionY;

	private bool multi;

	private int windowOffset;

	private int storedPopupHeight;

	private bool runCustomFunction;

	private __UniFileBrowser_closeWindowFunction_0024callable2_0024120_35__ customFunction;

	private bool doFolderFunction;

	private bool doFileFunction;

	private __UniFileBrowser_folderFunction_0024callable3_0024149_30__ folderFunction;

	private __UniFileBrowser_fileFunction_0024callable4_0024150_28__ fileFunction;

	private Vector2 touchPos;

	private bool touchScrolling;

	private MessageWindowType messageWindowType;

	private string button1Text;

	private string button2Text;

	private string message;

	private bool showMessageWindow;

	private string messageWindowTitle;

	private Rect messageWindowRect;

	private bool confirm;

	private bool messageWindowDelay;

	public override bool allowMultiSelect
	{
		get
		{
			return _allowMultiSelect;
		}
		set
		{
			if (!fileWindowOpen)
			{
				_allowMultiSelect = value;
			}
		}
	}

	public UniFileBrowser()
	{
		maxFileNameLength = 32;
		dateWidthAdd = 8;
		sortType = SortType.Name;
		preventIllegalCharInput = true;
		illegalChars = "\\/|:<>?\"+[]";
		allowWindowResize = true;
		allowWindowDrag = true;
		fileWindowID = 50;
		messageWindowID = 51;
		doubleClickTime = 0.5f;
		fileWindowInset = 25;
		defaultFileWindowRect = new Rect(400f, 150f, 500f, 600f);
		minWindowWidth = 435;
		minWindowHeight = 300;
		messageWindowSize = new Vector2(400f, 160f);
		popupRect = new Rect(0f, 0f, 300f, 33f);
		buttonSize = new Vector2(72f, 35f);
		windowTabOffset = new Vector2(0f, 3f);
		guiDepth = -1;
		highlightTextColor = Color.black;
		selectedFileNumber = -1;
		oldSelectedFileNumber = -1;
		fileName = string.Empty;
		oldFileName = string.Empty;
		frameDone = true;
		pathChar = "/"[0];
		fileWindowTitles = new string[3] { "Open file", "Save file", "Select folder" };
		selectButtonText = new string[3] { "Open", "Save", "Open" };
		fileType = FileType.Open;
		showFiles = true;
		confirm = true;
	}

	public virtual void Awake()
	{
		enabled = false;
		if (!guiSkin)
		{
			Debug.LogError("UniFileBrowser GUI skin missing");
			return;
		}
		if (m_allowMultiSelect)
		{
			multiFileList = new System.Collections.Generic.List<int>();
			_allowMultiSelect = m_allowMultiSelect;
		}
		if (use != null)
		{
			UnityEngine.Object.Destroy(this);
			return;
		}
		use = this;
		UnityEngine.Object.DontDestroyOnLoad(this);
		showPopup = new RefBool(false);
		selectedPath = new RefInt(-1);
		SetDefaultPath();
		fileWindowRect = defaultFileWindowRect;
		fileWindowRect.width = Screen.width;
		fileWindowRect.height = Screen.height;
		try
		{
			scrollViewStyle = guiSkin.GetStyle("listScrollview");
			scrollViewStyle.clipping = TextClipping.Clip;
		}
		catch (Exception)
		{
			Debug.LogError("The GUISkin for UniFileBrowser must contain a style called \"listScrollview\"");
			return;
		}
		scrollViewStyleSelected = new GUIStyle(scrollViewStyle);
		scrollViewStyleSelected.normal.background = highlightTexture as Texture2D;
		scrollViewStyleSelected.normal.textColor = highlightTextColor;
		try
		{
			popupListStyle = guiSkin.GetStyle("popupList");
		}
		catch (Exception)
		{
			Debug.LogError("The GUISkin for UniFileBrowser must contain a style called \"popupList\"");
			return;
		}
		try
		{
			popupButtonStyle = guiSkin.GetStyle("popupButton");
		}
		catch (Exception)
		{
			Debug.LogError("The GUISkin for UniFileBrowser must contain a style called \"popupButton\"");
			return;
		}
		try
		{
			popupBoxStyle = guiSkin.GetStyle("popupBox");
		}
		catch (Exception)
		{
			Debug.LogError("The GUISkin for UniFileBrowser must contain a style called \"popupBox\"");
			return;
		}
		messageWindowStyle = new GUIStyle(guiSkin.window);
		messageWindowStyle.normal.background = messageWindowTexture as Texture2D;
		messageWindowStyle.onHover.background = messageWindowTexture as Texture2D;
		SetupExtensions();
		if (autoAddExtension && !addedExtension.StartsWith("."))
		{
			addedExtension = "." + addedExtension;
		}
		maxFileNameLength = Mathf.Clamp(maxFileNameLength, 3, 128);
		windowOffset = guiSkin.GetStyle("window").padding.top;
		popupRect.x += (float)fileWindowInset;
		popupRect.y += (float)(fileWindowInset + windowOffset);
		storedPopupHeight = (int)popupRect.height;
		if (limitToInitialFolder)
		{
			popupRect.height = 0f;
		}
		xIndent = guiSkin.GetStyle("box").padding.left;
		yIndent = guiSkin.GetStyle("box").padding.top;
		linePixelHeight = (int)scrollViewStyle.CalcHeight(new GUIContent(folderIcon), 1f);
		iconWidth = (int)scrollViewStyle.CalcSize(new GUIContent(folderIcon)).x;
		dateWidth = (int)(scrollViewStyle.CalcSize(new GUIContent(DateTime.MinValue.ToString())).x + (float)dateWidthAdd);
		GUIStyle style = guiSkin.GetStyle("verticalscrollbar");
		scrollbarWidth = (int)(style.fixedWidth + (float)style.margin.left + (float)style.margin.right);
		dirList = new System.Collections.Generic.List<FileData>();
		fileList = new System.Collections.Generic.List<FileData>();
	}

	public virtual void SetFileExtensions(string[] extensions)
	{
		filterFileExtensions = extensions;
		if (extensions == null || extensions.Length == 0)
		{
			filterFiles = false;
		}
		else
		{
			filterFiles = true;
		}
		SetupExtensions();
	}

	private void SetupExtensions()
	{
		int i = 0;
		string[] array = filterFileExtensions;
		for (int length = array.Length; i < length; i++)
		{
			if (!array[i].StartsWith("."))
			{
				array[i] = "." + array[i];
			}
		}
	}

	public virtual void SetAutoAddedExtension(string extension)
	{
		if (!extension.StartsWith("."))
		{
			extension = "." + extension;
		}
		addedExtension = extension;
	}

	public virtual void RefreshFileList()
	{
		GetCurrentFileInfo();
		if (autoAddExtension)
		{
			fileName = Path.GetFileNameWithoutExtension(fileName) + addedExtension;
		}
	}

	public virtual void SetDefaultPath()
	{
		filePath = Application.dataPath;
		switch (Application.platform)
		{
		case RuntimePlatform.OSXEditor:
			filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar)) + pathChar;
			cmdKey1 = 310;
			cmdKey2 = 309;
			osxSystem = true;
			break;
		case RuntimePlatform.OSXPlayer:
			filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar));
			filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar)) + pathChar;
			cmdKey1 = 310;
			cmdKey2 = 309;
			osxSystem = true;
			break;
		case RuntimePlatform.WindowsPlayer:
		case RuntimePlatform.WindowsEditor:
			pathChar = "\\"[0];
			filePath = filePath.Replace("/", "\\");
			filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar)) + pathChar;
			cmdKey1 = 306;
			cmdKey2 = 305;
			windowsSystem = true;
			break;
		case RuntimePlatform.LinuxPlayer:
			filePath = filePath.Substring(0, filePath.LastIndexOf(pathChar)) + pathChar;
			cmdKey1 = 306;
			cmdKey2 = 305;
			linuxSystem = true;
			break;
		case RuntimePlatform.IPhonePlayer:
			filePath = Application.persistentDataPath + pathChar;
			break;
		case RuntimePlatform.Android:
			filePath = Application.persistentDataPath + pathChar;
			androidSystem = true;
			break;
		default:
			Debug.LogError("You are not using a supported platform");
			Application.Quit();
			break;
		}
	}

	public virtual bool FileWindowOpen()
	{
		return fileWindowOpen;
	}

	public virtual Rect GetFileWindowRect()
	{
		return fileWindowRect;
	}

	private void UpdateRects()
	{
		fileBoxRect = new Rect(fileWindowInset, textfieldRect.y + ((fileType != FileType.Save) ? 0f : (textfieldRect.height + 10f)), 0f, 0f);
		fileBoxRect.width = fileWindowRect.width - (float)(fileWindowInset * 2);
		if (showVolumes && volumesAreSeparate && !limitToInitialFolder)
		{
			volumeBoxRect = fileBoxRect;
			volumeBoxRect.height = numberOfVolumes * linePixelHeight + yIndent * 2;
			fileBoxRect.y += volumeBoxRect.height + 10f;
			fileBoxRect.height = fileWindowRect.height - fileBoxRect.y - (float)bottomAreaSpace;
			fileAreaRect = fileBoxRect;
			fileAreaRect.x += (float)xIndent;
			fileAreaRect.y += (float)yIndent;
			fileAreaRect.width -= (float)(xIndent * 2);
			fileAreaRect.height -= (float)(yIndent * 2);
			viewRect = new Rect(fileAreaRect.x, fileAreaRect.y, fileAreaRect.width - (float)(scrollbarWidth + xIndent), (fileDisplayList.Length - numberOfVolumes) * linePixelHeight);
		}
		else
		{
			fileBoxRect.height = fileWindowRect.height - fileBoxRect.y - (float)bottomAreaSpace;
			fileAreaRect = fileBoxRect;
			fileAreaRect.x += (float)xIndent;
			fileAreaRect.y += (float)yIndent;
			fileAreaRect.width -= (float)(xIndent * 2);
			fileAreaRect.height -= (float)(yIndent * 2);
			viewRect = new Rect(fileAreaRect.x, fileAreaRect.y, fileAreaRect.width - (float)(scrollbarWidth + xIndent), fileDisplayList.Length * linePixelHeight);
		}
	}

	public virtual void Update()
	{
		if (Input.touchCount > 0)
		{
			Touch touch = Input.GetTouch(0);
			Vector2 position = touch.position;
			position.y = (float)Screen.height - position.y;
			position -= new Vector2(fileWindowRect.x, fileWindowRect.y);
			if (touch.phase == TouchPhase.Began && fileAreaRect.Contains(position))
			{
				touchPos = position;
				touchScrolling = true;
				return;
			}
			if (touchScrolling && touch.phase == TouchPhase.Moved)
			{
				scrollPos -= position - touchPos;
				touchPos = position;
			}
			if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
			{
				touchScrolling = false;
			}
		}
		else if (touchScrolling)
		{
			touchScrolling = false;
		}
	}

	public virtual void OnGUI()
	{
		defaultSkin = GUI.skin;
		GUI.skin = guiSkin;
		GUI.depth = guiDepth;
		fileWindowRect = GUI.Window(fileWindowID, fileWindowRect, _0024adaptor_0024__UniFileBrowser_closeWindowFunction_0024callable2_0024120_35___0024WindowFunction_00240.Adapt(DrawFileWindow), windowTitle);
		if (!showMessageWindow)
		{
			GUI.FocusWindow(fileWindowID);
		}
		else if (!messageWindowDelay || !messageWindowRect.Contains(Event.current.mousePosition))
		{
			GUI.Window(messageWindowID, messageWindowRect, _0024adaptor_0024__UniFileBrowser_closeWindowFunction_0024callable2_0024120_35___0024WindowFunction_00240.Adapt(MessageWindow), messageWindowTitle, messageWindowStyle);
			GUI.BringWindowToFront(messageWindowID);
		}
		messageWindowDelay = false;
		if (allowWindowResize)
		{
			if (Event.current.type == EventType.MouseDown && new Rect(fileWindowRect.width - 25f, fileWindowRect.height - 25f, 25f, 25f).Contains(mousePos))
			{
				handleClicked = true;
				clickedPosition = mousePos;
				originalWindowRect = fileWindowRect;
			}
			if (handleClicked)
			{
				if (Event.current.type == EventType.MouseDrag)
				{
					fileWindowRect.width = Mathf.Clamp(originalWindowRect.width + (mousePos.x - clickedPosition.x), minWindowWidth, 1600f);
					fileWindowRect.height = Mathf.Clamp(originalWindowRect.height + (mousePos.y - clickedPosition.y), minWindowHeight, 1200f);
					UpdateRects();
				}
				else if (Event.current.type == EventType.MouseUp)
				{
					handleClicked = false;
				}
			}
		}
		if (sendChangeMessage && Event.current.type == EventType.Repaint)
		{
			if (lastRect != fileWindowRect)
			{
				changeWindowFunction();
			}
			lastRect = fileWindowRect;
		}
		GUI.skin = defaultSkin;
	}

	private void DrawFileWindow()
	{
		if (allowWindowDrag)
		{
			GUI.DragWindow(new Rect(0f, 0f, 10000f, windowOffset));
		}
		mousePos = Event.current.mousePosition;
		if (showMessageWindow)
		{
			GUI.enabled = false;
		}
		if (fileType == FileType.Folder && GUI.Button(new Rect(fileWindowRect.width - buttonSize.x - (float)fileWindowInset, popupRect.y, buttonSize.x, popupRect.height), "Select"))
		{
			if (selectedFileNumber > -1 && selectedFileNumber < dirList.Count)
			{
				@delegate(filePath + fileDisplayList[selectedFileNumber] + pathChar);
			}
			else
			{
				@delegate(filePath);
			}
			CloseFileWindow();
			return;
		}
		if (fileType == FileType.Save)
		{
			if (preventIllegalCharInput)
			{
				for (int i = 0; i < illegalChars.Length; i++)
				{
					if (Event.current.character == illegalChars[i])
					{
						Event.current.character = "\0"[0];
						break;
					}
				}
			}
			if (fileName.Length == maxFileNameLength + addedExtension.Length)
			{
				Event.current.character = "\0"[0];
			}
			textfieldRect.width = fileWindowRect.width - 90f - (float)(fileWindowInset * 2);
			GUI.SetNextControlName("TextEntry");
			fileName = GUI.TextField(textfieldRect, fileName, 60);
			if (autoAddExtension)
			{
				if (!fileName.EndsWith(addedExtension))
				{
					fileName = oldFileName;
				}
				else
				{
					oldFileName = fileName;
				}
			}
			if (windowOpenedCounter > 0)
			{
				if (--windowOpenedCounter == 1)
				{
					GUI.FocusControl("TextEntry");
				}
				if (windowOpenedCounter == 0)
				{
					keyboardControlID = GUIUtility.keyboardControl;
					fileName = defaultFileName;
					if (autoAddExtension)
					{
						fileName += addedExtension;
					}
				}
			}
		}
		if (fileType == FileType.Save)
		{
			GUI.Label(new Rect(fileWindowInset, textfieldRect.y, 90f, textfieldRect.height), "Save as:");
		}
		GUI.SetNextControlName("Area");
		GUI.Box(fileBoxRect, string.Empty);
		bool flag = false;
		Rect position;
		if (showVolumes && volumesAreSeparate && !limitToInitialFolder)
		{
			GUI.Box(volumeBoxRect, string.Empty);
			position = new Rect(fileAreaRect.x, volumeBoxRect.y + (float)yIndent, iconWidth, linePixelHeight);
		}
		else
		{
			scrollPos = GUI.BeginScrollView(fileAreaRect, scrollPos, viewRect);
			position = new Rect(fileAreaRect.x, fileAreaRect.y, iconWidth, linePixelHeight);
			flag = true;
		}
		bool flag2 = default(bool);
		bool shift = default(bool);
		if (multi)
		{
			bool num = osxSystem;
			if (num)
			{
				num = Event.current.command;
			}
			if (!num)
			{
				num = !osxSystem;
				if (num)
				{
					num = Event.current.control;
				}
			}
			flag2 = num;
			shift = Event.current.shift;
		}
		int num2 = 0;
		if (showDate && !(fileAreaRect.height >= (float)((fileDisplayList.Length - ((showVolumes && volumesAreSeparate) ? numberOfVolumes : 0)) * linePixelHeight)))
		{
			num2 = scrollbarWidth;
		}
		for (int i = 0; i < fileDisplayList.Length; i++)
		{
			if (showVolumes && volumesAreSeparate && !limitToInitialFolder && i == numberOfVolumes)
			{
				scrollPos = GUI.BeginScrollView(fileAreaRect, scrollPos, viewRect);
				position.y = fileAreaRect.y;
				flag = true;
			}
			bool num3 = multi;
			if (num3)
			{
				num3 = multiFileList.Contains(i);
			}
			if (!num3)
			{
				num3 = !multi;
				if (num3)
				{
					num3 = i == selectedFileNumber;
				}
			}
			bool flag3 = num3;
			position.x = fileAreaRect.x;
			GUI.Label(position, iconDisplayList[i], (!flag3) ? scrollViewStyle : scrollViewStyleSelected);
			position.x += iconWidth;
			position.width = fileAreaRect.width - (float)iconWidth - (float)(showDate ? dateWidth : 0) - (float)num2;
			if (GUI.Button(position, fileDisplayList[i], (!flag3) ? scrollViewStyle : scrollViewStyleSelected) && frameDone)
			{
				GUI.FocusControl("Area");
				selectedFileNumber = i;
				if (multi)
				{
					if (flag2)
					{
						if (multiFileList.Contains(i))
						{
							multiFileList.Remove(i);
						}
						else
						{
							multiFileList.Add(i);
						}
					}
					else if (!multiFileList.Contains(i))
					{
						multiFileList.Add(i);
					}
					if (!shift)
					{
						anchorFileNumber = i;
					}
					fileName = ((multiFileList.Count != 1 || i < dirList.Count || i - dirList.Count >= fileList.Count) ? string.Empty : fileDisplayList[multiFileList[0]]);
				}
			}
			if (showDate)
			{
				position.x += position.width;
				position.width = dateWidth;
				GUI.Label(position, dateDisplayList[i], (!flag3) ? scrollViewStyle : scrollViewStyleSelected);
			}
			position.y += linePixelHeight;
		}
		if (flag)
		{
			GUI.EndScrollView();
		}
		if (GUIUtility.keyboardControl == keyboardControlID)
		{
			selectedFileNumber = -1;
		}
		if (selectedFileNumber != oldSelectedFileNumber && frameDone)
		{
			if (multi)
			{
				if (shift)
				{
					if (selectedFileNumber != anchorFileNumber)
					{
						int num4 = ((selectedFileNumber > anchorFileNumber) ? 1 : (-1));
						for (int i = anchorFileNumber; i != selectedFileNumber; i += num4)
						{
							if (!multiFileList.Contains(i))
							{
								multiFileList.Add(i);
							}
						}
					}
					if (selectedFileNumber <= anchorFileNumber && oldSelectedFileNumber > anchorFileNumber)
					{
						for (int i = anchorFileNumber + 1; i <= oldSelectedFileNumber; i++)
						{
							multiFileList.Remove(i);
						}
					}
					else if (selectedFileNumber >= anchorFileNumber && oldSelectedFileNumber < anchorFileNumber)
					{
						for (int i = anchorFileNumber - 1; i >= oldSelectedFileNumber; i--)
						{
							multiFileList.Remove(i);
						}
					}
					if (selectedFileNumber > anchorFileNumber && selectedFileNumber < oldSelectedFileNumber)
					{
						for (int i = selectedFileNumber; i <= oldSelectedFileNumber; i++)
						{
							multiFileList.Remove(i);
						}
					}
					else if (selectedFileNumber < anchorFileNumber && selectedFileNumber > oldSelectedFileNumber)
					{
						for (int i = selectedFileNumber; i >= oldSelectedFileNumber; i--)
						{
							multiFileList.Remove(i);
						}
					}
					if (!multiFileList.Contains(selectedFileNumber))
					{
						multiFileList.Add(selectedFileNumber);
					}
				}
				else if (!flag2)
				{
					if (multiFileList.Count > 0)
					{
						multiFileList.Clear();
					}
					multiFileList.Add(selectedFileNumber);
					anchorFileNumber = selectedFileNumber;
				}
				if (!flag2)
				{
					fileName = ((multiFileList.Count != 1 || selectedFileNumber < dirList.Count || selectedFileNumber - dirList.Count >= fileList.Count) ? string.Empty : fileDisplayList[multiFileList[0]]);
				}
			}
			else if (fileType == FileType.Save)
			{
				if (selectedFileNumber >= dirList.Count && selectedFileNumber - dirList.Count < fileList.Count)
				{
					fileName = fileList[selectedFileNumber - dirList.Count].name;
				}
			}
			else
			{
				fileName = ((selectedFileNumber < dirList.Count || selectedFileNumber - dirList.Count >= fileList.Count) ? string.Empty : fileList[selectedFileNumber - dirList.Count].name);
			}
			oldSelectedFileNumber = selectedFileNumber;
			if (fileType == FileType.Save && autoAddExtension && !fileName.EndsWith(addedExtension))
			{
				fileName += addedExtension;
			}
		}
		int j = 0;
		Touch[] touches = Input.touches;
		for (int length = touches.Length; j < length; j++)
		{
			if (touches[j].tapCount == 2 && (fileAreaRect.Contains(mousePos) || volumeBoxRect.Contains(mousePos)) && frameDone)
			{
				StartCoroutine(SelectFile());
				StartCoroutine(WaitForFrame());
				messageWindowDelay = true;
				break;
			}
		}
		if (useFilterButton)
		{
			GUI.Label(new Rect(40f + windowTabOffset.x, fileWindowRect.height - 76f + windowTabOffset.y, 120f, 50f), windowTab);
			if (GUI.Button(new Rect(50f, fileWindowRect.height - 76f + windowTabOffset.y + 8f, 80f, 33f), (!filterFiles) ? "Filter" : "Show all"))
			{
				filterFiles = !filterFiles;
				GetCurrentFileInfo();
			}
		}
		if (useDeleteButton && fileType != FileType.Folder)
		{
			if (fileType == FileType.Open)
			{
				if ((!_allowMultiSelect && fileName == string.Empty) || (_allowMultiSelect && multiFileList.Count == 0))
				{
					GUI.enabled = false;
				}
			}
			else if (fileName == string.Empty || (autoAddExtension && fileName == addedExtension))
			{
				GUI.enabled = false;
			}
			if (GUI.Button(new Rect(fileWindowRect.width - (float)buttonPositionX - (buttonSize.x + 25f) * 2f, fileWindowRect.height - (float)buttonPositionY, buttonSize.x, buttonSize.y), "Delete"))
			{
				StartCoroutine(DeleteFile());
			}
		}
		if (!showMessageWindow)
		{
			GUI.enabled = true;
		}
		if (GUI.Button(new Rect(fileWindowRect.width - (float)buttonPositionX - (buttonSize.x + 15f), fileWindowRect.height - (float)buttonPositionY, buttonSize.x, buttonSize.y), "Cancel"))
		{
			CloseFileWindow();
		}
		if (fileType == FileType.Open)
		{
			if ((!multi && selectedFileNumber == -1) || (multi && multiFileList.Count == 0))
			{
				GUI.enabled = false;
			}
		}
		else if (fileType == FileType.Folder)
		{
			if (selectedFileNumber == -1 || selectedFileNumber >= dirList.Count)
			{
				GUI.enabled = false;
			}
		}
		else if ((selectedFileNumber == -1 || selectedFileNumber >= dirList.Count) && ((!autoAddExtension && fileName == string.Empty) || (autoAddExtension && fileName == addedExtension)))
		{
			GUI.enabled = false;
		}
		if (GUI.Button(new Rect(fileWindowRect.width - (float)buttonPositionX, fileWindowRect.height - (float)buttonPositionY, buttonSize.x, buttonSize.y), (fileType != FileType.Save || selectedFileNumber <= -1 || selectedFileNumber >= dirList.Count) ? selectButtonText[(int)fileType] : selectButtonText[0]))
		{
			StartCoroutine(SelectFile());
		}
		if (!showMessageWindow)
		{
			GUI.enabled = true;
		}
		if (runCustomFunction)
		{
			customFunction();
		}
		if (!limitToInitialFolder && pathList.Length > 0 && Popup.List(popupRect, showPopup, selectedPath, pathList[0], pathList, popupButtonStyle, popupBoxStyle, popupListStyle) && selectedPath.i > 0)
		{
			BuildPathList(selectedPath.i);
		}
	}

	private void ShowError(string msg)
	{
		message = msg;
		messageWindowTitle = "Error";
		showMessageWindow = true;
		messageWindowType = MessageWindowType.Error;
		fileName = string.Empty;
	}

	private void ShowConfirmMessage(string title, string msg, string b1Text, string b2Text)
	{
		messageWindowRect = new Rect((float)(Screen.width / 2) - messageWindowSize.x / 2f, (float)(Screen.height / 2) - messageWindowSize.y / 2f, messageWindowSize.x, messageWindowSize.y);
		message = msg;
		button1Text = b1Text;
		button2Text = b2Text;
		messageWindowTitle = title;
		showMessageWindow = true;
		messageWindowType = MessageWindowType.Confirm;
	}

	private void MessageWindow()
	{
		GUI.Label(new Rect(fileWindowInset, windowOffset + fileWindowInset, messageWindowRect.width - (float)(fileWindowInset * 2), messageWindowRect.height - ((float)(windowOffset + fileWindowInset) + buttonSize.y + 25f)), message);
		if (messageWindowType == MessageWindowType.Error)
		{
			if (GUI.Button(new Rect(messageWindowSize.x - ((float)fileWindowInset + buttonSize.x), messageWindowSize.y - (buttonSize.y + (float)fileWindowInset), buttonSize.x, buttonSize.y), "OK") && frameDone)
			{
				CloseMessageWindow(false);
			}
		}
		else if (messageWindowType == MessageWindowType.Confirm)
		{
			if (GUI.Button(new Rect(messageWindowSize.x - ((float)fileWindowInset + buttonSize.x * 2f + 25f), messageWindowSize.y - (buttonSize.y + (float)fileWindowInset), buttonSize.x, buttonSize.y), button1Text) && frameDone)
			{
				CloseMessageWindow(false);
			}
			if (GUI.Button(new Rect(messageWindowSize.x - ((float)fileWindowInset + buttonSize.x), messageWindowSize.y - (buttonSize.y + (float)fileWindowInset), buttonSize.x, buttonSize.y), button2Text) && frameDone)
			{
				CloseMessageWindow(true);
			}
		}
	}

	private void CloseMessageWindow(bool isConfirmed)
	{
		showMessageWindow = false;
		confirm = isConfirmed;
	}

	private IEnumerator WaitForFrame()
	{
		return new _0024WaitForFrame_002439(this).GetEnumerator();
	}

	private void BuildPathList(int pathEntry)
	{
		filePath = string.Empty;
		for (int num = pathList.Length - 1; num >= pathEntry; num--)
		{
			filePath += pathList[num].text;
			if (num < pathList.Length - 1 || windowsSystem)
			{
				filePath += pathChar;
			}
		}
		selectedPath.i = -1;
		GetCurrentFileInfo();
	}

	private void GetCurrentFileInfo()
	{
		dirList.Clear();
		fileList.Clear();
		bool flag = true;
		DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
		if (!directoryInfo.Exists)
		{
			flag = false;
		}
		FileInfo[] array;
		DirectoryInfo[] array2;
		if (flag)
		{
			try
			{
				array = directoryInfo.GetFiles();
				array2 = directoryInfo.GetDirectories();
			}
			catch (Exception ex)
			{
				HandleError(ex.Message);
				return;
			}
		}
		else
		{
			array = new FileInfo[0];
			array2 = new DirectoryInfo[0];
		}
		if (!limitToInitialFolder && array2.Length > 0)
		{
			for (int i = 0; i < array2.Length; i++)
			{
				if ((array2[i].Name.EndsWith(".app") && !allowAppBundleBrowsing) || (array2[i].Name.StartsWith(".") && !showHiddenOSXFiles))
				{
					continue;
				}
				if (doFolderFunction)
				{
					FileInfo[] files = new DirectoryInfo(array2[i].FullName).GetFiles();
					if (!folderFunction(files))
					{
						continue;
					}
				}
				DateTime date = ((!showDate) ? DateTime.MinValue : array2[i].LastWriteTime);
				dirList.Add(new FileData(array2[i].Name, date));
			}
			if (sortType == SortType.Name)
			{
				dirList.Sort(_0024adaptor_0024__UniFileBrowser_0024callable5_0024991_39___0024Comparison_00241.Adapt((FileData a, FileData b) => a.name.CompareTo(b.name)));
			}
			else
			{
				dirList.Sort(_0024adaptor_0024__UniFileBrowser_0024callable5_0024991_39___0024Comparison_00241.Adapt((FileData a, FileData b) => a.date.CompareTo(b.date)));
				if (sortType == SortType.DateNewest)
				{
					dirList.Reverse();
				}
			}
		}
		if (showVolumes && !limitToInitialFolder)
		{
			try
			{
				if (windowsSystem || linuxSystem)
				{
					string[] logicalDrives = Directory.GetLogicalDrives();
					int num = 0;
					for (int i = 0; i < logicalDrives.Length; i++)
					{
						if (logicalDrives[i].Length > 1)
						{
							dirList.Insert(num++, new FileData(logicalDrives[i].Substring(0, logicalDrives[i].Length - 1), DateTime.MinValue));
						}
					}
					numberOfVolumes = num;
					if (windowsSystem)
					{
						filePath = filePath.Replace(":" + pathChar + pathChar, ":" + pathChar);
					}
				}
				else
				{
					directoryInfo = ((!androidSystem) ? new DirectoryInfo("/Volumes") : new DirectoryInfo("/mnt"));
					array2 = directoryInfo.GetDirectories();
					int num = 0;
					for (int i = 0; i < array2.Length; i++)
					{
						if (array2[i].GetFiles().Length != 0 || array2[i].GetDirectories().Length != 0)
						{
							dirList.Insert(num++, new FileData(array2[i].Name, DateTime.MinValue));
						}
					}
					numberOfVolumes = num;
				}
			}
			catch (Exception ex2)
			{
				HandleError(ex2.Message);
			}
		}
		if (showFiles && array.Length > 0)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i].Name.StartsWith(".") && !showHiddenOSXFiles)
				{
					continue;
				}
				if (filterFiles && filterFileExtensions.Length > 0)
				{
					bool flag2 = true;
					for (int num2 = 0; num2 < filterFileExtensions.Length; num2++)
					{
						if (array[i].Name.EndsWith(filterFileExtensions[num2]))
						{
							flag2 = false;
							break;
						}
					}
					if (flag2)
					{
						continue;
					}
				}
				if (!doFileFunction || fileFunction(array[i].Name))
				{
					DateTime date2 = ((!showDate) ? DateTime.MinValue : array[i].LastWriteTime);
					fileList.Add(new FileData(array[i].Name, date2));
				}
			}
			if (sortType == SortType.Name)
			{
				fileList.Sort(_0024adaptor_0024__UniFileBrowser_0024callable5_0024991_39___0024Comparison_00241.Adapt((FileData a, FileData b) => a.name.CompareTo(b.name)));
			}
			else
			{
				fileList.Sort(_0024adaptor_0024__UniFileBrowser_0024callable5_0024991_39___0024Comparison_00241.Adapt((FileData a, FileData b) => a.date.CompareTo(b.date)));
				if (sortType == SortType.DateNewest)
				{
					fileList.Reverse();
				}
			}
		}
		iconDisplayList = new Texture[dirList.Count + fileList.Count];
		fileDisplayList = new string[iconDisplayList.Length];
		dateDisplayList = new string[iconDisplayList.Length];
		for (int i = 0; i < dirList.Count; i++)
		{
			if (showVolumes && i < numberOfVolumes)
			{
				iconDisplayList[i] = driveIcon;
				fileDisplayList[i] = dirList[i].name;
				dateDisplayList[i] = string.Empty;
			}
			else
			{
				iconDisplayList[i] = folderIcon;
				fileDisplayList[i] = dirList[i].name;
				dateDisplayList[i] = "  " + dirList[i].date;
			}
		}
		for (int i = 0; i < fileList.Count; i++)
		{
			iconDisplayList[i + dirList.Count] = fileIcon;
			fileDisplayList[i + dirList.Count] = fileList[i].name;
			dateDisplayList[i + dirList.Count] = "  " + fileList[i].date;
		}
		string[] array3 = filePath.Split(pathChar);
		System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
		for (int i = 0; i < Extensions.get_length((System.Array)array3) - 1; i++)
		{
			if (array3[i] == string.Empty)
			{
				list.Add(pathChar.ToString());
			}
			else
			{
				list.Add(array3[i]);
			}
		}
		list.Reverse();
		pathList = new GUIContent[list.Count];
		for (int i = 0; i < pathList.Length; i++)
		{
			pathList[i] = new GUIContent(list[i], folderIcon);
		}
		selectedFileNumber = (oldSelectedFileNumber = -1);
		scrollPos = Vector2.zero;
		if (_allowMultiSelect)
		{
			multiFileList.Clear();
			anchorFileNumber = 0;
		}
		UpdateRects();
	}

	private void HandleError(string errorMessage)
	{
		messageWindowRect = new Rect((float)(Screen.width / 2) - messageWindowSize.x / 2f, (float)(Screen.height / 2) - messageWindowSize.y / 2f, messageWindowSize.x, messageWindowSize.y);
		ShowError(errorMessage);
		SetDefaultPath();
		fileDisplayList = new string[0];
		pathList = new GUIContent[0];
	}

	public virtual void SetPath(string thisPath)
	{
		filePath = thisPath;
		if (!filePath.EndsWith(pathChar.ToString()))
		{
			filePath += pathChar;
		}
		if (windowsSystem)
		{
			filePath = filePath.Replace("/", "\\");
		}
	}

	public virtual void OpenFileWindow(__UniFileBrowser_delegate_0024callable0_0024117_24__ openFileFunction)
	{
		if (!fileWindowOpen)
		{
			if (_allowMultiSelect)
			{
				Debug.LogError("When using allowMultiSelect, you must supply a function that accepts a string array");
				return;
			}
			@delegate = openFileFunction;
			showFiles = true;
			fileType = FileType.Open;
			ShowFileWindow();
		}
	}

	public virtual void OpenFileWindow(__UniFileBrowser_multiDelegate_0024callable1_0024118_29__ openFileFunction)
	{
		if (!fileWindowOpen)
		{
			if (!_allowMultiSelect)
			{
				Debug.LogError("When not using allowMultiSelect, you must supply a function that accepts a string, not a string array");
				return;
			}
			multiDelegate = openFileFunction;
			showFiles = true;
			fileType = FileType.Open;
			ShowFileWindow();
		}
	}

	public virtual void OpenFolderWindow(bool showFiles, __UniFileBrowser_delegate_0024callable0_0024117_24__ openFolderFunction)
	{
		if (!fileWindowOpen)
		{
			@delegate = openFolderFunction;
			this.showFiles = showFiles;
			fileType = FileType.Folder;
			limitToInitialFolder = false;
			popupRect.height = storedPopupHeight;
			ShowFileWindow();
		}
	}

	public virtual void SaveFileWindow(__UniFileBrowser_delegate_0024callable0_0024117_24__ saveFileFunction)
	{
		if (!fileWindowOpen)
		{
			@delegate = saveFileFunction;
			showFiles = true;
			fileType = FileType.Save;
			ShowFileWindow();
			windowOpenedCounter = 2;
		}
	}

	private void ShowFileWindow()
	{
		GetCurrentFileInfo();
		windowTitle = fileWindowTitles[(int)fileType];
		fileWindowOpen = true;
		lastRect = fileWindowRect;
		enabled = true;
		fileName = string.Empty;
		bool num = _allowMultiSelect;
		if (num)
		{
			num = fileType == FileType.Open;
		}
		multi = num;
		GUIStyle style = guiSkin.GetStyle("textfield");
		textfieldRect = new Rect(fileWindowInset + 90, (!limitToInitialFolder) ? (popupRect.y + popupRect.height + 15f) : ((float)(fileWindowInset + windowOffset)), 0f, 0f);
		if (fileType != FileType.Folder)
		{
			textfieldRect.height = style.CalcHeight(new GUIContent(" "), 1f) + (float)style.padding.top;
		}
		bottomAreaSpace = (int)((float)(fileWindowInset + 15) + buttonSize.y);
		buttonPositionX = (int)((float)fileWindowInset + buttonSize.x);
		buttonPositionY = (int)((float)fileWindowInset + buttonSize.y);
		UpdateRects();
	}

	public virtual void SendWindowCloseMessage(__UniFileBrowser_closeWindowFunction_0024callable2_0024120_35__ closeWindowFunction)
	{
		sendCloseMessage = true;
		this.closeWindowFunction = closeWindowFunction;
	}

	public virtual void DontSendWindowCloseMessage()
	{
		sendCloseMessage = false;
	}

	public virtual void SendWindowChangeMessage(__UniFileBrowser_closeWindowFunction_0024callable2_0024120_35__ changeWindowFunction)
	{
		sendChangeMessage = true;
		this.changeWindowFunction = changeWindowFunction;
	}

	public virtual void DontSendWindowChangeMessage()
	{
		sendChangeMessage = false;
	}

	public virtual void SetCustomFunction(__UniFileBrowser_closeWindowFunction_0024callable2_0024120_35__ func)
	{
		runCustomFunction = true;
		customFunction = func;
	}

	public virtual void RemoveCustomFunction()
	{
		runCustomFunction = false;
	}

	public virtual void UseFolderFilterFunction(__UniFileBrowser_folderFunction_0024callable3_0024149_30__ folderFunction)
	{
		doFolderFunction = true;
		this.folderFunction = folderFunction;
	}

	public virtual void DontUseFolderFilterFunction()
	{
		doFolderFunction = false;
	}

	public virtual void UseFileFilterFunction(__UniFileBrowser_fileFunction_0024callable4_0024150_28__ fileFunction)
	{
		doFileFunction = true;
		this.fileFunction = fileFunction;
	}

	public virtual void DontUseFileFilterFunction()
	{
		doFileFunction = false;
	}

	public virtual void CloseFileWindow()
	{
		if (!showMessageWindow)
		{
			fileWindowOpen = false;
			selectedFileNumber = (oldSelectedFileNumber = -1);
			fileName = string.Empty;
			if (sendCloseMessage)
			{
				closeWindowFunction();
			}
			enabled = false;
		}
	}

	public virtual void SetWindowTitle(string title)
	{
		windowTitle = title;
	}

	public virtual void SetFileWindowPosition(Vector2 windowPosition)
	{
		windowPosition.x = Mathf.Max(0f, windowPosition.x);
		windowPosition.y = Mathf.Max(0f, windowPosition.y);
		fileWindowRect = GetClampedWindowRect(new Rect(windowPosition.x, windowPosition.y, fileWindowRect.width, fileWindowRect.height));
		if (!RuntimeServices.EqualityOperator(fileDisplayList, null))
		{
			UpdateRects();
		}
	}

	public virtual void SetFileWindowSize(Vector2 windowSize)
	{
		windowSize.x = Mathf.Max(0f, windowSize.x);
		windowSize.y = Mathf.Max(0f, windowSize.y);
		fileWindowRect = GetClampedWindowRect(new Rect(fileWindowRect.x, fileWindowRect.y, windowSize.x, windowSize.y));
		if (!RuntimeServices.EqualityOperator(fileDisplayList, null))
		{
			UpdateRects();
		}
	}

	private Rect GetClampedWindowRect(Rect newWindowRect)
	{
		float num = (float)Screen.width - newWindowRect.x;
		if (!(num >= (float)minWindowWidth))
		{
			newWindowRect.x -= (float)minWindowWidth - num;
			num = minWindowWidth;
		}
		float num2 = (float)Screen.height - newWindowRect.y;
		if (!(num2 >= (float)minWindowHeight))
		{
			newWindowRect.y -= (float)minWindowHeight - num2;
			num2 = minWindowHeight;
		}
		return new Rect(newWindowRect.x, newWindowRect.y, Mathf.Clamp(newWindowRect.width, minWindowWidth, num), Mathf.Clamp(newWindowRect.height, minWindowHeight, num2));
	}

	private IEnumerator DeleteFile()
	{
		return new _0024DeleteFile_002442(this).GetEnumerator();
	}

	private System.Collections.Generic.List<string> GetMultiFileNames()
	{
		System.Collections.Generic.List<string> list = new System.Collections.Generic.List<string>();
		for (int i = 0; i < multiFileList.Count; i++)
		{
			if (multiFileList[i] >= dirList.Count)
			{
				list.Add(filePath + fileList[multiFileList[i] - dirList.Count].name);
			}
		}
		return list;
	}

	private IEnumerator SelectFile()
	{
		return new _0024SelectFile_002448(this).GetEnumerator();
	}

	internal int _0024GetCurrentFileInfo_0024closure_002425(FileData a, FileData b)
	{
		return a.name.CompareTo(b.name);
	}

	internal int _0024GetCurrentFileInfo_0024closure_002426(FileData a, FileData b)
	{
		return a.date.CompareTo(b.date);
	}

	internal int _0024GetCurrentFileInfo_0024closure_002429(FileData a, FileData b)
	{
		return a.name.CompareTo(b.name);
	}

	internal int _0024GetCurrentFileInfo_0024closure_002430(FileData a, FileData b)
	{
		return a.date.CompareTo(b.date);
	}
}
