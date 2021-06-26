SET SQL_MODE="NO_AUTO_VALUE_ON_ZERO";
SET time_zone = "+00:00";

--
-- Database: `redrp`
--

-- --------------------------------------------------------

--
-- redrp_animation_categories table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_animation_categories` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `categoryName` varchar(32) NOT NULL,
  `parent` int(11) NOT NULL DEFAULT '-1',
  PRIMARY KEY (`id`),
  FOREIGN KEY (`parent`) REFERENCES redrp_animation_categories(id)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_animation table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_animation` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `animationName` varchar(32) NOT NULL,
  `scenario` varchar(32) NOT NULL DEFAULT '' COMMENT 'The scenario disables all the other fields',
  `entryMale` varchar(64) NOT NULL COMMENT 'Entry point animation',
  `delayToMainMale` int(11) NOT NULL DEFAULT '0' COMMENT 'Delay from entry animation to the main one',
  `mainMale` varchar(64) NOT NULL COMMENT 'Main animation',
  `delayToEndMale` int(11) NOT NULL DEFAULT '0' COMMENT 'Delay from main animation to the ending one',
  `endMale` varchar(64) NOT NULL COMMENT 'Ending animation',
  `complementaryMale` varchar(64) NOT NULL COMMENT 'Complementary animation',
  `delayComplementaryMale` int(11) NOT NULL DEFAULT '0',
  `directoryMale` varchar(64) NOT NULL COMMENT 'Animation directory',
  `entryFemale` varchar(64) NOT NULL,
  `delayToMainFemale` int(11) NOT NULL DEFAULT '0',
  `mainFemale` varchar(64) NOT NULL,
  `delayToEndFemale` int(11) NOT NULL DEFAULT '0',
  `endFemale` varchar(64) NOT NULL,
  `complementaryFemale` varchar(64) NOT NULL,
  `delayComplementaryFemale` int(11) NOT NULL DEFAULT '0',
  `directoryFemale` varchar(64) NOT NULL,
  `command` varchar(32) NOT NULL,
  `category` int(11) NOT NULL,
  `isLooped` bit NOT NULL DEFAULT b'0',
  `isStopped` bit NOT NULL DEFAULT b'0',
  `isUpperBody` bit NOT NULL DEFAULT b'0',
  `isControllable` bit NOT NULL DEFAULT b'0',
  `isCancellable` bit NOT NULL DEFAULT b'0',
  PRIMARY KEY (`id`),
  FOREIGN KEY (`category`) REFERENCES redrp_animation(id)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_bank_interaction_point table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_bank_interaction_point` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `x` double NOT NULL,
  `y` double NOT NULL,
  `z` double NOT NULL,
  `heading` double NOT NULL,
  `dimension` int(11) NOT NULL,
  `bank` int(11) NOT NULL,
  `hasBlip` bit(1) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8 AUTO_INCREMENT=94;

--
-- redrp_bank_account table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_bank_account` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `number` varchar(32) NOT NULL DEFAULT '',
  `ownerId` int(11) NOT NULL,
  `ownerType` tinyint(4) NOT NULL,
  `bank` tinyint(1) NOT NULL,
  `accountType` tinyint(4) NOT NULL,
  `isPrimary` bit NOT NULL,
  `cash` int(11) NOT NULL DEFAULT 0,
  `debt` int(11) NOT NULL DEFAULT 0,
  `withdrawnCash` int(11) NOT NULL DEFAULT 0,
  `withdrawnCashTimestamp` int(32) NOT NULL,
  `lockedUntil` int(32) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_bank_account_history table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_bank_account_history` (
  `account` int(11) NOT NULL,
  `movementType` tinyint(1) NOT NULL,
  `amount` int(11) NOT NULL,
  `concept` varchar(64) DEFAULT '-',
  `movementTimestamp` int(32) NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`account`, `movementTimestamp`),
  FOREIGN KEY (`account`) REFERENCES redrp_bank_account(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_languages table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_languages` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `fullName` varchar(32) NOT NULL,
  `abreviation` varchar(3) NOT NULL,
  `talk` varchar(32) NOT NULL,
  `whisper` varchar(32) NOT NULL,
  `shout` varchar(32) NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

--
-- redrp_languages data
--

INSERT INTO `Languages` (`fullName`, `abreviation`, `talk`, `whisper`, `shout`) VALUES
('Español', 'ES', 'dice', 'susurra', 'grita'),
('Italiano', 'IT', 'dice', 'sussurri', 'grida'),
('Français', 'FR', 'dit-il', 'chuchotements', 'cris'),
('Deutsch', 'DE', 'er sagt', 'flüstert', 'geschrei'),
('Dutch', 'DU', 'zegt hij', 'fluistert', 'geschreeuw'),
('Irish', 'IR', 'a deir sé', 'whispers', 'shouts'),

-- --------------------------------------------------------

--
-- redrp_global table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_global` (
  `dataKey` varchar(32) NOT NULL,
  `dataContent` text NOT NULL,
  PRIMARY KEY (`dataKey`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_inventory table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_inventory` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `ownerId` int(11) NOT NULL,
  `ownerType` tinyint(4) NOT NULL,
  `content` text NOT NULL,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_item table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_item` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `nameSingular` varchar(32) NOT NULL DEFAULT 'DummyItem',
  `namePlural` varchar(32) NOT NULL DEFAULT 'DummyItem',
  `sex` tinyint(4) NOT NULL DEFAULT '0' COMMENT '0 - male, 1 - female',
  `itemType` tinyint(4) NOT NULL DEFAULT '0',
  `quantity` int(11) NOT NULL DEFAULT '0',
  `itemWeight` tinyint(4) NOT NULL DEFAULT '100',
  `itemImage` varchar(32) NOT NULL DEFAULT 'unknown.png',
  `isHeavy` bit(1) NOT NULL DEFAULT b'0',
  `maxContentWeight` int(32) NOT NULL DEFAULT '1000',
  `canContainTheseItems` text NOT NULL,
  `isAmmoOf` text NOT NULL,
  `distributionPrice` int(11) NOT NULL DEFAULT '0',
  `sellPrice` int(11) NOT NULL DEFAULT '0',
  `propModel` bigint(20) NOT NULL,
  `addictionLevel` tinyint(4) NOT NULL DEFAULT '0',
  `nutritionalLevel` tinyint(4) NOT NULL DEFAULT '0',
  `thirstLevel` tinyint(4) NOT NULL DEFAULT '0',
  `alcoholLevel` tinyint(4) NOT NULL DEFAULT '0',
  `weaponModel` varchar(32) NOT NULL,
  `maleVariation` smallint(6) NOT NULL DEFAULT '-1',
  `maleVariationTexture` smallint(6) NOT NULL DEFAULT '0',
  `maleAlternativeVariation` smallint(6) NOT NULL DEFAULT '-1',
  `femaleVariation` smallint(6) NOT NULL DEFAULT '-1',
  `femaleVariationTexture` smallint(6) NOT NULL DEFAULT '0',
  `femaleAlternativeVariation` smallint(6) NOT NULL DEFAULT '-1',
  `rightHandXOffset` float NOT NULL DEFAULT '0',
  `rightHandYOffset` float NOT NULL DEFAULT '0',
  `rightHandZOffset` float NOT NULL DEFAULT '0',
  `rightHandXRotation` float NOT NULL DEFAULT '0',
  `rightHandYRotation` float NOT NULL DEFAULT '0',
  `rightHandZRotation` float NOT NULL DEFAULT '0',
  `leftHandXOffset` float NOT NULL DEFAULT '0',
  `leftHandYOffset` float NOT NULL DEFAULT '0',
  `leftHandZOffset` float NOT NULL DEFAULT '0',
  `leftHandXRotation` float NOT NULL DEFAULT '0',
  `leftHandYRotation` float NOT NULL DEFAULT '0',
  `leftHandZRotation` float NOT NULL DEFAULT '0',
  `chestXOffset` float NOT NULL DEFAULT '0',
  `chestYOffset` float NOT NULL DEFAULT '0',
  `chestZOffset` float NOT NULL DEFAULT '0',
  `chestXRotation` float NOT NULL DEFAULT '0',
  `chestYRotation` float NOT NULL DEFAULT '0',
  `chestZRotation` float NOT NULL DEFAULT '0',
  `backXOffset` float NOT NULL DEFAULT '0',
  `backYOffset` float NOT NULL DEFAULT '0',
  `backZOffset` float NOT NULL DEFAULT '0',
  `backXRotation` float NOT NULL DEFAULT '0',
  `backYRotation` float NOT NULL DEFAULT '0',
  `backZRotation` float NOT NULL DEFAULT '0',
  `worldXRotation` float NOT NULL DEFAULT '0',
  `worldYRotation` float NOT NULL DEFAULT '0',
  `worldZRotation` double NOT NULL DEFAULT '0',
  `worldZOffset` double NOT NULL DEFAULT '0',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_global_container
--

CREATE TABLE IF NOT EXISTS `redrp_global_container` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `x` float NOT NULL DEFAULT '0',
  `y` float NOT NULL DEFAULT '0',
  `z` float NOT NULL DEFAULT '0',
  `dimension` int(11) NOT NULL DEFAULT '0',
  `inventory` int(11) NOT NULL DEFAULT '0' COMMENT 'Inventory identifier',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

--
-- redrp_admin_teleports table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_admin_teleports` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `tpName` varchar(32) NOT NULL,
  `x` double NOT NULL,
  `y` double NOT NULL,
  `z` double NOT NULL,
  `a` double NOT NULL,
  `dimension` int(11) NOT NULL DEFAULT '1',
  `active` bit NOT NULL DEFAULT b'1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_banlog table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_banlog` (
  `playerId` int(11) NOT NULL COMMENT 'Player Id',
  `playerName` varchar(40) NOT NULL COMMENT 'Player name',
  `adminId` int(11) NOT NULL COMMENT 'Admin Player Id',
  `adminName` varchar(32) NOT NULL COMMENT 'Admin name',
  `reason` varchar(64) NOT NULL,
  `banDuration` int(20) NOT NULL,
  `banDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  `active` bit NOT NULL DEFAULT b'1',
  PRIMARY KEY (`playerId`,`banDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_connectionlog table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_connectionlog` (
  `connectionType` int(1) NOT NULL COMMENT '0 - External (panel), 1 - Game login, 2 - Character selected',
  `targetId` int(11) NOT NULL COMMENT 'Player or character Id',
  `targetName` varchar(40) NOT NULL COMMENT 'Player or character name',
  `connectionIp` varchar(16) NOT NULL,
  `connectionDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`connectionType`,`targetId`,`connectionDate`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_kicklog table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_kicklog` (
  `playerId` int(11) NOT NULL COMMENT 'Player Id',
  `playerName` varchar(40) NOT NULL COMMENT 'Player name',
  `adminId` int(11) NOT NULL COMMENT 'Admin player id',
  `adminName` varchar(32) NOT NULL COMMENT 'Admin name',
  `reason` varchar(64) NOT NULL,
  `kickDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`playerId`, `kickDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_namechangelog table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_namechangelog` (
  `id` int(11) NOT NULL AUTO_INCREMENT,
  `characterId` int(11) NOT NULL,
  `oldName` varchar(40) NOT NULL,
  `newName` varchar(40) NOT NULL,
  `changeDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_eventlog table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_eventlog` (
  `entityId` int(11) NOT NULL COMMENT 'Entity that triggers the event',
  `entityType` int(4) NOT NULL COMMENT 'Entity type, 0 = system, 1 = player, 2 = vehicle, etc',
  `targetEntityId` int(11) NOT NULL COMMENT 'Entity that is affected by the event',
  `targetEntityType` int(4) NOT NULL COMMENT 'Same as entity type',
  `eventDescription` varchar(64) NOT NULL,
  `miscData` varchar(64) NOT NULL,
  `eventDate` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`entityId`, `targetEntityId`, `eventDate`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_character table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_character` (
  `id` int(32) unsigned NOT NULL AUTO_INCREMENT,
  `playerId` bigint(20) NOT NULL,
  `characterName` varchar(255) NOT NULL,
  `validated` int(11) NOT NULL,
  `nameChange` int(1) NOT NULL DEFAULT '0',
  `playTime` int(11) NOT NULL,
  `createdAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `status` int(1) NOT NULL DEFAULT '-1',
  `logins` int(11) NOT NULL DEFAULT '0',
  `sex` int(11) NOT NULL,
  `age` int(11) NOT NULL,
  `banStatus` int(1) NOT NULL,
  `banReason` varchar(200) NOT NULL,
  `banDate` timestamp NOT NULL,
  `banTime` int(20) NOT NULL,
  `x` float NOT NULL DEFAULT '0',
  `y` float NOT NULL DEFAULT '0',
  `z` float NOT NULL DEFAULT '5',
  `heading` float NOT NULL DEFAULT '0',
  `interior` int(11) NOT NULL DEFAULT '0',
  `dimension` int(11) NOT NULL DEFAULT '0',
  `health` smallint(6) NOT NULL DEFAULT '100',
  `dying` int(11) NOT NULL DEFAULT '0',
  `cash` int(11) NOT NULL DEFAULT '1000',
  `inventory` int(11) NOT NULL DEFAULT '-1',
  `paydayTime` smallint(6) NOT NULL DEFAULT '0',
  `cuffed` tinyint(4) NOT NULL DEFAULT '0',
  `walkingAnimation` varchar(32) NOT NULL DEFAULT 'default',
  `mood` int(2) NOT NULL DEFAULT '0',
  `talkAnimation` bit(1) NOT NULL DEFAULT b'1',
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_character_experience table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_character_experience` (
  `characterId` int(32) unsigned NOT NULL,
  `security` smallint(6) NOT NULL DEFAULT '0',
  `mechanic` smallint(6) NOT NULL DEFAULT '0',
  `vehicleTheft` smallint(6) NOT NULL DEFAULT '0',
  `criminal` smallint(6) NOT NULL DEFAULT '0',
  `delivery` smallint(6) NOT NULL DEFAULT '0',
  `taxi` smallint(6) NOT NULL DEFAULT '0',
  `fish` int(11) NOT NULL DEFAULT '0',
  PRIMARY KEY (`characterId`),
  FOREIGN KEY (`characterId`) REFERENCES redrp_character(id)
) ENGINE=InnoDB DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_player table scheme
--

CREATE TABLE IF NOT EXISTS `redrp_player` (
  `id` int(10) unsigned NOT NULL AUTO_INCREMENT,
  `playerName` varchar(32) NOT NULL UNIQUE,
  `socialClub` varchar(64) NOT NULL UNIQUE,
  `password` varchar(64) NOT NULL,
  `adminLevel` int(4) NOT NULL,
  `createdAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `updatedAt` timestamp NOT NULL DEFAULT CURRENT_TIMESTAMP,
  `active` bit NOT NULL DEFAULT b'0',
  `banStatus` int(1) NOT NULL DEFAULT 0,
  `banReason` text NOT NULL DEFAULT '',
  `banDate` timestamp,
  `banDuration` int(20) NOT NULL DEFAULT 0,
  PRIMARY KEY (`id`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;

-- --------------------------------------------------------

--
-- redrp_whitelist
--

CREATE TABLE IF NOT EXISTS `redrp_whitelist` (
  `socialClub` varchar(64) NOT NULL,
  PRIMARY KEY (`socialClub`)
) ENGINE=InnoDB  DEFAULT CHARSET=utf8;
