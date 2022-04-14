﻿using MySql.Data.MySqlClient;
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
	NOT EXISTS `users` ( username VARCHAR ( 255 ) PRIMARY KEY, fName VARCHAR ( 255 ) NOT NULL, lName VARCHAR ( 255 ), `password` VARCHAR ( 255 ) NOT NULL, `email` VARCHAR ( 255 ), dateCreated TIMESTAMP DEFAULT UTC_TIMESTAMP NOT NULL );
CREATE TABLE
IF
	NOT EXISTS `rewards` ( username VARCHAR ( 255 ) NOT NULL, numPoints INT NOT NULL, `teamId` INT NOT NULL, `projectId` INT NOT NULL, dateGiven TIMESTAMP DEFAULT UTC_TIMESTAMP NOT NULL, FOREIGN KEY ( username ) REFERENCES users ( username ) ON DELETE CASCADE, FOREIGN KEY ( teamId ) REFERENCES teams ( id ) ON DELETE CASCADE, FOREIGN KEY ( projectId ) REFERENCES projects ( id ) ON DELETE CASCADE, INDEX ( username, teamId, projectId ) );
CREATE TABLE
IF
	NOT EXISTS `teams` ( `id` INT AUTO_INCREMENT PRIMARY KEY, `name` VARCHAR ( 255 ), `mgrUsername` VARCHAR ( 255 ), FOREIGN KEY ( mgrUsername ) REFERENCES users ( username ) ON DELETE CASCADE );
CREATE TABLE
IF
	NOT EXISTS `team_members` ( `teamId` INT NOT NULL, `username` VARCHAR ( 255 ) NOT NULL, PRIMARY KEY ( teamId, username ), FOREIGN KEY ( teamId ) REFERENCES teams ( id ) ON DELETE CASCADE, FOREIGN KEY ( username ) REFERENCES users ( username ) ON DELETE CASCADE );
CREATE TABLE
IF
	NOT EXISTS `projects` ( `id` INT AUTO_INCREMENT PRIMARY KEY, `teamId` INT NOT NULL, `name` VARCHAR ( 255 ) NOT NULL, `budget` INT DEFAULT 0 NOT NULL, `dateCreated` TIMESTAMP DEFAULT UTC_TIMESTAMP NOT NULL, `description` TEXT DEFAULT "", FOREIGN KEY ( teamId ) REFERENCES teams ( id ) ON DELETE CASCADE );
CREATE TABLE
IF
	NOT EXISTS `lists` (`id` INT AUTO_INCREMENT PRIMARY KEY, `projectId` INT NOT NULL, `name` VARCHAR ( 255 ), `dateCreated` TIMESTAMP DEFAULT UTC_TIMESTAMP, `position` INT NOT NULL, FOREIGN KEY ( `projectId` ) REFERENCES projects ( `id` ) ON DELETE CASCADE );
CREATE TABLE
IF
	NOT EXISTS `tasks` (`id` INT AUTO_INCREMENT PRIMARY KEY, `projectId` INT NOT NULL, `listId` INT NOT NULL, `name` VARCHAR ( 255 ) NOT NULL, description TEXT DEFAULT "", dateCreated TIMESTAMP DEFAULT UTC_TIMESTAMP, priority INT NOT NULL, deadline DATE DEFAULT NULL, completed BOOLEAN DEFAULT FALSE, completionPoints INT DEFAULT 1, FOREIGN KEY ( projectId, listId ) REFERENCES lists ( `projectId`, `id` ) ON DELETE CASCADE );
CREATE TABLE
IF
	NOT EXISTS `labels` (`id` INT AUTO_INCREMENT PRIMARY KEY , `projectId` INT NOT NULL, `name` VARCHAR ( 255 ) NOT NULL, `color` CHAR ( 7 ) NOT NULL, FOREIGN KEY ( projectId ) REFERENCES projects ( id ) ON DELETE CASCADE );
CREATE TABLE
IF
	NOT EXISTS `categorizes` (`taskId` INT NOT NULL, `labelId` INT NOT NULL, PRIMARY KEY (taskId, labelId), FOREIGN KEY ( `taskId` ) REFERENCES tasks ( `id` ) ON DELETE CASCADE, FOREIGN KEY ( `labelId` ) REFERENCES labels ( `id` ) ON DELETE CASCADE );
CREATE TABLE
IF
	NOT EXISTS `checklists` (`id` INT AUTO_INCREMENT PRIMARY KEY, `taskId` INT NOT NULL, `title` VARCHAR ( 255 ) NOT NULL, FOREIGN KEY ( taskId ) REFERENCES tasks ( `id` ) ON DELETE CASCADE );
CREATE TABLE
IF
	NOT EXISTS `checklist_items` (`id` INT AUTO_INCREMENT PRIMARY KEY, `checklistId` INT NOT NULL, `description` TEXT NOT NULL, `completed` BOOLEAN DEFAULT FALSE NOT NULL, FOREIGN KEY ( checklistId ) REFERENCES checklists ( `id` ) ON DELETE CASCADE );
CREATE TABLE
IF
	NOT EXISTS `assigned_to` (`taskId` INT NOT NULL, `username` VARCHAR ( 255 ), PRIMARY KEY ( taskId, username ), FOREIGN KEY ( taskId ) REFERENCES tasks ( `id` ) ON DELETE CASCADE, FOREIGN KEY ( username ) REFERENCES users ( username ) ON DELETE CASCADE );
CREATE TABLE
IF
	NOT EXISTS `comments` ( `taskId` INT NOT NULL, `username` VARCHAR ( 255 ), `date` TIMESTAMP DEFAULT UTC_TIMESTAMP NOT NULL, `comment` VARCHAR ( 255 ), FOREIGN KEY ( taskId ) REFERENCES tasks ( id ) ON DELETE CASCADE, FOREIGN KEY ( username ) REFERENCES users ( username ) ON DELETE CASCADE );
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
			CREATE PROCEDURE IF NOT EXISTS `createUser`(IN `_username` varchar(255),IN `_fName` varchar(255),IN `_lName` varchar(255), IN `_password` VARCHAR(255),IN `_email` VARCHAR(255))
			BEGIN

			INSERT INTO `users` (`username`, `fName`, `lName`, `password`, `email`) VALUES (_username, _fName, _lName, `_password`, _email);

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

            procedures.Add(@"DROP PROCEDURE IF EXISTS `createLabel`; CREATE PROCEDURE IF NOT EXISTS `createLabel`(IN _projectId INT, IN _name VARCHAR(255), IN _color CHAR(7))
BEGIN

	INSERT INTO `labels` (projectId, name, color) VALUES (_projectId, _name, _color);
	
END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `getLabels`; CREATE PROCEDURE IF NOT EXISTS `getLabels`(IN _projectId INT)
BEGIN
	
	SELECT * FROM labels WHERE projectId=_projectId;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `deleteLabel`; CREATE PROCEDURE IF NOT EXISTS `deleteLabel`(IN `_projectId` INT, IN `_name` VARCHAR(255))
BEGIN
	
	DELETE FROM labels WHERE projectId = _projectId AND name=_name;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `createList`; CREATE PROCEDURE IF NOT EXISTS `createList`(IN _projectId INT, IN _name VARCHAR(255), IN _position INT)
BEGIN
	
	INSERT INTO `lists` (projectId, name, position) VALUES (_projectId, _name, _position);
	
	SELECT * FROM `lists` WHERE projectId=_projectId AND name=_name;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `getLists`; CREATE PROCEDURE IF NOT EXISTS `getLists`(IN _projectId INT)
BEGIN

	SELECT * FROM `lists` WHERE projectId=_projectId ORDER BY position ASC;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `moveList`; CREATE PROCEDURE IF NOT EXISTS `moveList`(IN `_projectId` int,IN `_name` varchar(255),IN `_newPosition` int)
BEGIN
	DECLARE _oldPosition INT;
	
	SELECT position INTO _oldPosition FROM `lists` WHERE projectId=_projectId AND `name`=_name;
	
	IF _newPosition > _oldPosition THEN
		UPDATE `lists` SET position=position-1 WHERE projectId=_projectId AND position > _oldPosition AND position <= _newPosition;
	ELSE
		UPDATE `lists` SET position=position+1 WHERE projectId=_projectId AND position >= _newPosition AND position < _oldPosition;
	END IF;
	
	UPDATE `lists` SET position=_newPosition WHERE projectId=_projectId AND `name`=_name;

	SELECT * FROM `lists` WHERE projectId=_projectId AND _name=name;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `createTask`; CREATE PROCEDURE IF NOT EXISTS `createTask`(IN _projectId INT, IN _listName VARCHAR(255), IN _name VARCHAR(255), IN _description TEXT, IN _priority INT, IN _completionPoints INT)
BEGIN

	INSERT INTO `tasks` (projectId, listName, `name`, description, priority, completionPoints) VALUES (_projectId, _listName, _description, _priority, _completionPoints);
	
	SELECT * FROM tasks WHERE projectId=_projectId AND listName=_listName AND `name`=_name;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `deleteTask`; CREATE PROCEDURE IF NOT EXISTS `deleteTask`(IN _projectId INT, IN _listName VARCHAR(255), IN _name VARCHAR(255))
BEGIN
	
	DELETE FROM tasks WHERE projectId=_projectId AND listName=_listName AND `name`=_name;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `getTasks`; CREATE PROCEDURE IF NOT EXISTS `getTasks`(IN _projectId INT, IN _listName VARCHAR(255))
BEGIN
	
	SELECT * FROM tasks WHERE projectId=_projectId AND listName=_listName ORDER BY position ASC;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `moveTask`; CREATE PROCEDURE IF NOT EXISTS `moveTask`(IN _projectId INT, IN _listName VARCHAR(255), IN _name VARCHAR(255), IN _newPriority INT)
BEGIN
	DECLARE _oldPriority INT;
	
	SELECT priority INTO _oldPriority FROM `tasks` WHERE projectId=_projectId AND listName=_listName AND `name`=_name;
	
	IF _newPriority > _oldPriority THEN
		UPDATE `tasks` SET priority=priority-1 WHERE projectId=_projectId AND listName=_listName AND priority > _oldPriority AND priority <= _newPriority;
	ELSE
		UPDATE `tasks` SET priority=priority+1 WHERE projectId=_projectId AND listName=_listName AND priority >= _newPriority AND priority < _oldPriority;
	END IF;
	
	UPDATE `tasks` SET priority=_newPriority WHERE projectId=_projectId AND listName=_listName AND `name`=_name;

	SELECT * FROM `tasks` WHERE projectId=_projectId AND listName=_listName AND _name=name;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `updateTask`(IN _projectId INT, IN _listName VARCHAR(255), IN _name VARCHAR(255), IN _description TEXT, IN _deadline DATE, IN _completionPoints INT, IN _completed BOOLEAN)
BEGIN
	
	UPDATE `tasks` SET description=_description, deadline=_deadline, completionPoints=_completionPoints, completed=_completed WHERE projectId=_projectId AND listName=_listName AND _name=name;
	
	SELECT * FROM `tasks` WHERE projectId=_projectId AND listName=_listName AND _name=name;

END;");

            foreach (string query in procedures)
                dbContext.ExecuteNonQueryCommand(query, null, out string message);
        }

        public static void updateTables(DbContext dbContext)
        {
            // Put table changes here
        }
    }
}
