SET NOCOUNT ON;
GO

PRINT '========================================';
PRINT '';

BEGIN TRY
    -- ============== TABELA 1: T_MODELO_PROGRAMACAO ==============
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'T_MODELO_PROGRAMACAO')
    BEGIN
        PRINT '📋 1/6 - Criando T_MODELO_PROGRAMACAO...';

        CREATE TABLE T_MODELO_PROGRAMACAO (
            cd_modelo_programacao INT IDENTITY(1,1) PRIMARY KEY,
            cd_curso INT NULL,
            cd_produto INT NULL,
            cd_regime INT NULL,
            cd_duracao INT NULL,
            cd_duracao_aula INT NULL,
            cd_pessoa_escola INT NULL,
            id_modelo_ativo BIT NULL DEFAULT 1,
            dt_cadastro DATETIME NULL DEFAULT GETDATE(),
            dt_atualizacao DATETIME NULL
        );

        CREATE INDEX IDX_MODELO_PROG_CURSO ON T_MODELO_PROGRAMACAO(cd_curso, cd_duracao);
        CREATE INDEX IDX_MODELO_PROG_ESCOLA ON T_MODELO_PROGRAMACAO(cd_pessoa_escola);
        CREATE INDEX IDX_MODELO_PROG_ATIVO ON T_MODELO_PROGRAMACAO(id_modelo_ativo);

        PRINT '   ✅ T_MODELO_PROGRAMACAO criada!';
    END
    ELSE
        PRINT '   ⚠️  T_MODELO_PROGRAMACAO já existe, pulando...';

    -- ============== TABELA 2: T_ITEM_MODELO_PROGRAMACAO ==============
    IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'T_ITEM_MODELO_PROGRAMACAO')
    BEGIN
        PRINT '📋 2/6 - Criando T_ITEM_MODELO_PROGRAMACAO...';

        CREATE TABLE T_ITEM_MODELO_PROGRAMACAO (
            cd_item_modelo_programacao INT IDENTITY(1,1) PRIMARY KEY,
            cd_modelo_programacao INT NULL,
            nm_aula_modelo INT NULL,
            dia_semana VARCHAR(20) NULL,
            hr_inicial TIME NULL,
            hr_final TIME NULL,
            dc_aula_modelo VARCHAR(500) NULL,
            nm_ordem_aula INT NULL
        );

        CREATE INDEX IDX_ITEM_MODELO_PROG ON T_ITEM_MODELO_PROGRAMACAO(cd_modelo_programacao);
        CREATE INDEX IDX_ITEM_MODELO_ORDEM ON T_ITEM_MODELO_PROGRAMACAO(cd_modelo_programacao, nm_ordem_aula);

        PRINT '   ✅ T_ITEM_MODELO_PROGRAMACAO criada!';
    END
    ELSE
        PRINT '   ⚠️  T_ITEM_MODELO_PROGRAMACAO já existe, pulando...';

    -- ============== ATUALIZAÇÃO 3: T_PROGRAMACAO_CURSO ==============
    PRINT '📋 3/6 - Atualizando T_PROGRAMACAO_CURSO...';

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('T_PROGRAMACAO_CURSO') AND name = 'cd_produto')
        ALTER TABLE T_PROGRAMACAO_CURSO ADD cd_produto INT NULL;

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('T_PROGRAMACAO_CURSO') AND name = 'cd_regime')
        ALTER TABLE T_PROGRAMACAO_CURSO ADD cd_regime INT NULL;

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('T_PROGRAMACAO_CURSO') AND name = 'cd_duracao_aula')
        ALTER TABLE T_PROGRAMACAO_CURSO ADD cd_duracao_aula INT NULL;

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('T_PROGRAMACAO_CURSO') AND name = 'id_programacao_ativa')
        ALTER TABLE T_PROGRAMACAO_CURSO ADD id_programacao_ativa BIT NULL DEFAULT 1;

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('T_PROGRAMACAO_CURSO') AND name = 'dt_cadastro')
        ALTER TABLE T_PROGRAMACAO_CURSO ADD dt_cadastro DATETIME NULL DEFAULT GETDATE();

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_PROG_CURSO_BUSCA' AND object_id = OBJECT_ID('T_PROGRAMACAO_CURSO'))
        CREATE INDEX IDX_PROG_CURSO_BUSCA ON T_PROGRAMACAO_CURSO(cd_curso, cd_produto, cd_regime, cd_duracao, cd_duracao_aula, id_programacao_ativa);

    PRINT '   ✅ T_PROGRAMACAO_CURSO atualizada!';

    -- ============== ATUALIZAÇÃO 4: T_ITEM_PROGRAMACAO ==============
    PRINT '📋 4/6 - Atualizando T_ITEM_PROGRAMACAO...';

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('T_ITEM_PROGRAMACAO') AND name = 'dia_semana')
        ALTER TABLE T_ITEM_PROGRAMACAO ADD dia_semana VARCHAR(20) NULL;

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('T_ITEM_PROGRAMACAO') AND name = 'hr_inicial')
        ALTER TABLE T_ITEM_PROGRAMACAO ADD hr_inicial TIME NULL;

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('T_ITEM_PROGRAMACAO') AND name = 'hr_final')
        ALTER TABLE T_ITEM_PROGRAMACAO ADD hr_final TIME NULL;

    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('T_ITEM_PROGRAMACAO') AND name = 'nm_ordem_aula')
        ALTER TABLE T_ITEM_PROGRAMACAO ADD nm_ordem_aula INT NULL;

    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IDX_ITEM_PROG_ORDEM' AND object_id = OBJECT_ID('T_ITEM_PROGRAMACAO'))
        CREATE INDEX IDX_ITEM_PROG_ORDEM ON T_ITEM_PROGRAMACAO(cd_programacao_curso, nm_ordem_aula);

    PRINT '   ✅ T_ITEM_PROGRAMACAO atualizada!';

    -- ============== VIEW 5: vi_programacao_completa ==============
    PRINT '📋 5/6 - Criando view vi_programacao_completa...';

    IF EXISTS (SELECT * FROM sys.views WHERE name = 'vi_programacao_completa')
        DROP VIEW vi_programacao_completa;

    EXEC('
    CREATE VIEW vi_programacao_completa AS
    SELECT
        pc.cd_programacao_curso, pc.cd_curso, pc.cd_produto, pc.cd_regime, pc.cd_duracao,
        pc.cd_duracao_aula, pc.cd_escola, pc.id_programacao_ativa, pc.dt_cadastro,
        ip.cd_item_programacao, ip.nm_aula_programacao, ip.dc_aula_programacao,
        ip.dia_semana, ip.hr_inicial, ip.hr_final, ip.nm_ordem_aula,
        c.no_curso, p.no_produto, r.dc_regime, d.dc_duracao
    FROM T_PROGRAMACAO_CURSO pc
    LEFT JOIN T_ITEM_PROGRAMACAO ip ON pc.cd_programacao_curso = ip.cd_programacao_curso
    LEFT JOIN T_CURSO c ON pc.cd_curso = c.cd_curso
    LEFT JOIN T_PRODUTO p ON pc.cd_produto = p.cd_produto
    LEFT JOIN T_REGIME r ON pc.cd_regime = r.cd_regime
    LEFT JOIN T_DURACAO d ON pc.cd_duracao = d.cd_duracao
    ');

    PRINT '   ✅ View vi_programacao_completa criada!';

    -- ============== VIEW 6: vi_modelo_programacao_completa ==============
    PRINT '📋 6/6 - Criando view vi_modelo_programacao_completa...';

    IF EXISTS (SELECT * FROM sys.views WHERE name = 'vi_modelo_programacao_completa')
        DROP VIEW vi_modelo_programacao_completa;

    EXEC('
    CREATE VIEW vi_modelo_programacao_completa AS
    SELECT
        mp.cd_modelo_programacao, mp.cd_curso, mp.cd_produto, mp.cd_regime, mp.cd_duracao,
        mp.cd_duracao_aula, mp.cd_pessoa_escola, mp.id_modelo_ativo, mp.dt_cadastro, mp.dt_atualizacao,
        imp.cd_item_modelo_programacao, imp.nm_aula_modelo, imp.dia_semana, imp.hr_inicial, imp.hr_final,
        imp.dc_aula_modelo, imp.nm_ordem_aula,
        c.no_curso, p.no_produto, r.dc_regime, d.dc_duracao
    FROM T_MODELO_PROGRAMACAO mp
    LEFT JOIN T_ITEM_MODELO_PROGRAMACAO imp ON mp.cd_modelo_programacao = imp.cd_modelo_programacao
    LEFT JOIN T_CURSO c ON mp.cd_curso = c.cd_curso
    LEFT JOIN T_PRODUTO p ON mp.cd_produto = p.cd_produto
    LEFT JOIN T_REGIME r ON mp.cd_regime = r.cd_regime
    LEFT JOIN T_DURACAO d ON mp.cd_duracao = d.cd_duracao
    ');

    PRINT '   ✅ View vi_modelo_programacao_completa criada!';

    PRINT '';
    PRINT '========================================';
    PRINT '✅ MIGRAÇÃO CONCLUÍDA COM SUCESSO!';
    PRINT '========================================';
    PRINT 'Resumo:';
    PRINT '  - 2 novas tabelas criadas';
    PRINT '  - 2 tabelas atualizadas';
    PRINT '  - 2 views criadas';
    PRINT '  - Todos os campos são opcionais (NULL)';
    PRINT '  - Nenhuma FK criada';
    PRINT '  - Seguro para produção';
    PRINT '========================================';

END TRY
BEGIN CATCH
    PRINT '';
    PRINT '========================================';
    PRINT '❌ ERRO DURANTE A MIGRAÇÃO!';
    PRINT '========================================';
    PRINT 'Erro: ' + ERROR_MESSAGE();
    PRINT 'Linha: ' + CAST(ERROR_LINE() AS VARCHAR);
    PRINT '========================================';
END CATCH;
GO
