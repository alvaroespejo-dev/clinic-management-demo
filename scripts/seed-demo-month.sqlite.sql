/*
============================================================================
 AEspejo.Clinic - Demo seed for SQLite (one month of realistic activity)
============================================================================
 SQLite port of scripts/seed-demo-month.sql (that one is T-SQL / SQL Server).

 Target: a TENANT clinical database file, e.g. .sqlite-data/AEspejo_Clinic_Demo.db.
 The schema must already exist (run the app once with Database:Provider=Sqlite).
 This script only INSERTs data.

 How to run (this script does NOT open its own transaction, so it works in tools that
 already wrap execution in one, like DB Browser):
   - DB Browser for SQLite: open the .db, "Execute SQL" tab, paste/open this file, Run (F5),
     then click "Write Changes" to persist.
   - sqlite3 CLI:  sqlite3 "path/AEspejo_Clinic_Demo.db" ".read scripts/seed-demo-month.sqlite.sql"
   - Python:  import sqlite3; con=sqlite3.connect("path.db")
              con.executescript(open("scripts/seed-demo-month.sqlite.sql", encoding="utf-8").read())
              con.commit(); con.close()

 Storage formats this script must honour so EF Core reads the data back correctly:
   - Guid           -> TEXT, UPPERCASE, hyphenated (EF compares GUID text case-sensitively).
   - DateTimeOffset -> INTEGER UTC ticks (a value converter stores these columns as ticks).
                       ticks = 621355968000000000 + unix_seconds * 10000000
   - DateOnly       -> TEXT 'yyyy-MM-dd'.
   - decimal        -> TEXT (we write with 2 decimals via printf).
   - bool           -> INTEGER 0/1;  enums -> INTEGER.

 Login: every seeded staff user has password  Clinic12345
        (the provisioning admin admin@demo.local keeps its own password).

 Idempotent: re-running first deletes everything tied to the two seeded branch
 GUIDs, then re-inserts. The provisioning admin/branch/OrgConfig are left untouched.

 Comments are in English per the repo standard; seeded values are Spanish
 because the demo clinic's default language is "es".
============================================================================
*/

-- No explicit BEGIN/COMMIT: DB Browser (and other GUI tools) already wrap the whole
-- script in a transaction, and SQLite forbids nesting one. The idempotent cleanup below
-- makes re-runs safe regardless.

-- ---------------------------------------------------------------------------
-- 0) Parameters (SQLite has no variables -> a one-row temp table)
--    now_s = current UTC unix seconds; mid_s = today's UTC midnight; pwd = bcrypt hash.
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS p;
CREATE TEMP TABLE p AS SELECT
    CAST(strftime('%s', 'now') AS INTEGER)                 AS now_s,
    CAST(strftime('%s', 'now', 'start of day') AS INTEGER) AS mid_s,
    '$2a$11$BrwazRc/4Y4pt52PZkkDCuG8zbzJ5XeolXpO4ctMxehTSK5tdMldu' AS pwd;

-- ---------------------------------------------------------------------------
-- 1) Clean previous seed (child -> parent), scoped to the two seeded branches
-- ---------------------------------------------------------------------------
DELETE FROM Payments WHERE InvoiceId IN (
    SELECT Id FROM Invoices WHERE BranchId IN (
        '11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-000000000002'));
DELETE FROM Invoices WHERE BranchId IN (
    '11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-000000000002');
DELETE FROM AppointmentDetails WHERE AppointmentId IN (
    SELECT Id FROM Appointments WHERE BranchId IN (
        '11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-000000000002'));
DELETE FROM TreatmentPlanItems WHERE TreatmentPlanId IN (
    SELECT tp.Id FROM TreatmentPlans tp JOIN Patients pt ON pt.Id = tp.PatientId
    WHERE pt.BranchId IN ('11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-000000000002'));
DELETE FROM TreatmentPlans WHERE PatientId IN (
    SELECT Id FROM Patients WHERE BranchId IN (
        '11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-000000000002'));
DELETE FROM Appointments WHERE BranchId IN (
    '11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-000000000002');
DELETE FROM ToothRecords WHERE OdontogramId IN (
    SELECT o.Id FROM Odontograms o JOIN Patients pt ON pt.Id = o.PatientId
    WHERE pt.BranchId IN ('11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-000000000002'));
DELETE FROM Odontograms WHERE PatientId IN (
    SELECT Id FROM Patients WHERE BranchId IN (
        '11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-000000000002'));
DELETE FROM PatientAllergies WHERE PatientId IN (
    SELECT Id FROM Patients WHERE BranchId IN (
        '11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-000000000002'));
DELETE FROM MedicalConditions WHERE PatientId IN (
    SELECT Id FROM Patients WHERE BranchId IN (
        '11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-000000000002'));
DELETE FROM Patients WHERE BranchId IN (
    '11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-000000000002');
DELETE FROM Professionals WHERE Id IN (
    '33333333-3333-3333-3333-000000000002', '33333333-3333-3333-3333-000000000003', '33333333-3333-3333-3333-000000000004');
DELETE FROM Users WHERE Id IN (
    '33333333-3333-3333-3333-000000000001', '33333333-3333-3333-3333-000000000002', '33333333-3333-3333-3333-000000000003',
    '33333333-3333-3333-3333-000000000004', '33333333-3333-3333-3333-000000000005', '33333333-3333-3333-3333-000000000006');
DELETE FROM Rooms WHERE BranchId IN (
    '11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-000000000002');
DELETE FROM ServiceCatalogTranslations WHERE ServiceId IN (SELECT Id FROM ServiceCatalogs WHERE Code LIKE 'SEED-%');
DELETE FROM ServiceCatalogs WHERE Code LIKE 'SEED-%';
DELETE FROM Branches WHERE Id IN (
    '11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-000000000002');

-- ---------------------------------------------------------------------------
-- 2) Branches   (CreatedAt = 45 days ago, in UTC ticks)
-- ---------------------------------------------------------------------------
INSERT INTO Branches (Id, Name, Address, Phone, Email, IsActive, CreatedAt)
SELECT '11111111-1111-1111-1111-000000000001', 'Sede Central', 'Av. Reforma 123, Ciudad', '+34 910 000 001', 'central@clinicademo.local', 1,
       621355968000000000 + (now_s - 45*86400)*10000000 FROM p
UNION ALL
SELECT '11111111-1111-1111-1111-000000000002', 'Sede Norte', 'Calle Norte 45, Ciudad', '+34 910 000 002', 'norte@clinicademo.local', 1,
       621355968000000000 + (now_s - 45*86400)*10000000 FROM p;

-- ---------------------------------------------------------------------------
-- 3) Rooms (2 per branch) — kept in a temp map for later modulo joins
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS _rooms;
CREATE TEMP TABLE _rooms (Id TEXT, BranchIdx INTEGER, RoomIdx INTEGER);
INSERT INTO _rooms VALUES
 ('22222222-2222-2222-2222-000000000001', 0, 0),
 ('22222222-2222-2222-2222-000000000002', 0, 1),
 ('22222222-2222-2222-2222-000000000003', 1, 0),
 ('22222222-2222-2222-2222-000000000004', 1, 1);

INSERT INTO Rooms (Id, BranchId, Name, IsActive, CreatedAt)
SELECT r.Id,
       CASE r.BranchIdx WHEN 0 THEN '11111111-1111-1111-1111-000000000001' ELSE '11111111-1111-1111-1111-000000000002' END,
       'Gabinete ' || (r.RoomIdx + 1), 1,
       621355968000000000 + (p.now_s - 45*86400)*10000000
FROM _rooms r, p;

-- ---------------------------------------------------------------------------
-- 4) Staff users (+ professional profiles for the dentists)
--    Role: 1 Admin, 2 Dentist, 3 Assistant, 4 Receptionist
-- ---------------------------------------------------------------------------
INSERT INTO Users (Id, Email, PasswordHash, FirstName, LastName, Role, PreferredLanguage, BranchId, IsActive, CreatedAt)
SELECT * FROM (
    SELECT '33333333-3333-3333-3333-000000000001' AS Id, 'admin@clinicademo.local' AS Email, (SELECT pwd FROM p) AS PasswordHash, 'Lucía' AS FirstName, 'Ramírez' AS LastName, 1 AS Role, 'es' AS PreferredLanguage, '11111111-1111-1111-1111-000000000001' AS BranchId, 1 AS IsActive, (SELECT 621355968000000000 + (now_s - 45*86400)*10000000 FROM p) AS CreatedAt
    UNION ALL SELECT '33333333-3333-3333-3333-000000000002', 'c.torres@clinicademo.local', (SELECT pwd FROM p), 'Carlos', 'Torres', 2, 'es', '11111111-1111-1111-1111-000000000001', 1, (SELECT 621355968000000000 + (now_s - 45*86400)*10000000 FROM p)
    UNION ALL SELECT '33333333-3333-3333-3333-000000000003', 'm.diaz@clinicademo.local',   (SELECT pwd FROM p), 'María',  'Díaz',   2, 'es', '11111111-1111-1111-1111-000000000001', 1, (SELECT 621355968000000000 + (now_s - 45*86400)*10000000 FROM p)
    UNION ALL SELECT '33333333-3333-3333-3333-000000000004', 'j.vega@clinicademo.local',    (SELECT pwd FROM p), 'Javier', 'Vega',   2, 'es', '11111111-1111-1111-1111-000000000002', 1, (SELECT 621355968000000000 + (now_s - 45*86400)*10000000 FROM p)
    UNION ALL SELECT '33333333-3333-3333-3333-000000000005', 's.moreno@clinicademo.local',  (SELECT pwd FROM p), 'Sara',   'Moreno', 3, 'es', '11111111-1111-1111-1111-000000000001', 1, (SELECT 621355968000000000 + (now_s - 45*86400)*10000000 FROM p)
    UNION ALL SELECT '33333333-3333-3333-3333-000000000006', 'p.gil@clinicademo.local',     (SELECT pwd FROM p), 'Pablo',  'Gil',    4, 'es', '11111111-1111-1111-1111-000000000001', 1, (SELECT 621355968000000000 + (now_s - 45*86400)*10000000 FROM p)
);

DROP TABLE IF EXISTS _prof;
CREATE TEMP TABLE _prof (Id TEXT, Idx INTEGER);
INSERT INTO _prof VALUES
 ('33333333-3333-3333-3333-000000000002', 0),
 ('33333333-3333-3333-3333-000000000003', 1),
 ('33333333-3333-3333-3333-000000000004', 2);

INSERT INTO Professionals (Id, LicenseNumber, Specialty, Color, CreatedAt)
SELECT * FROM (
    SELECT '33333333-3333-3333-3333-000000000002' AS Id, 'COL-10231' AS LicenseNumber, 'Odontología general' AS Specialty, '#2563eb' AS Color, (SELECT 621355968000000000 + (now_s - 45*86400)*10000000 FROM p) AS CreatedAt
    UNION ALL SELECT '33333333-3333-3333-3333-000000000003', 'COL-10488', 'Endodoncia', '#16a34a', (SELECT 621355968000000000 + (now_s - 45*86400)*10000000 FROM p)
    UNION ALL SELECT '33333333-3333-3333-3333-000000000004', 'COL-10592', 'Ortodoncia', '#db2777', (SELECT 621355968000000000 + (now_s - 45*86400)*10000000 FROM p)
);

-- ---------------------------------------------------------------------------
-- 5) Service catalog (+ EN/ES translations) — decimals stored as 2-dp TEXT
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS _svc;
CREATE TEMP TABLE _svc (Id TEXT, Idx INTEGER, Code TEXT, Price REAL, NameEs TEXT, NameEn TEXT, Category TEXT);
INSERT INTO _svc VALUES
 ('44444444-4444-4444-4444-000000000001', 0, 'SEED-CONS',  40, 'Consulta',                 'Consultation',            'General'),
 ('44444444-4444-4444-4444-000000000002', 1, 'SEED-LIMP',  60, 'Limpieza dental',          'Dental cleaning',         'Higiene'),
 ('44444444-4444-4444-4444-000000000003', 2, 'SEED-EMPA',  80, 'Empaste',                  'Filling',                 'Conservadora'),
 ('44444444-4444-4444-4444-000000000004', 3, 'SEED-ENDO', 250, 'Endodoncia',               'Root canal',              'Endodoncia'),
 ('44444444-4444-4444-4444-000000000005', 4, 'SEED-EXTR',  90, 'Extracción',               'Extraction',              'Cirugía'),
 ('44444444-4444-4444-4444-000000000006', 5, 'SEED-CORO', 400, 'Corona',                   'Crown',                   'Prótesis'),
 ('44444444-4444-4444-4444-000000000007', 6, 'SEED-IMPL', 900, 'Implante',                 'Implant',                 'Cirugía'),
 ('44444444-4444-4444-4444-000000000008', 7, 'SEED-BLAN', 200, 'Blanqueamiento',           'Whitening',               'Estética'),
 ('44444444-4444-4444-4444-000000000009', 8, 'SEED-ORTO', 120, 'Ortodoncia (mensualidad)', 'Orthodontics (monthly)',  'Ortodoncia'),
 ('44444444-4444-4444-4444-000000000010', 9, 'SEED-RADI',  35, 'Radiografía',              'X-ray',                   'Diagnóstico');

INSERT INTO ServiceCatalogs (Id, Code, DefaultPrice, IsActive, CreatedAt)
SELECT s.Id, s.Code, printf('%.2f', s.Price), 1, 621355968000000000 + (p.now_s - 45*86400)*10000000
FROM _svc s, p;

INSERT INTO ServiceCatalogTranslations (Id, ServiceId, LanguageCode, Name, Description, Category, CreatedAt)
SELECT upper(substr(hex(randomblob(4)),1,8)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(6)),1,12)),
       s.Id, 'es', s.NameEs, NULL, s.Category, 621355968000000000 + (p.now_s - 45*86400)*10000000 FROM _svc s, p
UNION ALL
SELECT upper(substr(hex(randomblob(4)),1,8)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(6)),1,12)),
       s.Id, 'en', s.NameEn, NULL, s.Category, 621355968000000000 + (p.now_s - 45*86400)*10000000 FROM _svc s, p;

-- ---------------------------------------------------------------------------
-- 6) Patients (20; 10 per branch) with fixed GUIDs, + odontograms
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS _pat;
CREATE TEMP TABLE _pat (Id TEXT, BranchIdx INTEGER, Idx INTEGER, First TEXT, Last TEXT, Gender INTEGER, Doc TEXT);
INSERT INTO _pat
WITH names(n, First, Last, Gender) AS (
    VALUES
      (1 ,'Ana',      'García',    2), (2 ,'Miguel',   'Fernández', 1), (3 ,'Laura',    'Martín',    2),
      (4 ,'David',    'López',     1), (5 ,'Carmen',   'Sánchez',   2), (6 ,'Jorge',    'Romero',    1),
      (7 ,'Elena',    'Navarro',   2), (8 ,'Raúl',     'Ortiz',     1), (9 ,'Marta',    'Ruiz',      2),
      (10,'Sergio',   'Molina',    1), (11,'Isabel',   'Delgado',   2), (12,'Andrés',   'Castro',    1),
      (13,'Patricia', 'Ortega',    2), (14,'Fernando', 'Rubio',     1), (15,'Nuria',    'Serrano',   2),
      (16,'Alberto',  'Márquez',   1), (17,'Cristina', 'Blanco',    2), (18,'Óscar',    'Suárez',    1),
      (19,'Beatriz',  'Iglesias',  2), (20,'Hugo',     'Medina',    1)
)
SELECT '55555555-5555-5555-5555-' || substr('000000000000' || n, -12),
       CASE WHEN n <= 10 THEN 0 ELSE 1 END,
       (n - 1) % 10,
       First, Last, Gender,
       'DNI' || substr('00000000' || (10000000 + n * 37), -8) || 'X'
FROM names;

INSERT INTO Patients (Id, BranchId, FirstName, LastName, DateOfBirth, Gender, DocumentType, DocumentNumber,
                      Email, Phone, Address, PreferredLanguage, BloodType,
                      EmergencyContactName, EmergencyContactPhone, IsActive, CreatedAt)
SELECT pt.Id,
       CASE pt.BranchIdx WHEN 0 THEN '11111111-1111-1111-1111-000000000001' ELSE '11111111-1111-1111-1111-000000000002' END,
       pt.First, pt.Last,
       date('now', '-' || (20 + (pt.Idx * 3) % 45) || ' years'),           -- DateOnly 'yyyy-MM-dd', ages ~20-65
       pt.Gender,
       1,                                                                    -- DocumentType.NationalId
       pt.Doc,
       lower(pt.First || '.' || pt.Last || '@example.local'),
       '+34 6' || substr('00000000' || (10000000 + pt.Idx * 131), -8),
       'Calle ' || (1 + pt.Idx) || ', nº ' || (1 + (pt.Idx * 7) % 90),
       'es',
       CASE pt.Idx % 4 WHEN 0 THEN 'O+' WHEN 1 THEN 'A+' WHEN 2 THEN 'B+' ELSE 'AB+' END,
       'Contacto de emergencia', '+34 600 000 000',
       1,
       621355968000000000 + (p.now_s - (40 - pt.Idx) * 86400) * 10000000
FROM _pat pt, p;

INSERT INTO Odontograms (Id, PatientId, CreatedAt)
SELECT upper(substr(hex(randomblob(4)),1,8)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(6)),1,12)),
       pt.Id, 621355968000000000 + (p.now_s - (40 - pt.Idx) * 86400) * 10000000
FROM _pat pt, p;

-- Allergies for a subset of patients
INSERT INTO PatientAllergies (Id, PatientId, Substance, Severity, Notes, CreatedAt)
SELECT upper(substr(hex(randomblob(4)),1,8)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(6)),1,12)),
       pt.Id,
       CASE pt.Idx % 3 WHEN 0 THEN 'Penicilina' WHEN 1 THEN 'Látex' ELSE 'Ibuprofeno' END,
       1 + pt.Idx % 3,                                                       -- Mild / Moderate / Severe
       'Registrado en anamnesis inicial.',
       621355968000000000 + (p.now_s - (38 - pt.Idx) * 86400) * 10000000
FROM _pat pt, p
WHERE pt.Idx % 3 = 0;

-- Chronic medical conditions for a smaller subset
INSERT INTO MedicalConditions (Id, PatientId, Name, Notes, IsActive, CreatedAt)
SELECT upper(substr(hex(randomblob(4)),1,8)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(6)),1,12)),
       pt.Id,
       CASE pt.Idx % 4 WHEN 0 THEN 'Hipertensión' WHEN 1 THEN 'Diabetes tipo 2' WHEN 2 THEN 'Asma' ELSE 'Anticoagulado' END,
       'Requiere valoración previa a procedimientos invasivos.',
       1,
       621355968000000000 + (p.now_s - (38 - pt.Idx) * 86400) * 10000000
FROM _pat pt, p
WHERE pt.Idx % 4 = 1;

-- A few tooth records per patient (varied FDI teeth and statuses)
INSERT INTO ToothRecords (Id, OdontogramId, ToothNumber, Surface, Status, Notes, UpdatedByUserId, CreatedAt)
SELECT upper(substr(hex(randomblob(4)),1,8)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(6)),1,12)),
       o.Id,
       CASE (pt.Idx + t.n) % 6 WHEN 0 THEN 11 WHEN 1 THEN 16 WHEN 2 THEN 24 WHEN 3 THEN 36 WHEN 4 THEN 37 ELSE 46 END,
       1 + (pt.Idx + t.n) % 6,                                              -- Surface 1..6
       2 + (pt.Idx + t.n) % 4,                                              -- Status 2..5 (Caries..Crown)
       'Hallazgo en revisión.',
       '33333333-3333-3333-3333-000000000002',                             -- dentist 1
       621355968000000000 + (p.now_s - (30 - pt.Idx) * 86400) * 10000000
FROM _pat pt
JOIN Odontograms o ON o.PatientId = pt.Id
CROSS JOIN (SELECT 0 AS n UNION ALL SELECT 1 UNION ALL SELECT 2) t
CROSS JOIN p
WHERE pt.Idx % 2 = 0;                                                       -- ~half the patients

-- ---------------------------------------------------------------------------
-- 7) Appointments over the last ~30 days (+ a few upcoming), weekdays only.
--    Built into a temp table so details/invoices/payments can reference the ids.
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS _appt;
CREATE TEMP TABLE _appt AS
WITH RECURSIVE days(dayoff) AS (
    SELECT -29 UNION ALL SELECT dayoff + 1 FROM days WHERE dayoff < 5
),
slots(slot, hr) AS (
    VALUES (0, 9), (1, 10), (2, 11), (3, 14), (4, 16)
),
cal AS (
    SELECT d.dayoff, s.slot, s.hr,
           (SELECT mid_s FROM p) + d.dayoff * 86400 + s.hr * 3600 AS dsec
    FROM days d CROSS JOIN slots s
    WHERE strftime('%w', (SELECT mid_s FROM p) + d.dayoff * 86400, 'unixepoch') NOT IN ('0', '6')  -- skip Sun/Sat
),
numbered AS (
    SELECT dayoff, slot, hr, dsec, ROW_NUMBER() OVER (ORDER BY dsec) - 1 AS k FROM cal
)
SELECT
    upper(substr(hex(randomblob(4)),1,8)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(6)),1,12)) AS Id,
    n.k AS k,
    (n.k % 2) AS BranchIdx,
    CASE WHEN n.k % 2 = 0 THEN '11111111-1111-1111-1111-000000000001' ELSE '11111111-1111-1111-1111-000000000002' END AS BranchId,
    n.slot AS slot,
    n.dsec AS dsec,
    621355968000000000 + n.dsec * 10000000 AS ScheduledTicks,
    30 + (n.k % 2) * 30 AS DurationMinutes,
    CASE
        WHEN n.dsec <  (SELECT mid_s FROM p)          AND n.k % 13 = 5 THEN 6   -- NoShow
        WHEN n.dsec <  (SELECT mid_s FROM p)          AND n.k % 9  = 4 THEN 5   -- Cancelled
        WHEN n.dsec <  (SELECT mid_s FROM p)                          THEN 4   -- Completed
        WHEN n.dsec <  (SELECT mid_s FROM p) + 86400  AND n.slot <= 1 THEN 3   -- InProgress (today)
        WHEN n.dsec <  (SELECT mid_s FROM p) + 86400                  THEN 2   -- Confirmed (today)
        WHEN n.k % 3 = 0                                              THEN 2   -- Confirmed (future)
        ELSE 1                                                                 -- Scheduled (future)
    END AS Status,
    rm.Id  AS RoomId,
    pr.Id  AS ProfessionalId,
    pt.Id  AS PatientId,
    sv.Id  AS SvcId,  sv.Price  AS SvcPrice,
    sv2.Id AS Svc2Id, sv2.Price AS Svc2Price
FROM numbered n
JOIN _rooms rm ON rm.BranchIdx = (n.k % 2)      AND rm.RoomIdx = ((n.k / 2) % 2)
JOIN _prof  pr ON pr.Idx       = (n.k % 3)
JOIN _pat   pt ON pt.BranchIdx = (n.k % 2)      AND pt.Idx     = ((n.k / 2) % 10)
JOIN _svc   sv ON sv.Idx       = (n.k % 10)
JOIN _svc  sv2 ON sv2.Idx      = ((n.k + 5) % 10);

INSERT INTO Appointments (Id, BranchId, PatientId, ProfessionalId, RoomId, CreatedByUserId,
                          ScheduledAt, DurationMinutes, Status, Notes, CancelReason, CreatedAt)
SELECT a.Id, a.BranchId, a.PatientId, a.ProfessionalId, a.RoomId,
       '33333333-3333-3333-3333-000000000006',                             -- receptionist
       a.ScheduledTicks, a.DurationMinutes, a.Status,
       CASE WHEN a.Status = 4 THEN 'Tratamiento realizado según lo previsto.' END,
       CASE WHEN a.Status = 5 THEN 'Cancelada por el paciente.' END,
       621355968000000000 + (a.dsec - 3 * 86400) * 10000000               -- booked 3 days before
FROM _appt a;

-- ---------------------------------------------------------------------------
-- 8) Appointment service lines (billed statuses: InProgress / Completed)
-- ---------------------------------------------------------------------------
INSERT INTO AppointmentDetails (Id, AppointmentId, ServiceId, Quantity, UnitPrice, ToothNumber, Notes, CreatedAt)
SELECT upper(substr(hex(randomblob(4)),1,8)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(6)),1,12)),
       a.Id, a.SvcId,
       CASE WHEN a.k % 7 = 0 THEN 2 ELSE 1 END,
       printf('%.2f', a.SvcPrice),
       CASE WHEN a.k % 4 = 0 THEN (CASE (a.k % 4) WHEN 0 THEN 11 WHEN 1 THEN 26 WHEN 2 THEN 36 ELSE 46 END) END,
       NULL,
       a.ScheduledTicks
FROM _appt a
WHERE a.Status IN (3, 4);

-- Second line on every third billed appointment
INSERT INTO AppointmentDetails (Id, AppointmentId, ServiceId, Quantity, UnitPrice, ToothNumber, Notes, CreatedAt)
SELECT upper(substr(hex(randomblob(4)),1,8)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(6)),1,12)),
       a.Id, a.Svc2Id, 1, printf('%.2f', a.Svc2Price), NULL, NULL, a.ScheduledTicks
FROM _appt a
WHERE a.Status IN (3, 4) AND a.k % 3 = 0;

-- ---------------------------------------------------------------------------
-- 9) Invoices for completed appointments (total = sum of its service lines)
-- ---------------------------------------------------------------------------
INSERT INTO Invoices (Id, AppointmentId, BranchId, PatientId, InvoiceNumber, IssueDate,
                      TotalAmount, PaidAmount, Status, Notes, CreatedAt)
SELECT upper(substr(hex(randomblob(4)),1,8)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(6)),1,12)),
       b.AppointmentId, b.BranchId, b.PatientId,
       'INV-' || strftime('%Y', b.dsec, 'unixepoch') || '-' || substr('000000' || (b.i + 1), -6),
       strftime('%Y-%m-%d', b.dsec, 'unixepoch'),
       printf('%.2f', b.Total),
       CASE WHEN b.i % 11 = 0 THEN '0.00'                                   -- Issued (unpaid)
            WHEN b.i % 5  = 0 THEN printf('%.2f', b.Total / 2.0)            -- Partially paid
            ELSE printf('%.2f', b.Total) END,                              -- Paid
       CASE WHEN b.i % 11 = 0 THEN 2 WHEN b.i % 5 = 0 THEN 3 ELSE 4 END,
       NULL,
       621355968000000000 + b.dsec * 10000000
FROM (
    SELECT a.Id AS AppointmentId, a.BranchId, a.PatientId, a.dsec,
           SUM(d.Quantity * d.UnitPrice) AS Total,
           ROW_NUMBER() OVER (ORDER BY a.dsec) - 1 AS i
    FROM _appt a
    JOIN AppointmentDetails d ON d.AppointmentId = a.Id
    WHERE a.Status = 4
    GROUP BY a.Id, a.BranchId, a.PatientId, a.dsec
) b;

-- ---------------------------------------------------------------------------
-- 10) Payments for paid / partially-paid invoices
-- ---------------------------------------------------------------------------
INSERT INTO Payments (Id, InvoiceId, Amount, Method, PaidAt, ReceivedByUserId, Reference, CreatedAt)
SELECT upper(substr(hex(randomblob(4)),1,8)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(6)),1,12)),
       i.Id, i.PaidAmount,
       1 + (abs(random()) % 4),                                            -- Method 1..4
       621355968000000000 + CAST(strftime('%s', i.IssueDate) AS INTEGER) * 10000000,
       '33333333-3333-3333-3333-000000000006',                            -- receptionist
       'REF-' || substr('000000' || (abs(random()) % 999999), -6),
       621355968000000000 + CAST(strftime('%s', i.IssueDate) AS INTEGER) * 10000000
FROM Invoices i
WHERE i.BranchId IN ('11111111-1111-1111-1111-000000000001', '11111111-1111-1111-1111-000000000002')
  AND i.Status IN (3, 4);

-- ---------------------------------------------------------------------------
-- 11) Treatment plans (first 8 patients of branch 0) with two items each
-- ---------------------------------------------------------------------------
DROP TABLE IF EXISTS _plans;
CREATE TEMP TABLE _plans AS
SELECT upper(substr(hex(randomblob(4)),1,8)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(6)),1,12)) AS Id,
       pt.Id AS PatientId, pt.Idx AS Idx
FROM _pat pt
WHERE pt.BranchIdx = 0 AND pt.Idx < 8;

INSERT INTO TreatmentPlans (Id, PatientId, ProfessionalId, Title, Description, Status, CreatedAt)
SELECT pl.Id, pl.PatientId,
       CASE pl.Idx % 3 WHEN 0 THEN '33333333-3333-3333-3333-000000000002'
                       WHEN 1 THEN '33333333-3333-3333-3333-000000000003'
                       ELSE        '33333333-3333-3333-3333-000000000004' END,
       'Plan de tratamiento ' || (pl.Idx + 1),
       'Plan integral acordado con el paciente.',
       CASE pl.Idx % 3 WHEN 0 THEN 3 WHEN 1 THEN 2 ELSE 1 END,             -- Completed/Active/Draft
       621355968000000000 + (p.now_s - (25 - pl.Idx) * 86400) * 10000000
FROM _plans pl, p;

INSERT INTO TreatmentPlanItems (Id, TreatmentPlanId, ServiceId, Quantity, EstimatedPrice, Status, ToothNumber, AppointmentDetailId, CreatedAt)
SELECT upper(substr(hex(randomblob(4)),1,8)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(2)),1,4)||'-'||substr(hex(randomblob(6)),1,12)),
       pl.Id, s.Id, 1, printf('%.2f', s.Price),
       CASE WHEN pl.Idx % 2 = 0 THEN 3 ELSE 1 END,                         -- Completed / Pending
       CASE (pl.Idx + it.n) % 4 WHEN 0 THEN 11 WHEN 1 THEN 21 WHEN 2 THEN 36 ELSE 46 END,
       NULL,
       621355968000000000 + (p.now_s - (25 - pl.Idx) * 86400) * 10000000
FROM _plans pl
CROSS JOIN (SELECT 0 AS n UNION ALL SELECT 1) it
JOIN _svc s ON s.Idx = ((pl.Idx + it.n * 3 + 2) % 10)
CROSS JOIN p;

-- ---------------------------------------------------------------------------
-- 12) Cleanup temp tables + summary
-- ---------------------------------------------------------------------------
DROP TABLE _rooms;
DROP TABLE _prof;
DROP TABLE _svc;
DROP TABLE _pat;
DROP TABLE _appt;
DROP TABLE _plans;
DROP TABLE p;

SELECT 'Seed completed' AS status,
       (SELECT COUNT(*) FROM Branches     WHERE Id IN ('11111111-1111-1111-1111-000000000001','11111111-1111-1111-1111-000000000002')) AS branches,
       (SELECT COUNT(*) FROM Users        WHERE Email LIKE '%@clinicademo.local') AS users,
       (SELECT COUNT(*) FROM Patients     WHERE BranchId IN ('11111111-1111-1111-1111-000000000001','11111111-1111-1111-1111-000000000002')) AS patients,
       (SELECT COUNT(*) FROM Appointments WHERE BranchId IN ('11111111-1111-1111-1111-000000000001','11111111-1111-1111-1111-000000000002')) AS appointments,
       (SELECT COUNT(*) FROM Invoices     WHERE BranchId IN ('11111111-1111-1111-1111-000000000001','11111111-1111-1111-1111-000000000002')) AS invoices;
