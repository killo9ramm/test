using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using System.Net;
using System.Threading;
using System.Data;

using System.Net.Security;
using Config_classes;
using System.IO;

using RBClient.Classes.CustomClasses;

namespace RBClient.Classes
{
    class StaticConstants
    {
        #region global string constants
           public const string TREPORT_FOLDER = "TReport";
           public const int TREPORT_FOLDER_SIZE = 100000000;//100mb

           public const string POSDISPLAY_FOLDER = "posdisplay";
           public const string POSDISPLAY_CONFIG = "rbclientposconfig.xml";
           public const string CONFIGS_FOLDER = "Configs";
           public const string POS_CONFIG_KEY = "folder_tokkm_emark";
           public const string EMARK_FOLDER_NAME = "Emark";

           public const string INNER_CONFIG = "rbclientinnerconfig.xml";

           public const string ADVIMAGE_CONFIG = "rbclientadvimageconfig.xml";
           public const string ADV_IMAGE_CONFIG_KEY = "adv_image_emark_path";

           public const string ADVVIDEO_CONFIG = "rbclientadvvideoconfig.xml";
           public const string ADV_VIDEO_CONFIG_KEY = "adv_video_emark_path";

           public const string Z_BACK_FOLDER = "ztemp";
           public const string TEMP_FOLDER = "temp";
           public const int Z_BACK_FOLDER_SIZE = 50000000;

           public const string OUTBOX_FOLDER = "outbox";

           public const string LOCAL_KILLER_DIRECTORY = @"prkiller";
           public const string KILLER_PATH = @"prkiller\ProcessKillerLauncher.exe";
           public const string KILLER_CONFIG_NAME = @"Config.xml";
           public const string KILLER_INC_NAME = "ProcessKillerLauncher.exe.lnk";

           public const string KILLER_STOP_FLAG_NAME = "killer_flag";
           public const string KILLER_START_FLAG_NAME = "starter_flag";
           public const string KILLER_START_FILE_PATH = "starter_name";
           public const string KILLER_PROCESS_NAME = "process_name";
           public const string KILLER_TIMEOUT_MS_VALUE = "1000";
           public const string KILLER_TIMEOUT_MS_NAME = "timeout_ms";

           public static string KILLER_OLD_TIMEOUT_PERIOD = "1000";    

           public const string AUTO_LOAD_PATH_XP = @"Documents and Settings\All Users\Главное меню\Программы\Автозагрузка";

           public const string DATA_FOLDER = "Data";
           public const int DEFAULT_DB_BACK_COUNT = 3;

           public const string INSTALL_PACKAGE_FOLDER = "InstallPackages";
           public const string INSTALL_PACKAGE_CONFIG = "config.xml";
           public const string TRASH_FOLDER = "Trash";

           public const string EDUC_VIDEO_FOLDER = "video";
           public const string ADV_VIDEO_FOLDER = "ADV_VIDEO";

        #endregion
    }
}
