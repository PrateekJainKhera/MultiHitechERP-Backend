-- Lets a drawing be tagged as belonging to a specific child part template (e.g. "PR-GEAR"),
-- so the production screen can show a "View Drawing" option per child part row, in addition
-- to the existing order/product-level drawing linkage.

SET QUOTED_IDENTIFIER ON;

ALTER TABLE Masters_Drawings ADD LinkedChildPartTemplateId INT NULL;

ALTER TABLE Masters_Drawings
    ADD CONSTRAINT FK_Drawings_ChildPartTemplate
    FOREIGN KEY (LinkedChildPartTemplateId) REFERENCES Masters_ChildPartTemplates(Id);

CREATE INDEX IX_Drawings_LinkedChildPartTemplateId ON Masters_Drawings(LinkedChildPartTemplateId);
