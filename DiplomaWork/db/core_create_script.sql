-- MySQL Script generated by MySQL Workbench
-- Thu Jun  1 00:08:24 2023
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
-- Table `profiles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `profiles` ;

CREATE TABLE IF NOT EXISTS `profiles` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `users`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `users` ;

CREATE TABLE IF NOT EXISTS `users` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `username` VARCHAR(255) NOT NULL,
  `e-mail` VARCHAR(255) NOT NULL,
  `date_of_birth` DATE NULL,
  `phone_number` VARCHAR(15) NULL,
  `password` VARCHAR(255) NOT NULL,
  `password_salt` VARCHAR(255) NOT NULL,
  `first_name` VARCHAR(64) NOT NULL,
  `middle_name` VARCHAR(64) NULL DEFAULT NULL,
  `last_name` VARCHAR(64) NOT NULL,
  `is_locked` TINYINT(1) NOT NULL DEFAULT 0,
  `password_reset_required` TINYINT(1) NOT NULL DEFAULT 0,
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
  UNIQUE INDEX `e-mail_UNIQUE` (`e-mail` ASC),
  CONSTRAINT `fk_users_users1`
    FOREIGN KEY (`created_by`)
    REFERENCES `users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users_users2`
    FOREIGN KEY (`updated_by`)
    REFERENCES `users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `profile_has_lengths_perimeter`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `profile_has_lengths_perimeter` ;

CREATE TABLE IF NOT EXISTS `profile_has_lengths_perimeter` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `profile_id` INT(10) UNSIGNED NOT NULL,
  `length` DECIMAL(21,13) UNSIGNED NULL DEFAULT NULL,
  `perimeter` DECIMAL(21,13) UNSIGNED NULL DEFAULT NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT NOW(),
  `updated_at` TIMESTAMP NOT NULL DEFAULT NOW(),
  `deleted_at` TIMESTAMP NULL,
  `created_by` INT(10) UNSIGNED NOT NULL,
  `updated_by` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `fk_profile_profile_has_lengths_perimeter_idx` (`profile_id` ASC),
  INDEX `fk_users1_profile_has_lengths_perimeter_idx` (`created_by` ASC),
  INDEX `fk_users2_profile_has_lengths_perimeter_idx` (`updated_by` ASC),
  CONSTRAINT `fk_profile_profile_has_lengths_perimeter`
    FOREIGN KEY (`profile_id`)
    REFERENCES `profiles` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users1_profile_has_lengths_perimeter`
    FOREIGN KEY (`created_by`)
    REFERENCES `users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users2_profile_has_lengths_perimeter`
    FOREIGN KEY (`updated_by`)
    REFERENCES `users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `months`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `months` ;

CREATE TABLE IF NOT EXISTS `months` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  `slug` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  UNIQUE INDEX `slug_UNIQUE` (`slug` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `laboratory_day`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `laboratory_day` ;

CREATE TABLE IF NOT EXISTS `laboratory_day` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `day` DATE NOT NULL,
  `month_id` INT(10) UNSIGNED NOT NULL,
  `year` SMALLINT(5) UNSIGNED NOT NULL,
  `profile_has_lengths_perimeter_id` INT(10) UNSIGNED NOT NULL,
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
  INDEX `fk_months_laboratory_day_idx` (`month_id` ASC),
  INDEX `fk_profile_has_lengths_perimeter_laboratory_day_idx` (`profile_has_lengths_perimeter_id` ASC),
  CONSTRAINT `fk_profile_has_lengths_perimeter_laboratory_day`
    FOREIGN KEY (`profile_has_lengths_perimeter_id`)
    REFERENCES `profile_has_lengths_perimeter` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users_laboratory_day1`
    FOREIGN KEY (`created_by`)
    REFERENCES `users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users_laboratory_day2`
    FOREIGN KEY (`updated_by`)
    REFERENCES `users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_months_laboratory_day`
    FOREIGN KEY (`month_id`)
    REFERENCES `months` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `laboratory_month_chemicals`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `laboratory_month_chemicals` ;

CREATE TABLE IF NOT EXISTS `laboratory_month_chemicals` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `month_id` INT(10) UNSIGNED NOT NULL,
  `year` SMALLINT(5) UNSIGNED NOT NULL,
  `name` VARCHAR(64) NOT NULL,
  `chemical_expenditure` DECIMAL(7,3) UNSIGNED NOT NULL,
  `expense_per_meter_squared` DECIMAL(7,3) UNSIGNED NOT NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT NOW(),
  `updated_at` TIMESTAMP NOT NULL DEFAULT NOW(),
  `deleted_at` TIMESTAMP NULL DEFAULT NULL,
  `created_by` INT(10) UNSIGNED NOT NULL,
  `updated_by` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `fk_users_laboratory_month_chemicals_idx` (`created_by` ASC),
  INDEX `fk_users2_laboratory_month_chemicals_idx` (`updated_by` ASC),
  INDEX `fk_laboratory_month_chemicals_months_idx` (`month_id` ASC),
  CONSTRAINT `fk_laboratory_month_chemicals_months`
    FOREIGN KEY (`month_id`)
    REFERENCES `months` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users_laboratory_month_chemicals`
    FOREIGN KEY (`created_by`)
    REFERENCES `users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users2_laboratory_month_chemicals`
    FOREIGN KEY (`updated_by`)
    REFERENCES `users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `laboratory_months`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `laboratory_months` ;

CREATE TABLE IF NOT EXISTS `laboratory_months` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `date` DATE NOT NULL,
  `month_id` INT(10) UNSIGNED NOT NULL,
  `year` SMALLINT(5) NOT NULL,
  `kilograms` DECIMAL(7,3) UNSIGNED NULL DEFAULT NULL,
  `meters_squared` DECIMAL(7,3) UNSIGNED NOT NULL,
  `created_at` TIMESTAMP NOT NULL DEFAULT NOW(),
  `updated_at` TIMESTAMP NOT NULL DEFAULT NOW(),
  `deleted_at` TIMESTAMP NULL DEFAULT NULL,
  `created_by` INT(10) UNSIGNED NOT NULL,
  `updated_by` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `fk_users_laboratory_months_idx` (`created_by` ASC),
  INDEX `fk_users2_laboratory_months_idx` (`updated_by` ASC),
  CONSTRAINT `fk_months_laboratory_months`
    FOREIGN KEY (`month_id`)
    REFERENCES `months` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users_laboratory_months`
    FOREIGN KEY (`created_by`)
    REFERENCES `users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users2_laboratory_months`
    FOREIGN KEY (`updated_by`)
    REFERENCES `users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `permissions`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `permissions` ;

CREATE TABLE IF NOT EXISTS `permissions` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  `slug` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  UNIQUE INDEX `slug_UNIQUE` (`slug` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `roles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `roles` ;

CREATE TABLE IF NOT EXISTS `roles` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `name` VARCHAR(50) NOT NULL,
  `slug` VARCHAR(50) NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  UNIQUE INDEX `slug_UNIQUE` (`slug` ASC))
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `role_has_permissions`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `role_has_permissions` ;

CREATE TABLE IF NOT EXISTS `role_has_permissions` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `role_id` INT(10) UNSIGNED NOT NULL,
  `permission_id` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `fk_roles_role_has_permissions_idx` (`role_id` ASC),
  INDEX `fk_permissions_role_has_permissions_idx` (`permission_id` ASC),
  CONSTRAINT `fk_permissions_role_has_permissions`
    FOREIGN KEY (`permission_id`)
    REFERENCES `permissions` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_roles_role_has_permissions`
    FOREIGN KEY (`role_id`)
    REFERENCES `roles` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `user_has_roles`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `user_has_roles` ;

CREATE TABLE IF NOT EXISTS `user_has_roles` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_id` INT(10) UNSIGNED NOT NULL,
  `role_id` INT(10) UNSIGNED NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `fk_users_user_has_roles_idx` (`user_id` ASC),
  INDEX `fk_roles_user_has_roles_idx` (`role_id` ASC),
  CONSTRAINT `fk_roles_user_has_roles`
    FOREIGN KEY (`role_id`)
    REFERENCES `roles` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION,
  CONSTRAINT `fk_users_user_has_roles`
    FOREIGN KEY (`user_id`)
    REFERENCES `users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB
DEFAULT CHARACTER SET = utf8mb4;


-- -----------------------------------------------------
-- Table `email_codes`
-- -----------------------------------------------------
DROP TABLE IF EXISTS `email_codes` ;

CREATE TABLE IF NOT EXISTS `email_codes` (
  `id` INT(10) UNSIGNED NOT NULL AUTO_INCREMENT,
  `user_id` INT(10) UNSIGNED NOT NULL,
  `code` VARCHAR(6) NOT NULL,
  `is_valid` TINYINT(1) UNSIGNED NOT NULL DEFAULT 1,
  `expired_at` DATETIME NOT NULL,
  PRIMARY KEY (`id`),
  UNIQUE INDEX `id_UNIQUE` (`id` ASC),
  INDEX `fk_users_email_codes_idx` (`user_id` ASC),
  CONSTRAINT `fk_users_email_codes`
    FOREIGN KEY (`user_id`)
    REFERENCES `users` (`id`)
    ON DELETE NO ACTION
    ON UPDATE NO ACTION)
ENGINE = InnoDB;


SET SQL_MODE=@OLD_SQL_MODE;
SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS;
SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS;
