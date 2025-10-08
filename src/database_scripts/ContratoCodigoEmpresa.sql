  -- ============== Unifica todos CD_PESSOA_ESCOLA E CD_EMPRESA EM UM ÚNICO CAMPO CD_TENANT atráves de VIEWS ==============
CREATE OR ALTER VIEW vw_T_ACAO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_ACAO;
GO

CREATE OR ALTER VIEW vw_T_ALUNO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_ALUNO;
GO

CREATE OR ALTER VIEW vw_T_ATIVIDADE_CURSO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_ATIVIDADE_CURSO;
GO

CREATE OR ALTER VIEW vw_T_ATIVIDADE_EXTRA_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_ATIVIDADE_EXTRA;
GO

CREATE OR ALTER VIEW vw_T_AULA_REPOSICAO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_AULA_REPOSICAO;
GO

CREATE OR ALTER VIEW vw_T_BIBLIOTECA_SEC_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_BIBLIOTECA_SEC;
GO

CREATE OR ALTER VIEW vw_T_CAIXA_tenant AS
SELECT cd_empresa AS cd_tenant, * FROM T_CAIXA;
GO

CREATE OR ALTER VIEW vw_T_CALENDARIO_ACADEMICO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_CALENDARIO_ACADEMICO;
GO

CREATE OR ALTER VIEW vw_T_CALENDARIO_EVENTO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_CALENDARIO_EVENTO;
GO

CREATE OR ALTER VIEW vw_T_CONTATO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_CONTATO;
GO

CREATE OR ALTER VIEW vw_T_CONTATO_ARQUIVO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_CONTATO_ARQUIVO;
GO

CREATE OR ALTER VIEW vw_T_CONTRATO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_CONTRATO;
GO

CREATE OR ALTER VIEW vw_T_DESCONTO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_DESCONTO;
GO

CREATE OR ALTER VIEW vw_T_EMPRESA_tenant AS
SELECT cd_empresa AS cd_tenant, * FROM T_EMPRESA;
GO

CREATE OR ALTER VIEW vw_T_FERIADO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_FERIADO;
GO

CREATE OR ALTER VIEW vw_T_FERIAS_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_FERIAS;
GO

CREATE OR ALTER VIEW vw_T_FILA_MATRICULA_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_FILA_MATRICULA;
GO

CREATE OR ALTER VIEW vw_T_HISTORICO_PESSOA_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_HISTORICO_PESSOA;
GO

CREATE OR ALTER VIEW vw_T_HORARIO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_HORARIO;
GO

CREATE OR ALTER VIEW vw_T_ITEM_ESCOLA_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_ITEM_ESCOLA;
GO

CREATE OR ALTER VIEW vw_T_NOME_CONTRATO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_NOME_CONTRATO;
GO

CREATE OR ALTER VIEW vw_T_PARAMETRO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_PARAMETRO;
GO

CREATE OR ALTER VIEW vw_T_PESSOA_EMPRESA_tenant AS
SELECT cd_empresa AS cd_tenant, * FROM T_PESSOA_EMPRESA;
GO

CREATE OR ALTER VIEW vw_T_POLITICA_DESCONTO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_POLITICA_DESCONTO;
GO

CREATE OR ALTER VIEW vw_T_PROSPECT_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_PROSPECT;
GO

CREATE OR ALTER VIEW vw_T_REAJUSTE_ANUAL_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_REAJUSTE_ANUAL;
GO

CREATE OR ALTER VIEW vw_T_SALA_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_SALA;
GO

CREATE OR ALTER VIEW vw_T_TABELA_MODIFICADA_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_TABELA_MODIFICADA;
GO

CREATE OR ALTER VIEW vw_T_TABELA_PRECO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_TABELA_PRECO;
GO

CREATE OR ALTER VIEW vw_T_TABELA_PRECO_MATERIAL_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_TABELA_PRECO_MATERIAL;
GO

CREATE OR ALTER VIEW vw_T_TIPO_DESCONTO_ESCOLA_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_TIPO_DESCONTO_ESCOLA;
GO

CREATE OR ALTER VIEW vw_T_TURMA_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM T_TURMA;
GO

CREATE OR ALTER VIEW vw_V_aditamento_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM V_aditamento;
GO

CREATE OR ALTER VIEW vw_v_aditamento_historico_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM v_aditamento_historico;
GO

CREATE OR ALTER VIEW vw_V_ALUNO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM V_ALUNO;
GO

CREATE OR ALTER VIEW vw_V_ALUNO_BOLSA_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM V_ALUNO_BOLSA;
GO

CREATE OR ALTER VIEW vw_V_CONTRATO_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM V_CONTRATO;
GO

CREATE OR ALTER VIEW vw_V_FILAMATRICULA_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM V_FILAMATRICULA;
GO

CREATE OR ALTER VIEW vw_V_PESSOA_tenant AS
SELECT cd_empresa AS cd_tenant, * FROM V_PESSOA;
GO

CREATE OR ALTER VIEW vw_vi_acao_listagem_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_acao_listagem;
GO

CREATE OR ALTER VIEW vw_vi_acao_pipeline_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_acao_pipeline;
GO

CREATE OR ALTER VIEW vw_vi_aluno_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_aluno;
GO

CREATE OR ALTER VIEW vw_vi_bairro_contato_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_bairro_contato;
GO

CREATE OR ALTER VIEW vw_vi_contato_listagem_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_contato_listagem;
GO

CREATE OR ALTER VIEW vw_vi_contrato_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_contrato;
GO

CREATE OR ALTER VIEW vw_vi_contrato_grid_turma_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_contrato_grid_turma;
GO

CREATE OR ALTER VIEW vw_vi_contrato_id_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_contrato_id;
GO

CREATE OR ALTER VIEW vw_vi_contrato_titulos_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_contrato_titulos;
GO

CREATE OR ALTER VIEW vw_vi_contrato1_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_contrato1;
GO

CREATE OR ALTER VIEW vw_vi_curso_aluno_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_curso_aluno;
GO

CREATE OR ALTER VIEW vw_vi_curso_mensagem_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_curso_mensagem;
GO

CREATE OR ALTER VIEW vw_vi_desistencia_carga_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_desistencia_carga;
GO

CREATE OR ALTER VIEW vw_vi_duracao_turma_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_duracao_turma;
GO

CREATE OR ALTER VIEW vw_vi_fila_matricula_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_fila_matricula;
GO

CREATE OR ALTER VIEW vw_vi_funcionario_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_funcionario;
GO

CREATE OR ALTER VIEW vw_vi_horario_turma_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_horario_turma;
GO

CREATE OR ALTER VIEW vw_vi_informacao_contrato_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_informacao_contrato;
GO

CREATE OR ALTER VIEW vw_vi_item_tabela_material_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_item_tabela_material;
GO

CREATE OR ALTER VIEW vw_vi_nome_turma_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_nome_turma;
GO

CREATE OR ALTER VIEW vw_vi_pessoa_tenant AS
SELECT cd_empresa AS cd_tenant, * FROM vi_pessoa;
GO

CREATE OR ALTER VIEW vw_vi_pre_turma_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_pre_turma;
GO

CREATE OR ALTER VIEW vw_vi_produto_aluno_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_produto_aluno;
GO

CREATE OR ALTER VIEW vw_vi_produto_curso_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_produto_curso;
GO

CREATE OR ALTER VIEW vw_vi_produto_mensagem_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_produto_mensagem;
GO

CREATE OR ALTER VIEW vw_vi_professor_tenant AS
SELECT cd_empresa AS cd_tenant, * FROM vi_professor;
GO

CREATE OR ALTER VIEW vw_vi_professor_turma_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_professor_turma;
GO

CREATE OR ALTER VIEW vw_vi_raf_sem_diario_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_raf_sem_diario;
GO

CREATE OR ALTER VIEW vw_vi_sala_turma_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_sala_turma;
GO

CREATE OR ALTER VIEW vw_vi_tabela_preco_curso_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_tabela_preco_curso;
GO

CREATE OR ALTER VIEW vw_vi_turma_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_turma;
GO

CREATE OR ALTER VIEW vw_vi_turma_professor_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_turma_professor;
GO

CREATE OR ALTER VIEW vw_vi_usuario_pipeline_tenant AS
SELECT cd_pessoa_escola AS cd_tenant, * FROM vi_usuario_pipeline;
GO
