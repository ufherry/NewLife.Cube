﻿using System;
using System.Collections.Generic;

namespace NewLife.Web.OAuth
{
    /// <summary>身份验证提供者</summary>
    public class WeixinClient : OAuthClient
    {
        /// <summary>实例化</summary>
        public WeixinClient()
        {
            Server = "https://open.weixin.qq.com/connect/oauth2/";

            AuthUrl = "qrconnect?response_type={response_type}&appid={key}&redirect_uri={redirect}&state={state}&scope={scope}";
            AccessUrl = "https://api.weixin.qq.com/sns/oauth2/access_token?grant_type=authorization_code&appid={key}&secret={secret}&code={code}";
            UserUrl = "https://api.weixin.qq.com/sns/userinfo?access_token={token}&openid={openid}&lang=zh_CN";

            Scope = "snsapi_login";
        }

        /// <summary>应用参数</summary>
        /// <param name="mi"></param>
        public override void Apply(NewLife.Cube.Entity.OAuthConfig mi)
        {
            base.Apply(mi);

            SetMode(Scope);
        }

        /// <summary>设置工作模式</summary>
        /// <param name="mode"></param>
        public virtual void SetMode(String mode)
        {
            switch (mode)
            {
                // 扫码登录
                case "snsapi_login":
                    AuthUrl = "qrconnect?response_type={response_type}&appid={key}&redirect_uri={redirect}&state={state}&scope={scope}";
                    Scope = mode;
                    break;
                // 静默授权，用户无感知
                case "snsapi_base":
                    AuthUrl = "authorize?response_type={response_type}&appid={key}&redirect_uri={redirect}&state={state}&scope={scope}#wechat_redirect";
                    Scope = mode;
                    break;
                // 授权需要用户手动同意
                case "snsapi_userinfo":
                    AuthUrl = "authorize?response_type={response_type}&appid={key}&redirect_uri={redirect}&state={state}&scope={scope}#wechat_redirect";
                    Scope = mode;
                    break;
            }
        }

        /// <summary>是否支持指定用户端，也就是判断是否在特定应用内打开，例如QQ/DingDing/WeiXin</summary>
        /// <param name="userAgent"></param>
        /// <returns></returns>
        public override Boolean Support(String userAgent) => !userAgent.IsNullOrEmpty() && (userAgent.Contains("MICROMESSENGER", StringComparison.OrdinalIgnoreCase) || userAgent.Contains("WINDOWS PHONE", StringComparison.OrdinalIgnoreCase));

        /// <summary>针对指定客户端进行初始化</summary>
        /// <param name="userAgent"></param>
        public override void Init(String userAgent)
        {
            // 应用内打开时，自动切换为应用内免登
            if (Support(userAgent))
            {
                Scope = "snsapi_userinfo";
                SetMode(Scope);
            }
        }

        /// <summary>从响应数据中获取信息</summary>
        /// <param name="dic"></param>
        protected override void OnGetInfo(IDictionary<String, String> dic)
        {
            base.OnGetInfo(dic);

            if (dic.TryGetValue("headimgurl", out var str)) Avatar = str.Trim();
        }
    }
}