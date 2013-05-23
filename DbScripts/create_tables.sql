/*
DROP TABLE dbo.Specification
DROP TABLE dbo.Attributes
DROP TABLE dbo.Product
DROP TABLE dbo.ProductType
*/
CREATE TABLE ProductType
(
	id INT PRIMARY KEY IDENTITY NOT NULL,
	parent_id INT FOREIGN KEY REFERENCES ProductType(id) NULL,
	name NVARCHAR(50) NOT NULL	
)

CREATE TABLE Product
(
	id INT PRIMARY KEY IDENTITY NOT NULL,
	name nvarchar(100) NOT NULL,
	price money NOT NULL,
	product_type INT NOT NULL FOREIGN KEY REFERENCES ProductType(id),
	description nvarchar(max) NULL,
	image_name varchar(100) NULL,
)

-- Технические характеристики товаров (ттх). Вес, габариты итд
CREATE TABLE Specification
(
	id INT PRIMARY KEY IDENTITY NOT NULL,
	name NVARCHAR(500) NOT NULL,
	value NVARCHAR(500) NOT NULL,
	product INT NOT NULL FOREIGN KEY REFERENCES	Product(id)
)

-- Аттрибуты товаров. В основном используется для колес (скоростной аттрибут)
CREATE TABLE Attributes
(
	id INT PRIMARY KEY IDENTITY NOT NULL,
	name VARCHAR(50) NOT NULL,
	treatment NVARCHAR(100) NULL,
	product INT NOT NULL FOREIGN KEY REFERENCES	Product(id)
)