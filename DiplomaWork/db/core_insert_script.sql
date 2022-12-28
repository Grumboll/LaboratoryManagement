-- CORE INSERT SCRIPT LABORATORY

INSERT INTO users(id, username, password, first_name, last_name, created_by, updated_by) VALUES 
(1, 'admin', 'admin', 'Admin_first_name', 'Admin_last_name', 1, 1);

INSERT INTO roles(id, name, slug) VALUES 
(1, 'Админ', 'roles.admin'),
(2, 'Лаборант', 'roles.laboratory_assistant'),
(3, 'Счетоводител', 'roles.accountant'),
(4, 'Мениджър Качество', 'roles.manager_quality');

INSERT INTO permissions(id, name, slug) VALUES 
(1, 'Всичко', 'permissions.all'),
(2, 'Вижда проби', 'permissions.show_profiles'),
(3, 'Добавя проби', 'permissions.create_profiles'),
(4, 'Редактира проби', 'permissions.edit_profiles'),
(5, 'Изтрива проби', 'permissions.delete_profiles'),
(6, 'Вижда дни', 'permissions.show_days'),
(7, 'Добавя дни', 'permissions.create_days'),
(8, 'Редактира дни', 'permissions.edit_days'),
(9, 'Изтрива дни', 'permissions.delete_days'),
(10, 'Вижда месеци', 'permissions.show_months'),
(11, 'Добавя месеци', 'permissions.create_months'),
(12, 'Редактира месеци', 'permissions.edit_months'),
(13, 'Изтрива месеци', 'permissions.delete_months'),
(14, 'Прави справки', 'permissions.create_inquiry'),
(15, 'Вижда всички потребители', 'permissions.show_users'),
(16, 'Създава потребители', 'permissions.create_users'),
(17, 'Изтрива потребители', 'permissions.delete_users'),
(18, 'Заключва потребители', 'permissions.lock_users');

INSERT INTO role_has_permissions(id, role_id, permission_id) VALUES 
(1, 1, 1),
(2, 2, 2),
(3, 2, 3),
(4, 2, 4),
(5, 2, 5),
(6, 2, 6),
(7, 2, 7),
(8, 2, 8),
(9, 2, 9),
(10, 2, 10),
(11, 2, 11),
(12, 2, 12),
(13, 2, 13),
(14, 2, 14),
(16, 3, 2),
(17, 3, 6),
(18, 3, 10),
(19, 3, 14),
(20, 4, 2),
(21, 4, 6),
(22, 4, 10),
(23, 4, 14),
(24, 4, 15);