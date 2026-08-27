-- Masters_Materials.MaterialCode and Masters_ChildPartTemplates.TemplateCode were the
-- only two auto-generated business codes in the schema with no uniqueness backstop —
-- a race between two concurrent creates (same Grade/Shape/Dimension, or same child part
-- type) could silently insert two rows with the identical code. Every other
-- auto-generated code (PartCode, CustomerCode, MachineCode, OrderNo, JobCardNo,
-- DrawingNumber) already has one.

SET QUOTED_IDENTIFIER ON;

ALTER TABLE Masters_Materials
    ADD CONSTRAINT UQ_Masters_Materials_MaterialCode UNIQUE (MaterialCode);

ALTER TABLE Masters_ChildPartTemplates
    ADD CONSTRAINT UQ_Masters_ChildPartTemplates_TemplateCode UNIQUE (TemplateCode);
