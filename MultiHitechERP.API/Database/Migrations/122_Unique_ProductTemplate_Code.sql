-- Masters_ProductTemplates.TemplateCode was auto-generated (MAX+1 per RollerType) with
-- no uniqueness backstop — a race between two concurrent creates for the same RollerType
-- could silently insert two templates with the identical code. Every other
-- auto-generated code in the schema (PartCode, CustomerCode, MachineCode, OrderNo,
-- JobCardNo, DrawingNumber, MaterialCode, ChildPartTemplate.TemplateCode) already has one.

SET QUOTED_IDENTIFIER ON;

ALTER TABLE Masters_ProductTemplates
    ADD CONSTRAINT UQ_Masters_ProductTemplates_TemplateCode UNIQUE (TemplateCode);
