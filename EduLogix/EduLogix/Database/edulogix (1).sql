-- phpMyAdmin SQL Dump
-- version 5.2.1
-- https://www.phpmyadmin.net/
--
-- Host: 127.0.0.1
-- Generation Time: Mar 21, 2026 at 04:47 PM
-- Server version: 10.4.32-MariaDB
-- PHP Version: 8.2.12

SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";


/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `edulogix`
CREATE DATABASE IF NOT EXISTS edulogix;
USE edulogix;
--

-- --------------------------------------------------------

--
-- Table structure for table `reg_attendance`
--

CREATE TABLE `reg_attendance` (
  `rfid_number` varchar(255) NOT NULL,
  `student_num` varchar(20) DEFAULT NULL,
  `student_name` varchar(255) DEFAULT NULL,
  `grade_level` int(11) DEFAULT NULL,
  `section` varchar(50) DEFAULT NULL,
  `status` varchar(255) NOT NULL,
  `date_and_time` datetime DEFAULT NULL,
  `education` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `reg_attendance`
--

INSERT INTO `reg_attendance` (`rfid_number`, `student_num`, `student_name`, `grade_level`, `section`, `status`, `date_and_time`, `education`) VALUES
('04 43 71 01 6e 09 03', '20240357-C', 'ALBA, JAMES GOMEZ', 6, '1A', 'departed', NULL, 'elementary'),
('04 73 46 01 57 09 03', '20240238-C', 'ALMONIA, JOHN VINCENT AYUCO', 5, '1A', 'departed', NULL, 'elementary'),
('04 73 cc 01 38 09 03', '20240538-C', 'CAGOMOC, EDDION DEREK R.', 7, '1A', 'departed', NULL, 'junior'),
('04 73 cc 01 38 09 03', '20240538-C', 'CAGOMOC, EDDION DEREK R.', 7, '1A', 'in premises', NULL, 'junior'),
('04 43 71 01 6e 09 03', '20240357-C', 'ALBA, JAMES GOMEZ', 6, '1A', 'departed', NULL, 'elementary'),
('04 43 83 01 26 09 03', '20240542-C', 'ALAPIDE, RALPH LAUREN AMUTAN', 8, '1A', 'in premises', NULL, 'junior'),
('04 43 71 01 6e 09 03', '20240357-C', 'ALBA, JAMES GOMEZ', 6, '1A', 'departed', NULL, 'elementary'),
('04 73 9b 01 32 09 03', '20240136-C', 'AQUINO, JESTINE ASHLEY BONGAT', 4, '1A', 'in premises', NULL, 'elementary'),
('11001', '20261001-C', 'DELA CRUZ, JUAN', 3, 'A', 'in premises', '2026-01-05 08:12:11', 'elementary'),
('11002', '20261002-C', 'SANTOS, MARIA', 5, 'B', 'departed', '2026-01-06 08:22:45', 'elementary'),
('11003', '20261003-C', 'REYES, CARLO', 6, 'C', 'in premises', '2026-01-07 08:35:12', 'elementary'),
('11004', '20261004-C', 'GARCIA, ANNA', 2, 'A', 'departed', '2026-01-08 08:05:33', 'elementary'),
('11005', '20261005-C', 'MENDOZA, PAOLO', 4, 'B', 'in premises', '2026-01-09 08:44:21', 'elementary'),
('11006', '20261006-C', 'RAMOS, KAREN', 1, 'C', 'in premises', '2026-01-10 08:11:09', 'elementary'),
('11007', '20261007-C', 'TORRES, LUIS', 6, 'A', 'departed', '2026-01-11 08:28:14', 'elementary'),
('11008', '20261008-C', 'FLORES, ANGEL', 3, 'B', 'in premises', '2026-01-12 08:49:02', 'elementary'),
('11009', '20261009-C', 'CASTRO, JAMES', 2, 'C', 'departed', '2026-01-13 08:19:37', 'elementary'),
('11010', '20261010-C', 'CRUZ, NICOLE', 5, 'A', 'in premises', '2026-01-14 08:07:55', 'elementary'),
('11011', '20261011-C', 'BAUTISTA, JOHN', 7, 'B', 'departed', '2026-01-15 08:31:10', 'junior'),
('11012', '20261012-C', 'AQUINO, JESSA', 8, 'C', 'in premises', '2026-01-16 08:52:44', 'junior'),
('11013', '20261013-C', 'VILLANUEVA, MARK', 9, 'A', 'departed', '2026-01-17 08:14:29', 'junior'),
('11014', '20261014-C', 'LOPEZ, KIM', 10, 'B', 'in premises', '2026-01-18 08:40:11', 'junior'),
('11015', '20261015-C', 'GONZALES, RYAN', 7, 'C', 'departed', '2026-01-19 08:18:05', 'junior'),
('11016', '20261016-C', 'NAVARRO, JAMES', 8, 'A', 'in premises', '2026-01-20 08:55:21', 'junior'),
('11017', '20261017-C', 'PEREZ, ALLEN', 9, 'B', 'departed', '2026-01-21 08:23:36', 'junior'),
('11018', '20261018-C', 'RODRIGUEZ, IVAN', 10, 'C', 'in premises', '2026-01-22 08:09:48', 'junior'),
('11019', '20261019-C', 'DOMINGO, ERIKA', 7, 'A', 'departed', '2026-01-23 08:37:59', 'junior'),
('11020', '20261020-C', 'DE GUZMAN, PAUL', 8, 'B', 'in premises', '2026-01-24 08:16:22', 'junior'),
('11021', '20261021-C', 'SALAZAR, KYLE', 11, 'C', 'departed', '2026-01-25 08:41:10', 'senior'),
('11022', '20261022-C', 'ORTEGA, LARA', 12, 'A', 'in premises', '2026-01-26 08:13:34', 'senior'),
('11023', '20261023-C', 'PADILLA, JEROME', 11, 'B', 'departed', '2026-01-27 08:58:01', 'senior'),
('11024', '20261024-C', 'SANTIAGO, JANELLE', 12, 'C', 'in premises', '2026-01-28 08:25:43', 'senior'),
('11025', '20261025-C', 'RAMIREZ, KEVIN', 11, 'A', 'departed', '2026-01-29 08:06:19', 'senior'),
('11026', '20261026-C', 'VELASCO, JOHN', 12, 'B', 'in premises', '2026-02-01 08:32:11', 'senior'),
('11027', '20261027-C', 'ALVAREZ, CARLO', 11, 'C', 'departed', '2026-02-02 08:21:44', 'senior'),
('11028', '20261028-C', 'RIVERA, NATHAN', 12, 'A', 'in premises', '2026-02-03 08:14:08', 'senior'),
('11029', '20261029-C', 'FERNANDEZ, KIM', 11, 'B', 'departed', '2026-02-04 08:53:36', 'senior'),
('11030', '20261030-C', 'HERNANDEZ, JOSH', 12, 'C', 'in premises', '2026-02-05 08:07:25', 'senior'),
('10037', '20220737-C', 'SANTOS, CHRISTIAN BERGONIO', 0, 'A', 'in premises', '2026-01-03 08:12:15', 'elementary'),
('10029', '20220852-N', 'PAMBID, EMMAN LUTHER', 0, 'B', 'in premises', '2026-01-05 08:45:22', 'elementary'),
('04 43 EF 01 47 09 03', '20230222-N', 'LEONEN, SHAWN', 0, 'A', 'in premises', '2026-01-07 08:33:11', 'elementary'),
('10035', '20230974-C', 'ROSALES, ROVIC CANILLAS', 0, 'B', 'in premises', '2026-01-09 08:05:44', 'elementary'),
('10040', '20231517-C', 'SORIANO, JAMES AGNO', 0, 'A', 'in premises', '2026-01-11 08:27:39', 'elementary'),
('10043', '20231725-N', 'VILLAREAL, CINJHI QUILANTANG', 0, 'B', 'in premises', '2026-01-13 08:18:50', 'elementary'),
('10045', '20231952-C', 'YOINGCO, DAN DEXTER PACLIBAR', 0, 'A', 'in premises', '2026-01-15 08:56:12', 'elementary'),
('10033', '20231953-C', 'RANIDO ALLEN JHORDAN D', 0, 'B', 'in premises', '2026-01-17 08:22:41', 'elementary'),
('04 83 02 01 E2 09 03', '20240092-C', 'ALCAIDE, ZYRUS PASION', 0, 'A', 'in premises', '2026-01-19 08:10:05', 'elementary'),
('10041', '20240093-C', 'TARROZA, NATHANIEL CIRIACO', 0, 'B', 'in premises', '2026-01-21 08:48:30', 'elementary'),
('04 73 FB 01 B1 09 03', '20240097-C', 'ENRIQUEZ, SHAN DAVID H', 0, 'A', 'in premises', '2026-01-23 08:35:17', 'junior'),
('04 73 DA 01 91 09 03', '20240118-C', 'MAROMAS, BENJO QUIZON', 0, 'B', 'in premises', '2026-01-25 08:14:28', 'junior'),
('04 43 6B 01 E0 09 03', '20240131-C', 'CAMACHO, KYLE CHRISTIAN BELLENA', 0, 'A', 'in premises', '2026-01-27 08:59:02', 'junior'),
('04 73 9B 01 32 09 03', '20240136-C', 'AQUINO, JESTINE ASHLEY BONGAT', 0, 'B', 'in premises', '2026-01-29 08:06:55', 'junior'),
('20001', '20240138-C', 'ALCANTARA, ASHLEY GABRIELLE CATAPANG', 0, 'A', 'in premises', '2026-02-01 08:40:13', 'junior'),
('10032', '20240163-C', 'QUINTELA, WION MEDINA', 0, 'B', 'in premises', '2026-02-03 08:21:36', 'junior'),
('10030', '20240188-C', 'PANGILINAN, RODGE PATRICK TISMO', 0, 'A', 'in premises', '2026-02-05 08:11:49', 'junior'),
('10042', '20240191-C', 'VIÑAS, YZEKEIL EISEN CAGADAS', 0, 'B', 'in premises', '2026-02-07 08:54:20', 'junior'),
('10046', '20240194-C', 'ZANTUA, ANDREI INTO', 0, 'A', 'in premises', '2026-02-09 08:30:45', 'junior'),
('04 73 46 01 57 09 03', '20240238-C', 'ALMONIA, JOHN VINCENT AYUCO', 0, 'B', 'in premises', '2026-02-11 08:17:33', 'junior'),
('10031', '20240257-C', 'PELGONE, JUSTINE ANDRIE CEBALLOS', 0, 'A', 'in premises', '2026-02-13 08:44:09', 'senior'),
('10027', '20240320-C', 'MIRANDA, KIELL JAN ROWYN BERGONIO', 0, 'B', 'in premises', '2026-02-15 08:09:51', 'senior'),
('04 43 71 01 6E 09 03', '20240357-C', 'ALBA, JAMES GOMEZ', 0, 'A', 'in premises', '2026-02-17 08:26:14', 'senior'),
('04 73 AD 01 92 09 03', '20240381-C', 'MANGULABNAN, AEDRI TARRAGON BANTAYAN', 0, 'B', 'in premises', '2026-02-19 08:37:58', 'senior'),
('10044', '20240396-C', 'VILLAREAL, JAMIN MALLE', 0, 'A', 'in premises', '2026-02-21 08:13:27', 'senior'),
('04 73 B0 01 59 09 03', '20240441-C', 'DELOS SANTOS, ASHBEY MAGBANUA', 0, 'B', 'in premises', '2026-02-23 08:52:46', 'senior'),
('10038', '20240519-C', 'SERASPE, IRENEO III VILLAGRACIA', 0, 'A', 'in premises', '2026-02-25 08:08:19', 'senior'),
('04 73 CC 01 38 09 03', '20240538-C', 'CAGOMOC, EDDION DEREK R.', 0, 'B', 'in premises', '2026-02-26 08:41:55', 'senior'),
('04 43 83 01 26 09 03', '20240542-C', 'ALAPIDE, RALPH LAUREN AMUTAN', 0, 'A', 'in premises', '2026-02-27 08:19:22', 'senior'),
('20002', '20240553-C', 'CRUZ, ALYSSA VERANO', 0, 'B', 'in premises', '2026-02-28 08:57:10', 'senior'),
('04 43 DB 01 EE 09 03', '20240560-C', 'JUANITEZ, JAMES JHARED', 0, 'A', 'in premises', '2026-01-04 08:16:34', 'elementary'),
('04 73 51 01 B3 09 03', '20240571-C', 'DAYAPERA, JOSHUA VILLANUEVA', 0, 'B', 'in premises', '2026-01-06 08:43:12', 'elementary'),
('10034', '20240572-C', 'RIVERA, KAEL', 0, 'A', 'in premises', '2026-01-08 08:28:40', 'elementary'),
('10039', '20240578-C', 'SEVERO, JOHN ZILDJAIN ABUTIN', 0, 'B', 'in premises', '2026-01-10 08:07:25', 'elementary'),
('04 53 06 01 3C 09 03', '20240593-C', 'LOTA, DAVE', 0, 'A', 'in premises', '2026-01-12 08:49:58', 'elementary'),
('04 73 60 01 6E 09 03', '20240601-C', 'DENIÑA, MARK SHELO ADUVISO', 0, 'B', 'in premises', '2026-01-14 08:20:13', 'elementary'),
('04 73 6F 01 5C 09 03', '20240608-C', 'DELA CRUZ, RAIN JEHAN SERRANO', 0, 'A', 'in premises', '2026-01-16 08:34:29', 'junior'),
('20004', '20240610-C', 'FLORANO, CRYSTAL MANANSALA', 0, 'B', 'in premises', '2026-01-18 08:11:07', 'junior'),
('20006', '20240775-C', 'TURGO, RESHELL KYLA MORO', 0, 'A', 'in premises', '2026-01-20 08:53:16', 'junior'),
('04 73 47 01 B2 09 03', '20240837-C', 'DAGOHOY, ALVIN GIRAY', 0, 'B', 'in premises', '2026-01-22 08:24:37', 'junior'),
('04 43 5C 01 B3 09 03', '20240872-C', 'BERNARDO, EDREN VIC T', 0, 'A', 'in premises', '2026-01-24 08:36:42', 'senior'),
('04 43 76 01 06 09 03', '20240875-C', 'CASERES, MARVIN TIMBAL', 0, 'B', 'in premises', '2026-01-26 08:15:55', 'senior'),
('10028', '20240920-C', 'MONTOYA, JOHN BENEDICT RODMAR A.', 0, 'A', 'in premises', '2026-01-28 08:47:03', 'senior'),
('04 73 E8 01 78 09 03', '20240921-C', 'CUMPA, BOBBY H.', 0, 'B', 'in premises', '2026-01-30 08:09:18', 'senior'),
('20005', '20240941-C', 'PUERTA, HANNAH BATA', 0, 'A', 'in premises', '2026-02-02 08:42:27', 'senior'),
('04 73 CB 01 B7 09 03', '20240956-C', 'FABI, DAVID JOHN HOMBRE', 0, 'B', 'in premises', '2026-02-04 08:18:36', 'senior'),
('04 73 CC 01 34 09 03', '20241022-C', 'ARMIA JR., JOSEPH DUMO', 0, 'A', 'in premises', '2026-02-06 08:55:44', 'senior'),
('10026', '20241048-C', 'MEJIA, JHAN MYKHAIL RAVAGO', 0, 'B', 'in premises', '2026-02-08 08:23:59', 'senior'),
('10036', '20241164-C', 'SANTIAGO, JOSHUA L.', 0, 'A', 'in premises', '2026-02-10 08:31:12', 'senior'),
('04 73 67 01 38 09 03', '20241200-C', 'GOMEZ, GABRIELLE WINDSER ASIADO', 0, 'B', 'in premises', '2026-02-12 08:14:26', 'senior');

-- --------------------------------------------------------

--
-- Table structure for table `reg_attendance_live`
--

CREATE TABLE `reg_attendance_live` (
  `rfid_number` varchar(255) NOT NULL,
  `student_num` varchar(255) NOT NULL,
  `student_name` varchar(255) NOT NULL,
  `grade_level` varchar(255) NOT NULL,
  `section` varchar(255) NOT NULL,
  `status` varchar(255) NOT NULL,
  `date_and_time` datetime DEFAULT NULL,
  `education` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `reg_attendance_live`
--

INSERT INTO `reg_attendance_live` (`rfid_number`, `student_num`, `student_name`, `grade_level`, `section`, `status`, `date_and_time`, `education`) VALUES
('04 73 cc 01 38 09 03', '20240538-C', 'CAGOMOC, EDDION DEREK R.', '7', '1A', 'in premises', NULL, 'junior'),
('04 43 83 01 26 09 03', '20240542-C', 'ALAPIDE, RALPH LAUREN AMUTAN', '8', '1A', 'in premises', NULL, 'junior'),
('04 73 9b 01 32 09 03', '20240136-C', 'AQUINO, JESTINE ASHLEY BONGAT', '4', '1A', 'in premises', NULL, 'elementary');

-- --------------------------------------------------------

--
-- Table structure for table `reg_dashboard`
--

CREATE TABLE `reg_dashboard` (
  `elementary` int(11) DEFAULT NULL,
  `junior_high` int(11) DEFAULT NULL,
  `senior_high` int(11) DEFAULT NULL,
  `in_premises` int(11) DEFAULT NULL,
  `arrived` int(11) DEFAULT NULL,
  `departed` int(11) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `reg_dashboard`
--

INSERT INTO `reg_dashboard` (`elementary`, `junior_high`, `senior_high`, `in_premises`, `arrived`, `departed`) VALUES
(0, 0, 52, 12, 45, 33);

-- --------------------------------------------------------

--
-- Table structure for table `reg_logs`
--

CREATE TABLE `reg_logs` (
  `name` varchar(256) NOT NULL,
  `role` varchar(256) NOT NULL,
  `action` varchar(256) NOT NULL,
  `date_and_time` datetime NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `reg_logs`
--

INSERT INTO `reg_logs` (`name`, `role`, `action`, `date_and_time`) VALUES
('Admin User', 'Administrator', 'System Startup', '2026-02-09 06:00:00'),
('Registrar Staff', 'Registrar', 'Imported Official Class List', '2026-02-09 08:30:00'),
('System', 'Automated', 'Database Reset & Clean', '2026-02-09 08:31:00');

-- --------------------------------------------------------

--
-- Table structure for table `reg_regstudents`
--

CREATE TABLE `reg_regstudents` (
  `student_num` varchar(20) NOT NULL,
  `name` varchar(256) NOT NULL,
  `grade_level` int(11) NOT NULL,
  `section` varchar(256) NOT NULL,
  `date_registered` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `reg_regstudents`
--

INSERT INTO `reg_regstudents` (`student_num`, `name`, `grade_level`, `section`, `date_registered`) VALUES
('20220737-C', 'SANTOS, CHRISTIAN BERGONIO', 1, 'BSCS - 1A', '2026-02-13'),
('20220852-N', 'PAMBID, EMMAN LUTHER', 1, 'BSCS - 1A', '2026-02-13'),
('20230222-N', 'LEONEN, SHAWN', 1, 'BSCS - 1A', '2026-02-13'),
('20230974-C', 'ROSALES, ROVIC CANILLAS', 1, 'BSCS - 1A', '2026-02-13'),
('20231517-C', 'SORIANO, JAMES AGNO', 1, 'BSCS - 1A', '2026-02-13'),
('20231725-N', 'VILLAREAL, CINJHI QUILANTANG', 1, 'BSCS - 1A', '2026-02-13'),
('20231952-C', 'YOINGCO, DAN DEXTER PACLIBAR', 1, 'BSCS - 1A', '2026-02-13'),
('20231953-C', 'RANIDO ALLEN JHORDAN D', 1, 'BSCS - 1A', '2026-02-13'),
('20240092-C', 'ALCAIDE, ZYRUS PASION', 1, 'BSCS - 1A', '2026-02-13'),
('20240093-C', 'TARROZA, NATHANIEL CIRIACO', 1, 'BSCS - 1A', '2026-02-13'),
('20240097-C', 'ENRIQUEZ, SHAN DAVID H', 1, 'BSCS - 1A', '2026-02-13'),
('20240118-C', 'MAROMAS, BENJO QUIZON', 1, 'BSCS - 1A', '2026-02-13'),
('20240131-C', 'CAMACHO, KYLE CHRISTIAN BELLENA', 1, 'BSCS - 1A', '2026-02-13'),
('20240136-C', 'AQUINO, JESTINE ASHLEY BONGAT', 1, 'BSCS - 1A', '2026-02-13'),
('20240138-C', 'ALCANTARA, ASHLEY GABRIELLE CATAPANG', 1, 'BSCS - 1A', '2026-02-13'),
('20240163-C', 'QUINTELA, WION MEDINA', 1, 'BSCS - 1A', '2026-02-13'),
('20240188-C', 'PANGILINAN, RODGE PATRICK TISMO', 1, 'BSCS - 1A', '2026-02-13'),
('20240191-C', 'VIÑAS, YZEKEIL EISEN CAGADAS', 1, 'BSCS - 1A', '2026-02-13'),
('20240194-C', 'ZANTUA, ANDREI INTO', 1, 'BSCS - 1A', '2026-02-13'),
('20240238-C', 'ALMONIA, JOHN VINCENT AYUCO', 1, 'BSCS - 1A', '2026-02-13'),
('20240257-C', 'PELGONE, JUSTINE ANDRIE CEBALLOS', 1, 'BSCS - 1A', '2026-02-13'),
('20240320-C', 'MIRANDA, KIELL JAN ROWYN BERGONIO', 1, 'BSCS - 1A', '2026-02-13'),
('20240357-C', 'ALBA, JAMES GOMEZ', 1, 'BSCS - 1A', '2026-02-13'),
('20240381-C', 'MANGULABNAN, AEDRI TARRAGON BANTAYAN', 1, 'BSCS - 1A', '2026-02-13'),
('20240396-C', 'VILLAREAL, JAMIN MALLE', 1, 'BSCS - 1A', '2026-02-13'),
('20240441-C', 'DELOS SANTOS, ASHBEY MAGBANUA', 1, 'BSCS - 1A', '2026-02-13'),
('20240519-C', 'SERASPE, IRENEO III VILLAGRACIA', 1, 'BSCS - 1A', '2026-02-13'),
('20240538-C', 'CAGOMOC, EDDION DEREK R.', 1, 'BSCS - 1A', '2026-02-13'),
('20240542-C', 'ALAPIDE, RALPH LAUREN AMUTAN', 1, 'BSCS - 1A', '2026-02-13'),
('20240553-C', 'CRUZ, ALYSSA VERANO', 1, 'BSCS - 1A', '2026-02-13'),
('20240560-C', 'JUANITEZ, JAMES JHARED', 1, 'BSCS - 1A', '2026-02-13'),
('20240571-C', 'DAYAPERA, JOSHUA VILLANUEVA', 1, 'BSCS - 1A', '2026-02-13'),
('20240572-C', 'RIVERA, KAEL', 1, 'BSCS - 1A', '2026-02-13'),
('20240578-C', 'SEVERO, JOHN ZILDJAIN ABUTIN', 1, 'BSCS - 1A', '2026-02-13'),
('20240593-C', 'LOTA, DAVE', 1, 'BSCS - 1A', '2026-02-13'),
('20240601-C', 'DENIÑA, MARK SHELO ADUVISO', 1, 'BSCS - 1A', '2026-02-13'),
('20240608-C', 'DELA CRUZ, RAIN JEHAN SERRANO', 1, 'BSCS - 1A', '2026-02-13'),
('20240610-C', 'FLORANO, CRYSTAL MANANSALA', 1, 'BSCS - 1A', '2026-02-13'),
('20240775-C', 'TURGO, RESHELL KYLA MORO', 1, 'BSCS - 1A', '2026-02-13'),
('20240837-C', 'DAGOHOY, ALVIN GIRAY', 1, 'BSCS - 1A', '2026-02-13'),
('20240872-C', 'BERNARDO, EDREN VIC T', 1, 'BSCS - 1A', '2026-02-13'),
('20240875-C', 'CASERES, MARVIN TIMBAL', 1, 'BSCS - 1A', '2026-02-13'),
('20240920-C', 'MONTOYA, JOHN BENEDICT RODMAR A.', 1, 'BSCS - 1A', '2026-02-13'),
('20240921-C', 'CUMPA, BOBBY H.', 1, 'BSCS - 1A', '2026-02-13'),
('20240941-C', 'PUERTA, HANNAH BATA', 1, 'BSCS - 1A', '2026-02-13'),
('20240956-C', 'FABI, DAVID JOHN HOMBRE', 1, 'BSCS - 1A', '2026-02-13'),
('20241022-C', 'ARMIA JR., JOSEPH DUMO', 1, 'BSCS - 1A', '2026-02-13'),
('20241048-C', 'MEJIA, JHAN MYKHAIL RAVAGO', 1, 'BSCS - 1A', '2026-02-13'),
('20241164-C', 'SANTIAGO, JOSHUA L.', 1, 'BSCS - 1A', '2026-02-13'),
('20241200-C', 'GOMEZ, GABRIELLE WINDSER ASIADO', 1, 'BSCS - 1A', '2026-02-13'),
('20241217-C', 'ESCABAL, ALEXA MARGARETTE A.', 1, 'BSCS - 1A', '2026-02-13'),
('20241555-C', 'GONZAL, JAEL PATAJO', 1, 'BSCS - 1A', '2026-02-13');

-- --------------------------------------------------------

--
-- Table structure for table `reg_settings`
--

CREATE TABLE `reg_settings` (
  `school_name` varchar(256) NOT NULL,
  `auto_logout` tinyint(1) NOT NULL,
  `school_logo` varchar(256) NOT NULL,
  `kiosk_idle_slideshow` varchar(256) NOT NULL,
  `slideshow_duration` int(11) NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `reg_settings`
--

INSERT INTO `reg_settings` (`school_name`, `auto_logout`, `school_logo`, `kiosk_idle_slideshow`, `slideshow_duration`) VALUES
('Edulogix High School', 1, 'assets/logo.png', 'assets/idle_loop.mp4', 30);

-- --------------------------------------------------------

--
-- Table structure for table `reg_studentinfo`
--

CREATE TABLE `reg_studentinfo` (
  `id` int(11) NOT NULL,
  `image_path` varchar(256) NOT NULL,
  `rfid_number` varchar(256) NOT NULL,
  `student_id` varchar(20) NOT NULL,
  `name` varchar(256) NOT NULL,
  `phone_number` varchar(20) NOT NULL DEFAULT '09000000000',
  `guardian_name` varchar(256) NOT NULL DEFAULT 'N/A',
  `guardian_phone_number` varchar(20) NOT NULL DEFAULT '09000000000',
  `address` varchar(256) NOT NULL DEFAULT 'Caloocan City',
  `grade` int(11) DEFAULT NULL,
  `section` varchar(10) DEFAULT NULL,
  `level` varchar(20) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `reg_studentinfo`
--

INSERT INTO `reg_studentinfo` (`id`, `image_path`, `rfid_number`, `student_id`, `name`, `phone_number`, `guardian_name`, `guardian_phone_number`, `address`, `grade`, `section`, `level`) VALUES
(1, 'uploads/default.png', '10037', '20220737-C', 'SANTOS, CHRISTIAN BERGONIO', '09000000000', 'N/A', '09000000000', 'Caloocan City', 1, '1A', 'elementary'),
(2, 'uploads/default.png', '10029', '20220852-N', 'PAMBID, EMMAN LUTHER', '09000000000', 'N/A', '09000000000', 'Caloocan City', 1, '1A', 'elementary'),
(3, 'uploads/default.png', '04 43 EF 01 47 09 03', '20230222-N', 'LEONEN, SHAWN', '09000000000', 'N/A', '09000000000', 'Caloocan City', 1, '1A', 'elementary'),
(4, 'uploads/default.png', '10035', '20230974-C', 'ROSALES, ROVIC CANILLAS', '09000000000', 'N/A', '09000000000', 'Caloocan City', 1, '1A', 'elementary'),
(5, 'uploads/default.png', '10040', '20231517-C', 'SORIANO, JAMES AGNO', '09000000000', 'N/A', '09000000000', 'Caloocan City', 2, '1A', 'elementary'),
(6, 'uploads/default.png', '10043', '20231725-N', 'VILLAREAL, CINJHI QUILANTANG', '09000000000', 'N/A', '09000000000', 'Caloocan City', 2, '1A', 'elementary'),
(7, 'uploads/default.png', '10045', '20231952-C', 'YOINGCO, DAN DEXTER PACLIBAR', '09000000000', 'N/A', '09000000000', 'Caloocan City', 2, '1A', 'elementary'),
(8, 'uploads/default.png', '10033', '20231953-C', 'RANIDO ALLEN JHORDAN D', '09000000000', 'N/A', '09000000000', 'Caloocan City', 2, '1A', 'elementary'),
(9, 'uploads/default.png', '04 83 02 01 E2 09 03', '20240092-C', 'ALCAIDE, ZYRUS PASION', '09000000000', 'N/A', '09000000000', 'Manila', 3, '1A', 'elementary'),
(10, 'uploads/default.png', '10041', '20240093-C', 'TARROZA, NATHANIEL CIRIACO', '09000000000', 'N/A', '09000000000', 'Caloocan City', 3, '1A', 'elementary'),
(11, 'uploads/default.png', '04 73 FB 01 B1 09 03', '20240097-C', 'ENRIQUEZ, SHAN DAVID H', '09000000000', 'N/A', '09000000000', 'Caloocan City', 3, '1A', 'elementary'),
(12, 'uploads/default.png', '04 73 DA 01 91 09 03', '20240118-C', 'MAROMAS, BENJO QUIZON', '09000000000', 'N/A', '09000000000', 'Caloocan City', 3, '1A', 'elementary'),
(13, 'uploads/default.png', '04 43 6B 01 E0 09 03', '20240131-C', 'CAMACHO, KYLE CHRISTIAN BELLENA', '09000000000', 'N/A', '09000000000', 'Caloocan City', 4, '1A', 'elementary'),
(14, 'uploads/default.png', '04 73 9B 01 32 09 03', '20240136-C', 'AQUINO, JESTINE ASHLEY BONGAT', '09000000000', 'N/A', '09000000000', 'Caloocan City', 4, '1A', 'elementary'),
(15, 'uploads/default.png', '20001', '20240138-C', 'ALCANTARA, ASHLEY GABRIELLE CATAPANG', '09000000000', 'N/A', '09000000000', 'Caloocan City', 4, '1A', 'elementary'),
(16, 'uploads/default.png', '10032', '20240163-C', 'QUINTELA, WION MEDINA', '09000000000', 'N/A', '09000000000', 'Caloocan City', 4, '1A', 'elementary'),
(17, 'uploads/default.png', '10030', '20240188-C', 'PANGILINAN, RODGE PATRICK TISMO', '09000000000', 'N/A', '09000000000', 'Caloocan City', 5, '1A', 'elementary'),
(18, 'uploads/default.png', '10042', '20240191-C', 'VIÑAS, YZEKEIL EISEN CAGADAS', '09000000000', 'N/A', '09000000000', 'Caloocan City', 5, '1A', 'elementary'),
(19, 'uploads/default.png', '10046', '20240194-C', 'ZANTUA, ANDREI INTO', '09000000000', 'N/A', '09000000000', 'Caloocan City', 5, '1A', 'elementary'),
(20, 'uploads/default.png', '04 73 46 01 57 09 03', '20240238-C', 'ALMONIA, JOHN VINCENT AYUCO', '09000000000', 'N/A', '09000000000', 'Quezon City', 5, '1A', 'elementary'),
(21, 'uploads/default.png', '10031', '20240257-C', 'PELGONE, JUSTINE ANDRIE CEBALLOS', '09000000000', 'N/A', '09000000000', 'Caloocan City', 6, '1A', 'elementary'),
(22, 'uploads/default.png', '10027', '20240320-C', 'MIRANDA, KIELL JAN ROWYN BERGONIO', '09000000000', 'N/A', '09000000000', 'Caloocan City', 6, '1A', 'elementary'),
(23, 'uploads/default.png', '04 43 71 01 6E 09 03', '20240357-C', 'ALBA, JAMES GOMEZ', '09000000000', 'N/A', '09000000000', 'Caloocan City', 6, '1A', 'elementary'),
(24, 'uploads/default.png', '04 73 AD 01 92 09 03', '20240381-C', 'MANGULABNAN, AEDRI TARRAGON BANTAYAN', '09000000000', 'N/A', '09000000000', 'Caloocan City', 6, '1A', 'elementary'),
(25, 'uploads/default.png', '10044', '20240396-C', 'VILLAREAL, JAMIN MALLE', '09000000000', 'N/A', '09000000000', 'Caloocan City', 7, '1A', 'junior'),
(26, 'uploads/default.png', '04 73 B0 01 59 09 03', '20240441-C', 'DELOS SANTOS, ASHBEY MAGBANUA', '09000000000', 'N/A', '09000000000', 'Caloocan City', 7, '1A', 'junior'),
(27, 'uploads/default.png', '10038', '20240519-C', 'SERASPE, IRENEO III VILLAGRACIA', '09000000000', 'N/A', '09000000000', 'Caloocan City', 7, '1A', 'junior'),
(28, 'uploads/default.png', '04 73 CC 01 38 09 03', '20240538-C', 'CAGOMOC, EDDION DEREK R.', '09000000000', 'N/A', '09000000000', 'Caloocan City', 7, '1A', 'junior'),
(29, 'uploads/default.png', '04 43 83 01 26 09 03', '20240542-C', 'ALAPIDE, RALPH LAUREN AMUTAN', '09000000000', 'N/A', '09000000000', 'Caloocan City', 8, '1A', 'junior'),
(30, 'uploads/default.png', '20002', '20240553-C', 'CRUZ, ALYSSA VERANO', '09000000000', 'N/A', '09000000000', 'Caloocan City', 8, '1A', 'junior'),
(31, 'uploads/default.png', '04 43 DB 01 EE 09 03', '20240560-C', 'JUANITEZ, JAMES JHARED', '09000000000', 'N/A', '09000000000', 'Caloocan City', 8, '1A', 'junior'),
(32, 'uploads/default.png', '04 73 51 01 B3 09 03', '20240571-C', 'DAYAPERA, JOSHUA VILLANUEVA', '09000000000', 'N/A', '09000000000', 'Caloocan City', 8, '1A', 'junior'),
(33, 'uploads/default.png', '10034', '20240572-C', 'RIVERA, KAEL', '09000000000', 'N/A', '09000000000', 'Caloocan City', 9, '1A', 'junior'),
(34, 'uploads/default.png', '10039', '20240578-C', 'SEVERO, JOHN ZILDJAIN ABUTIN', '09000000000', 'N/A', '09000000000', 'Caloocan City', 9, '1A', 'junior'),
(35, 'uploads/default.png', '04 53 06 01 3C 09 03', '20240593-C', 'LOTA, DAVE', '09000000000', 'N/A', '09000000000', 'Caloocan City', 9, '1A', 'junior'),
(36, 'uploads/default.png', '04 73 60 01 6E 09 03', '20240601-C', 'DENIÑA, MARK SHELO ADUVISO', '09000000000', 'N/A', '09000000000', 'Caloocan City', 9, '1A', 'junior'),
(37, 'uploads/default.png', '04 73 6F 01 5C 09 03', '20240608-C', 'DELA CRUZ, RAIN JEHAN SERRANO', '09000000000', 'N/A', '09000000000', 'Bulacan', 10, '1A', 'junior'),
(38, 'uploads/default.png', '20004', '20240610-C', 'FLORANO, CRYSTAL MANANSALA', '09000000000', 'N/A', '09000000000', 'Caloocan City', 10, '1A', 'junior'),
(39, 'uploads/default.png', '20006', '20240775-C', 'TURGO, RESHELL KYLA MORO', '09000000000', 'N/A', '09000000000', 'Caloocan City', 10, '1A', 'junior'),
(40, 'uploads/default.png', '04 73 47 01 B2 09 03', '20240837-C', 'DAGOHOY, ALVIN GIRAY', '09000000000', 'N/A', '09000000000', 'Caloocan City', 10, '1A', 'junior'),
(41, 'uploads/default.png', '04 43 5C 01 B3 09 03', '20240872-C', 'BERNARDO, EDREN VIC T', '09000000000', 'N/A', '09000000000', 'Navotas', 11, '1A', 'senior'),
(42, 'uploads/default.png', '04 43 76 01 06 09 03', '20240875-C', 'CASERES, MARVIN TIMBAL', '09000000000', 'N/A', '09000000000', 'Malabon', 11, '1A', 'senior'),
(43, 'uploads/default.png', '10028', '20240920-C', 'MONTOYA, JOHN BENEDICT RODMAR A.', '09000000000', 'N/A', '09000000000', 'Caloocan City', 11, '1A', 'senior'),
(44, 'uploads/default.png', '04 73 E8 01 78 09 03', '20240921-C', 'CUMPA, BOBBY H.', '09000000000', 'N/A', '09000000000', 'Caloocan City', 11, '1A', 'senior'),
(45, 'uploads/default.png', '20005', '20240941-C', 'PUERTA, HANNAH BATA', '09000000000', 'N/A', '09000000000', 'Caloocan City', 11, '1A', 'senior'),
(46, 'uploads/default.png', '04 73 CB 01 B7 09 03', '20240956-C', 'FABI, DAVID JOHN HOMBRE', '09000000000', 'N/A', '09000000000', 'Caloocan City', 11, '1A', 'senior'),
(47, 'uploads/default.png', '04 73 CC 01 34 09 03', '20241022-C', 'ARMIA JR., JOSEPH DUMO', '09000000000', 'N/A', '09000000000', 'Caloocan City', 12, '1A', 'senior'),
(48, 'uploads/default.png', '10026', '20241048-C', 'MEJIA, JHAN MYKHAIL RAVAGO', '09000000000', 'N/A', '09000000000', 'Caloocan City', 12, '1A', 'senior'),
(49, 'uploads/default.png', '10036', '20241164-C', 'SANTIAGO, JOSHUA L.', '09000000000', 'N/A', '09000000000', 'Caloocan City', 12, '1A', 'senior'),
(50, 'uploads/default.png', '04 73 67 01 38 09 03', '20241200-C', 'GOMEZ, GABRIELLE WINDSER ASIADO', '09000000000', 'N/A', '09000000000', 'Caloocan City', 12, '1A', 'senior'),
(51, 'uploads/default.png', '20003', '20241217-C', 'ESCABAL, ALEXA MARGARETTE A.', '09000000000', 'N/A', '09000000000', 'Caloocan City', 12, '1A', 'senior'),
(52, 'uploads/default.png', '04 73 94 01 6D 09 03', '20241555-C', 'GONZAL, JAEL PATAJO', '09000000000', 'N/A', '09000000000', 'Caloocan City', 12, '1A', 'senior');

-- --------------------------------------------------------

--
-- Table structure for table `reg_theme`
--

CREATE TABLE `reg_theme` (
  `id` int(11) NOT NULL,
  `theme_red` tinyint(3) UNSIGNED NOT NULL DEFAULT 255,
  `theme_green` tinyint(3) UNSIGNED NOT NULL DEFAULT 255,
  `theme_blue` tinyint(3) UNSIGNED NOT NULL DEFAULT 255,
  `updated_by` varchar(100) DEFAULT NULL,
  `updated_at` datetime DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `reg_theme`
--

INSERT INTO `reg_theme` (`id`, `theme_red`, `theme_green`, `theme_blue`, `updated_by`, `updated_at`) VALUES
(1, 0, 0, 0, 'Registrar', '2026-03-21 23:05:01');

-- --------------------------------------------------------

--
-- Table structure for table `sys_users`
--

CREATE TABLE `sys_users` (
  `rfid_number` varchar(255) DEFAULT NULL,
  `username` varchar(256) NOT NULL,
  `password` varchar(255) NOT NULL,
  `role` varchar(256) DEFAULT 'user',
  `created_at` timestamp NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_general_ci;

--
-- Dumping data for table `sys_users`
--

INSERT INTO `sys_users` (`rfid_number`, `username`, `password`, `role`, `created_at`) VALUES
('04 43 77 01 07 09 03', 'Librarian', 'LibAdmin2026', 'librarian', '2026-02-13 04:56:26'),
('04 43 EB 01 54 09 03', 'Registrar', 'RegAdmin2026', 'registrar', '2026-02-13 04:56:26'),
('04 53 04 01 84 09 03', 'Security', 'Sec2026', 'security', '2026-02-13 04:56:26');

--
-- Indexes for dumped tables
--

--
-- Indexes for table `reg_regstudents`
--
ALTER TABLE `reg_regstudents`
  ADD PRIMARY KEY (`student_num`);

--
-- Indexes for table `reg_studentinfo`
--
ALTER TABLE `reg_studentinfo`
  ADD PRIMARY KEY (`student_id`),
  ADD UNIQUE KEY `id` (`id`);

--
-- Indexes for table `reg_theme`
--
ALTER TABLE `reg_theme`
  ADD PRIMARY KEY (`id`);

--
-- Indexes for table `sys_users`
--
ALTER TABLE `sys_users`
  ADD PRIMARY KEY (`username`);

--
-- AUTO_INCREMENT for dumped tables
--

--
-- AUTO_INCREMENT for table `reg_studentinfo`
--
ALTER TABLE `reg_studentinfo`
  MODIFY `id` int(11) NOT NULL AUTO_INCREMENT, AUTO_INCREMENT=53;
COMMIT;

/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
