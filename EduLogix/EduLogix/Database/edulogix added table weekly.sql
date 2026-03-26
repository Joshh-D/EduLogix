-- EduLogix Integrated Database Script with Data
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

CREATE TABLE `edulogix`.`reg_attedance_weekly` 
(`rfid_number` VARCHAR(255) NOT NULL ,
 `student_num` VARCHAR(255) NOT NULL ,
 `student_name` VARCHAR(255) NOT NULL ,
 `grade_level` VARCHAR(255) NOT NULL ,
 `section` VARCHAR(255) NOT NULL ,
 `status` VARCHAR(255) NOT NULL ,
 `date_and_time` DATETIME NOT NULL ,
 `education` VARCHAR(255) NOT NULL ) ENGINE = InnoDB;