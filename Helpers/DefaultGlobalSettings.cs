using CMS.Areas.SettingManager.Models;
using System;

namespace CMS.Helpers
{
    public static class DefaultGlobalSettings
    {
        private const string _URL = "http://daotao.mitechcenter.vn/";
        private const string _UnitName = "TRƯỜNG ĐẠI HỌC THÔNG TIN LIÊN LẠC";
        private const string _UnitNameEn = "TELECOMMUNICATIONS UNIVERSITY";
        private const string _LogoHeader = ".assets/web/images/huanchuong.png";
        private const string _LogoFooter = ".assets/web/images/logo.png";
        private const string _SiteType = "CỔNG THÔNG TIN ĐIỆN TỬ";
        private const string _Email = "tcu@tsqtt.edu.vn;info@tcu.edu.vn";
        private const string _Address = "Số 101, Mai Xuân Thưởng, Nha Trang, Khánh Hòa";
        private const string _PhoneNumber = "0583-801-805";
        private const string _Fax = "058-832-055";
        private const string _MoreInfo = "® Ghi rõ nguồn <a href=\"http://www.tcu.edu.vn/\" class=\"link\">www.tcu.edu.vn</a> khi phát hành lại <br> thông tin từ <strong> Cổng TTÐT Trường ĐẠI HỌC THÔNG TIN LIÊN LẠC</strong>.";
        private const string _EmbedMapURL = "<iframe src=\"https://www.google.com/maps/embed?pb=!1m18!1m12!1m3!1d15592.901528392935!2d109.19989735!3d12.300603350000001!2m3!1f0!2f0!3f0!3m2!1i1024!2i768!4f13.1!3m3!1m2!1s0x317067fb4e284bd7%3A0x66bee30fdf4d2331!2zVHLGsOG7nW5nIMSQ4bqhaSBo4buNYyBUaMO0bmcgdGluIExpw6puIGzhuqFj!5e0!3m2!1svi!2s!4v1597380845858!5m2!1svi!2s\" width=\"100%\" height=\"452\" frameborder=\"0\" style=\"border:0;\" allowfullscreen=\"\" aria-hidden=\"false\" tabindex=\"0\"></iframe>";
        private const string _ScriptChatBot = "";
        private const string _ScriptGoogleTag = "";
        public static GlobalSetting _DefaultSettings = new GlobalSetting()
        {
            Address = _Address,
            Email = _Email,
            EmbedMapURL = _EmbedMapURL,
            ScriptChatBot = _ScriptChatBot,
            ScriptGoogleTag = _ScriptGoogleTag,
            Fax = _Fax,
            LogoFooter = _LogoFooter,
            LogoHeader = _LogoHeader,
            MoreInfo = _MoreInfo,
            PhoneNumber = _PhoneNumber,
            SiteType = _SiteType,
            UnitName = _UnitName,
            UnitNameEn = _UnitNameEn,
            URL = _URL
        };
        public static GlobalSetting MapSettings(this GlobalSetting globalSetting)
        {
            GlobalSetting mapSettings = new GlobalSetting()
            {
                Address = String.IsNullOrEmpty(globalSetting.Address) ? _Address : globalSetting.Address,
                Email = String.IsNullOrEmpty(globalSetting.Email) ? _Email : globalSetting.Email,
                EmbedMapURL = String.IsNullOrEmpty(globalSetting.EmbedMapURL) ? _EmbedMapURL : globalSetting.EmbedMapURL,
                ScriptChatBot = String.IsNullOrEmpty(globalSetting.ScriptChatBot) ? _ScriptChatBot : globalSetting.ScriptChatBot,
                ScriptGoogleTag = String.IsNullOrEmpty(globalSetting.ScriptGoogleTag) ? _ScriptGoogleTag : globalSetting.ScriptGoogleTag,
                Fax = String.IsNullOrEmpty(globalSetting.Fax) ? _Fax : globalSetting.Fax,
                LogoFooter = String.IsNullOrEmpty(globalSetting.LogoFooter) ? _LogoFooter : globalSetting.LogoFooter,
                LogoHeader = String.IsNullOrEmpty(globalSetting.LogoHeader) ? _LogoHeader : globalSetting.LogoHeader,
                MoreInfo = String.IsNullOrEmpty(globalSetting.MoreInfo) ? _MoreInfo : globalSetting.MoreInfo,
                PhoneNumber = String.IsNullOrEmpty(globalSetting.PhoneNumber) ? _PhoneNumber : globalSetting.PhoneNumber,
                SiteType = String.IsNullOrEmpty(globalSetting.SiteType) ? _SiteType : globalSetting.SiteType,
                UnitName = String.IsNullOrEmpty(globalSetting.UnitName) ? _UnitName : globalSetting.UnitName,
                UnitNameEn = String.IsNullOrEmpty(globalSetting.UnitNameEn) ? _UnitNameEn : globalSetting.UnitNameEn,
                URL = String.IsNullOrEmpty(globalSetting.URL) ? _URL : globalSetting.URL
            };
            return mapSettings;
        }
    }
}
