/*
Copyright (C) 2020-2021 Tris Shores

This program is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace marc_common
{
    internal static class c_ExtensionCore
    {
        internal static bool m_IsNumeric(this string v_str)
        {
            return Regex.IsMatch(v_str, @"^\d+$");
            //return int.TryParse(v_str, out _);
        }

        internal static bool m_IsNotEmpty(this string v_str)
        {
            return !string.IsNullOrWhiteSpace(v_str);
        }

        internal static bool m_IsEmpty(this string v_str)
        {
            return string.IsNullOrWhiteSpace(v_str);
        }

        internal static bool m_IsNotEmpty<T>(this T[] v_array)
        {
            if (v_array == null) return false;
            return v_array.Length > 0;
        }

        internal static bool m_IsEmpty<T>(this T[] v_array)
        {
            if (v_array == null) return true;
            return v_array.Length == 0;
        }

        internal static bool m_IsNotEmpty<T>(this List<T> v_list)
        {
            if (v_list == null) return false;
            return v_list.Count > 0;
        }

        internal static bool m_IsEmpty<T>(this List<T> v_list)
        {
            if (v_list == null) return true;
            return v_list.Count == 0;
        }

        internal static bool m_ContainsXx(this string v_str, string v_substring)
        {
            if (v_str == null || v_substring.m_IsEmpty()) return false;
            return v_str.ToLower().Contains(v_substring.ToLower());
        }

        internal static bool m_ContainsXx(this IEnumerable<string> v_array, string v_str)
        {
            if (v_array == null) return false;
            foreach (var v_item in v_array)
            {
                if (string.Equals(v_item, v_str, StringComparison.OrdinalIgnoreCase)) return true;
            }
            return false;
        }

        internal static bool m_RemoveXx(this List<string> v_array, string v_str)
        {
            if (v_array == null) return false;
            foreach (var v_item in v_array)
            {
                if (string.Equals(v_item, v_str, StringComparison.OrdinalIgnoreCase))
                {
                    v_array.Remove(v_item);
                    return true;
                }
            }
            return false;
        }

        internal static string m_RemoveAll(this string v_str, string v_str1, string v_str2, string v_str3 = null)
        {
            if (v_str == null) return null;
            var v_ret = v_str.Replace(v_str1, "").Replace(v_str2, "");
            if (v_str3 != null) v_ret = v_ret.Replace(v_str3 ?? "", "");
            return v_ret;
        }

        internal static string m_RemoveAllXx(this string v_str, IEnumerable<string> v_strArray)
        {
            if (v_str == null) return null;
            if (v_strArray == null) return v_str;

            foreach (var v_strArr in v_strArray)
            {
                v_str = v_str.m_RemoveXx(v_strArr, StringComparison.OrdinalIgnoreCase);
            }
            return v_str;
        }

        internal static string m_RemoveXx(this string v_str, string v_str2, StringComparison v_comp)
        {
            if (v_str == null) return null;
            if (v_str2 == null) return v_str;

            var idx = v_str.IndexOf(v_str2, v_comp);
            if (idx < 0) return v_str;
            var v_str3 = v_str.Remove(idx, v_str2.Length);
            return v_str3;
        }

        internal static IEnumerable<string> m_RemoveAll(this IEnumerable<string> v_strArray, IEnumerable<string> v_strArray2)
        {
            foreach (var v_str in v_strArray2)
            {
                if (v_strArray.Any(v_x => v_x.Equals(v_str)))
                {
                    v_strArray.ToList().Remove(v_str);
                }
            }
            return v_strArray;
        }

        internal static IEnumerable<TSource> m_DistinctBy<TSource, TKey>(this IEnumerable<TSource> v_source, Func<TSource, TKey> v_keySelector)
        {
            if (v_source == null) throw new ArgumentNullException(nameof(v_source));
            var v_seenKeys = new HashSet<TKey>();
            foreach (var v_source1 in v_source)
            {
                var v_element = v_source1;
                if (v_seenKeys.Add(v_keySelector(v_element)))
                    yield return v_element;
            }
        }

        internal static string m_SubstringX(this string v_str, int v_startPos, int v_endPos)
        {
            return v_str.Substring(v_startPos, v_endPos - v_startPos);
        }

        internal static string m_ToFlatJson(this ExpandoObject v_expObj)
        {
            if (v_expObj == null) return null;

            var v_arr = string.Join(",", v_expObj.Select(v_x => $"\"{v_x.Key}\": \"{v_x.Value}\""));
            var v_jsonStr = $"{{ {v_arr} }}";

            return v_jsonStr;
        }

        internal static string m_SeparateNonEmptyStrings(this IEnumerable<string> v_array, string v_separator)
        {
            var v_arr = v_array.Where(v_x => v_x.m_IsNotEmpty());
            var v_str = string.Join(v_separator, v_arr);
            return v_str;
        }

        internal static bool m_EqualsAny(this IEnumerable<string> v_strArray, IEnumerable<string> v_strArray2)
        {
            var v_list = new List<string>();
            v_list.AddRange(v_strArray2);
            foreach (var v_listItem in v_list)
            {
                if (v_strArray.Any(v_x => v_x.Equals(v_listItem)))
                    return true;
            }
            return false;
        }

        internal static bool m_EqualsAny(this string v_str, IEnumerable<string> v_array)
        {
            var v_equalToAny = v_array.Any(v_x => v_x.Equals(v_str));
            return v_equalToAny;
        }

        internal static bool m_EqualsAnyXx(this string v_str, IEnumerable<string> v_array)
        {
            var v_equalToAny = v_array.Any(v_x => v_x.Equals(v_str, StringComparison.OrdinalIgnoreCase));
            return v_equalToAny;
        }

        internal static bool m_EqualsAnyXx(this string v_str, string v_str1, string v_str2 = null, string v_str3 = null, string v_str4 = null, string v_str5 = null, string v_str6 = null)
        {
            var v_list = new List<string>();
            v_list.Add(v_str1);
            if (v_str2 != null) v_list.Add(v_str2);
            if (v_str3 != null) v_list.Add(v_str3);
            if (v_str4 != null) v_list.Add(v_str4);
            if (v_str5 != null) v_list.Add(v_str5);
            if (v_str6 != null) v_list.Add(v_str6);
            var v_equalToAny = v_list.Any(v_x => v_x.Equals(v_str, StringComparison.OrdinalIgnoreCase));
            return v_equalToAny;
        }

        internal static bool m_EqualsAny(this string v_str, string v_str1, string v_str2 = null, string v_str3 = null, string v_str4 = null, string v_str5 = null, string v_str6 = null)
        {
            var v_list = new List<string>();
            v_list.Add(v_str1);
            if (v_str2 != null) v_list.Add(v_str2);
            if (v_str3 != null) v_list.Add(v_str3);
            if (v_str4 != null) v_list.Add(v_str4);
            if (v_str5 != null) v_list.Add(v_str5);
            if (v_str6 != null) v_list.Add(v_str6);
            var v_equalToAny = v_list.Any(v_x => v_x.Equals(v_str));
            return v_equalToAny;
        }

        internal static string m_RemoveNewlines(this string v_str)
        {
            var v_lines = v_str.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            var v_str2 = string.Join("", v_lines);
            return v_str2;
        }


        internal static bool m_StartsWithAny(this IEnumerable<string> v_strArray, string v_str1, string v_str2 = null, string v_str3 = null, string v_str4 = null, string v_str5 = null, bool v_caseInsensitive = false)
        {
            var v_list = new List<string>();
            v_list.Add(v_str1);
            if (v_str2 != null) v_list.Add(v_str2);
            if (v_str3 != null) v_list.Add(v_str3);
            if (v_str4 != null) v_list.Add(v_str4);
            if (v_str5 != null) v_list.Add(v_str5);
            foreach (var v_listItem in v_list)
            {
                if (v_caseInsensitive)
                {
                    if (v_strArray.Any(v_x => v_x.ToLower().StartsWith(v_listItem.ToLower()))) return true;
                }
                else
                {
                    if (v_strArray.Any(v_x => v_x.StartsWith(v_listItem))) return true;
                }
            }
            return false;
        }

        internal static bool m_StartsWithAny(this string v_str, string v_str1, string v_str2 = null, string v_str3 = null, string v_str4 = null, string v_str5 = null, bool v_caseInsensitive = false)
        {
            var v_list = new List<string>();
            v_list.Add(v_str1);
            if (v_str2 != null) v_list.Add(v_str2);
            if (v_str3 != null) v_list.Add(v_str3);
            if (v_str4 != null) v_list.Add(v_str4);
            if (v_str5 != null) v_list.Add(v_str5);
            foreach (var v_listItem in v_list)
            {
                if (v_caseInsensitive)
                {
                    if (v_str.ToLower().StartsWith(v_listItem.ToLower())) return true;
                }
                else
                {
                    if (v_str.StartsWith(v_listItem)) return true;
                }
            }
            return false;
        }

        internal static bool m_StartsWithAny(this string v_str, IEnumerable<string> v_strArray, bool v_caseInsensitive = false)
        {
            foreach (var v_arrayItem in v_strArray)
            {
                if (v_caseInsensitive)
                {
                    if (v_str.ToLower().StartsWith(v_arrayItem.ToLower())) return true;
                }
                else
                {
                    if (v_str.StartsWith(v_arrayItem)) return true;
                }
            }
            return false;
        }

        internal static bool m_ContainsOnly(this string v_str, IEnumerable<string> v_strArray)
        {
            foreach (var v_ch in v_str)
            {
                if (!v_strArray.Contains(v_ch.ToString())) return false;
            }
            return true;
        }

        internal static bool m_ContainsOnly(this string[] v_strArr, IEnumerable<string> v_strArray)
        {
            foreach (var v_str in v_strArr)
            {
                if (!v_strArray.Contains(v_str.ToString())) return false;
            }
            return true;
        }

        internal static bool m_ContainsAny(this string v_str, IEnumerable<string> v_strArray)
        {
            foreach (var v_arrayItem in v_strArray)
            {
                if (v_str.Contains(v_arrayItem))
                    return true;
            }
            return false;
        }

        internal static bool m_ContainsAnyXx(this string v_str, IEnumerable<string> v_strArray)
        {
            foreach (var v_arrayItem in v_strArray)
            {
                if (v_str.m_ContainsXx(v_arrayItem))
                    return true;
            }
            return false;
        }

        internal static bool m_ContainsAny(this string v_str, string v_str1, string v_str2 = null, string v_str3 = null, string v_str4 = null, string v_str5 = null, bool v_caseInsensitive = false)
        {
            if (v_str.m_IsEmpty()) return false;

            var v_list = new List<string>();
            v_list.Add(v_str1);
            if (v_str2 != null) v_list.Add(v_str2);
            if (v_str3 != null) v_list.Add(v_str3);
            if (v_str4 != null) v_list.Add(v_str4);
            if (v_str5 != null) v_list.Add(v_str5);
            foreach (var v_listItem in v_list)
            {
                if (v_caseInsensitive)
                {
                    if (v_str.ToLower().Contains(v_listItem.ToLower())) return true;
                }
                else
                {
                    if (v_str.Contains(v_listItem)) return true;
                }
                
            }
            return false;
        }

        internal static bool m_ContainsAny(this IEnumerable<string> v_strArray, string v_str1, string v_str2 = null, string v_str3 = null, string v_str4 = null, string v_str5 = null, bool v_caseInsensitive = false)
        {
            var v_list = new List<string>();
            v_list.Add(v_str1);
            if (v_str2 != null) v_list.Add(v_str2);
            if (v_str3 != null) v_list.Add(v_str3);
            if (v_str4 != null) v_list.Add(v_str4);
            if (v_str5 != null) v_list.Add(v_str5);
            foreach (var v_listItem in v_list)
            {
                if (v_caseInsensitive)
                {
                    if (v_strArray.Any(v_x => v_x.ToLower().Contains(v_listItem.ToLower()))) return true;
                }
                else
                {
                    if (v_strArray.Any(v_x => v_x.Contains(v_listItem))) return true;
                }
            }
            return false;
        }

        internal static bool m_ContainsAny(this IEnumerable<string> v_strArray, IEnumerable<string> v_strArray2)
        {
            var v_list = new List<string>();
            v_list.AddRange(v_strArray2);
            foreach (var v_listItem in v_list)
            {
                if (v_strArray.Any(v_x => v_x.Contains(v_listItem)))
                    return true;
            }
            return false;
        }

        internal static bool m_ContainsAll(this IEnumerable<string> v_strArray, IEnumerable<string> v_strArray2, bool v_caseInsensitive = false)
        {
            foreach (var v_str2 in v_strArray2)
            {
                if (v_caseInsensitive)
                {
                    if (!v_strArray.m_ContainsXx(v_str2)) return false;
                }
                else
                {
                    if (!v_strArray.Contains(v_str2)) return false;
                }
            }
            return true;
        }

        internal static bool m_Contains(this IEnumerable<XElement> v_elementArray, XElement v_element)
        {
            foreach (var v_ElementArrayItem in v_elementArray)
            {
                if (XElement.DeepEquals(v_ElementArrayItem, v_element)) return true;
            }
            return false;
        }

        internal static bool m_IsSubDir(this string v_subDir, string v_parentDir)
        {
            return ((v_subDir.ToLower().TrimEnd('\\') + '\\').StartsWith(v_parentDir.ToLower().TrimEnd('\\') + '\\'));
        }

        internal static void m_AddIfNotExist(this List<string> v_strArray, string v_str)
        {
            if (v_strArray.Contains(v_str)) return;
            v_strArray.Add(v_str);
        }

        internal static void m_RemoveIfExist(this List<string> v_strArray, string v_str)
        {
            if (!v_strArray.Contains(v_str)) return;
            v_strArray.Remove(v_str);
        }

        internal static int m_IndexOf(this IEnumerable<string> v_strArr, string v_str)
        {
            return Array.IndexOf(v_strArr.ToArray(), v_str);
        }

        internal static bool m_EqualsIgnoreDiacritics(this string v_str1, string v_str2, bool v_caseInsensitive = true)
        {
            if (v_str1 == null) return v_str1 == v_str2;

            if (v_caseInsensitive)
            {
                return String.Compare(v_str1, v_str2, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace | CompareOptions.IgnoreCase) == 0;
            }
            else
            {
                return String.Compare(v_str1, v_str2, CultureInfo.CurrentCulture, CompareOptions.IgnoreNonSpace) == 0;
            }
        }

        internal static string m_Escape(this string str)
        {
            try { return Uri.EscapeDataString(str); }
            catch { return ""; }
        }

        internal static Color m_FromHexColor(this string v_hexStr)
        {
            if (v_hexStr.Length != 6) throw new InvalidOperationException();

            var v_r = int.Parse(v_hexStr.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
            var v_g = int.Parse(v_hexStr.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
            var v_b = int.Parse(v_hexStr.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
            return Color.FromArgb(v_r, v_g, v_b);
        }

        internal static void m_SaveToFile(this XDocument v_xdoc, string v_filePath)
        {
            var v_xws = new XmlWriterSettings();
            v_xws.Encoding = new UTF8Encoding(false);
            v_xws.NewLineHandling = NewLineHandling.Replace;
            v_xws.NewLineChars = "\r\n";
            v_xws.OmitXmlDeclaration = false;
            v_xws.Indent = true;
            using (var v_xmlWriter = XmlWriter.Create(v_filePath, v_xws))
            {
                v_xdoc.Save(v_xmlWriter);    // UTF-8 no BOM (XDocument.Parse will fail if BOM present).
            }
        }

        internal static string m_ToString(this XDocument v_xdoc)
        {
            var v_xws = new XmlWriterSettings();
            v_xws.Encoding = new UTF8Encoding(false);  // UTF-8 no BOM (XDocument.Parse will fail if BOM present).
            v_xws.NewLineHandling = NewLineHandling.Replace;
            v_xws.NewLineChars = "\r\n";
            v_xws.OmitXmlDeclaration = false;
            v_xws.Indent = true;
            string v_xmlStr;
            using (var v_sw = new Utf8StringWriter())
            {
                using (var v_xmlWriter = XmlWriter.Create(v_sw, v_xws))
                {
                    v_xdoc.Save(v_xmlWriter);
                }
                var v_str = v_sw.ToString();
                return v_sw.ToString();
            }
        }

        internal class Utf8StringWriter : StringWriter
        {
            public override Encoding Encoding
            {
                get { return Encoding.UTF8; }
            }
        }
    }
}
