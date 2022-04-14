using MySql.Data.MySqlClient;
using DatabaseLibrary.Core;

namespace DatabaseLibrary.Managers
{
    public class TableUpdateManager
    {

        public static void createTables(DbContext dbContext)
        {
            List<string> queries = new List<string>();

            queries.Add(@"CREATE TABLE
IF
	NOT EXISTS `users` ( username VARCHAR ( 255 ) PRIMARY KEY, fName VARCHAR ( 255 ) NOT NULL, lName VARCHAR ( 255 ), `password` VARCHAR ( 255 ) NOT NULL, `email` VARCHAR ( 255 ), dateCreated TIMESTAMP DEFAULT UTC_TIMESTAMP NOT NULL, userType VARCHAR ( 32 ) NOT NULL );
CREATE TABLE
IF
	NOT EXISTS `rewards` ( username VARCHAR ( 255 ) NOT NULL, numPoints INT NOT NULL, dateGiven TIMESTAMP DEFAULT UTC_TIMESTAMP NOT NULL, FOREIGN KEY ( username ) REFERENCES users ( username ), INDEX ( username ) );
CREATE TABLE
IF
	NOT EXISTS `teams` ( `id` INT AUTO_INCREMENT PRIMARY KEY, `name` VARCHAR ( 255 ), `mgrUsername` VARCHAR ( 255 ), FOREIGN KEY ( mgrUsername ) REFERENCES users ( username ) );
CREATE TABLE
IF
	NOT EXISTS `team_members` ( `teamId` INT NOT NULL, `username` VARCHAR ( 255 ) NOT NULL, PRIMARY KEY ( teamId, username ), FOREIGN KEY ( teamId ) REFERENCES teams ( id ), FOREIGN KEY ( username ) REFERENCES users ( username ) );
CREATE TABLE
IF
	NOT EXISTS `projects` ( `id` INT AUTO_INCREMENT PRIMARY KEY, `teamId` INT NOT NULL, `name` VARCHAR ( 255 ) NOT NULL, `budget` INT DEFAULT 0 NOT NULL, `dateCreated` TIMESTAMP DEFAULT UTC_TIMESTAMP NOT NULL, FOREIGN KEY ( teamId ) REFERENCES teams ( id ) );
CREATE TABLE
IF
	NOT EXISTS `lists` ( `projectId` INT NOT NULL, `name` VARCHAR ( 255 ), `dateCreated` TIMESTAMP DEFAULT UTC_TIMESTAMP, PRIMARY KEY ( projectId, `name` ), FOREIGN KEY ( `projectId` ) REFERENCES projects ( `id` ) );
CREATE TABLE
IF
	NOT EXISTS `tasks` ( `projectId` INT NOT NULL, `listName` VARCHAR ( 255 ) NOT NULL, `name` VARCHAR ( 255 ) NOT NULL, description TEXT DEFAULT '', dateCreated TIMESTAMP DEFAULT UTC_TIMESTAMP, priority INT NOT NULL, deadline DATE DEFAULT NULL, completed BOOLEAN DEFAULT FALSE, completionPoints INT DEFAULT 1, PRIMARY KEY ( projectId, listName, `name` ), FOREIGN KEY ( projectId, listName ) REFERENCES lists ( `projectId`, `name` ) );
CREATE TABLE
IF
	NOT EXISTS `labels` ( `projectId` INT NOT NULL, `name` VARCHAR ( 255 ) NOT NULL, `color` CHAR ( 7 ) NOT NULL, PRIMARY KEY ( projectId, `name` ), FOREIGN KEY ( projectId ) REFERENCES projects ( id ) );
CREATE TABLE
IF
	NOT EXISTS `categorizes` ( `projectId` INT, `listName` VARCHAR ( 255 ), `taskName` VARCHAR ( 255 ), `labelName` VARCHAR ( 255 ), PRIMARY KEY ( projectId, listName, taskName, labelName ), FOREIGN KEY ( projectId, listName, taskName ) REFERENCES tasks ( projectId, listName, `name` ), FOREIGN KEY ( projectId, labelName ) REFERENCES labels ( projectId, `name` ) );
CREATE TABLE
IF
	NOT EXISTS `checklists` ( `projectId` INT, `listName` VARCHAR ( 255 ), `taskName` VARCHAR ( 255 ), `id` INT NOT NULL, `title` VARCHAR ( 255 ) NOT NULL, PRIMARY KEY ( projectId, listName, taskName, id ), FOREIGN KEY ( projectId, listName, taskName ) REFERENCES tasks ( projectId, listName, `name` ) );
CREATE TABLE
IF
	NOT EXISTS `checklist_items` ( `projectId` INT, `listName` VARCHAR ( 255 ), `taskName` VARCHAR ( 255 ), `checklistId` INT NOT NULL, `description` TEXT NOT NULL, `completed` BOOLEAN DEFAULT FALSE NOT NULL, PRIMARY KEY ( projectId, listName, taskName, checklistId, description ( 255 )), FOREIGN KEY ( projectId, listName, taskName, checklistId ) REFERENCES checklists ( projectId, listName, taskName, `id` ) );
CREATE TABLE
IF
	NOT EXISTS `assigned_to` ( `projectId` INT, `listName` VARCHAR ( 255 ), `taskName` VARCHAR ( 255 ), `username` VARCHAR ( 255 ), PRIMARY KEY ( projectId, listName, taskName, username ), FOREIGN KEY ( projectId, listName, taskName ) REFERENCES tasks ( projectId, listName, `name` ), FOREIGN KEY ( username ) REFERENCES users ( username ) );
CREATE TABLE
IF
	NOT EXISTS `comments` ( `projectId` INT, `listName` VARCHAR ( 255 ), `taskName` VARCHAR ( 255 ), `username` VARCHAR ( 255 ), `date` TIMESTAMP DEFAULT UTC_TIMESTAMP NOT NULL, `comment` VARCHAR ( 255 ), FOREIGN KEY ( projectId, listName, taskName ) REFERENCES tasks ( projectId, listName, `name` ), FOREIGN KEY ( username ) REFERENCES users ( username ), INDEX ( projectId, listName, taskName ) );
");

            string message;
            foreach (string query in queries)
                dbContext.ExecuteNonQueryCommand(query, null, out message);
        }

        public static void createProcedures(DbContext dbContext)
        {
            List<string> procedures = new List<string>();

            procedures.Add(@"DROP PROCEDURE IF EXISTS `loginUser`;
			CREATE PROCEDURE IF NOT EXISTS `loginUser`(IN `_username` varchar(255),IN _password varchar(255))
			BEGIN
			DECLARE _actualPassword VARCHAR(255);
			DECLARE success BOOLEAN;

			SELECT `password` INTO _actualPassword FROM `users` WHERE username = _username;
			SET success = _actualPassword = _password;

			IF success THEN
				SELECT users.*, success FROM `users` WHERE username = _username;
			ELSE
				SELECT success;
			END IF;
			END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `createProject`;
			CREATE PROCEDURE `createProject`(IN `_teamId` integer,IN `_name` varchar(255),IN `_budget` integer)
BEGIN

			INSERT INTO `projects` (`teamId`, `name`, `budget`, `mgrUsername`) VALUES (_teamId, _name, _budget);

			SELECT * FROM `projects` WHERE projects.id=@@IDENTITY;

			END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `createUser`;
			PROCEDURE `createUser`(IN `_username` varchar(255),IN `_fName` varchar(255),IN `_lName` varchar(255), IN `_password` VARCHAR(255),IN `_email` VARCHAR(255),IN `_userType` varchar(32))
			BEGIN

			INSERT INTO `users` (`username`, `fName`, `lName`, `password`, `email`, `userType`) VALUES (_username, _fName, _lName, `_password`, _email, _userType);

			SELECT * FROM `users` WHERE username=_username;

			END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `getUser`;
			CREATE PROCEDURE IF NOT EXISTS `getUser`(IN `_username` varchar(255))
			BEGIN

			SELECT * FROM `users` WHERE `username`=_username;

			END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `getParticipatingProjects`; CREATE PROCEDURE IF NOT EXISTS `getParticipatingProjects`(IN `_username` varchar(255))
BEGIN

SELECT projects.* FROM `projects` AS projects INNER JOIN `team_members` AS members ON (projects.teamId = members.teamId) WHERE members.username=_username;

END;");

            string message;
            foreach (string query in procedures)
                dbContext.ExecuteNonQueryCommand(query, null, out message);
        }

        public static void updateTables(DbContext dbContext)
        {
            // Put table changes here
        }
    }
}
