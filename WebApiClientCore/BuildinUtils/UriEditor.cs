﻿using System;

namespace WebApiClientCore
{
    /// <summary>
    /// 表示Uri编辑器
    /// </summary>
    class UriEditor
    {
        /// <summary>
        /// 获取当前的Uri
        /// </summary>
        public Uri Uri { get; private set; }

        /// <summary>
        /// Url创建者
        /// </summary>
        /// <param name="uri">绝对路径的uri</param>
        /// <exception cref="UriFormatException"></exception>
        public UriEditor(Uri uri)
        {
            if (uri.IsAbsoluteUri == false)
            {
                throw new UriFormatException(Resx.required_AbsoluteUri.Format(nameof(uri)));
            }
            this.Uri = uri;
        }


        /// <summary>
        /// 替换带有花括号的参数的值
        /// </summary>
        /// <param name="name">参数名称，不带花括号</param>
        /// <param name="value">参数的值</param>
        /// <returns>替换成功则返回true</returns>
        public bool Replace(string name, string? value)
        {
            var uri = this.Uri.OriginalString;
            if (uri.Contains('{') == false)
            {
                return false;
            }

            if (!string.IsNullOrEmpty(value))
            {
                value = Uri.EscapeDataString(value);
            }

            if (uri.RepaceIgnoreCase($"{{{name}}}", value, out var newUri) == true)
            {
                this.Uri = new Uri(newUri);
                return true;
            }

            return false;
        }

        /// <summary>
        /// 添加参数
        /// </summary>
        /// <param name="name">参数名称</param>
        /// <param name="value">参数的值</param>
        public void AddQuery(string name, string? value)
        {
            var uriSpan = this.Uri.OriginalString.AsSpan();
            var fragmentSpan = this.Uri.Fragment.AsSpan();
            var baseSpan = uriSpan.Slice(0, uriSpan.Length - fragmentSpan.Length).TrimEnd('?').TrimEnd('&');
            var concat = uriSpan.LastIndexOf('?') < 0 ? '?' : '&';
            var nameSpan = Uri.EscapeDataString(name);
            var valueSpan = string.IsNullOrEmpty(value) ? ReadOnlySpan<char>.Empty : Uri.EscapeDataString(value).AsSpan();

            var builder = new ValueStringBuilder(stackalloc char[256]);
            builder.Append(baseSpan);
            builder.Append(concat);
            builder.Append(nameSpan);
            builder.Append('=');
            builder.Append(valueSpan);
            builder.Append(fragmentSpan);

            this.Uri = new Uri(builder.ToString());
        }

        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return this.Uri.ToString();
        }
    }
}
