-- EduLogix Integrated Database Script
-- Updated: March 21, 2026
SET SQL_MODE = "NO_AUTO_VALUE_ON_ZERO";
START TRANSACTION;
SET time_zone = "+00:00";

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8mb4 */;

--
-- Database: `edulogix`
--
CREATE DATABASE IF NOT EXISTS edulogix;
USE edulogix;

-- --------------------------------------------------------
-- 1. reg_settings
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS `reg_settings` (
  `id` INT PRIMARY KEY AUTO_INCREMENT UNIQUE,
  `school_name` varchar(256) NOT NULL,
  `auto_logout_seconds` INT DEFAULT 0,
  `school_logo` varchar(256) NOT NULL,
  `kiosk_idle_slideshow` varchar(256) NOT NULL,
  `slideshow_duration` INT DEFAULT 30,
  `kiosk_idle_seconds` INT DEFAULT 60,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `reg_settings` (`id`, `school_name`, `auto_logout_seconds`, `school_logo`, `kiosk_idle_slideshow`, `slideshow_duration`) 
VALUES (1, 'Edulogix High School', 0, 'assets/logo.png', 'assets/idle_loop.mp4', 30);

-- --------------------------------------------------------
-- 2. reg_theme
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS `reg_theme` (
  `id` int(11) NOT NULL PRIMARY KEY,
  `theme_red` tinyint(3) UNSIGNED NOT NULL DEFAULT 255,
  `theme_green` tinyint(3) UNSIGNED NOT NULL DEFAULT 255,
  `theme_blue` tinyint(3) UNSIGNED NOT NULL DEFAULT 255,
  `updated_by` varchar(100) DEFAULT NULL,
  `updated_at` TIMESTAMP DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `reg_theme` (`id`, `theme_red`, `theme_green`, `theme_blue`, `updated_by`) VALUES
(1, 0, 0, 0, 'Registrar'),
(2, 33, 150, 243, 'System');

-- --------------------------------------------------------
-- 3. reg_logs
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS `reg_logs` (
  `name` varchar(256) NOT NULL,
  `role` varchar(256) NOT NULL,
  `action` varchar(256) NOT NULL,
  `log_date` DATETIME DEFAULT CURRENT_TIMESTAMP
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------
-- 4. reg_attendance
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS `reg_attendance` (
  `rfid_number` varchar(255) NOT NULL,
  `student_num` varchar(20) DEFAULT NULL,
  `student_name` varchar(255) DEFAULT NULL,
  `grade` int(11) DEFAULT NULL,
  `section` varchar(50) DEFAULT NULL,
  `status` varchar(255) NOT NULL,
  `attendance_date` datetime DEFAULT NULL,
  `level` varchar(255) DEFAULT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------
-- 5. reg_studentinfo
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS `reg_studentinfo` (
  `id` int(11) NOT NULL AUTO_INCREMENT UNIQUE,
  `image_path` varchar(256) NOT NULL,
  `rfid_number` varchar(256) NOT NULL,
  `student_id` varchar(20) NOT NULL PRIMARY KEY,
  `name` varchar(256) NOT NULL,
  `phone_number` varchar(20) NOT NULL DEFAULT '09000000000',
  `guardian_name` varchar(256) NOT NULL DEFAULT 'N/A',
  `guardian_phone_number` varchar(20) NOT NULL DEFAULT '09000000000',
  `address` varchar(256) NOT NULL DEFAULT 'Caloocan City',
  `grade` int(11) DEFAULT NULL,
  `section` varchar(10) DEFAULT NULL,
  `level` varchar(20) DEFAULT NULL,
  `email` VARCHAR(255) DEFAULT 'Not Provided',
  `date_of_birth` DATE DEFAULT NULL,
  `phone` VARCHAR(20) DEFAULT '09000000000'
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------
-- 6. reg_regstudents
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS `reg_regstudents` (
  `student_num` varchar(20) NOT NULL PRIMARY KEY,
  `name` varchar(256) NOT NULL,
  `grade_level` int(11) NOT NULL,
  `section` varchar(256) NOT NULL,
  `date_registered` date NOT NULL
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

-- --------------------------------------------------------
-- 7. sys_users
-- --------------------------------------------------------
CREATE TABLE IF NOT EXISTS `sys_users` (
  `rfid_number` varchar(255) DEFAULT NULL,
  `username` varchar(256) NOT NULL PRIMARY KEY,
  `password` varchar(255) NOT NULL,
  `role` varchar(256) DEFAULT 'user',
  `created_at` timestamp NULL DEFAULT current_timestamp()
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;

INSERT INTO `sys_users` (`rfid_number`, `username`, `password`, `role`) VALUES
('04 43 77 01 07 09 03', 'Librarian', 'LibAdmin2026', 'librarian'),
('04 43 EB 01 54 09 03', 'Registrar', 'RegAdmin2026', 'registrar'),
('04 53 04 01 84 09 03', 'Security', 'Sec2026', 'security');

COMMIT;