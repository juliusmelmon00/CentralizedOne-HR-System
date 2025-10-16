-- ==============================================
-- CentralizedOne Debug Queries
-- Safe read-only queries to check database state
-- ==============================================

-- 1. List all users with their roles
SELECT u.Id, u.Username, r.Name AS RoleName
FROM Users u
LEFT JOIN UserRoles ur ON u.Id = ur.UserId
LEFT JOIN Roles r ON ur.RoleId = r.Id;

-- 2. Check if default SuperAdmin and Employee exist
SELECT Username, RoleId FROM Users;

-- 3. List all appointments with employee usernames
SELECT a.Id, a.Title, a.Date, u.Username
FROM Appointments a
JOIN Users u ON a.UserId = u.Id
ORDER BY a.Date;

-- 4. See document submissions per user
SELECT d.Id, d.FileName, d.UploadedAt, u.Username
FROM Documents d
JOIN Users u ON d.UserId = u.Id
ORDER BY d.UploadedAt DESC;

-- 5. Track document versions (history)
SELECT dv.DocumentId, dv.VersionNumber, dv.FilePath, dv.UploadedAt
FROM DocumentVersions dv
ORDER BY dv.UploadedAt DESC;

-- 6. Notifications for employees
SELECT n.Id, n.Message, n.CreatedAt, u.Username
FROM Notifications n
JOIN Users u ON n.UserId = u.Id
ORDER BY n.CreatedAt DESC;

-- 7. Audit log (who did what and when)
SELECT a.Id, a.Action, a.Timestamp, u.Username
FROM AuditLogs a
JOIN Users u ON a.UserId = u.Id
ORDER BY a.Timestamp DESC;

-- 8. Quick check: list all tables
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE';

SELECT Username, Role FROM Users;
