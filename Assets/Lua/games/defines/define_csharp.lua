-- U开头代表Unity的CSharp Class, C开头代表自身封装的CSharp Class

-- UAnimation = UnityEngine.Animation
UGameObject = UnityEngine.GameObject
UTransform = UnityEngine.Transform
URectTransform = UnityEngine.RectTransform
UWebRequest = UnityEngine.Networking.UnityWebRequest
UText = UnityEngine.UI.Text
UImage = UnityEngine.UI.Image
URawImage = UnityEngine.UI.RawImage


CWWWMgr = Core.Kernel.WWWMgr
CBtBuffer = TNet.ByteBuffer
CNetMgr = NetworkManager
CGameFile = Core.GameFile
CGobjLife = GobjLifeListener;
CPElement = PrefabElement;
CHelper = LuaHelper;
CDebug = Debugger;

-- Charpe 的 常量 cost 属性 ([[初始化后，不会在变化的属性]])
GM_IsEditor = CGameFile.isEditor
-- TP_UText = typeof(UText)