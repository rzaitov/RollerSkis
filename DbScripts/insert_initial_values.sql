INSERT INTO ProductType (parent_id, name)
VALUES
	(NULL, 'Speed Reducers'),
	(NULL, 'Brakes'),
	(NULL, 'Wheels'),
	(NULL, 'Accessories'),
	(NULL, 'Nordixc'),
	(NULL, 'Roller skis')
	
DECLARE @LastId INT;
SET @LastId = SCOPE_IDENTITY();

INSERT INTO ProductType (parent_id, name)
VALUES
	(@LastId, 'Combi'),
	(@LastId, 'Classic'),
	(@LastId, 'Skate')
	
/*
SELECT *
FROM ProductType
*/