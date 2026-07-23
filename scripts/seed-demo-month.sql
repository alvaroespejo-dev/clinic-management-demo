/*
============================================================================
 AEspejo.Clinic - Demo seed: one month of realistic activity
============================================================================
 Target: a TENANT clinical database (AppDbContext), e.g. AEspejo_Clinic_Demo.
 DO NOT run this against the master database (MasterDbContext).

 The schema must already exist (run the app once, or `dotnet ef database update`
 for the Infrastructure project). This script only INSERTs data.

 What it creates (all hung off two seeded branches so it can be re-run cleanly):
   - 2 branches, 4 rooms
   - 6 staff users (1 admin, 3 dentists, 1 assistant, 1 receptionist)
     + 3 professional profiles (the dentists)
   - 10 catalog services with EN/ES translations
   - 20 patients with odontograms, some allergies / medical conditions
   - ~120 appointments spread over the last ~30 days (+ a few upcoming),
     with statuses that look real (completed / cancelled / no-show / scheduled)
   - appointment service lines, invoices and payments
   - a handful of treatment plans with items

 Login: every seeded staff user has password  Clinic12345
        (the provisioning admin admin@demo.local keeps its own password).

 Idempotent: re-running first deletes everything tied to the two seeded
 branch GUIDs, then re-inserts. The provisioning admin/branch/OrgConfig are
 left untouched.

 Comments are in English per the repo standard; seeded values are Spanish
 because the demo clinic's default language is "es".
============================================================================
*/

SET NOCOUNT ON;
SET XACT_ABORT ON;

BEGIN TRY
BEGIN TRAN;

-- ---------------------------------------------------------------------------
-- 0) Anchors and fixed dimension GUIDs
-- ---------------------------------------------------------------------------
DECLARE @Now  datetimeoffset(7) = SYSDATETIMEOFFSET();
DECLARE @Today date              = CAST(@Now AS date);
DECLARE @tz   int                = DATEPART(TZOFFSET, @Now);

-- BCrypt hash of 'Clinic12345' (cost 11, same as PasswordHasher.Hash default)
DECLARE @Pwd nvarchar(512) = N'$2a$11$BrwazRc/4Y4pt52PZkkDCuG8zbzJ5XeolXpO4ctMxehTSK5tdMldu';

DECLARE @B1 uniqueidentifier = '11111111-1111-1111-1111-000000000001'; -- Sede Central
DECLARE @B2 uniqueidentifier = '11111111-1111-1111-1111-000000000002'; -- Sede Norte

DECLARE @UAdmin uniqueidentifier = '33333333-3333-3333-3333-000000000001';
DECLARE @UDen1  uniqueidentifier = '33333333-3333-3333-3333-000000000002';
DECLARE @UDen2  uniqueidentifier = '33333333-3333-3333-3333-000000000003';
DECLARE @UDen3  uniqueidentifier = '33333333-3333-3333-3333-000000000004';
DECLARE @UAsst  uniqueidentifier = '33333333-3333-3333-3333-000000000005';
DECLARE @URecep uniqueidentifier = '33333333-3333-3333-3333-000000000006';

-- ---------------------------------------------------------------------------
-- 1) Clean previous seed (child -> parent), scoped to the two seeded branches
-- ---------------------------------------------------------------------------
DECLARE @Branches TABLE (Id uniqueidentifier PRIMARY KEY);
INSERT INTO @Branches VALUES (@B1), (@B2);

DELETE p FROM Payments p
    JOIN Invoices i ON i.Id = p.InvoiceId
    WHERE i.BranchId IN (SELECT Id FROM @Branches);

DELETE FROM Invoices WHERE BranchId IN (SELECT Id FROM @Branches);

DELETE d FROM AppointmentDetails d
    JOIN Appointments a ON a.Id = d.AppointmentId
    WHERE a.BranchId IN (SELECT Id FROM @Branches);

DELETE ti FROM TreatmentPlanItems ti
    JOIN TreatmentPlans tp ON tp.Id = ti.TreatmentPlanId
    JOIN Patients pt ON pt.Id = tp.PatientId
    WHERE pt.BranchId IN (SELECT Id FROM @Branches);

DELETE tp FROM TreatmentPlans tp
    JOIN Patients pt ON pt.Id = tp.PatientId
    WHERE pt.BranchId IN (SELECT Id FROM @Branches);

DELETE FROM Appointments WHERE BranchId IN (SELECT Id FROM @Branches);

DELETE tr FROM ToothRecords tr
    JOIN Odontograms o ON o.Id = tr.OdontogramId
    JOIN Patients pt ON pt.Id = o.PatientId
    WHERE pt.BranchId IN (SELECT Id FROM @Branches);

DELETE o FROM Odontograms o
    JOIN Patients pt ON pt.Id = o.PatientId
    WHERE pt.BranchId IN (SELECT Id FROM @Branches);

DELETE al FROM PatientAllergies al
    JOIN Patients pt ON pt.Id = al.PatientId
    WHERE pt.BranchId IN (SELECT Id FROM @Branches);

DELETE mc FROM MedicalConditions mc
    JOIN Patients pt ON pt.Id = mc.PatientId
    WHERE pt.BranchId IN (SELECT Id FROM @Branches);

DELETE FROM Patients WHERE BranchId IN (SELECT Id FROM @Branches);

DELETE FROM Professionals
    WHERE Id IN (@UDen1, @UDen2, @UDen3);

DELETE FROM Users
    WHERE Id IN (@UAdmin, @UDen1, @UDen2, @UDen3, @UAsst, @URecep);

DELETE FROM Rooms WHERE BranchId IN (SELECT Id FROM @Branches);

DELETE t FROM ServiceCatalogTranslations t
    JOIN ServiceCatalogs s ON s.Id = t.ServiceId
    WHERE s.Code LIKE 'SEED-%';
DELETE FROM ServiceCatalogs WHERE Code LIKE 'SEED-%';

DELETE FROM Branches WHERE Id IN (SELECT Id FROM @Branches);

-- ---------------------------------------------------------------------------
-- 2) Branches
-- ---------------------------------------------------------------------------
INSERT INTO Branches (Id, Name, Address, Phone, Email, IsActive, CreatedAt) VALUES
 (@B1, N'Sede Central', N'Av. Reforma 123, Ciudad',  N'+34 910 000 001', N'central@clinicademo.local', 1, DATEADD(day,-45,@Now)),
 (@B2, N'Sede Norte',   N'Calle Norte 45, Ciudad',   N'+34 910 000 002', N'norte@clinicademo.local',   1, DATEADD(day,-45,@Now));

-- ---------------------------------------------------------------------------
-- 3) Rooms (2 per branch)
-- ---------------------------------------------------------------------------
DECLARE @Rooms TABLE (Id uniqueidentifier, BranchIdx int, RoomIdx int);
INSERT INTO @Rooms VALUES
 ('22222222-2222-2222-2222-000000000001', 0, 0),
 ('22222222-2222-2222-2222-000000000002', 0, 1),
 ('22222222-2222-2222-2222-000000000003', 1, 0),
 ('22222222-2222-2222-2222-000000000004', 1, 1);

INSERT INTO Rooms (Id, BranchId, Name, IsActive, CreatedAt)
SELECT r.Id,
       CASE r.BranchIdx WHEN 0 THEN @B1 ELSE @B2 END,
       CONCAT(N'Gabinete ', r.RoomIdx + 1),
       1, DATEADD(day,-45,@Now)
FROM @Rooms r;

-- ---------------------------------------------------------------------------
-- 4) Staff users (+ professional profiles for dentists)
-- ---------------------------------------------------------------------------
-- Role: 1 Admin, 2 Dentist, 3 Assistant, 4 Receptionist
INSERT INTO Users (Id, Email, PasswordHash, FirstName, LastName, Role, PreferredLanguage, BranchId, IsActive, CreatedAt) VALUES
 (@UAdmin, N'admin@clinicademo.local',      @Pwd, N'Lucía',   N'Ramírez', 1, N'es', @B1, 1, DATEADD(day,-45,@Now)),
 (@UDen1,  N'c.torres@clinicademo.local',   @Pwd, N'Carlos',  N'Torres',  2, N'es', @B1, 1, DATEADD(day,-45,@Now)),
 (@UDen2,  N'm.diaz@clinicademo.local',     @Pwd, N'María',   N'Díaz',    2, N'es', @B1, 1, DATEADD(day,-45,@Now)),
 (@UDen3,  N'j.vega@clinicademo.local',     @Pwd, N'Javier',  N'Vega',    2, N'es', @B2, 1, DATEADD(day,-45,@Now)),
 (@UAsst,  N's.moreno@clinicademo.local',   @Pwd, N'Sara',    N'Moreno',  3, N'es', @B1, 1, DATEADD(day,-45,@Now)),
 (@URecep, N'p.gil@clinicademo.local',      @Pwd, N'Pablo',   N'Gil',     4, N'es', @B1, 1, DATEADD(day,-45,@Now));

INSERT INTO Professionals (Id, LicenseNumber, Specialty, Color, CreatedAt) VALUES
 (@UDen1, N'COL-10231', N'Odontología general', N'#2563eb', DATEADD(day,-45,@Now)),
 (@UDen2, N'COL-10488', N'Endodoncia',          N'#16a34a', DATEADD(day,-45,@Now)),
 (@UDen3, N'COL-10592', N'Ortodoncia',          N'#db2777', DATEADD(day,-45,@Now));

DECLARE @Prof TABLE (Id uniqueidentifier, Idx int);
INSERT INTO @Prof VALUES (@UDen1,0), (@UDen2,1), (@UDen3,2);

-- ---------------------------------------------------------------------------
-- 5) Service catalog (+ EN/ES translations)
-- ---------------------------------------------------------------------------
DECLARE @Svc TABLE (Id uniqueidentifier, Idx int, Code nvarchar(50), Price decimal(18,2),
                    NameEs nvarchar(200), NameEn nvarchar(200), Category nvarchar(150));
INSERT INTO @Svc VALUES
 ('44444444-4444-4444-4444-000000000001', 0, 'SEED-CONS',  40.00, N'Consulta',                 N'Consultation',        N'General'),
 ('44444444-4444-4444-4444-000000000002', 1, 'SEED-LIMP',  60.00, N'Limpieza dental',          N'Dental cleaning',     N'Higiene'),
 ('44444444-4444-4444-4444-000000000003', 2, 'SEED-EMPA',  80.00, N'Empaste',                  N'Filling',             N'Conservadora'),
 ('44444444-4444-4444-4444-000000000004', 3, 'SEED-ENDO', 250.00, N'Endodoncia',               N'Root canal',          N'Endodoncia'),
 ('44444444-4444-4444-4444-000000000005', 4, 'SEED-EXTR',  90.00, N'Extracción',               N'Extraction',          N'Cirugía'),
 ('44444444-4444-4444-4444-000000000006', 5, 'SEED-CORO', 400.00, N'Corona',                   N'Crown',               N'Prótesis'),
 ('44444444-4444-4444-4444-000000000007', 6, 'SEED-IMPL', 900.00, N'Implante',                 N'Implant',             N'Cirugía'),
 ('44444444-4444-4444-4444-000000000008', 7, 'SEED-BLAN', 200.00, N'Blanqueamiento',           N'Whitening',           N'Estética'),
 ('44444444-4444-4444-4444-000000000009', 8, 'SEED-ORTO', 120.00, N'Ortodoncia (mensualidad)', N'Orthodontics (monthly)', N'Ortodoncia'),
 ('44444444-4444-4444-4444-000000000010', 9, 'SEED-RADI',  35.00, N'Radiografía',              N'X-ray',               N'Diagnóstico');

INSERT INTO ServiceCatalogs (Id, Code, DefaultPrice, IsActive, CreatedAt)
SELECT Id, Code, Price, 1, DATEADD(day,-45,@Now) FROM @Svc;

INSERT INTO ServiceCatalogTranslations (Id, ServiceId, LanguageCode, Name, Description, Category, CreatedAt)
SELECT NEWID(), Id, N'es', NameEs, NULL, Category, DATEADD(day,-45,@Now) FROM @Svc
UNION ALL
SELECT NEWID(), Id, N'en', NameEn, NULL, Category, DATEADD(day,-45,@Now) FROM @Svc;

-- ---------------------------------------------------------------------------
-- 6) Patients (20; 10 per branch) + odontograms
-- ---------------------------------------------------------------------------
DECLARE @Pat TABLE (Id uniqueidentifier, BranchIdx int, Idx int,
                    First nvarchar(100), Last nvarchar(100), Gender int, Doc nvarchar(50));
;WITH names(n, First, Last, Gender) AS (
    SELECT * FROM (VALUES
      (1 ,N'Ana',      N'García',    2),
      (2 ,N'Miguel',   N'Fernández', 1),
      (3 ,N'Laura',    N'Martín',    2),
      (4 ,N'David',    N'López',     1),
      (5 ,N'Carmen',   N'Sánchez',   2),
      (6 ,N'Jorge',    N'Romero',    1),
      (7 ,N'Elena',    N'Navarro',   2),
      (8 ,N'Raúl',     N'Ortiz',     1),
      (9 ,N'Marta',    N'Ruiz',      2),
      (10,N'Sergio',   N'Molina',    1),
      (11,N'Isabel',   N'Delgado',   2),
      (12,N'Andrés',   N'Castro',    1),
      (13,N'Patricia', N'Ortega',    2),
      (14,N'Fernando', N'Rubio',     1),
      (15,N'Nuria',    N'Serrano',   2),
      (16,N'Alberto',  N'Márquez',   1),
      (17,N'Cristina', N'Blanco',    2),
      (18,N'Óscar',    N'Suárez',    1),
      (19,N'Beatriz',  N'Iglesias',  2),
      (20,N'Hugo',     N'Medina',    1)
    ) v(n, First, Last, Gender)
)
INSERT INTO @Pat (Id, BranchIdx, Idx, First, Last, Gender, Doc)
SELECT CAST('55555555-5555-5555-5555-' + RIGHT('000000000000' + CAST(n AS varchar(12)), 12) AS uniqueidentifier),
       CASE WHEN n <= 10 THEN 0 ELSE 1 END,
       (n - 1) % 10,
       First, Last, Gender,
       CONCAT(N'DNI', RIGHT('00000000' + CAST(10000000 + n * 37 AS varchar(12)), 8), N'X')
FROM names;

INSERT INTO Patients (Id, BranchId, FirstName, LastName, DateOfBirth, Gender, DocumentType, DocumentNumber,
                      Email, Phone, Address, PreferredLanguage, BloodType,
                      EmergencyContactName, EmergencyContactPhone, IsActive, CreatedAt)
SELECT p.Id,
       CASE p.BranchIdx WHEN 0 THEN @B1 ELSE @B2 END,
       p.First, p.Last,
       DATEADD(year, -(20 + (p.Idx * 3) % 45), CAST(@Today AS datetime2)),  -- ages ~20-65
       p.Gender,
       1,                                                                    -- DocumentType.NationalId
       p.Doc,
       LOWER(CONCAT(p.First, N'.', p.Last, N'@example.local')),
       CONCAT(N'+34 6', RIGHT('00000000' + CAST(10000000 + p.Idx * 131 AS varchar(12)), 8)),
       CONCAT(N'Calle ', 1 + p.Idx, N', nº ', 1 + (p.Idx * 7) % 90),
       N'es',
       CHOOSE(1 + p.Idx % 4, N'O+', N'A+', N'B+', N'AB+'),
       N'Contacto de emergencia', N'+34 600 000 000',
       1,
       DATEADD(day, -(40 - p.Idx), @Now)
FROM @Pat p;

INSERT INTO Odontograms (Id, PatientId, CreatedAt)
SELECT NEWID(), p.Id, DATEADD(day, -(40 - p.Idx), @Now) FROM @Pat p;

-- Allergies for a subset of patients
INSERT INTO PatientAllergies (Id, PatientId, Substance, Severity, Notes, CreatedAt)
SELECT NEWID(), p.Id,
       CHOOSE(1 + p.Idx % 3, N'Penicilina', N'Látex', N'Ibuprofeno'),
       1 + p.Idx % 3,                                                        -- Mild / Moderate / Severe
       N'Registrado en anamnesis inicial.',
       DATEADD(day, -(38 - p.Idx), @Now)
FROM @Pat p
WHERE p.Idx % 3 = 0;

-- Chronic medical conditions for a smaller subset
INSERT INTO MedicalConditions (Id, PatientId, Name, Notes, IsActive, CreatedAt)
SELECT NEWID(), p.Id,
       CHOOSE(1 + p.Idx % 4, N'Hipertensión', N'Diabetes tipo 2', N'Asma', N'Anticoagulado'),
       N'Requiere valoración previa a procedimientos invasivos.',
       1,
       DATEADD(day, -(38 - p.Idx), @Now)
FROM @Pat p
WHERE p.Idx % 4 = 1;

-- A few tooth records per patient (varied FDI teeth and statuses)
INSERT INTO ToothRecords (Id, OdontogramId, ToothNumber, Surface, Status, Notes, UpdatedByUserId, CreatedAt)
SELECT NEWID(), o.Id,
       CHOOSE(1 + (p.Idx + t.n) % 6, 11, 16, 24, 36, 37, 46),               -- FDI tooth
       1 + (p.Idx + t.n) % 6,                                               -- Surface 1..6
       2 + (p.Idx + t.n) % 4,                                               -- Status 2..5 (Caries..Crown)
       N'Hallazgo en revisión.',
       @UDen1,
       DATEADD(day, -(30 - p.Idx), @Now)
FROM @Pat p
JOIN Odontograms o ON o.PatientId = p.Id
CROSS APPLY (VALUES (0),(1),(2)) t(n)
WHERE p.Idx % 2 = 0;                                                        -- ~half the patients

-- ---------------------------------------------------------------------------
-- 7) Appointments over the last ~30 days (+ a few upcoming), weekdays only
-- ---------------------------------------------------------------------------
-- Day offsets -29..+5; five time slots per day.
;WITH days AS (
    SELECT CAST(-29 AS int) AS dayoff
    UNION ALL SELECT dayoff + 1 FROM days WHERE dayoff < 5
),
slots AS (
    SELECT * FROM (VALUES (0,9),(1,10),(2,11),(3,14),(4,16)) s(slot, hr)
),
cal AS (
    SELECT d.dayoff, s.slot, s.hr,
           DATEADD(day, d.dayoff, @Today) AS ad
    FROM days d CROSS JOIN slots s
    WHERE DATEDIFF(day, '19000106', DATEADD(day, d.dayoff, @Today)) % 7 NOT IN (0, 1)  -- skip Sat/Sun
),
numbered AS (
    SELECT ad, slot, hr,
           ROW_NUMBER() OVER (ORDER BY ad, slot) - 1 AS k
    FROM cal
)
-- Everything is resolved here via deterministic-modulo joins so we never have to
-- ALTER + reference a temp column in the same batch (which would fail to compile).
SELECT
    NEWID() AS Id,
    n.k,
    (n.k % 2) AS BranchIdx,
    CASE WHEN n.k % 2 = 0 THEN @B1 ELSE @B2 END AS BranchId,
    n.ad, n.hr, n.slot,
    TODATETIMEOFFSET(DATEADD(hour, n.hr, CAST(n.ad AS datetime2(0))), @tz) AS ScheduledAt,
    30 + (n.k % 2) * 30 AS DurationMinutes,
    -- Status: past -> Completed, every 9th Cancelled, every 13th NoShow;
    --         today -> InProgress/Confirmed; future -> Scheduled/Confirmed
    CAST(CASE
        WHEN n.ad < @Today AND n.k % 13 = 5 THEN 6            -- NoShow
        WHEN n.ad < @Today AND n.k % 9  = 4 THEN 5            -- Cancelled
        WHEN n.ad < @Today                  THEN 4            -- Completed
        WHEN n.ad = @Today AND n.slot <= 1  THEN 3            -- InProgress
        WHEN n.ad = @Today                  THEN 2            -- Confirmed
        WHEN n.k % 3 = 0                    THEN 2            -- Confirmed (future)
        ELSE 1                                                -- Scheduled (future)
    END AS int) AS Status,
    rm.Id  AS RoomId,
    pr.Id  AS ProfessionalId,
    pt.Id  AS PatientId,
    sv.Id  AS SvcId,  sv.Price  AS SvcPrice,
    sv2.Id AS Svc2Id, sv2.Price AS Svc2Price
INTO #Appt
FROM numbered n
JOIN @Rooms rm ON rm.BranchIdx = (n.k % 2)      AND rm.RoomIdx = (n.k / 2) % 2
JOIN @Prof  pr ON pr.Idx       = n.k % 3
JOIN @Pat   pt ON pt.BranchIdx = (n.k % 2)      AND pt.Idx     = (n.k / 2) % 10
JOIN @Svc   sv ON sv.Idx       = n.k % 10
JOIN @Svc  sv2 ON sv2.Idx      = (n.k + 5) % 10;

INSERT INTO Appointments (Id, BranchId, PatientId, ProfessionalId, RoomId, CreatedByUserId,
                          ScheduledAt, DurationMinutes, Status, Notes, CancelReason, CreatedAt)
SELECT a.Id, a.BranchId, a.PatientId, a.ProfessionalId, a.RoomId, @URecep,
       a.ScheduledAt, a.DurationMinutes, a.Status,
       CASE WHEN a.Status = 4 THEN N'Tratamiento realizado según lo previsto.' ELSE NULL END,
       CASE WHEN a.Status = 5 THEN N'Cancelada por el paciente.' ELSE NULL END,
       -- booked a few days before the visit (clamped so it stays within the month)
       DATEADD(day, -3, a.ScheduledAt)
FROM #Appt a;

-- ---------------------------------------------------------------------------
-- 8) Appointment service lines (billed statuses: InProgress / Completed)
-- ---------------------------------------------------------------------------
INSERT INTO AppointmentDetails (Id, AppointmentId, ServiceId, Quantity, UnitPrice, ToothNumber, Notes, CreatedAt)
SELECT NEWID(), a.Id, a.SvcId,
       CASE WHEN a.k % 7 = 0 THEN 2 ELSE 1 END,
       a.SvcPrice,
       CASE WHEN a.k % 4 = 0 THEN CHOOSE(1 + a.k % 4, 11, 26, 36, 46) ELSE NULL END,
       NULL,
       a.ScheduledAt
FROM #Appt a
WHERE a.Status IN (3, 4);

-- Second line on every third billed appointment
INSERT INTO AppointmentDetails (Id, AppointmentId, ServiceId, Quantity, UnitPrice, ToothNumber, Notes, CreatedAt)
SELECT NEWID(), a.Id, a.Svc2Id, 1, a.Svc2Price, NULL, NULL, a.ScheduledAt
FROM #Appt a
WHERE a.Status IN (3, 4) AND a.k % 3 = 0;

-- ---------------------------------------------------------------------------
-- 9) Invoices for completed appointments (total = sum of its service lines)
-- ---------------------------------------------------------------------------
;WITH billed AS (
    SELECT a.Id AS AppointmentId, a.BranchId, a.PatientId, a.ScheduledAt,
           SUM(d.Quantity * d.UnitPrice) AS Total,
           ROW_NUMBER() OVER (ORDER BY a.ScheduledAt) - 1 AS i
    FROM #Appt a
    JOIN AppointmentDetails d ON d.AppointmentId = a.Id
    WHERE a.Status = 4
    GROUP BY a.Id, a.BranchId, a.PatientId, a.ScheduledAt
)
INSERT INTO Invoices (Id, AppointmentId, BranchId, PatientId, InvoiceNumber, IssueDate,
                      TotalAmount, PaidAmount, Status, Notes, CreatedAt)
SELECT NEWID(), b.AppointmentId, b.BranchId, b.PatientId,
       CONCAT(N'INV-', YEAR(b.ScheduledAt), N'-', RIGHT('000000' + CAST(b.i + 1 AS varchar(10)), 6)),
       CAST(b.ScheduledAt AS date),
       b.Total,
       -- payment status pattern
       CASE WHEN b.i % 11 = 0 THEN CAST(0 AS decimal(18,2))            -- Issued (unpaid)
            WHEN b.i % 5  = 0 THEN CAST(ROUND(b.Total / 2, 2) AS decimal(18,2))  -- Partially paid
            ELSE b.Total END,                                          -- Paid
       CASE WHEN b.i % 11 = 0 THEN 2
            WHEN b.i % 5  = 0 THEN 3
            ELSE 4 END,
       NULL,
       b.ScheduledAt
FROM billed b;

-- ---------------------------------------------------------------------------
-- 10) Payments for paid / partially-paid invoices
-- ---------------------------------------------------------------------------
INSERT INTO Payments (Id, InvoiceId, Amount, Method, PaidAt, ReceivedByUserId, Reference, CreatedAt)
SELECT NEWID(), i.Id, i.PaidAmount,
       1 + (ABS(CHECKSUM(i.Id)) % 4),                                  -- Method 1..4
       TODATETIMEOFFSET(DATEADD(day, 0, CAST(i.IssueDate AS datetime2(0))), @tz),
       @URecep,                                                        -- ReceivedByUserId
       CONCAT(N'REF-', RIGHT('000000' + CAST(ABS(CHECKSUM(i.Id)) % 999999 AS varchar(10)), 6)),
       DATEADD(day, 0, CAST(i.IssueDate AS datetimeoffset))
FROM Invoices i
WHERE i.BranchId IN (SELECT Id FROM @Branches)
  AND i.Status IN (3, 4);

-- ---------------------------------------------------------------------------
-- 11) Treatment plans (first 8 patients) with two items each
-- ---------------------------------------------------------------------------
DECLARE @Plans TABLE (Id uniqueidentifier, PatientId uniqueidentifier, Idx int);
INSERT INTO @Plans
SELECT NEWID(), p.Id, p.Idx
FROM @Pat p
WHERE p.BranchIdx = 0 AND p.Idx < 8;

INSERT INTO TreatmentPlans (Id, PatientId, ProfessionalId, Title, Description, Status, CreatedAt)
SELECT pl.Id, pl.PatientId,
       CHOOSE(1 + pl.Idx % 3, @UDen1, @UDen2, @UDen3),
       CONCAT(N'Plan de tratamiento ', pl.Idx + 1),
       N'Plan integral acordado con el paciente.',
       CASE WHEN pl.Idx % 3 = 0 THEN 3 WHEN pl.Idx % 3 = 1 THEN 2 ELSE 1 END,  -- Completed/Active/Draft
       DATEADD(day, -(25 - pl.Idx), @Now)
FROM @Plans pl;

INSERT INTO TreatmentPlanItems (Id, TreatmentPlanId, ServiceId, Quantity, EstimatedPrice, Status, ToothNumber, AppointmentDetailId, CreatedAt)
SELECT NEWID(), pl.Id, s.Id, 1, s.Price,
       CASE WHEN pl.Idx % 2 = 0 THEN 3 ELSE 1 END,                    -- Completed / Pending
       CHOOSE(1 + (pl.Idx + it.n) % 4, 11, 21, 36, 46),
       NULL,
       DATEADD(day, -(25 - pl.Idx), @Now)
FROM @Plans pl
CROSS APPLY (VALUES (0),(1)) it(n)
JOIN @Svc s ON s.Idx = (pl.Idx + it.n * 3 + 2) % 10;

-- ---------------------------------------------------------------------------
-- 12) Summary
-- ---------------------------------------------------------------------------
DECLARE @Msg nvarchar(max) = CONCAT(
    N'Seed completed. ',
    N'Branches=',    (SELECT COUNT(*) FROM Branches WHERE Id IN (SELECT Id FROM @Branches)),
    N', Users=',     (SELECT COUNT(*) FROM Users WHERE Id IN (@UAdmin,@UDen1,@UDen2,@UDen3,@UAsst,@URecep)),
    N', Patients=',  (SELECT COUNT(*) FROM Patients WHERE BranchId IN (SELECT Id FROM @Branches)),
    N', Appointments=', (SELECT COUNT(*) FROM Appointments WHERE BranchId IN (SELECT Id FROM @Branches)),
    N', Invoices=',  (SELECT COUNT(*) FROM Invoices WHERE BranchId IN (SELECT Id FROM @Branches)),
    N', Payments=',  (SELECT COUNT(*) FROM Payments p JOIN Invoices i ON i.Id=p.InvoiceId WHERE i.BranchId IN (SELECT Id FROM @Branches)));
PRINT @Msg;

DROP TABLE #Appt;
COMMIT TRAN;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0 ROLLBACK TRAN;
    DECLARE @err nvarchar(max) = ERROR_MESSAGE();
    RAISERROR (N'Seed failed: %s', 16, 1, @err);
END CATCH;
