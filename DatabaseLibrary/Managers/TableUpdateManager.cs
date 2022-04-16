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

			INSERT INTO `projects` (`teamId`, `name`, `budget`) VALUES (_teamId, _name, _budget);

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
            procedures.Add(@"DROP PROCEDURE IF EXISTS `getParticipatingTeams`; CREATE PROCEDURE IF NOT EXISTS `getParticipatingTeams`(IN `_username` varchar(255))
BEGIN

SELECT teams.* FROM `teams` AS teams INNER JOIN `team_members` AS members ON(teams.id = members.teamId) WHERE members.username = _username;

			END;");


            procedures.Add(@"DROP PROCEDURE IF EXISTS `createLabel`; CREATE PROCEDURE IF NOT EXISTS `createLabel`(IN _projectId INT, IN _name VARCHAR(255), IN _color CHAR(7))
BEGIN

	INSERT INTO `labels` (projectId, name, color) VALUES (_projectId, _name, _color);
	
	SELECT * FROM labels WHERE id=@@IDENTITY;
	
END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `getLabels`; CREATE PROCEDURE IF NOT EXISTS `getLabels`(IN _projectId INT)
BEGIN
	
	SELECT * FROM labels WHERE projectId=_projectId;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `deleteLabel`; CREATE PROCEDURE IF NOT EXISTS `deleteLabel`(IN `_id` INT)
BEGIN
	
	DELETE FROM labels WHERE id=_id;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `createList`; CREATE PROCEDURE IF NOT EXISTS `createList`(IN _projectId INT, IN _name VARCHAR(255))
BEGIN
	DECLARE _position INT;
	
	SELECT COUNT(position) INTO _position FROM `lists` WHERE projectId=_projectId;
	
	INSERT INTO `lists` (projectId, name, position) VALUES (_projectId, _name, _position);
	
	SELECT * FROM `lists` WHERE id=@@IDENTITY;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `getLists`; CREATE PROCEDURE IF NOT EXISTS `getLists`(IN _projectId INT)
BEGIN

	SELECT * FROM `lists` WHERE projectId=_projectId ORDER BY position ASC;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `moveList`; CREATE PROCEDURE IF NOT EXISTS `moveList`(IN _projectId INT, IN `_listId` INT,IN `_newPosition` int)
BEGIN
	DECLARE _oldPosition INT;
	
	SELECT position INTO _oldPosition FROM `lists` WHERE id=_listId;
	
	IF _newPosition > _oldPosition THEN
		UPDATE `lists` SET position=position-1 WHERE projectId=_projectId AND position > _oldPosition AND position <= _newPosition;
	ELSE
		UPDATE `lists` SET position=position+1 WHERE projectId=_projectId AND position >= _newPosition AND position < _oldPosition;
	END IF;
	
	UPDATE `lists` SET position=_newPosition WHERE id=_listId;

	SELECT * FROM `lists` WHERE id=_listId;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `deleteList`;CREATE PROCEDURE IF NOT EXISTS `deleteList`(IN `_id` INT)
BEGIN

	DELETE FROM `lists` WHERE id=_id;

END");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `createTask`; CREATE PROCEDURE IF NOT EXISTS `createTask`(IN _projectId INT, IN _listId INT, IN _name VARCHAR(255), IN _description TEXT, IN _deadline DATE, IN _completionPoints INT)
BEGIN
	DECLARE _priority INT(11);
	
	SELECT COUNT(id) INTO _priority FROM tasks WHERE projectId=_projectId AND listId=_listId;

	INSERT INTO `tasks` (projectId, listId, `name`, description, deadline, priority, completionPoints) VALUES (_projectId, _listId, _name, _description, _deadline, _priority, _completionPoints);
	
	SELECT * FROM tasks WHERE id=@@IDENTITY;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `deleteTask`; CREATE PROCEDURE IF NOT EXISTS `deleteTask`(IN _id INT)
BEGIN
	
	DELETE FROM tasks WHERE id=_id;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `getTasks`; CREATE PROCEDURE IF NOT EXISTS `getTasks`(IN _listId INT)
BEGIN
	
	SELECT * FROM tasks WHERE listId=_listId ORDER BY priority ASC;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `moveTask`; CREATE PROCEDURE IF NOT EXISTS `moveTask`(IN _listId INT, IN _taskId INT, IN _newPriority INT)
BEGIN
	DECLARE _oldPriority INT;
	
	SELECT priority INTO _oldPriority FROM `tasks` WHERE id=_taskId;
	
	IF _newPriority > _oldPriority THEN
		UPDATE `tasks` SET priority=priority-1 WHERE listId=_listId AND priority > _oldPriority AND priority <= _newPriority;
	ELSE
		UPDATE `tasks` SET priority=priority+1 WHERE listId=_listId AND listName=_listName AND priority >= _newPriority AND priority < _oldPriority;
	END IF;
	
	UPDATE `tasks` SET priority=_newPriority WHERE id=_taskId;

	SELECT * FROM `tasks` WHERE id=_taskId;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `updateTask`; CREATE PROCEDURE IF NOT EXISTS `updateTask`(IN _id INT, IN _name VARCHAR(255), IN _description TEXT, IN _deadline DATE, IN _completionPoints INT, IN _completed BOOLEAN)
BEGIN
	
	UPDATE `tasks` SET name=_name, description=_description, deadline=_deadline, completionPoints=_completionPoints, completed=_completed WHERE id=_id;
	
	SELECT * FROM `tasks` WHERE id=_id;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `getAssignedToTask`; CREATE PROCEDURE IF EXISTS PROCEDURE `getAssignedToTask`(IN _id INT)
BEGIN

SELECT * FROM USERS WHERE username IN (SELECT username FROM assigned_to WHERE taskId=_id);

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `assignToTask`; CREATE PROCEDURE IF NOT EXISTS `assignToTask`(IN _taskId INT, IN _username varchar(255))
BEGIN
	
	INSERT INTO assigned_to (taskId, username) VALUES (_taskId, _username);
	
	SELECT * from tasks WHERE id=_taskId;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `unassignFromTask`; CREATE PROCEDURE IF NOT EXISTS `unassignFromTask`(IN _taskId INT, IN _username varchar(255))
BEGIN
	
	DELETE FROM assigned_to WHERE taskId=_taskId AND username=_username;
	
	SELECT * from tasks WHERE id=_taskId;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `updateUser`; 
			CREATE PROCEDURE IF NOT EXISTS `updateUser`(IN _username varchar(255),IN _fName varchar(255),IN _lName varchar(255),IN _password VARCHAR(255),IN _email VARCHAR(255))
			BEGIN
			UPDATE `users` SET fName = _fName, lName = _lName, `password` = _password, email = _email WHERE username = _username;
			
			SELECT * FROM `users` WHERE username = _username;

			END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `getProject`; CREATE PROCEDURE IF NOT EXISTS `getProject`(IN `_username` varchar(255), IN _projectId INT)
BEGIN

SELECT projects.* FROM `projects` AS projects INNER JOIN `team_members` AS members ON (projects.teamId = members.teamId) WHERE projects.id=_projectID AND members.username=_username LIMIT 1;

END;");
            procedures.Add(@"DROP PROCEDURE IF EXISTS `getTeamManager`; CREATE PROCEDURE IF NOT EXISTS `getTeamManager`(IN `_teamId` integer, IN `_user` varchar(255))
BEGIN

	SELECT teams.* FROM `teams` AS teams WHERE teams.mgrUsername = _user AND teams.id = _teamId;

			END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `updateList; CREATE PROCEDURE IF NOT EXISTS `updateList`(IN _id INT, IN _name varchar(255))
BEGIN
	
	UPDATE lists SET name=_name WHERE id=_id;
	
	SELECT * FROM lists WHERE id=_id;

END");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `getTeamMembers`;
			CREATE PROCEDURE IF NOT EXISTS `getTeamMembers`(IN `_teamId` integer)
			BEGIN

				SELECT users.* FROM `users` AS users INNER JOIN `team_members` AS members ON(users.username = members.username) WHERE members.teamId = _teamId;

			END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `getAssignedToTask`; CREATE PROCEDURE IF NOT EXISTS `getAssignedToTask`(IN _taskId INT)
BEGIN
	
	SELECT username FROM assigned_to WHERE taskId=_taskId;

END;");
            procedures.Add(@"DROP PROCEDURE IF EXISTS `getTeamProjects`;
			CREATE PROCEDURE IF NOT EXISTS `getTeamProjects`(IN `_teamId` integer)
			BEGIN

				SELECT projects.* FROM `projects` AS projects INNER JOIN `teams` AS teams ON(projects.teamId = teams.id) WHERE teams.id = _teamId;

			END;");
			procedures.Add(@"DROP PROCEDURE IF EXISTS `deleteProject`;
			CREATE PROCEDURE IF NOT EXISTS `deleteProject`(IN _projectId INT)
BEGIN

			DELETE FROM `projects` WHERE `id` = _projectId;

			END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `deleteTeam`; CREATE PROCEDURE IF NOT EXISTS `deleteTeam`(IN _id INT)
			BEGIN
	
				DELETE FROM team_members WHERE teamId = _id;
				DELETE FROM teams WHERE id = _id;

			END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `createTeam`; 
			CREATE PROCEDURE IF NOT EXISTS `createTeam`(IN `_name` varchar(255),IN `_mgrUsername` varchar(255))
			BEGIN

				INSERT INTO `teams` (`name`, `mgrUsername`) VALUES (_name, _mgrUsername);
				INSERT INTO `team_members` (`teamId`, `username`) VALUES (@@IDENTITY, _mgrUsername);

				SELECT * FROM `teams` WHERE id = @@IDENTITY;
	
			END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `getAllUsers`; 
			CREATE PROCEDURE IF NOT EXISTS `getAllUsers`()
BEGIN

				SELECT * FROM users;
	
			END;");

			procedures.Add(@"DROP PROCEDURE IF EXISTS `addMember`; 
			CREATE PROCEDURE IF NOT EXISTS `addMember`(IN `_id` INT,IN `_username` varchar(255))
			BEGIN

				INSERT INTO team_members (`teamId`, `username`) VALUES (_id, _username);
			
				SELECT * FROM `team_members` WHERE teamId = _id AND username = _username;
	
			END;");

			procedures.Add(@"DROP PROCEDURE IF EXISTS `updateProject`;
			CREATE PROCEDURE IF NOT EXISTS `updateProject`(IN _projectId INT, IN _teamId INT, IN _name varchar(255), IN _budget INT)
BEGIN
			UPDATE `projects` SET `teamId` = _teamId, `name` = _name, `budget` = _budget WHERE `id` = _projectId;

			SELECT* FROM `projects` WHERE `id` = _projectId;

			END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `createChecklist`; CREATE PROCEDURE IF NOT EXISTS `createChecklist`(IN `_taskId` INT, IN _title VARCHAR(255))
BEGIN

	INSERT INTO checklists (`taskId`, `title`) VALUES (_taskId, _title);
	
	SELECT * FROM checklists WHERE id=@@IDENTITY;

END");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `deleteChecklistItem`;
CREATE PROCEDURE IF NOT EXISTS `deleteChecklistItem`(IN _itemId INT)
BEGIN

	DELETE FROM checlist_items WHERE id=_itemId;

END;");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `updateChecklistItem`; CREATE PROCEDURE IF NOT EXISTS `updateChecklistItem`(IN _id INT, IN _description TEXT, IN _completed BOOLEAN)
BEGIN

	UPDATE checklist_items SET description=_description, completed=_completed WHERE id=_id;

END");

            procedures.Add(@"DROP PROCEDURE IF EXISTS `updateChecklist`; CREATE PROCEDURE IF NOT EXISTS PROCEDURE `updateChecklist`(IN _id INT, IN _title VARCHAR(255))
BEGIN
	
	UPDATE checklist SET title=_title WHERE id=_id;

END");

            foreach (string query in procedures)
                dbContext.ExecuteNonQueryCommand(query, null, out string message);
        }

        public static void updateTables(DbContext dbContext)
        {
            // Put table changes here
        }
    }
}
