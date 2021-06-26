/**
 *  RedRP Gamemode
 *  
 *  Author: Atunero (atunerin@gmail.com)
 *  Copyright(c) Atunero (MIT License)
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
    /// Defines the character data structure
    /// </summary>
    public class Character : Script
    {

        //Character persistent attributes

        /// <summary>
        /// Character unique identifier
        /// </summary>
        public int sqlid { get; set; }

        /// <summary>
        /// Player unique identifier that owns this character
        /// </summary>
        public int ownerSqlid { get; set; }

        /// <summary>
        /// Character raw name with underscore
        /// </summary>
        public string name { get; set; }

        /// <summary>
        /// Character name without underscore
        /// </summary>
        public string cleanName { get; set; }

        /// <summary>
        /// Name that is currently showing ingame
        /// </summary>
        public string showName { get; set; }

        /// <summary>
        /// Total played hours
        /// </summary>
        public int playedHours { get; set; }

        /// <summary>
        /// Total logins (character selections)
        /// </summary>
        public int logins { get; set; }

        /// <summary>
        /// Character's sex
        /// </summary>
        public int sex { get; set; }
        
        /// <summary>
        /// Race (deprecated)
        /// </summary>
        public int race { get; set; }

        /// <summary>
        /// Age
        /// </summary>
        public int age { get; set; }

        /// <summary>
        /// Main predefined skin (deprecated)
        /// </summary>
        public string mainSkin { get; set; }

        /// <summary>
        /// Current position
        /// </summary>
        public Vector3 position { get; set; }

        /// <summary>
        /// Heading angle
        /// </summary>
        public float heading { get; set; }

        /// <summary>
        /// Character dimension
        /// </summary>
        public uint dimension { get; set; }

        /// <summary>
        /// Health value
        /// </summary>
        public int health { get; set; }

        /// <summary>
        /// Death status of the character
        /// </summary>
        public int dying { get; set; }

        /// <summary>
        /// Pocket money value
        /// </summary>
        public int money { get; set; }

        /// <summary>
        /// Acumulated payday time in minutes (when reaches 60 a payday is executed)
        /// </summary>
        public int paydayTime { get; set; }

        /// <summary>
        /// Cuffed status and type
        /// </summary>
        public int cuffed { get; set; }

        /// <summary>
        /// Type of current character mood
        /// </summary>
        public int mood { get; set; }

        /// <summary>
        /// Type of default walking style
        /// </summary>
        public string walkingStyle { get; set; }

        //Character experience

        /// <summary>
        /// Security related experience (police, pmc, military, etc)
        /// </summary>
        public int securityExp { get; set; }

        /// <summary>
        /// Mechanic related experience (repairing cars, systems, etc)
        /// </summary>
        public int mechanicExp { get; set; }

        /// <summary>
        /// Grand theft auto experience (car theft)
        /// </summary>
        public int grandTheftAutoExp { get; set; }

        /// <summary>
        /// General criminal experience (heist related experience)
        /// </summary>
        public int criminalExp { get; set; }

        /// <summary>
        /// Transport related experience
        /// </summary>
        public int transportistExp { get; set; }

        /// <summary>
        /// Taxi experience
        /// </summary>
        public int taxistExp { get; set; }

        /// <summary>
        /// Fishing experience
        /// </summary>
        public int fishingExp { get; set; }

        //Character non-persistent attributes

        /// <summary>
        /// Indicates if the character is hiding his identity or not
        /// </summary>
        public bool hiddenIdentity { get; set; }

        /// <summary>
        /// Indicates if the character has already spawned in the world
        /// </summary>
        public bool spawned { get; set; }

        /// <summary>
        /// Indicates if the character has the private messages enabled or not (this should be on the player!!!)
        /// </summary>
        public bool mp { get; set; }

        /// <summary>
        /// Anti chat flood control variable (also should be on the player...)
        /// </summary>
        public int antiFlood { get; set; }

        /// <summary>
        /// Current character's active language
        /// </summary>
        public Language activeLanguage { get; set; }

        /// <summary>
        /// AME textlabel instance
        /// </summary>
        public TextLabel ameTextLabel { get; set; }

        /// <summary>
        /// AME delay counter
        /// </summary>
        public int ameCounter { get; set; }

        /// <summary>
        /// YO textlabel instance
        /// </summary>
        public TextLabel yoTextLabel { get; set; }

        /// <summary>
        /// Voice type
        /// </summary>
        public int voiceType { get; set; }

        /// <summary>
        /// Respawn counter after death
        /// </summary>
        public int dyingCounter { get; set; }

        /// <summary>
        /// List of bank accounts
        /// </summary>
        public List<BankAccount> bankAccounts { get; set; }

        // DEAL SYSTEM

        /// <summary>
        /// Deal system owner (who sends the deal request)
        /// </summary>
        public Player dealOwner { get; set; }

        /// <summary>
        /// Deal type
        /// </summary>
        public int dealType { get; set; }

        /// <summary>
        /// Deal remaining time before expiration
        /// </summary>
        public int dealRemainingTime { get; set; }

        /// <summary>
        /// Deal general description
        /// </summary>
        public string dealDescription { get; set; }

        // ATM SYSTEM

        /// <summary>
        /// ATM being used
        /// </summary>
        public ATM usingATM { get; set; }

        /// <summary>
        /// Bank account being used in the ATM
        /// </summary>
        public BankAccount usingAccountATM { get; set; }

        /// <summary>
        /// PIN being used in the ATM
        /// </summary>
        public int usingPinATM { get; set; }

        /// <summary>
        /// If character is authenticated or not in the ATM
        /// </summary>
        public bool authenticatedATM { get; set; }

        /// <summary>
        /// Current operation in process in the ATM
        /// </summary>
        public int operationATM { get; set; }

        /// <summary>
        /// Current operation data in process in the ATM
        /// </summary>
        public List<string> operationDataATM { get; set; }

        // INVENTORY

        /// <summary>
        /// Character inventory instance
        /// </summary>
        public Inventory inventory { get; set; }

        /// <summary>
        /// Character other inventory instance (external inventory)
        /// </summary>
        public Inventory otherInventory { get; set; }

        /// <summary>
        /// Item container opened on the left grid (inventory)
        /// </summary>
        public Item leftOpenedContainer { get; set; }

        /// <summary>
        /// World item container opened on the left grid
        /// </summary>
        public WorldItem leftOpenedContainerWorld { get; set; }

        /// <summary>
        /// Item container opened on the right grid (inventory)
        /// </summary>
        public Item rightOpenedContainer { get; set; }

        /// <summary>
        /// World item container opened on the right grid
        /// </summary>
        public List<WorldItem> nearWorldItems { get; set; }
    }
}
