-- MySQL Script generated by MySQL Workbench
-- Sun Mar  5 15:17:50 2023
-- Model: New Model    Version: 1.0
-- MySQL Workbench Forward Engineering

SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0;
SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0;
SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='';

-- -----------------------------------------------------
-- Schema mydb
-- -----------------------------------------------------
-- -----------------------------------------------------
-- Schema laboratory_2023
-- -----------------------------------------------------

-- -----------------------------------------------------
-- Schema laboratory_2023
-- -----------------------------------------------------
CREATE SCHEMA IF NOT EXISTS `laboratory_2023` DEFAULT CHARACTER SET utf8mb4 ;
USE `laboratory_2023` ;

-- -----------------------------------------------------
-- Table `laboratory_2023`.`users`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `laboratory_2023`.`users` ;

CREATE TABLE IF NOT EXISTS `laboratory_2023`.`users` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(255) NOT NULL,
  `password` VARCHAR(255) NOT NULL,
  `password_salt` VARCHAR(255) NOT NULL,
  `first_name` VARCHAR(64) NOT NULL,
  `middle_name` VARCHAR(64) NULL DEFAULT NULL,
  `last_name` VARCHAR(64) NOT NULL,
  `is_locked` TINYINT(1) NOT NULL DEFAULT 0,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP(),
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP(),
  `deleted_at` TIMESTAMP NULL DEFAULT NULL,
  `created_by` INT(10) UNSIGNED NOT NULL,
  `updated_by` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  UNIQUE INDEX `username_UNIQUE` (`username` ASC),
  INDEX `fk_users_users1_idx` (`created_by` ASC),
  INDEX `fk_users_users2_idx` (`updated_by` ASC),
  CONSTRAINT `fk_users_users1`
    FOREIGN KEY (`created_by`)
    REFERENCES `laboratory_2023`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users_users2`
    FOREIGN KEY (`updated_by`)
    REFERENCES `laboratory_2023`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 2
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `laboratory_2023`.`profiles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `laboratory_2023`.`profiles` ;

CREATE TABLE IF NOT EXISTS `laboratory_2023`.`profiles` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP(),
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP(),
  `deleted_at` TIMESTAMP NULL DEFAULT NULL,
  `created_by` INT(10) UNSIGNED NOT NULL,
  `updated_by` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `fk_users_users1_idx` (`created_by` ASC),
  INDEX `fk_users_users2_idx` (`updated_by` ASC),
  INDEX `fk_users_profile1_idx` (`created_by` ASC),
  INDEX `fk_users_profile2_idx` (`updated_by` ASC),
  CONSTRAINT `fk_users_profile1`
    FOREIGN KEY (`created_by`)
    REFERENCES `laboratory_2023`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users_profile2`
    FOREIGN KEY (`updated_by`)
    REFERENCES `laboratory_2023`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 1051
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `laboratory_2023`.`laboratory_day`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `laboratory_2023`.`laboratory_day` ;

CREATE TABLE IF NOT EXISTS `laboratory_2023`.`laboratory_day` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `day` DATE NOT NULL,
  `profile_id` INT(10) UNSIGNED NOT NULL,
  `meters_squared_per_sample` DECIMAL(7,3) UNSIGNED NOT NULL,
  `painted_samples_count` INT(10) UNSIGNED NOT NULL,
  `painted_meters_squared` DECIMAL(7,3) UNSIGNED NOT NULL,
  `kilograms_per_meter` DECIMAL(7,3) UNSIGNED NULL DEFAULT NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP(),
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP(),
  `deleted_at` TIMESTAMP NULL DEFAULT NULL,
  `created_by` INT(10) UNSIGNED NOT NULL,
  `updated_by` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `fk_users_laboratory_day_profile1_idx` (`created_by` ASC),
  INDEX `fk_users_laboratory_day_profile2_idx` (`updated_by` ASC),
  CONSTRAINT `fk_profile_laboratory_day`
    FOREIGN KEY (`profile_id`)
    REFERENCES `laboratory_2023`.`profiles` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users_laboratory_day1`
    FOREIGN KEY (`created_by`)
    REFERENCES `laboratory_2023`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users_laboratory_day2`
    FOREIGN KEY (`updated_by`)
    REFERENCES `laboratory_2023`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `laboratory_2023`.`months`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `laboratory_2023`.`months` ;

CREATE TABLE IF NOT EXISTS `laboratory_2023`.`months` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  `slug` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  UNIQUE INDEX `slug_UNIQUE` (`slug` ASC))
ENGINE = InnoDB
AUTO_INCREMENT = 13
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `laboratory_2023`.`laboratory_months`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `laboratory_2023`.`laboratory_months` ;

CREATE TABLE IF NOT EXISTS `laboratory_2023`.`laboratory_months` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `month_id` INT(10) UNSIGNED NOT NULL,
  `laboratory_day_id` INT(10) UNSIGNED NOT NULL,
  `year` SMALLINT(5) NOT NULL,
  `kilograms` DECIMAL(7,3) UNSIGNED NULL DEFAULT NULL,
  `meters_squared` DECIMAL(7,3) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  UNIQUE INDEX `laboratory_month_id_UNIQUE` (`month_id` ASC),
  UNIQUE INDEX `laboratory_day_id_UNIQUE` (`laboratory_day_id` ASC),
  CONSTRAINT `fk_laboratory_day_laboratory_months`
    FOREIGN KEY (`laboratory_day_id`)
    REFERENCES `laboratory_2023`.`laboratory_day` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_months_laboratory_months`
    FOREIGN KEY (`month_id`)
    REFERENCES `laboratory_2023`.`months` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `laboratory_2023`.`laboratory_month_has_chemicals`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `laboratory_2023`.`laboratory_month_has_chemicals` ;

CREATE TABLE IF NOT EXISTS `laboratory_2023`.`laboratory_month_has_chemicals` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `laboratory_month_id` INT(10) UNSIGNED NOT NULL,
  `name` VARCHAR(64) NOT NULL,
  `expense_per_meter_squared` DECIMAL(7,3) UNSIGNED NOT NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP(),
  `updated_at` TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP(),
  `deleted_at` TIMESTAMP NULL DEFAULT NULL,
  `created_by` INT(10) UNSIGNED NOT NULL,
  `updated_by` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  UNIQUE INDEX `laboratory_month_has_day_id_UNIQUE` (`laboratory_month_id` ASC),
  INDEX `fk_users_laboratory_month_chemicals1_idx` (`created_by` ASC),
  INDEX `fk_users_laboratory_month_chemicals2_idx` (`updated_by` ASC),
  CONSTRAINT `fk_laboratory_month_has_chemicals_laboratory_months`
    FOREIGN KEY (`laboratory_month_id`)
    REFERENCES `laboratory_2023`.`laboratory_months` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users_laboratory_month_has_chemicals1`
    FOREIGN KEY (`created_by`)
    REFERENCES `laboratory_2023`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users_laboratory_month_has_chemicals2`
    FOREIGN KEY (`updated_by`)
    REFERENCES `laboratory_2023`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `laboratory_2023`.`permissions`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `laboratory_2023`.`permissions` ;

CREATE TABLE IF NOT EXISTS `laboratory_2023`.`permissions` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  `slug` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  UNIQUE INDEX `slug_UNIQUE` (`slug` ASC))
ENGINE = InnoDB
AUTO_INCREMENT = 19
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `laboratory_2023`.`profile_has_lengths_perimeter`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `laboratory_2023`.`profile_has_lengths_perimeter` ;

CREATE TABLE IF NOT EXISTS `laboratory_2023`.`profile_has_lengths_perimeter` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `profile_id` INT(10) UNSIGNED NOT NULL,
  `length` DECIMAL(7,3) UNSIGNED NULL DEFAULT NULL,
  `perimeter` DECIMAL(7,3) UNSIGNED NULL DEFAULT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `fk_profile_profile_has_lengths_perimeter_idx` (`profile_id` ASC),
  CONSTRAINT `fk_profile_profile_has_lengths_perimeter`
    FOREIGN KEY (`profile_id`)
    REFERENCES `laboratory_2023`.`profiles` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 1818
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `laboratory_2023`.`roles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `laboratory_2023`.`roles` ;

CREATE TABLE IF NOT EXISTS `laboratory_2023`.`roles` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  `slug` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  UNIQUE INDEX `slug_UNIQUE` (`slug` ASC))
ENGINE = InnoDB
AUTO_INCREMENT = 5
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `laboratory_2023`.`role_has_permissions`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `laboratory_2023`.`role_has_permissions` ;

CREATE TABLE IF NOT EXISTS `laboratory_2023`.`role_has_permissions` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `role_id` INT(10) UNSIGNED NOT NULL,
  `permission_id` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `fk_roles_role_has_permissions_idx` (`role_id` ASC),
  INDEX `fk_permissions_role_has_permissions_idx` (`permission_id` ASC),
  CONSTRAINT `fk_permissions_role_has_permissions`
    FOREIGN KEY (`permission_id`)
    REFERENCES `laboratory_2023`.`permissions` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_roles_role_has_permissions`
    FOREIGN KEY (`role_id`)
    REFERENCES `laboratory_2023`.`roles` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
AUTO_INCREMENT = 25
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `laboratory_2023`.`user_has_roles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `laboratory_2023`.`user_has_roles` ;

CREATE TABLE IF NOT EXISTS `laboratory_2023`.`user_has_roles` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_id` INT(10) UNSIGNED NOT NULL,
  `role_id` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `fk_users_user_has_roles_idx` (`user_id` ASC),
  INDEX `fk_roles_user_has_roles_idx` (`role_id` ASC),
  CONSTRAINT `fk_roles_user_has_roles`
    FOREIGN KEY (`role_id`)
    REFERENCES `laboratory_2023`.`roles` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users_user_has_roles`
    FOREIGN KEY (`user_id`)
    REFERENCES `laboratory_2023`.`users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
