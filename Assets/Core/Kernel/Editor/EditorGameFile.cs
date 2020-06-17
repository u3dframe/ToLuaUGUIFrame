using UnityEngine;
using UnityEditor;
#if UI_SPRITE_ATLAS
using UnityEditor.U2D;
using UnityEngine.U2D;
#endif

namespace Core
{
    using Kernel;
    /// <summary>
    /// 类名 : 编辑模式下 - 文件管理
    /// 作者 : Canyon / 龚阳辉
    /// 日期 : 2020-03-26 09:29
    /// 功能 : 
    /// </summary>
    public class EditorGameFile : GameFile
    {
        // 相对目录(被打包的资源必须在该目录下，不然不会被打包)
        static public string m_rootRelative = "Builds";

        // 临时存放所有生成的文件(ab,txt等，可以进行Zip压缩，最后将生成zip文件或copy的所以文件再拷贝到流文件夹下)
        static public readonly string m_dirResCache = string.Format("{0}/../GameCache/{1}/", Application.dataPath, m_curPlatform);

        // zip 压缩文件
        static public readonly string m_fpZipCachePatch = string.Format("{0}res_patch.zip", m_dirResCache);
        static public readonly string m_fpZipObb = string.Format("{0}obb.zip", m_dirResCache);
        static public readonly string m_fpZipListCache = string.Concat(m_dirResCache, "ziplist.txt");
        static public readonly string m_fmtZipCache = string.Concat(m_dirResCache, "_zips/resource{0}.zip");

        static public bool IsInBuild(string fp)
        {
            if (string.IsNullOrEmpty(fp))
                return false;
            if (fp.Contains(m_assets) && fp.Contains(m_edtAssetPath))
            {
                return fp.Contains(m_rootRelative);
            }
            return false;
        }

        static public bool IsShader(string fp)
        {
            if (string.IsNullOrEmpty(fp))
                return false;
            return fp.Contains("shaders/");
        }

        static public bool IsFont(string fp)
        {
            if (string.IsNullOrEmpty(fp))
                return false;
            return fp.Contains("fnts/");
        }

        static public bool IsUI(string fp)
        {
            if (string.IsNullOrEmpty(fp))
                return false;
            return fp.Contains("ui/");
        }

        static public bool IsSingleTexture(string fp)
        {
            if (string.IsNullOrEmpty(fp))
                return false;
            return fp.Contains("ui_sngs/");
        }

        static public bool IsAtlasTexture(string fp)
        {
            if (string.IsNullOrEmpty(fp))
                return false;
            return fp.Contains("ui_atlas/");
        }

        static public bool IsMustAB(string fp)
        {
            if (string.IsNullOrEmpty(fp))
                return false;
            // 单图(icon,bg，大图片的) ，shader , 字体等必须要文件
            return IsShader(fp) || IsFont(fp) || IsSingleTexture(fp) || IsAtlasTexture(fp) || IsUI(fp);
        }

        static public bool IsInBuild(string fp, ref bool isMust)
        {
            isMust = false;
            if (IsInBuild(fp))
            {
                isMust = IsMustAB(fp);
                return true;
            }
            return false;
        }

        // 设置 资源 AssetBundle 信息
        static public void SetABInfo(string assetPath, string abName = "", string abSuffix = "")
        {
            AssetImporter _ai = AssetImporter.GetAtPath(assetPath);
            SetABInfo(_ai, abName, abSuffix);
        }

        static public void SetABInfo(AssetImporter assetImporter, string abName = "", string abSuffix = "")
        {
            if (!string.IsNullOrEmpty(assetImporter.assetBundleName))
            {
                assetImporter.assetBundleName = null;
            }

            if (!string.IsNullOrEmpty(assetImporter.assetBundleVariant))
            {
                assetImporter.assetBundleVariant = null;
            }

            if (abName != null)
                abName = abName.Trim();
            bool isABName = !string.IsNullOrEmpty(abName);
            if (isABName)
            {
                // 资源名
                assetImporter.assetBundleName = abName.ToLower();
            }

            if (!isABName)
            {
                return;
            }

            if (abSuffix != null)
                abSuffix = abSuffix.Trim();
            bool isABSuffix = !string.IsNullOrEmpty(abSuffix);
            if (isABSuffix)
            {
                // 资源后缀名
                assetImporter.assetBundleVariant = abSuffix.ToLower();
            }
        }

        static public void SetABInfo(UnityEngine.Object obj, string abName = "", string abSuffix = "")
        {
            string _assetPath = GetPath(obj);
            SetABInfo(_assetPath, abName, abSuffix);
        }

        static public string RelativePath(string fp)
        {
            // 去掉第一个Assets文件夹路径
            int index = fp.IndexOf(m_rootRelative);
            string ret = fp.Substring(index + m_rootRelative.Length + 1);
            return ret;
        }

        static public (string, bool, string) GetABEndName(UnityEngine.Object obj)
        {
            if(obj == null)
                return (null,false,null);

            string _assetPath = GetPath(obj);
            string _suffix = PathEx.GetSuffixNoPoint(_assetPath);
            System.Type _objType = obj.GetType();
            string _ret = null;
            bool _isBl = false;
            string abSuffix = null;
            if (IsFont(_assetPath))
            {
                _isBl = true;
                if (_objType == typeof(Font))
                    _ret = ".ab_fnt";
                else
                    _ret = string.Concat(".ab_fnt_", _suffix, "_error");
            }
            else if (IsShader(_assetPath))
            {
                _isBl = true;
                if (_objType == typeof(Shader))
                    _ret = ".ab_shader";
                else
                    _ret = string.Concat(".ab_shader_", _suffix, "_error");
            }
            else if (IsUI(_assetPath))
            {
                _isBl = true;
                if (_objType == typeof(GameObject))
                    _ret = ".ui";
                else
                    _ret = string.Concat(".ui_", _suffix, "_error");
            }
            else if (IsSingleTexture(_assetPath))
            {
                _isBl = true;
                if (_objType == typeof(Texture2D))
                    _ret = ".tex";
                else
                    _ret = string.Concat(".tex_sngs_", _suffix, "_error");
            }
            else if (IsAtlasTexture(_assetPath))
            {
                _isBl = true;
#if UI_SPRITE_ATLAS
                if (_objType == typeof(SpriteAtlas))
                    _ret = ".sa";
#else
                if (_objType == typeof(Texture2D))
                    _ret = ".tex_atlas";
#endif
                else
                    _ret = string.Concat(".tex_atlas_", _suffix, "_error");
            }
            else if (_objType == typeof(GameObject))
            {
                _isBl = true;
                _ret = ".fab";
            }

            if (_ret == null)
            {
                if (_objType == typeof(Texture2D))
                {
                    _ret = ".tex";
                }
                else if (_objType == typeof(Cubemap))
                {
                    _ret = ".tex_cub";
                }
                else if (_objType == typeof(AudioClip))
                {
                    _ret = ".audio";
                }
                else if (_objType == typeof(Material))
                {
                    _ret = ".ab_mat";
                }
                else if ("fbx".Equals(_suffix, System.StringComparison.OrdinalIgnoreCase))
                {
                    _ret = ".ab_fbx";
                }
            }
            return (_ret, _isBl, abSuffix);
        }

        static public (string, bool, string) GetABEndName(string assetPath)
        {
            return GetABEndName(Load4Develop(assetPath));
        }

        static string GetNameEndAndExtension(UnityEngine.Object obj, ref string abSuffix)
        {
            string _assetPath = GetPath(obj);
            string _suffix = PathEx.GetSuffixNoPoint(_assetPath);
            System.Type _objType = obj.GetType();
            (string _str, bool _isMust, string _abExtension) = GetABEndName(obj);
            abSuffix = _abExtension;
            if (_isMust)
            {
                return _str;
            }
            // var _obj = MgrABDataDependence.GetData(obj);
            int _count = MgrABDataDependence.GetCount(obj);
            bool _isBl = !string.IsNullOrEmpty(_str);
            if (_isBl && _count > 1)
            {
                // Debug.Log(_obj);
                return _str;
            }

            if (_isBl)
                return string.Format("{0}_{1}_error", _str, _suffix);

            return string.Format(".{0}_error", _suffix);
        }

        static public string GetAbName(UnityEngine.Object obj, ref string abSuffix)
        {
            string fp = GetPath(obj);
            abSuffix = null;
            if (!IsInBuild(fp))
            {
                string _sErr = string.Format("只能导出Assets目录下面的[{0}]目录下的[{1}]目录里面的资源,请移至[{1}]目录下面", m_edtAssetPath, m_rootRelative);
                throw new System.Exception(_sErr);
            }

            string _end = GetNameEndAndExtension(obj, ref abSuffix);
            string _fn = PathEx.GetFileNameNoSuffix(fp);
            string _sfp = RelativePath(fp);
            string _lft = _sfp.Split('.')[0];
            string _ret = _lft + _end;

            if (IsAtlasTexture(fp) && obj.GetType() == typeof(Texture2D))
            {
                TextureImporter _tImpt = TextureImporter.GetAtPath(fp) as TextureImporter;
                if (!string.IsNullOrEmpty(_tImpt.spritePackingTag))
                {
                    _ret = _lft.Replace(_fn, _tImpt.spritePackingTag) + _end; // 新版本
                    // _ret =  string.Format("ui/atlas/{0}{1}" ,_tImpt.spritePackingTag , _end); // old版本
                }
            }

            return _ret.ToLower();
        }

        static public string GetAbName(string fp, ref string abSuffix)
        {
            UnityEngine.Object obj = Load4Develop(fp);
            return GetAbName(obj, ref abSuffix);
        }

        static public string GetFullPath(string assetPath)
        {
            return string.Format("{0}{1}", m_dirDataNoAssets, assetPath);
        }

        static public bool IsExistsInAssets(string assetPath)
        {
            string _mt = GetFullPath(assetPath);
            return IsFile(_mt);
        }
    }
}