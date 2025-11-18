-- MySQL dump 10.13  Distrib 5.7.43, for Linux (x86_64)
--
-- Host: localhost    Database: edulit_assessment_db
-- ------------------------------------------------------
-- Server version	5.7.43

/*!40101 SET @OLD_CHARACTER_SET_CLIENT=@@CHARACTER_SET_CLIENT */;
/*!40101 SET @OLD_CHARACTER_SET_RESULTS=@@CHARACTER_SET_RESULTS */;
/*!40101 SET @OLD_COLLATION_CONNECTION=@@COLLATION_CONNECTION */;
/*!40101 SET NAMES utf8 */;
/*!40103 SET @OLD_TIME_ZONE=@@TIME_ZONE */;
/*!40103 SET TIME_ZONE='+00:00' */;
/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;

--
-- Current Database: `edulit_assessment_db`
--

CREATE DATABASE /*!32312 IF NOT EXISTS*/ `edulit_assessment_db` /*!40100 DEFAULT CHARACTER SET latin1 */;

USE `edulit_assessment_db`;

--
-- Table structure for table `__EFMigrationsHistory`
--

DROP TABLE IF EXISTS `__EFMigrationsHistory`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `__EFMigrationsHistory` (
  `MigrationId` varchar(150) NOT NULL,
  `ProductVersion` varchar(32) NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `__EFMigrationsHistory`
--

LOCK TABLES `__EFMigrationsHistory` WRITE;
/*!40000 ALTER TABLE `__EFMigrationsHistory` DISABLE KEYS */;
INSERT INTO `__EFMigrationsHistory` VALUES ('20251118082220_InitialSchema','8.0.21');
/*!40000 ALTER TABLE `__EFMigrationsHistory` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `assessment_answer`
--

DROP TABLE IF EXISTS `assessment_answer`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `assessment_answer` (
  `answer_id` int(11) NOT NULL AUTO_INCREMENT,
  `assessment_question_id` int(11) NOT NULL,
  `attempts_id` int(11) NOT NULL,
  `selected_option_id` varchar(36) NOT NULL COMMENT 'Reference to QuestionOption in Question Service',
  `is_correct` tinyint(1) NOT NULL,
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (`answer_id`),
  KEY `assessment_question_id` (`assessment_question_id`),
  KEY `attempts_id` (`attempts_id`),
  KEY `idx_selected_option` (`selected_option_id`),
  CONSTRAINT `assessment_answer_ibfk_1` FOREIGN KEY (`assessment_question_id`) REFERENCES `assessment_question` (`assessment_question_id`) ON DELETE CASCADE,
  CONSTRAINT `assessment_answer_ibfk_2` FOREIGN KEY (`attempts_id`) REFERENCES `assignment_attempts` (`attempts_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `assessment_answer`
--

LOCK TABLES `assessment_answer` WRITE;
/*!40000 ALTER TABLE `assessment_answer` DISABLE KEYS */;
/*!40000 ALTER TABLE `assessment_answer` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `assessment_question`
--

DROP TABLE IF EXISTS `assessment_question`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `assessment_question` (
  `assessment_question_id` int(11) NOT NULL AUTO_INCREMENT,
  `assessment_id` int(11) NOT NULL,
  `question_id` varchar(36) NOT NULL COMMENT 'Reference to Question Service',
  `is_active` tinyint(1) DEFAULT '1',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`assessment_question_id`),
  UNIQUE KEY `idx_assessment_question` (`assessment_id`,`question_id`),
  KEY `assessment_id` (`assessment_id`),
  CONSTRAINT `assessment_question_ibfk_1` FOREIGN KEY (`assessment_id`) REFERENCES `assessments` (`assessment_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `assessment_question`
--

LOCK TABLES `assessment_question` WRITE;
/*!40000 ALTER TABLE `assessment_question` DISABLE KEYS */;
/*!40000 ALTER TABLE `assessment_question` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `assessments`
--

DROP TABLE IF EXISTS `assessments`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `assessments` (
  `assessment_id` int(11) NOT NULL AUTO_INCREMENT,
  `course_id` varchar(255) NOT NULL,
  `creator_id` varchar(255) NOT NULL,
  `title` varchar(255) NOT NULL,
  `description` longtext,
  `total_questions` int(11) NOT NULL,
  `duration_minutes` int(11) NOT NULL,
  `status` varchar(255) DEFAULT NULL,
  `is_active` tinyint(1) DEFAULT '1',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`assessment_id`)
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `assessments`
--

LOCK TABLES `assessments` WRITE;
/*!40000 ALTER TABLE `assessments` DISABLE KEYS */;
/*!40000 ALTER TABLE `assessments` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `assignment_attempts`
--

DROP TABLE IF EXISTS `assignment_attempts`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `assignment_attempts` (
  `attempts_id` int(11) NOT NULL AUTO_INCREMENT,
  `assessment_id` int(11) NOT NULL,
  `user_id` varchar(255) NOT NULL,
  `started_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `completed_at` timestamp NULL DEFAULT NULL,
  `attempt_number` int(11) NOT NULL DEFAULT '1',
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`attempts_id`),
  KEY `assessment_id1` (`assessment_id`),
  CONSTRAINT `assignment_attempts_ibfk_1` FOREIGN KEY (`assessment_id`) REFERENCES `assessments` (`assessment_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `assignment_attempts`
--

LOCK TABLES `assignment_attempts` WRITE;
/*!40000 ALTER TABLE `assignment_attempts` DISABLE KEYS */;
/*!40000 ALTER TABLE `assignment_attempts` ENABLE KEYS */;
UNLOCK TABLES;

--
-- Table structure for table `grading_feedback`
--

DROP TABLE IF EXISTS `grading_feedback`;
/*!40101 SET @saved_cs_client     = @@character_set_client */;
/*!40101 SET character_set_client = utf8 */;
CREATE TABLE `grading_feedback` (
  `feedback_id` int(11) NOT NULL AUTO_INCREMENT,
  `attempts_id` int(11) NOT NULL,
  `total_score` decimal(5,2) NOT NULL COMMENT 'Tổng điểm trên thang 10',
  `correct_count` int(11) NOT NULL COMMENT 'Số câu trả lời đúng',
  `wrong_count` int(11) NOT NULL COMMENT 'Số câu trả lời sai',
  `correct_percentage` decimal(5,2) NOT NULL COMMENT 'Phần trăm câu đúng (%)',
  `wrong_percentage` decimal(5,2) NOT NULL COMMENT 'Phần trăm câu sai (%)',
  `created_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP,
  `updated_at` timestamp NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
  PRIMARY KEY (`feedback_id`),
  UNIQUE KEY `attempts_id1` (`attempts_id`),
  CONSTRAINT `grading_feedback_ibfk_1` FOREIGN KEY (`attempts_id`) REFERENCES `assignment_attempts` (`attempts_id`) ON DELETE CASCADE
) ENGINE=InnoDB DEFAULT CHARSET=latin1;
/*!40101 SET character_set_client = @saved_cs_client */;

--
-- Dumping data for table `grading_feedback`
--

LOCK TABLES `grading_feedback` WRITE;
/*!40000 ALTER TABLE `grading_feedback` DISABLE KEYS */;
/*!40000 ALTER TABLE `grading_feedback` ENABLE KEYS */;
UNLOCK TABLES;
/*!40103 SET TIME_ZONE=@OLD_TIME_ZONE */;

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40101 SET CHARACTER_SET_CLIENT=@OLD_CHARACTER_SET_CLIENT */;
/*!40101 SET CHARACTER_SET_RESULTS=@OLD_CHARACTER_SET_RESULTS */;
/*!40101 SET COLLATION_CONNECTION=@OLD_COLLATION_CONNECTION */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;

-- Dump completed on 2025-11-18  9:54:02
