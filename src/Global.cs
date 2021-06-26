/**
 *  RedRP Gamemode
 *  
 *  Author: Adrià Alberich (Atunero) (alberichjaumeadria@gmail.com / atunerin@gmail.com)
 *  Copyright(c) Adrià Alberich (Atunero) (MIT License)
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GTANetworkAPI;

namespace redrp
{
    /// <summary>
    /// Contains gamemode global variables, enums and constants
    /// </summary>
    public class Global : Script
    {

        /// <summary>
        /// Server status variable, it can be -1 if not initialized yet, 0 if lost DB connection or 1 if everything is OK
        /// </summary>
        public static int ServerStatus = -1;

        /// <summary>
        /// Gamemode name
        /// </summary>
        public const string ScriptName = "RedRP";

        /// <summary>
        /// Gamemode version
        /// </summary>
        public const string ScriptVersion = "1.0";

        /// <summary>
        /// Database connection string
        /// </summary>
        public const string ConnectionString =
            "Server=localhost;" +           //MySQL server
            "Uid=root;" +                   //MySQL user
            "Pwd=;" +                       //MySQL password
            "Database=redrp;";              //MySQL database name

        /// <summary>
        /// Login autokick system delay
        /// </summary>
        public const int LoginAutokickTime = 60;

        /// <summary>
        /// Anti AFK delay
        /// </summary>
        public const int AntiAfkTime = 300;

        /// <summary>
        /// Anti Ping Limit (in ms)
        /// </summary>
        public const int AntiPingLimit = 400;

        /// <summary>
        /// Anti Ping max warnings before kick
        /// </summary>
        public const int AntiPingMaxWarns = 3;

        /// <summary>
        /// Death time (delay until respawn)
        /// </summary>
        public const int DeathTime = 10;

        /// <summary>
        /// Max pocket money
        /// </summary>
        public const int PocketMoneyLimit = 100000;

        /// <summary>
        /// Command error message
        /// </summary>
        public const string CmdErrorMessage = "~r~Error: ~w~comando inexistente, usa ~y~F2~w~ para obtener ayuda.";

        /// <summary>
        /// Sex types (multisystem)
        /// </summary>
        public enum Sex
        {
            Male,
            Female
        }

        // GLOBAL ENTITY LISTS

        /// <summary>
        /// Admin teleports
        /// </summary>
        public static List<AdminTeleport> AdminTeleports;

        /// <summary>
        /// Admin reports
        /// </summary>
        public static List<AdminReport> AdminReports;

        /// <summary>
        /// Animation global list
        /// </summary>
        public static List<Animation> Animations;

        /// <summary>
        /// Global list of animation categories
        /// </summary>
        public static List<AnimationCategory> AnimationCategories;

        /// <summary>
        /// Public list of languages
        /// </summary>
        public static List<Language> Languages;

        /// <summary>
        /// List of loaded bank accounts
        /// </summary>
        public static List<BankAccount> BankAccounts;

        /// <summary>
        /// Public list of ATMs
        /// </summary>
        public static List<ATM> ATMs;

        /// <summary>
        /// Public list of item's data
        /// </summary>
        public static List<ItemData> ItemsData;

        /// <summary>
        /// Public list of world items
        /// </summary>
        public static List<WorldItem> WorldItems;

        /// <summary>
        /// Public list of garbage containers
        /// </summary>
        public static List<Container> Containers;

        /// <summary>
        /// Public list of inventories
        /// </summary>
        public static List<Inventory> Inventories;

        // CHAT

        /// <summary>
        /// Channel list
        /// </summary>
        public enum ChatChannel : int
        {
            Normal,
            Whisper,
            Shout,
            Ooc,
            Action,
            Environment
        }

        /// <summary>
        /// Channel distances
        /// </summary>
        public static Dictionary<int, double> ChatChannelDistance = new Dictionary<int, double>
        {
            {(int)ChatChannel.Normal, 10.0},
            {(int)ChatChannel.Whisper, 5.0},
            {(int)ChatChannel.Shout, 30.0},
            {(int)ChatChannel.Ooc,  10.0},
            {(int)ChatChannel.Action, 30.0},
            {(int)ChatChannel.Environment, 30.0},
        };

        /// <summary>
        /// Channel base colors
        /// </summary>
        public static Dictionary<int, string> ChatChannelColors = new Dictionary<int, string>
        {
            {(int)ChatChannel.Normal, ChatNormalColor},
            {(int)ChatChannel.Whisper, ChatNormalColor},
            {(int)ChatChannel.Shout, ChatNormalColor},
            {(int)ChatChannel.Ooc,  ChatNormalColor},
            {(int)ChatChannel.Action, ChatActionColor},
            {(int)ChatChannel.Environment, ChatEnvironmentColor},
        };

        /// <summary>
        /// Max chat line length
        /// </summary>
        public const int MaxChatLineLength = 85;

        /// <summary>
        /// Administrator hex color
        /// </summary>
        public const string AdministratorColor = "#B40404";

        /// <summary>
        /// Game Master hex color
        /// </summary>
        public const string GameMasterColor = "#0B610B";

        /// <summary>
        /// Collaborator hex color
        /// </summary>
        public const string CollaboratorColor = "#4B088A";

        /// <summary>
        /// Green hex color
        /// </summary>
        public const string GreenColor = "#00ff00";

        /// <summary>
        /// Red hex color
        /// </summary>
        public const string RedColor = "#ff0000";

        /// <summary>
        /// Chat normal color
        /// </summary>
        public const string ChatNormalColor = "#FFFFFF";

        /// <summary>
        /// Chat gradient color 1
        /// </summary>
        public const string ChatGradientColor1 = "#FFFFFF";

        /// <summary>
        /// Chat gradient color 2
        /// </summary>
        public const string ChatGradientColor2 = "#D8D8D8";

        /// <summary>
        /// Chat gradient color 3
        /// </summary>
        public const string ChatGradientColor3 = "#BDBDBD";

        /// <summary>
        /// Chat gradient color 4
        /// </summary>
        public const string ChatGradientColor4 = "#A4A4A4";

        /// <summary>
        /// Chat gradient color 5
        /// </summary>
        public const string ChatGradientColor5 = "#848484";

        /// <summary>
        /// Chat action color
        /// </summary>
        public const string ChatActionColor = "#C2A2DA";

        /// <summary>
        /// Chat environment color
        /// </summary>
        public const string ChatEnvironmentColor = "#9ACD32";

        /// <summary>
        /// Chat admin color
        /// </summary>
        public const string ChatAdminColor = "#F17F7F";

        /// <summary>
        /// Chat global color
        /// </summary>
        public const string ChatGlobalColor = "#01A9DB";

        /// <summary>
        /// Chat PM color
        /// </summary>
        public const string ChatPMColor = "#F7FE2E";

        /// <summary>
        /// Chat PM received color
        /// </summary>
        public const string ChatPMReceivedColor = "#F3F781";

        /// <summary>
        /// YO description color
        /// </summary>
        public const string YoColor = "#FE2E2E";

        /// <summary>
        /// AME message max length
        /// </summary>
        public const int AmeLength = 64;

        /// <summary>
        /// AME message duration in seconds
        /// </summary>
        public const int AmeDuration = 10;

        /// <summary>
        /// AME max visible range
        /// </summary>
        public const float AmeRange = 10f;

        /// <summary>
        /// YO description max length
        /// </summary>
        public const int YoLength = 128;

        /// <summary>
        /// YO description max range
        /// </summary>
        public const float YoRange = 20f;

        // INTERACTION

        /// <summary>
        /// External interaction front vector distance
        /// </summary>
        public const float ExternalInteractionPlayerVectorDistance = 0.5f;

        /// <summary>
        /// External interaction general range
        /// </summary>
        public const float ExternalInteractionPlayerDistance = 2.0f;

        /// <summary>
        /// External interaction vehicle range
        /// </summary>
        public const float ExternalInteractionVehicleDistance = 5.0f;

        /// <summary>
        /// Deal system expire time in seconds
        /// </summary>
        public const int DealExpiringTime = 20;

        /// <summary>
        /// Deal system max distance before autocancel
        /// </summary>
        public const float DealMaxDistance = 2.0f;

        /// <summary>
        /// Deal types
        /// </summary>
        public enum Deals
        {
            SoftCuff,
            HardCuff,
            Search,
            Rob
        }

        // ADMIN

        /// <summary>
        /// Max admin reports
        /// </summary>
        public const int MaxAdminReports = 20;

        /// <summary>
        /// Max admin report lifetime
        /// </summary>
        public const int MaxAdminReportLifetime = 300;

        // BANKING 

        /// <summary>
        /// Max money that can be withrawed from an ATM per hour
        /// </summary>
        public const int MaxWithrawMoneyPerHour = 5000;

        /// <summary>
        /// Bank account types
        /// </summary>
        public enum AccountTypes
        {
            Checking,
            Saving,
            Deposit
        }

        /// <summary>
        /// Bank account owner types
        /// </summary>
        public enum AccountOwnerTypes
        {
            Player,
            Business,
            Organization
        }

        /// <summary>
        /// Bank account movement types
        /// </summary>
        public enum AccountMovementType
        {
            Deposit,
            Withdraw,
            Transfer
        }

        /// <summary>
        /// Base comission per withraw
        /// </summary>
        public static Dictionary<int, double> baseWithdrawComission = new Dictionary<int, double>
        {
            {(int)AccountTypes.Checking, 0.0},
            {(int)AccountTypes.Saving, 1.0},
            {(int)AccountTypes.Deposit, 5.0}
        };

        /// <summary>
        /// Comission for transferring money to a bank account of the same bank
        /// </summary>
        public static Dictionary<int, double> baseTransferComissionSameBank = new Dictionary<int, double>
        {
            {(int)AccountTypes.Checking, 0.0},
            {(int)AccountTypes.Saving, 1.0},
            {(int)AccountTypes.Deposit, 5.0}
        };

        /// <summary>
        /// Comission for transferring money to a bank account of another bank
        /// </summary>
        public static Dictionary<int, double> baseTransferComissionDifferentBank = new Dictionary<int, double>
        {
            {(int)AccountTypes.Checking, 2.0},
            {(int)AccountTypes.Saving, 4.0},
            {(int)AccountTypes.Deposit, 8.0}
        };

        /// <summary>
        /// Base account interests per hour
        /// </summary>
        public static Dictionary<int, double> baseInterests = new Dictionary<int, double>
        {
            {(int)AccountTypes.Checking, 0.0},
            {(int)AccountTypes.Saving, 0.01},
            {(int)AccountTypes.Deposit, 0.02}
        };

        /// <summary>
        /// Base maintenance fee per hour
        /// </summary>
        public static Dictionary<int, int> baseMaintenanceFee = new Dictionary<int, int>
        {
            {(int)AccountTypes.Checking, 20},
            {(int)AccountTypes.Saving, 10},
            {(int)AccountTypes.Deposit, 0}
        };

        /// <summary>
        /// Account blocking time
        /// </summary>
        public static Dictionary<int, int> blockingTime = new Dictionary<int, int>
        {
            {(int)AccountTypes.Checking, 0},
            {(int)AccountTypes.Saving, 0},
            {(int)AccountTypes.Deposit, 24}
        };

        // ATMs

        /// <summary>
        /// ATM interaction distance for external interaction system
        /// </summary>
        public const double AtmInteractionDistance = 1.0;

        /// <summary>
        /// ATM max history registers for the history interface
        /// </summary>
        public const int AtmMaxHistoryRegisters = 50;

        // INVENTORY

        /// <summary>
        /// Player special slots
        /// </summary>
        public const int PlayerSpecialSlotsCount = 15;

        /// <summary>
        /// World items max distance detection
        /// </summary>
        public const float WorldItemsMaxDistanceDetection = 1.5f;

        /// <summary>
        /// Item max finger prints
        /// </summary>
        public const int ItemMaxFingerPrints = 10;

        /// <summary>
        /// Item types
        /// </summary>
        public enum ItemType
        {
            None,
            MeleeWeapon,
            FireWeapon,
            ThrowableWeapon,
            SpecialWeapon,
            WeaponAccessory,
            AmmoCrate,
            Drug,
            Food,
            Drink,
            Container,
            Tool,
            ElectronicDevice,
            Book,
            Backpack,
            Bodyarmor,
            Gloves,
            Glasses,
            Hat,
            Mask,
            Accessory,
            EarAccessory,
            TorsoClothes,
            LegClothes,
            Shoes,
            Watch,
            Bracelet,
            Spray,
            Chemical,
            Card,
            Toy
        }

        /// <summary>
        /// Inventory types
        /// </summary>
        public enum InventoryType
        {
            Player,
            VehicleTrunk,
            VehicleGlover,
            House,
            Business,
            GarbageContainer
        }

        /// <summary>
        /// Player body parts
        /// </summary>
        public enum InventoryBodypart
        {
            RightHand,
            LeftHand,
            Bodyarmor, // Bodyarmor
            Backpack, // Backpacks
            Gloves, // Gloves slot
            Hat, // Hats, helmets slot
            Glasses, //G lasses slot
            Mask, // Balaclava, masks, etc
            Accessory, // Accessory
            Ears, // Ear rings, etc
            Torso, // Torso clothes
            Legs, // Trausers
            Feet, // Shoes
            Watch, 
            Bracelet, 
            HeavyWeapon,
            MeleeWeapon,
        }

        /// <summary>
        /// Player looting options
        /// </summary>
        public enum InventoryLootingOptions
        {
            GetMoney,
            RobMoney,
            RightHand,
            Lefthand,
            LightWeapon1,
            LightWeapon2,
            LightMeleeWeapon1,
            LightMeleeWeapon2,
            HeavyWeapon,
            HeavyMeleeWeapon,
            ThrowableWeapon1,
            ThrowableWeapon2,
            SpecialWeapon,
            Bodyarmor, // Bodyarmor
            Backpack, // Backpacks
            Gloves, // Gloves slot
            Hat, // Hats, helmets slot
            Glasses, // Glasses slot
            Mask, // Balaclava, masks, etc
            Accessory, // Accessory
            Ears, // Ear rings, etc
            Torso, // Torso clothes
            Legs, // Trausers
            Feet, // Shoes
            Watch, 
            Bracelet,
            Pockets,
            Back
        }

        /// <summary>
        /// Default inventories maximum weight (in kg)
        /// </summary>
        public static Dictionary<int, int> InventoryCapacity = new Dictionary<int, int>
        {
            {(int)InventoryType.Player, 30},
            {(int)InventoryType.VehicleTrunk, 20},
            {(int)InventoryType.VehicleGlover, 10},
            {(int)InventoryType.House, 1000},
            {(int)InventoryType.Business, 2000},
            {(int)InventoryType.GarbageContainer, 5000}
        };

        /// <summary>
        /// Inventory can contain heavy items
        /// </summary>
        public static Dictionary<int, bool> InventoryCanContainHeavyItems = new Dictionary<int, bool>
        {
            {(int)InventoryType.Player, false},
            {(int)InventoryType.VehicleTrunk, true},
            {(int)InventoryType.VehicleGlover, false},
            {(int)InventoryType.House, true},
            {(int)InventoryType.Business, true},
            {(int)InventoryType.GarbageContainer, true}
        };

        /// <summary>
        /// Player bodypart bones
        /// </summary>
        public static Dictionary<int, string> PlayerBodypartBones = new Dictionary<int, string>
        {
            {(int)InventoryBodypart.RightHand, "57005"}, // SKEL_R_Hand
            {(int)InventoryBodypart.LeftHand, "18905"}, // SKEL_L_Hand
            {(int)InventoryBodypart.HeavyWeapon, "11816"}, // SKEL_Pelvis
            {(int)InventoryBodypart.MeleeWeapon, "11816"} // SKEL_Pelvis
        };

        // GARBAGE CONTAINERS

        /// <summary>
        /// Maximum garbage for each container
        /// </summary>
        public const int MaxGarbagePerContainer = 1000;

        /// <summary>
        /// Interaction distance for garbage containers
        /// </summary>
        public const double GarbageContainerInteractionDistance = 1.0;

        // MISCELANEOUS STATIC DATA

        /// <summary>
        /// Player walking styles
        /// </summary>
        public static Dictionary<string, string> PlayerWalkingStyles = new Dictionary<string, string>
        {
            {"default", "Normal"},
            {"move_m@intimidation@1h", "Intimidado"},
            {"move_m@sad@a", "Deprimido"},
            {"move_m@drunk@verydrunk", "Bebido"},
            {"move_m@injured", "Herido"},
            {"move_m@brave", "Orgulloso"},
            {"move_f@scared", "Temeroso"},
            {"move_lester_CaneUp", "Viejo"},
            {"move_m@casual@d", "Casual"},
            {"move_m@tool_belt@a", "Duro"},
            {"move_m@fat@a", "Gordo"},
            {"move_m@confident", "Valiente"},
            {"move_m@hurry@a", "Prisa 1"},
            {"move_m@quick", "Prisa 2"},
            {"move_f@sexy@a", "Sexy 1"},
            {"MOVE_F@FEMME@", "Sexy 2"},
            {"move_m@gangster@var_e", "Gangster 1"},
            {"move_m@gangster@var_f", "Gangster 2"},
            {"move_m@gangster@var_i", "Gangster 3"},
            {"MOVE_M@GANGSTER@NG", "Gangster 4"},
            {"MOVE_M@TOUGH_GUY@", "Gangster 5"},
            {"move_m@shadyped@a", "Gangster 6"}
        };

        /// <summary>
        /// Player moods
        /// </summary>
        public static Dictionary<int, string> PlayerMoods = new Dictionary<int, string>
        {
            {0, "Normal"},
            {1, "Apuntar"},
            {2, "Enfadado"},
            {3, "Bebido"},
            {4, "Feliz"},
            {5, "Herido"},
            {6, "Estresado"},
            {7, "Malhumorado"}
        };

    }

}
