USE [XTRACHEF_DB_DEV]
    -- Insert Sample Categories
INSERT INTO Categories (CategoryName, [Description], Menu, Active, OrderBy, Icon, TenantId) VALUES
('Brinjal', 'Electronic devices and accessories','Seed', 1, 1, 'fa-laptop', 1),
('Tomoto', 'Fashion and apparel','Seed', 1, 2, 'fa-tshirt', 1),
('Okra', 'Home improvement and garden supplies','Seed', 1, 3, 'fa-home', 1),
('Beens', 'Sports equipment and outdoor gear','Seed', 1, 4, 'fa-football-ball', 1),
('Greens', 'Books, movies, and digital media','Seed', 1, 5, 'fa-book', 1),
('Flower Seed', 'Health, wellness, and beauty products','Seed', 1, 6, 'fa-heart', 1),
('Chillis', 'Food, drinks, and groceries','Seed', 1, 7, 'fa-utensils', 1),
('Pomkin', 'Car parts and automotive accessories','Seed', 1, 8, 'fa-car', 1),

('Gova', 'Car parts and automotive accessories','Fruits', 1, 1, 'fa-car', 1),
('Sapota', 'Car parts and automotive accessories','Fruits', 1, 2, 'fa-car', 1),
('Mango', 'Car parts and automotive accessories','Fruits', 1, 3, 'fa-car', 1),
('Cheery', 'Car parts and automotive accessories','Fruits', 1, 4, 'fa-car', 1);

-- Insert Sample Menu Master
INSERT INTO MenuMaster (MenuName, OrderBy, Active, SubMenu, TenantId) VALUES
('Home', 1, 1, 0, 1),
('Shop All', 2, 1, 1, 1),
('Seed', 4, 1, 1, 1),
('Fruits', 3, 1, 1, 1),
('Contact', 5, 1, 0, 1);

-- Link Categories to Menu
UPDATE Categories SET MenuId = (SELECT MenuId FROM MenuMaster WHERE MenuName = 'Fruits' AND TenantId = 1)
WHERE Menu = 'Fruits';

UPDATE Categories SET MenuId = (SELECT MenuId FROM MenuMaster WHERE MenuName = 'Seed' AND TenantId = 1)
WHERE Menu = 'Seed';

-- UPDATE Categories SET MenuId = (SELECT MenuId FROM MenuMaster WHERE MenuName in('Seed','Fruits') AND TenantId = 1)
use XTRACHEF_DB_DEV
-- update users set roleid=1 

INSERT INTO Products (TenantId, ProductName, ProductDescription, ProductCode, FullDescription, Specification, Story, PackQuantity, Quantity, Total, Price, Category, Rating, Active, Trending, UserBuyCount, [Return], InStock, BestSeller, DeliveryDate, Offer, OrderBy, UserId, Overview, LongDescription, MetaTitle, MetaDescription, MetaKeywords, Slug, SKU, Barcode, Weight, Dimensions, MinStockLevel, MaxStockLevel, ReorderPoint, CostPrice, OriginalPrice, DiscountPercentage, Created, Modified, CreatedBy, ModifiedBy) VALUES

-- Vegetable Seeds (1-15)
(1, 'Tomato Seeds - Cherry Red', 'Premium cherry tomato seeds for home gardening', 'TOM-CHR-001', 'High-yield cherry tomato seeds perfect for containers and gardens. Produces sweet, juicy fruits in 70-80 days.', 'Germination: 7-14 days, Maturity: 70-80 days, Plant Spacing: 24-36 inches', 'Cherry tomatoes are perfect for snacking and salads. These seeds produce vigorous plants with abundant fruit.', 1, 100, 100, 2.99, 1, 4.5, 1, 1, 25, 30, 1, 1, 7, 'Buy 3 Get 1 Free', 1, 1, 'Sweet cherry tomatoes perfect for home gardens', 'High-quality cherry tomato seeds that produce abundant sweet fruits. Ideal for beginners and experienced gardeners alike.', 'Cherry Tomato Seeds - Premium Quality', 'Premium cherry tomato seeds for home gardening. High yield and sweet taste.', 'tomato seeds, cherry tomato, vegetable seeds, home garden', 'tomato-seeds-cherry-red', 'TOM-CHR-001', '1234567890123', 0.05, '3x2x0.5', 10, 500, 20, 1.50, 2.99, 0, GETUTCDATE(), GETUTCDATE(), 1, 1),

(1, 'Lettuce Seeds - Romaine', 'Crisp romaine lettuce seeds for fresh salads', 'LET-ROM-002', 'Classic romaine lettuce seeds producing large, crisp heads perfect for Caesar salads.', 'Germination: 7-10 days, Maturity: 60-70 days, Plant Spacing: 12-18 inches', 'Romaine lettuce is a staple in healthy diets. These seeds produce large, crisp heads with excellent flavor.', 1, 150, 150, 1.99, 2, 4.3, 1, 0, 18, 30, 1, 0, 7, NULL, 2, 1, 'Crisp romaine lettuce for fresh salads', 'Premium romaine lettuce seeds that produce large, crisp heads perfect for salads and sandwiches.', 'Romaine Lettuce Seeds - Fresh & Crisp', 'Crisp romaine lettuce seeds for fresh salads. Easy to grow and harvest.', 'lettuce seeds, romaine, salad greens, vegetable seeds', 'lettuce-seeds-romaine', 'LET-ROM-002', '1234567890124', 0.03, '2x1x0.3', 20, 1000, 30, 0.80, 1.99, 0, GETUTCDATE(), GETUTCDATE(), 1, 1),

(1, 'Carrot Seeds - Orange Supreme', 'Sweet orange carrot seeds for home gardens', 'CAR-ORG-003', 'Premium orange carrot seeds producing sweet, crunchy roots perfect for cooking and snacking.', 'Germination: 14-21 days, Maturity: 70-80 days, Plant Spacing: 2-3 inches', 'Carrots are rich in beta-carotene and perfect for healthy snacking. These seeds produce uniform, sweet roots.', 1, 200, 200, 2.49, 3, 4.4, 1, 1, 32, 30, 1, 1, 7, 'Free Shipping', 3, 1, 'Sweet orange carrots for healthy eating', 'High-quality carrot seeds that produce sweet, crunchy orange roots. Perfect for home gardens and healthy eating.', 'Orange Carrot Seeds - Sweet & Crunchy', 'Sweet orange carrot seeds for home gardens. Rich in nutrients and easy to grow.', 'carrot seeds, orange carrots, vegetable seeds, healthy eating', 'carrot-seeds-orange-supreme', 'CAR-ORG-003', '1234567890125', 0.08, '4x2x0.5', 15, 800, 25, 1.20, 2.49, 0, GETUTCDATE(), GETUTCDATE(), 1, 1),

(1, 'Bell Pepper Seeds - Red', 'Sweet red bell pepper seeds for cooking', 'PEP-RED-004', 'Premium red bell pepper seeds producing large, sweet peppers perfect for cooking and stuffing.', 'Germination: 10-14 days, Maturity: 75-85 days, Plant Spacing: 18-24 inches', 'Red bell peppers are rich in vitamins and perfect for cooking. These seeds produce large, sweet fruits.', 1, 80, 80, 3.99, 4, 4.6, 1, 1, 28, 30, 1, 1, 7, 'Buy 2 Get 1 Free', 4, 1, 'Sweet red bell peppers for cooking', 'Premium red bell pepper seeds that produce large, sweet peppers perfect for cooking, stuffing, and grilling.', 'Red Bell Pepper Seeds - Sweet & Large', 'Sweet red bell pepper seeds for cooking. Large fruits with excellent flavor.', 'bell pepper seeds, red peppers, vegetable seeds, cooking', 'bell-pepper-seeds-red', 'PEP-RED-004', '1234567890126', 0.12, '5x3x0.8', 8, 200, 15, 2.00, 3.99, 0, GETUTCDATE(), GETUTCDATE(), 1, 1),

(1, 'Cucumber Seeds - English', 'Crisp English cucumber seeds for salads', 'CUC-ENG-005', 'Premium English cucumber seeds producing long, crisp cucumbers perfect for salads and pickling.', 'Germination: 7-10 days, Maturity: 55-65 days, Plant Spacing: 36-48 inches', 'English cucumbers are perfect for salads and snacking. These seeds produce long, crisp fruits.', 1, 120, 120, 2.79, 1, 4.2, 3, 0, 22, 30, 1, 0, 7, NULL, 5, 1, 'Crisp English cucumbers for salads', 'High-quality English cucumber seeds that produce long, crisp cucumbers perfect for salads and fresh eating.', 'English Cucumber Seeds - Crisp & Fresh', 'Crisp English cucumber seeds for salads. Long fruits with excellent texture.', 'cucumber seeds, english cucumber, vegetable seeds, salads', 'cucumber-seeds-english', 'CUC-ENG-005', '1234567890127', 0.06, '4x2x0.4', 12, 600, 20, 1.40, 2.79, 0, GETUTCDATE(), GETUTCDATE(), 1, 1),

(1, 'Spinach Seeds - Baby Leaf', 'Tender baby spinach seeds for salads', 'SPI-BAB-006', 'Premium baby spinach seeds producing tender leaves perfect for salads and smoothies.', 'Germination: 5-7 days, Maturity: 25-35 days, Plant Spacing: 2-4 inches', 'Baby spinach is perfect for salads and smoothies. These seeds produce tender, nutritious leaves.', 1, 300, 300, 1.89, 1, 4.1, 1, 0, 35, 30, 1, 0, 7, 'Organic Certified', 6, 1, 'Tender baby spinach for healthy eating', 'Premium baby spinach seeds that produce tender, nutritious leaves perfect for salads and smoothies.', 'Baby Spinach Seeds - Tender & Nutritious', 'Tender baby spinach seeds for salads. Rich in iron and vitamins.', 'spinach seeds, baby spinach, leafy greens, vegetable seeds', 'spinach-seeds-baby-leaf', 'SPI-BAB-006', '1234567890128', 0.02, '2x1x0.2', 25, 1500, 40, 0.95, 1.89, 0, GETUTCDATE(), GETUTCDATE(), 1, 1),

(1, 'Radish Seeds - Cherry Belle', 'Quick-growing radish seeds for snacking', 'RAD-CHE-007', 'Fast-growing cherry belle radish seeds producing crisp, mild radishes perfect for snacking.', 'Germination: 3-5 days, Maturity: 22-28 days, Plant Spacing: 1-2 inches', 'Cherry belle radishes are perfect for quick harvests. These seeds produce crisp, mild radishes.', 1, 250, 250, 1.49, 2, 4.0, 1, 0, 40, 30, 1, 0, 7, 'Fast Growing', 7, 1, 'Quick-growing radishes for snacking', 'Fast-growing cherry belle radish seeds that produce crisp, mild radishes perfect for quick harvests.', 'Cherry Belle Radish Seeds - Fast Growing', 'Quick-growing radish seeds for snacking. Crisp and mild flavor.', 'radish seeds, cherry belle, quick growing, vegetable seeds', 'radish-seeds-cherry-belle', 'RAD-CHE-007', '1234567890129', 0.04, '3x2x0.3', 30, 2000, 50, 0.75, 1.49, 0, GETUTCDATE(), GETUTCDATE(), 1, 1),

(1, 'Broccoli Seeds - Green Sprouting', 'Nutritious broccoli seeds for healthy eating', 'BRO-GRE-008', 'Premium green sprouting broccoli seeds producing nutritious florets perfect for cooking.', 'Germination: 7-10 days, Maturity: 60-70 days, Plant Spacing: 18-24 inches', 'Broccoli is a superfood rich in vitamins. These seeds produce large, nutritious florets.', 1, 90, 90, 3.49, 3, 4.4, 1, 1, 26, 30, 1, 1, 7, 'Superfood', 8, 1, 'Nutritious broccoli for healthy eating', 'Premium green sprouting broccoli seeds that produce large, nutritious florets perfect for healthy cooking.', 'Green Sprouting Broccoli Seeds - Superfood', 'Nutritious broccoli seeds for healthy eating. Rich in vitamins and minerals.', 'broccoli seeds, green broccoli, superfood, vegetable seeds', 'broccoli-seeds-green-sprouting', 'BRO-GRE-008', '1234567890130', 0.10, '4x2x0.6', 10, 300, 18, 1.75, 3.49, 0, GETUTCDATE(), GETUTCDATE(), 1, 1),

(1, 'Onion Seeds - Yellow', 'Sweet yellow onion seeds for cooking', 'ONI-YEL-009', 'Premium yellow onion seeds producing large, sweet onions perfect for cooking and storage.', 'Germination: 10-14 days, Maturity: 100-120 days, Plant Spacing: 4-6 inches', 'Yellow onions are essential for cooking. These seeds produce large, sweet onions with good storage.', 1, 150, 150, 2.29, 1, 4.3, 3, 0, 20, 30, 1, 0, 7, 'Long Storage', 9, 1, 'Sweet yellow onions for cooking', 'Premium yellow onion seeds that produce large, sweet onions perfect for cooking and long-term storage.', 'Yellow Onion Seeds - Sweet & Large', 'Sweet yellow onion seeds for cooking. Excellent storage qualities.', 'onion seeds, yellow onions, cooking onions, vegetable seeds', 'onion-seeds-yellow', 'ONI-YEL-009', '1234567890131', 0.07, '3x2x0.5', 20, 800, 30, 1.15, 2.29, 0, GETUTCDATE(), GETUTCDATE(), 1, 1),

(1, 'Beet Seeds - Red Detroit', 'Sweet red beet seeds for cooking', 'BEE-RED-010', 'Premium red Detroit beet seeds producing sweet, tender beets perfect for cooking and pickling.', 'Germination: 7-14 days, Maturity: 55-65 days, Plant Spacing: 3-4 inches', 'Red Detroit beets are perfect for cooking and pickling. These seeds produce sweet, tender roots.', 1, 180, 180, 2.19, 1, 4.2, 1, 0, 24, 30, 1, 0, 7, 'Sweet & Tender', 10, 1, 'Sweet red beets for cooking', 'Premium red Detroit beet seeds that produce sweet, tender beets perfect for cooking and pickling.', 'Red Detroit Beet Seeds - Sweet & Tender', 'Sweet red beet seeds for cooking. Perfect for pickling and roasting.', 'beet seeds, red beets, cooking beets, vegetable seeds', 'beet-seeds-red-detroit', 'BEE-RED-010', '1234567890132', 0.06, '3x2x0.4', 15, 600, 25, 1.10, 2.19, 0, GETUTCDATE(), GETUTCDATE(), 1, 1),

(1, 'Green Bean Seeds - Bush', 'Tender green bean seeds for cooking', 'BEA-BUS-011', 'Premium bush green bean seeds producing tender, stringless beans perfect for cooking.', 'Germination: 7-10 days, Maturity: 50-60 days, Plant Spacing: 4-6 inches', 'Bush green beans are perfect for cooking. These seeds produce tender, stringless beans.', 1, 200, 200, 2.69, 1, 4.5, 2, 1, 30, 30, 1, 1, 7, 'Stringless', 11, 1, 'Tender green beans for cooking', 'Premium bush green bean seeds that produce tender, stringless beans perfect for cooking and canning.', 'Bush Green Bean Seeds - Tender & Stringless', 'Tender green bean seeds for cooking. Stringless and easy to prepare.', 'green bean seeds, bush beans, stringless beans, vegetable seeds', 'green-bean-seeds-bush', 'BEA-BUS-011', '1234567890133', 0.09, '4x3x0.7', 12, 500, 20, 1.35, 2.69, 0, GETUTCDATE(), GETUTCDATE(), 1, 1),

(1, 'Zucchini Seeds - Black Beauty', 'Productive zucchini seeds for summer cooking', 'ZUC-BLA-012', 'Premium black beauty zucchini seeds producing abundant, dark green squash perfect for summer cooking.', 'Germination: 7-10 days, Maturity: 45-55 days, Plant Spacing: 36-48 inches', 'Black beauty zucchini is perfect for summer cooking. These seeds produce abundant, dark green squash.', 1, 60, 60, 3.29, 3, 4.4, 1, 1, 19, 30, 1, 1, 7, 'High Yield', 12, 1, 'Productive zucchini for summer cooking', 'Premium black beauty zucchini seeds that produce abundant, dark green squash perfect for summer cooking.', 'Black Beauty Zucchini Seeds - High Yield', 'Productive zucchini seeds for summer cooking. Abundant harvests.', 'zucchini seeds, black beauty, summer squash, vegetable seeds', 'zucchini-seeds-black-beauty', 'ZUC-BLA-012', '1234567890134', 0.15, '6x4x1.0', 6, 200, 12, 1.65, 3.29, 0, GETUTCDATE(), GETUTCDATE(), 1, 1),

(1, 'Cauliflower Seeds - White', 'Crisp white cauliflower seeds for cooking', 'CAU-WHI-013', 'Premium white cauliflower seeds producing large, crisp heads perfect for cooking and roasting.', 'Germination: 7-10 days, Maturity: 60-70 days, Plant Spacing: 18-24 inches', 'White cauliflower is perfect for cooking and roasting. These seeds produce large, crisp heads.', 1, 70, 70, 3.79, 1, 4.3, 2, 0, 21, 30, 1, 0, 7, 'Large Heads', 13, 1, 'Crisp white cauliflower for cooking', 'Premium white cauliflower seeds that produce large, crisp heads perfect for cooking and roasting.', 'White Cauliflower Seeds - Large & Crisp', 'Crisp white cauliflower seeds for cooking. Large heads with excellent flavor.', 'cauliflower seeds, white cauliflower, cooking vegetables, vegetable seeds', 'cauliflower-seeds-white', 'CAU-WHI-013', '1234567890135', 0.11, '4x3x0.8', 8, 250, 15, 1.90, 3.79, 0, GETUTCDATE(), GETUTCDATE(), 1, 1),

(1, 'Cabbage Seeds - Green', 'Crisp green cabbage seeds for cooking', 'CAB-GRE-014', 'Premium green cabbage seeds producing large, crisp heads perfect for cooking and coleslaw.', 'Germination: 7-10 days, Maturity: 70-80 days, Plant Spacing: 18-24 inches', 'Green cabbage is perfect for cooking and coleslaw. These seeds produce large, crisp heads.', 1, 100, 100, 2.89, 1, 4.2, 3, 0, 23, 30, 1, 0, 7, 'Large Heads', 14, 1, 'Crisp green cabbage for cooking', 'Premium green cabbage seeds that produce large, crisp heads perfect for cooking and coleslaw.', 'Green Cabbage Seeds - Large & Crisp', 'Crisp green cabbage seeds for cooking. Large heads with excellent storage.', 'cabbage seeds, green cabbage, cooking vegetables, vegetable seeds', 'cabbage-seeds-green', 'CAB-GRE-014', '1234567890136', 0.08, '3x2x0.6', 12, 400, 20, 1.45, 2.89, 0, GETUTCDATE(), GETUTCDATE(), 1, 1),

(1, 'Pea Seeds - Sugar Snap', 'Sweet sugar snap pea seeds for snacking', 'PEA-SUG-015', 'Premium sugar snap pea seeds producing sweet, edible pods perfect for snacking and cooking.', 'Germination: 7-14 days, Maturity: 60-70 days, Plant Spacing: 2-3 inches', 'Sugar snap peas are perfect for snacking. These seeds produce sweet, edible pods.', 1, 160, 160, 2.99, 1, 4.6, 1, 1, 27, 30, 1, 1, 7, 'Edible Pods', 15, 1, 'Sweet sugar snap peas for snacking', 'Premium sugar snap pea seeds that produce sweet, edible pods perfect for snacking and cooking.', 'Sugar Snap Pea Seeds - Sweet & Edible', 'Sweet sugar snap pea seeds for snacking. Edible pods with sweet flavor.', 'pea seeds, sugar snap peas, edible pods, vegetable seeds', 'pea-seeds-sugar-snap', 'PEA-SUG-015', '1234567890137', 0.05, '3x2x0.4', 16, 800, 30, 1.50, 2.99, 0, GETUTCDATE(), GETUTCDATE(), 1, 1);

-- Continue with more products...

-- Insert System Roles
INSERT INTO Roles (RoleName, RoleDescription, RoleLevel, IsSystemRole) VALUES
('Admin', 'Administrator with full system access', 10, 1),
('Tenant Admin', 'Administrator with management privileges', 8, 1),
('Manager', 'Manager with operational privileges', 6, 1),
('Executive', 'Executive with extended privileges', 4, 1),
('Customer', 'Regular customer user', 1, 1);


UPDATE users set roleid=1 where userid=1


INSERT INTO States (StateCode, StateName, OrderBy) VALUES
('AN', 'Andaman and Nicobar Islands', 1),
('AP', 'Andhra Pradesh', 2),
('AR', 'Arunachal Pradesh', 3),
('AS', 'Assam', 4),
('BR', 'Bihar', 5),
('CH', 'Chandigarh', 6),
('CT', 'Chhattisgarh', 7),
('DL', 'Delhi', 8),
('GA', 'Goa', 9),
('GJ', 'Gujarat', 10),
('HR', 'Haryana', 11),
('HP', 'Himachal Pradesh', 12),
('JK', 'Jammu and Kashmir', 13),
('JH', 'Jharkhand', 14),
('KA', 'Karnataka', 15),
('KL', 'Kerala', 16),
('MP', 'Madhya Pradesh', 17),
('MH', 'Maharashtra', 18),
('MN', 'Manipur', 19),
('ML', 'Meghalaya', 20),
('MZ', 'Mizoram', 21),
('NL', 'Nagaland', 22),
('OR', 'Odisha', 23),
('PY', 'Puducherry', 24),
('PB', 'Punjab', 25),
('RJ', 'Rajasthan', 26),
('SK', 'Sikkim', 27),
('TN', 'Tamil Nadu', 28),
('TG', 'Telangana', 29),
('TR', 'Tripura', 30),
('UP', 'Uttar Pradesh', 31),
('UT', 'Uttarakhand', 32),
('WB', 'West Bengal', 33);

-- Insert Shipping Rates (8 records: 2 states × 2 couriers × 2 products)
-- Seeds - Tamil Nadu
INSERT INTO ShippingRates (TenantId, ProductType, StateCode, CourierType, BaseCharge, PerUnitCharge, MinCharge, FreeShippingThreshold) VALUES
(1, 'Seed', 'TN', 'Postal', 40, 4, 40, 1000),
(1, 'Seed', 'TN', 'Other', 50, 5, 50, 1000),
-- Seeds - Other States
(1, 'Seed', NULL, 'Postal', 60, 5, 60, 1000),
(1, 'Seed', NULL, 'Other', 75, 5, 75, 1000),
-- Plants - Tamil Nadu
(1, 'Plant', 'TN', 'Postal', 120, 12, 120, 2000),
(1, 'Plant', 'TN', 'Other', 150, 15, 150, 2000),
-- Plants - Other States
(1, 'Plant', NULL, 'Postal', 180, 15, 180, 2000),
(1, 'Plant', NULL, 'Other', 200, 15, 200, 2000);

GO

-- -- Insert System Permissions
-- INSERT INTO Permissions (PermissionName, PermissionDescription, PermissionCategory, ResourceType, ActionType) VALUES
-- -- Product Management
-- ('view_products', 'View product catalog', 'Product Management', 'Product', 'View'),
-- ('manage_products', 'Create, update, and delete products', 'Product Management', 'Product', 'Manage'),
-- ('manage_product_images', 'Upload and manage product images', 'Product Management', 'Product', 'Manage'),
-- ('view_inventory', 'View inventory levels and stock information', 'Inventory Management', 'Product', 'View'),
-- ('manage_inventory', 'Manage inventory levels and stock', 'Inventory Management', 'Product', 'Manage'),

-- -- Order Management
-- ('view_orders', 'View customer orders', 'Order Management', 'Order', 'View'),
-- ('manage_orders', 'Manage order status and details', 'Order Management', 'Order', 'Manage'),
-- ('process_refunds', 'Process order refunds and cancellations', 'Order Management', 'Order', 'Manage'),
-- ('view_order_analytics', 'View order analytics and reports', 'Analytics', 'Order', 'View'),

-- -- User Management
-- ('view_users', 'View user accounts and profiles', 'User Management', 'User', 'View'),
-- ('manage_users', 'Create, update, and manage user accounts', 'User Management', 'User', 'Manage'),
-- ('manage_user_roles', 'Assign and modify user roles', 'User Management', 'User', 'Manage'),
-- ('view_user_activity', 'View user activity logs', 'User Management', 'User', 'View'),

-- -- Category Management
-- ('view_categories', 'View product categories', 'Category Management', 'Category', 'View'),
-- ('manage_categories', 'Create, update, and delete categories', 'Category Management', 'Category', 'Manage'),

-- -- Cart and Wishlist
-- ('manage_cart', 'Add, remove, and modify cart items', 'Shopping', 'Cart', 'Manage'),
-- ('manage_wishlist', 'Add, remove, and modify wishlist items', 'Shopping', 'Wishlist', 'Manage'),

-- -- Reports and Analytics
-- ('view_reports', 'View system reports and analytics', 'Analytics', 'Report', 'View'),
-- ('view_analytics', 'View detailed analytics and insights', 'Analytics', 'Analytics', 'View'),
-- ('export_data', 'Export data and reports', 'Analytics', 'Data', 'Export'),

-- -- System Administration
-- ('manage_settings', 'Manage system settings and configuration', 'System', 'Settings', 'Manage'),
-- ('manage_roles', 'Create and modify system roles', 'System', 'Role', 'Manage'),
-- ('view_system_logs', 'View system and audit logs', 'System', 'Log', 'View'),
-- ('manage_notifications', 'Manage system notifications', 'System', 'Notification', 'Manage');

-- -- Insert Role-Permission Mappings
-- -- SuperAdmin - Full Access
-- INSERT INTO RolePermissions (RoleId, PermissionId)
-- SELECT r.RoleId, p.PermissionId
-- FROM Roles r
-- CROSS JOIN Permissions p
-- WHERE r.RoleName = 'SuperAdmin';

-- -- Admin - Management Access
-- INSERT INTO RolePermissions (RoleId, PermissionId)
-- SELECT r.RoleId, p.PermissionId
-- FROM Roles r
-- CROSS JOIN Permissions p
-- WHERE r.RoleName = 'Admin'
-- AND p.PermissionName IN (
-- 	'view_products', 'manage_products', 'manage_product_images', 'view_inventory', 'manage_inventory',
-- 	'view_orders', 'manage_orders', 'process_refunds', 'view_order_analytics',
-- 	'view_users', 'manage_users', 'view_user_activity',
-- 	'view_categories', 'manage_categories',
-- 	'view_reports', 'view_analytics', 'export_data',
-- 	'manage_notifications'
-- );

-- -- Manager - Operational Access
-- INSERT INTO RolePermissions (RoleId, PermissionId)
-- SELECT r.RoleId, p.PermissionId
-- FROM Roles r
-- CROSS JOIN Permissions p
-- WHERE r.RoleName = 'Manager'
-- AND p.PermissionName IN (
-- 	'view_products', 'manage_products', 'view_inventory', 'manage_inventory',
-- 	'view_orders', 'manage_orders', 'process_refunds',
-- 	'view_users', 'view_categories', 'manage_categories',
-- 	'view_reports', 'view_analytics'
-- );

-- -- Executive - Extended Customer Access
-- INSERT INTO RolePermissions (RoleId, PermissionId)
-- SELECT r.RoleId, p.PermissionId
-- FROM Roles r
-- CROSS JOIN Permissions p
-- WHERE r.RoleName = 'Executive'
-- AND p.PermissionName IN (
-- 	'view_products', 'view_orders', 'manage_cart', 'manage_wishlist',
-- 	'view_categories', 'view_inventory'
-- );

-- -- Support - Customer Service Access
-- INSERT INTO RolePermissions (RoleId, PermissionId)
-- SELECT r.RoleId, p.PermissionId
-- FROM Roles r
-- CROSS JOIN Permissions p
-- WHERE r.RoleName = 'Support'
-- AND p.PermissionName IN (
-- 	'view_products', 'view_orders', 'view_users', 'view_user_activity',
-- 	'view_categories', 'manage_notifications'
-- );

-- -- Customer - Basic Access
-- INSERT INTO RolePermissions (RoleId, PermissionId)
-- SELECT r.RoleId, p.PermissionId
-- FROM Roles r
-- CROSS JOIN Permissions p
-- WHERE r.RoleName = 'Customer'
-- AND p.PermissionName IN (
-- 	'view_products', 'manage_cart', 'manage_wishlist', 'view_categories'
-- );


-- select * from Users
-- truncate table users
-- select * from Roles
-- select * from ProductImages
-- select * from Products
-- select * from MenuMaster
-- select * from Categories

-- truncate table Categories


				-- SELECT * FROM Categories
				-- select * from MenuMaster
				-- SELECT * FROM Products
				-- SELECT * FROM CartItems
				-- SELECT * FROM USERS
				-- -- update users set roleid=1 x
				-- -- SELECT * FROM UserRoles
					
				SELECT * FROM OrderItems
				SELECT * FROM UserAddresses
				SELECT * FROM Coupons 
				select * from ProductReviews





