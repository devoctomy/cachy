using System;

namespace cachy.Fonts
{

    public static class CachyFont
    {

        #region public enums

        public enum Glyph
        {
            None = 0,
            Locker = 1,
            Login_01 = 2,
            User_Login = 3,
            Add_New = 4,
            Recycle_Bin = 5,
            Command_Undo = 6,
            New = 7,
            In = 8,
            Check = 9,
            Lock = 10,
            Up_Arrow_01 = 11,
            Expander_Down_01 = 12,
            Circle_Remove_01 = 13,
            Circle_Add_03 = 14,
            Google_Drive = 15,
            Gmail = 16,
            Android = 17,
            Google_Search = 18,
            Chrome = 19,
            YouTube1 = 20,
            Google_Talk = 21,
            Google_Plus = 22,
            Visual_Studio = 23,
            New_Microsoft = 24,
            Hotmail = 25,
            Apple = 26,
            Mac_PC = 27,
            I_Phone = 28,
            Device_Tablet = 29,
            E_Book = 30,
            Security_Group_Mem_WF = 31,
            Server_01 = 32,
            Network_Sharing = 33,
            Database_Connection = 34,
            Data_Information = 35,
            Database_WF = 36,
            User_Unlocked_02 = 37,
            User_Globe_WF = 38,
            Earth_WF = 39,
            Earth_Node = 40,
            Facebook = 41,
            Twitter = 42,
            Bird = 43,
            Twitter_Bird = 44,
            Pinterest = 45,
            Boy_01 = 46,
            Infant = 47,
            Nurse_01 = 48,
            Nurse = 49,
            Ninja_02 = 50,
            Beaker = 51,
            Audit = 52,
            Armour = 53,
            Menu = 54,
            Settings_02 = 55,
            Help = 56,
            Palette_01 = 57,
            Card_Image = 58,
            Link_WF = 59,
            Drop_Box = 60,
            Cloud_Download = 61,
            Cloud_Upload = 62,
            Warning_Message = 63,
            Cloud = 64,
            Cloud_Private_02 = 65,
            Onedrive = 66,
            Folder_01 = 67,
            Folder_Open = 68,
            Cloud_Recycle = 69,
            Data_Import = 70,
            Text_Highlight_Colour = 71,
            Data_Replace = 72,
            Export = 73,
            Show = 74,
            Tag_01 = 75,
            Amazon_01 = 76,
            Principal_02 = 77,
            Password_03_WF = 78,
            Paste = 79,
            Save = 80,
            Synchronize = 81,
            Love_01 = 82,
            New_Report = 83,
            Tools = 84
        }

        #endregion

        #region public constants

        public const String CACHYFONT_GLYPH_LOCKER = "\ue700";
        public const String CACHYFONT_GLYPH_LOGIN_01 = "\ue701";
        public const String CACHYFONT_GLYPH_USER_LOGIN = "\ue702";
        public const String CACHYFONT_GLYPH_ADD_NEW = "\ue703";
        public const String CACHYFONT_GLYPH_RECYCLE_BIN = "\ue704";
        public const String CACHYFONT_GLYPH_COMMAND_UNDO = "\ue705";
        public const String CACHYFONT_GLYPH_NEW = "\ue706";
        public const String CACHYFONT_GLYPH_IN = "\ue707";
        public const String CACHYFONT_GLYPH_CHECK = "\ue708";
        public const String CACHYFONT_GLYPH_LOCK = "\ue709";
        public const String CACHYFONT_GLYPH_UP_ARROW_01 = "\ue70a";
        public const String CACHYFONT_GLYPH_EXPANDER_DOWN_01 = "\ue70b";
        public const String CACHYFONT_GLYPH_CIRCLE_REMOVE_01 = "\ue70c";
        public const String CACHYFONT_GLYPH_CIRCLE_ADD_03 = "\ue70d";
        public const String CACHYFONT_GLYPH_GOOGLE_DRIVE = "\ue70e";
        public const String CACHYFONT_GLYPH_GMAIL = "\ue70f";
        public const String CACHYFONT_GLYPH_ANDROID = "\ue710";
        public const String CACHYFONT_GLYPH_GOOGLE_SEARCH = "\ue711";
        public const String CACHYFONT_GLYPH_CHROME = "\ue712";
        public const String CACHYFONT_GLYPH_YOUTUBE1 = "\ue713";
        public const String CACHYFONT_GLYPH_GOOGLE_TALK = "\ue714";
        public const String CACHYFONT_GLYPH_GOOGLE_PLUS = "\ue715";
        public const String CACHYFONT_GLYPH_VISUAL_STUDIO = "\ue716";
        public const String CACHYFONT_GLYPH_NEW_MICROSOFT = "\ue717";
        public const String CACHYFONT_GLYPH_HOTMAIL = "\ue718";
        public const String CACHYFONT_GLYPH_APPLE = "\ue719";
        public const String CACHYFONT_GLYPH_MAC_PC = "\ue71a";
        public const String CACHYFONT_GLYPH_I_PHONE = "\ue71b";
        public const String CACHYFONT_GLYPH_DEVICE_TABLET = "\ue71c";
        public const String CACHYFONT_GLYPH_E_BOOK = "\ue71d";
        public const String CACHYFONT_GLYPH_SECURITY_GROUP_MEM_WF = "\ue71e";
        public const String CACHYFONT_GLYPH_SERVER_01 = "\ue71f";
        public const String CACHYFONT_GLYPH_NETWORK_SHARING = "\ue720";
        public const String CACHYFONT_GLYPH_DATABASE_CONNECTION = "\ue721";
        public const String CACHYFONT_GLYPH_DATA_INFORMATION = "\ue722";
        public const String CACHYFONT_GLYPH_DATABASE_WF = "\ue723";
        public const String CACHYFONT_GLYPH_USER_UNLOCKED_02 = "\ue724";
        public const String CACHYFONT_GLYPH_USER_GLOBE_WF = "\ue725";
        public const String CACHYFONT_GLYPH_EARTH_WF = "\ue726";
        public const String CACHYFONT_GLYPH_EARTH_NODE = "\ue727";
        public const String CACHYFONT_GLYPH_FACEBOOK = "\ue728";
        public const String CACHYFONT_GLYPH_TWITTER = "\ue729";
        public const String CACHYFONT_GLYPH_BIRD = "\ue72a";
        public const String CACHYFONT_GLYPH_TWITTER_BIRD = "\ue72b";
        public const String CACHYFONT_GLYPH_PINTEREST = "\ue72c";
        public const String CACHYFONT_GLYPH_BOY_01 = "\ue72d";
        public const String CACHYFONT_GLYPH_INFANT = "\ue72e";
        public const String CACHYFONT_GLYPH_NURSE_01 = "\ue72f";
        public const String CACHYFONT_GLYPH_NURSE = "\ue730";
        public const String CACHYFONT_GLYPH_NINJA_02 = "\ue731";
        public const String CACHYFONT_GLYPH_BEAKER = "\ue732";
        public const String CACHYFONT_GLYPH_AUDIT = "\ue733";
        public const String CACHYFONT_GLYPH_ARMOUR = "\ue734";
        public const String CACHYFONT_GLYPH_MENU = "\ue735";
        public const String CACHYFONT_GLYPH_SETTINGS_02 = "\ue736";
        public const String CACHYFONT_GLYPH_HELP = "\ue737";
        public const String CACHYFONT_GLYPH_PALETTE_01 = "\ue738";
        public const String CACHYFONT_GLYPH_CARD_IMAGE = "\ue739";
        public const String CACHYFONT_GLYPH_LINK_WF = "\ue73a";
        public const String CACHYFONT_GLYPH_DROP_BOX = "\ue73b";
        public const String CACHYFONT_GLYPH_CLOUD_DOWNLOAD = "\ue73c";
        public const String CACHYFONT_GLYPH_CLOUD_UPLOAD = "\ue73d";
        public const String CACHYFONT_GLYPH_WARNING_MESSAGE = "\ue73e";
        public const String CACHYFONT_GLYPH_CLOUD = "\ue73f";
        public const String CACHYFONT_GLYPH_CLOUD_PRIVATE_02 = "\ue740";
        public const String CACHYFONT_GLYPH_ONEDRIVE = "\ue741";
        public const String CACHYFONT_GLYPH_FOLDER_01 = "\ue742";
        public const String CACHYFONT_GLYPH_FOLDER_OPEN = "\ue743";
        public const String CACHYFONT_GLYPH_CLOUD_RECYCLE = "\ue744";
        public const String CACHYFONT_GLYPH_DATA_IMPORT = "\ue745";
        public const String CACHYFONT_GLYPH_TEXT_HIGHLIGHT_COLOUR = "\ue746";
        public const String CACHYFONT_GLYPH_DATA_REPLACE = "\ue747";
        public const String CACHYFONT_GLYPH_EXPORT = "\ue748";
        public const String CACHYFONT_GLYPH_SHOW = "\ue749";
        public const String CACHYFONT_GLYPH_TAG_01 = "\ue74a";
        public const String CACHYFONT_GLYPH_AMAZON_01 = "\ue74b";
        public const String CACHYFONT_GLYPH_PRINCIPAL_02 = "\ue74c";
        public const String CACHYFONT_GLYPH_PASSWORD_03_WF = "\ue74d";
        public const String CACHYFONT_GLYPH_PASTE = "\ue74e";
        public const String CACHYFONT_GLYPH_SAVE = "\ue74f";
        public const String CACHYFONT_GLYPH_SYNCHRONIZE = "\ue750";
        public const String CACHYFONT_GLYPH_LOVE_01 = "\ue751";
        public const String CACHYFONT_GLYPH_NEW_REPORT = "\ue752";
        public const String CACHYFONT_GLYPH_TOOLS = "\ue753";

        #endregion

        #region public methods

        public static String GetString(Glyph glyph)
        {
            switch(glyph)
            {
                case Glyph.None:
                    {
                        return (String.Empty);
                    }
                case Glyph.Locker:
                    {
                        return (CACHYFONT_GLYPH_LOCKER);
                    }
                case Glyph.Login_01:
                    {
                        return (CACHYFONT_GLYPH_LOGIN_01);
                    }
                case Glyph.User_Login:
                    {
                        return (CACHYFONT_GLYPH_USER_LOGIN);
                    }
                case Glyph.Add_New:
                    {
                        return (CACHYFONT_GLYPH_ADD_NEW);
                    }
                case Glyph.Recycle_Bin:
                    {
                        return (CACHYFONT_GLYPH_RECYCLE_BIN);
                    }
                case Glyph.Command_Undo:
                    {
                        return (CACHYFONT_GLYPH_COMMAND_UNDO);
                    }
                case Glyph.New:
                    {
                        return (CACHYFONT_GLYPH_NEW);
                    }
                case Glyph.In:
                    {
                        return (CACHYFONT_GLYPH_IN);
                    }
                case Glyph.Check:
                    {
                        return (CACHYFONT_GLYPH_CHECK);
                    }
                case Glyph.Lock:
                    {
                        return (CACHYFONT_GLYPH_LOCK);
                    }
                case Glyph.Up_Arrow_01:
                    {
                        return (CACHYFONT_GLYPH_UP_ARROW_01);
                    }
                case Glyph.Expander_Down_01:
                    {
                        return (CACHYFONT_GLYPH_EXPANDER_DOWN_01);
                    }
                case Glyph.Circle_Remove_01:
                    {
                        return (CACHYFONT_GLYPH_CIRCLE_REMOVE_01);
                    }
                case Glyph.Circle_Add_03:
                    {
                        return (CACHYFONT_GLYPH_CIRCLE_ADD_03);
                    }
                case Glyph.Google_Drive:
                    {
                        return (CACHYFONT_GLYPH_GOOGLE_DRIVE);
                    }
                case Glyph.Gmail:
                    {
                        return (CACHYFONT_GLYPH_GMAIL);
                    }
                case Glyph.Android:
                    {
                        return (CACHYFONT_GLYPH_ANDROID);
                    }
                case Glyph.Google_Search:
                    {
                        return (CACHYFONT_GLYPH_GOOGLE_SEARCH);
                    }
                case Glyph.Chrome:
                    {
                        return (CACHYFONT_GLYPH_CHROME);
                    }
                case Glyph.YouTube1:
                    {
                        return (CACHYFONT_GLYPH_YOUTUBE1);
                    }
                case Glyph.Google_Talk:
                    {
                        return (CACHYFONT_GLYPH_GOOGLE_TALK);
                    }
                case Glyph.Google_Plus:
                    {
                        return (CACHYFONT_GLYPH_GOOGLE_PLUS);
                    }
                case Glyph.Visual_Studio:
                    {
                        return (CACHYFONT_GLYPH_VISUAL_STUDIO);
                    }
                case Glyph.New_Microsoft:
                    {
                        return (CACHYFONT_GLYPH_NEW_MICROSOFT);
                    }
                case Glyph.Hotmail:
                    {
                        return (CACHYFONT_GLYPH_HOTMAIL);
                    }
                case Glyph.Apple:
                    {
                        return (CACHYFONT_GLYPH_APPLE);
                    }
                case Glyph.Mac_PC:
                    {
                        return (CACHYFONT_GLYPH_MAC_PC);
                    }
                case Glyph.I_Phone:
                    {
                        return (CACHYFONT_GLYPH_I_PHONE);
                    }
                case Glyph.Device_Tablet:
                    {
                        return (CACHYFONT_GLYPH_DEVICE_TABLET);
                    }
                case Glyph.E_Book:
                    {
                        return (CACHYFONT_GLYPH_E_BOOK);
                    }
                case Glyph.Security_Group_Mem_WF:
                    {
                        return (CACHYFONT_GLYPH_SECURITY_GROUP_MEM_WF);
                    }
                case Glyph.Server_01:
                    {
                        return (CACHYFONT_GLYPH_SERVER_01);
                    }
                case Glyph.Network_Sharing:
                    {
                        return (CACHYFONT_GLYPH_NETWORK_SHARING);
                    }
                case Glyph.Database_Connection:
                    {
                        return (CACHYFONT_GLYPH_DATABASE_CONNECTION);
                    }
                case Glyph.Data_Information:
                    {
                        return (CACHYFONT_GLYPH_DATA_INFORMATION);
                    }
                case Glyph.Database_WF:
                    {
                        return (CACHYFONT_GLYPH_DATABASE_WF);
                    }
                case Glyph.User_Unlocked_02:
                    {
                        return (CACHYFONT_GLYPH_USER_UNLOCKED_02);
                    }
                case Glyph.User_Globe_WF:
                    {
                        return (CACHYFONT_GLYPH_USER_GLOBE_WF);
                    }
                case Glyph.Earth_WF:
                    {
                        return (CACHYFONT_GLYPH_EARTH_WF);
                    }
                case Glyph.Earth_Node:
                    {
                        return (CACHYFONT_GLYPH_EARTH_NODE);
                    }
                case Glyph.Facebook:
                    {
                        return (CACHYFONT_GLYPH_FACEBOOK);
                    }
                case Glyph.Twitter:
                    {
                        return (CACHYFONT_GLYPH_TWITTER);
                    }
                case Glyph.Bird:
                    {
                        return (CACHYFONT_GLYPH_BIRD);
                    }
                case Glyph.Twitter_Bird:
                    {
                        return (CACHYFONT_GLYPH_TWITTER_BIRD);
                    }
                case Glyph.Pinterest:
                    {
                        return (CACHYFONT_GLYPH_PINTEREST);
                    }
                case Glyph.Boy_01:
                    {
                        return (CACHYFONT_GLYPH_BOY_01);
                    }
                case Glyph.Infant:
                    {
                        return (CACHYFONT_GLYPH_INFANT);
                    }
                case Glyph.Nurse_01:
                    {
                        return (CACHYFONT_GLYPH_NURSE_01);
                    }
                case Glyph.Nurse:
                    {
                        return (CACHYFONT_GLYPH_NURSE);
                    }
                case Glyph.Ninja_02:
                    {
                        return (CACHYFONT_GLYPH_NINJA_02);
                    }
                case Glyph.Beaker:
                    {
                        return (CACHYFONT_GLYPH_BEAKER);
                    }
                case Glyph.Audit:
                    {
                        return (CACHYFONT_GLYPH_AUDIT);
                    }
                case Glyph.Armour:
                    {
                        return (CACHYFONT_GLYPH_ARMOUR);
                    }
                case Glyph.Menu:
                    {
                        return (CACHYFONT_GLYPH_MENU);
                    }
                case Glyph.Settings_02:
                    {
                        return (CACHYFONT_GLYPH_SETTINGS_02);
                    }
                case Glyph.Help:
                    {
                        return (CACHYFONT_GLYPH_HELP);
                    }
                case Glyph.Palette_01:
                    {
                        return (CACHYFONT_GLYPH_PALETTE_01);
                    }
                case Glyph.Card_Image:
                    {
                        return (CACHYFONT_GLYPH_CARD_IMAGE);
                    }
                case Glyph.Link_WF:
                    {
                        return (CACHYFONT_GLYPH_LINK_WF);
                    }
                case Glyph.Drop_Box:
                    {
                        return (CACHYFONT_GLYPH_DROP_BOX);
                    }
                case Glyph.Cloud_Download:
                    {
                        return (CACHYFONT_GLYPH_CLOUD_DOWNLOAD);
                    }
                case Glyph.Cloud_Upload:
                    {
                        return (CACHYFONT_GLYPH_CLOUD_UPLOAD);
                    }
                case Glyph.Warning_Message:
                    {
                        return (CACHYFONT_GLYPH_WARNING_MESSAGE);
                    }
                case Glyph.Cloud:
                    {
                        return (CACHYFONT_GLYPH_CLOUD);
                    }
                case Glyph.Cloud_Private_02:
                    {
                        return (CACHYFONT_GLYPH_CLOUD_PRIVATE_02);
                    }
                case Glyph.Onedrive:
                    {
                        return (CACHYFONT_GLYPH_ONEDRIVE);
                    }
                case Glyph.Folder_01:
                    {
                        return (CACHYFONT_GLYPH_FOLDER_01);
                    }
                case Glyph.Folder_Open:
                    {
                        return (CACHYFONT_GLYPH_FOLDER_OPEN);
                    }
                case Glyph.Cloud_Recycle:
                    {
                        return (CACHYFONT_GLYPH_CLOUD_RECYCLE);
                    }
                case Glyph.Data_Import:
                    {
                        return (CACHYFONT_GLYPH_DATA_IMPORT);
                    }
                case Glyph.Text_Highlight_Colour:
                    {
                        return (CACHYFONT_GLYPH_TEXT_HIGHLIGHT_COLOUR);
                    }
                case Glyph.Data_Replace:
                    {
                        return (CACHYFONT_GLYPH_DATA_REPLACE);
                    }
                case Glyph.Export:
                    {
                        return (CACHYFONT_GLYPH_EXPORT);
                    }
                case Glyph.Show:
                    {
                        return (CACHYFONT_GLYPH_SHOW);
                    }
                case Glyph.Tag_01:
                    {
                        return (CACHYFONT_GLYPH_TAG_01);
                    }
                case Glyph.Amazon_01:
                    {
                        return (CACHYFONT_GLYPH_AMAZON_01);
                    }
                case Glyph.Principal_02:
                    {
                        return (CACHYFONT_GLYPH_PRINCIPAL_02);
                    }
                case Glyph.Password_03_WF:
                    {
                        return (CACHYFONT_GLYPH_PASSWORD_03_WF);
                    }
                case Glyph.Paste:
                    {
                        return (CACHYFONT_GLYPH_PASTE);
                    }
                case Glyph.Save:
                    {
                        return (CACHYFONT_GLYPH_SAVE);
                    }
                case Glyph.Synchronize:
                    {
                        return (CACHYFONT_GLYPH_SYNCHRONIZE);
                    }
                case Glyph.Love_01:
                    {
                        return (CACHYFONT_GLYPH_LOVE_01);
                    }
                case Glyph.New_Report:
                    {
                        return (CACHYFONT_GLYPH_NEW_REPORT);
                    }
                case Glyph.Tools:
                    {
                        return (CACHYFONT_GLYPH_TOOLS);
                    }
                default:
                    {
                        throw new NotImplementedException(String.Format("Glyph '{0}' to String has not been implemented.", glyph));
                    }
            }
        }

        #endregion

    }

}
